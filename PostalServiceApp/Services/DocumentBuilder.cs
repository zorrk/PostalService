using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using PostalServiceApp.Infrastructure;
using PostalServiceApp.Models;
using PostalServiceApp.Views;
using PostalServiceClassLibrary.DataAccess;
using PostalServiceClassLibrary.Models;
using PostalServiceClassLibrary.Models.QueriesResults;

namespace PostalServiceApp.Services;

// Класс, обеспечивающий построение потоковых документов
public static class DocumentBuilder
{
	// Формирование документа кваитанции
	public static FlowDocument CreateReceiptDoc(Receipt receipt)
	{
		FlowDocument doc = new()
		{
			FontFamily = new FontFamily("Courier New"),
		};

		doc.Blocks.AddRange(new[]
		{
			new Paragraph(new Run($"Квитанция №{receipt.Number.PadLeft(6, '0')}"))
			{
				FontSize = 18, FontWeight = FontWeights.Bold, Margin = new Thickness(0)
			},
			TextLine(""),
			TextLine($"Фамилия И.О: {receipt.Customer}"),
			TextLine($"Тип: {receipt.PubType}"),
			TextLine($"Индекс: {receipt.PubIndex}"),
			TextLine($"Название: {receipt.PubTitle}"),
			TextLine($"Срок подписки: {receipt.Duration}мес."),
			TextLine($"Стоимость: {receipt.Cost}р."),
		});

		return doc;
	}

	// Формирование документа отчета
	public static FlowDocument CreateReportDoc()
	{
		FlowDocument doc = new() { FontFamily = new FontFamily("Tahoma") };
		using var repo = new UnitOfWork();

		// Заголовок документа
		doc.Blocks.Add(new Paragraph(new Run($"Отчет на {DateTime.Now.Date:dd.MM.yy}"))
		{
			FontSize = 20,
			FontWeight = FontWeights.Bold,
		});

		// Общая информация по отделению
		doc.Blocks.Add(new Paragraph(new Run($"Общая информация по отделению:"))
		{
			FontSize = 16,
			FontWeight = FontWeights.Bold,
		});

		var postalInfo = new PostalInfo()
		{
			Publications = repo.PublicationRepository.Get().Distinct().Count(),
			Districts = repo.DistrictRepository.Get().Count(),
			Postmans = repo.PostmanRepository.Get().Count()
		};

		doc.Blocks.Add(PostalServiceDetailsTable(postalInfo));

		// Общая информация по изданиям
		doc.Blocks.Add(new Paragraph(new Run($"Общая информация по изданиям:"))
		{
			FontSize = 16,
			FontWeight = FontWeights.Bold,
		});

		doc.Blocks.Add(PublicationsReportTable(repo.ReportPublications().OrderBy(r => r.Title)));

			
		// ОБщая информация по участкам
		doc.Blocks.Add(new Paragraph(new Run($"Общая информация по участкам:"))
		{
			FontSize = 16,
			FontWeight = FontWeights.Bold,
		});

		doc.Blocks.Add(DistrictsGeneralInfoTable(repo.ReportDistricts()));


		// Информация по доставкам участков
		doc.Blocks.Add(new Paragraph(new Run($"Доставляемые издания по участкам:"))
		{
			FontSize = 16,
			FontWeight = FontWeights.Bold,
		});

		var districts = repo.DistrictRepository.Get(includeProperties: "Postman.Person").ToList();
			
		districts.ForEach(d =>
		{
			var subscribes = repo.SubscribeRepository
				.Get(s => s.Subscriber.Address.IdDistrict == d.Id, includeProperties: "Publication,Subscriber.Address.Street");

			doc.Blocks.Add(DistrictReportTable(d, subscribes.OrderBy(s => s.Publication.Title)));
		});


		return doc;
	}


	// Формирование таблицы общей информации по отделению
	private static Table PostalServiceDetailsTable(PostalInfo postalInfo)
	{
		var table = new Table
		{
			CellSpacing = 0,
			Background = Brushes.White,
			FontFamily = new FontFamily("Tahoma"),
			BorderBrush = Brushes.Black,
			FontSize = 14,
			BorderThickness = new Thickness(0, 1, 0, 1)
		};

		table.RowGroups.Add(new TableRowGroup());

		table.Columns.Add(new TableColumn());
		table.Columns.Add(new TableColumn());

		table.RowGroups[0].Rows.Add(new TableRow());
		TableRow currentRow = table.RowGroups[0].Rows[0];
		currentRow.Cells.Add(MakeLeftCell("Почтальонов"));
		currentRow.Cells.Add(MakeRightCell($"{postalInfo.Postmans}", numeric: true));

		table.RowGroups[0].Rows.Add(new TableRow());
		currentRow = table.RowGroups[0].Rows[1];
		currentRow.Cells.Add(MakeLeftCell("Участков"));
		currentRow.Cells.Add(MakeRightCell($"{postalInfo.Postmans}", numeric: true));

		table.RowGroups[0].Rows.Add(new TableRow());
		currentRow = table.RowGroups[0].Rows[2];
		currentRow.Cells.Add(MakeLeftCell("Изданий"));
		currentRow.Cells.Add(MakeRightCell($"{postalInfo.Publications}", numeric: true));


		FitWidthToContent(table);

		return table;
	}


	// Формирование таблицы отчета по изданиям
	private static Table PublicationsReportTable(IEnumerable<ReportPublicationResult> pubsReport)
	{
		var table = new Table
		{
			CellSpacing = 0,
			Background = Brushes.White,
			FontFamily = new FontFamily("Tahoma"),
			BorderBrush = Brushes.Black,
			FontSize = 14,
			BorderThickness = new Thickness(0, 1, 0, 1)
		};

		table.Columns.Add(new TableColumn()); // Индекс
		table.Columns.Add(new TableColumn()); // Название издания
		table.Columns.Add(new TableColumn()); // Тип издания
		table.Columns.Add(new TableColumn()); // Средний срок подписки
		table.Columns.Add(new TableColumn()); // Количество экземпляров

		table.RowGroups.Add(new TableRowGroup());

		// Хедер таблицы 
		table.RowGroups[0].Rows.Add(new TableRow
		{
			FontWeight = FontWeights.Bold,
			Background = Brushes.Silver,
		});

		TableRow currentRow = table.RowGroups[0].Rows[0];

		currentRow.Cells.Add(MakeLeftCell("Индекс", header: true));
		currentRow.Cells.Add(MakeMidCell("Название издания", header: true));
		currentRow.Cells.Add(MakeMidCell("Тип издания", header: true));
		currentRow.Cells.Add(MakeRightCell("Ср. срок подписки", header: true));
		currentRow.Cells.Add(MakeRightCell("Кол-во экземпляров", header: true));

		// Данные таблицы

		int nRow = 1; // индекс строки с которой начинаются данные
		foreach (var item in pubsReport)
		{
			table.RowGroups[0].Rows.Add(new TableRow { FontWeight = FontWeights.Normal });

			currentRow = table.RowGroups[0].Rows[nRow++];

			currentRow.Cells.Add(MakeLeftCell(item.PubIndex));
			currentRow.Cells.Add(MakeMidCell(item.Title));
			currentRow.Cells.Add(MakeMidCell(item.PubType));
			currentRow.Cells.Add(MakeMidCell($"{item.Duration_Average}", numeric: true));
			currentRow.Cells.Add(MakeRightCell($"{item.Amount}", numeric: true));

		}

		FitWidthToContent(table);

		return table;
	}

	// Формирование таблицы общей информации по участкам
	private static Table DistrictsGeneralInfoTable(List<ReportDistrictResult> districtsReport)
	{
		var table = new Table
		{
			CellSpacing = 0,
			Background = Brushes.White,
			FontFamily = new FontFamily("Tahoma"),
			BorderBrush = Brushes.Black,
			FontSize = 14,
			BorderThickness = new Thickness(0, 1, 0, 1)
		};


		table.Columns.Add(new TableColumn()); // Название участка
		table.Columns.Add(new TableColumn()); // Количество различных подписных изданий

		table.RowGroups.Add(new TableRowGroup());

		// Хедер таблицы 
		table.RowGroups[0].Rows.Add(new TableRow
		{
			FontWeight = FontWeights.Bold,
			Background = Brushes.Silver,
		});

		TableRow currentRow = table.RowGroups[0].Rows[0];

		currentRow.Cells.Add(MakeLeftCell("Название участка", header: true));
		currentRow.Cells.Add(MakeRightCell("Кол-во различных изданий", header: true));

		// Данные таблицы

		int nRow = 1; // индекс строки с которой начинаются данные
		foreach (var item in districtsReport)
		{
			table.RowGroups[0].Rows.Add(new TableRow { FontWeight = FontWeights.Normal });

			currentRow = table.RowGroups[0].Rows[nRow++];

			currentRow.Cells.Add(MakeLeftCell(item.District));
			currentRow.Cells.Add(MakeRightCell($"{item.Amount}", numeric: true));
		}

		FitWidthToContent(table);

		return table;
	}


	// Формирование таблицы отчета по участку
	private static Table DistrictReportTable(District district, IEnumerable<Subscribe> subscribes)
	{
		var table = new Table
		{
			CellSpacing = 0,
			Background = Brushes.White,
			FontFamily = new FontFamily("Tahoma"),
			BorderBrush = Brushes.Black,
			BorderThickness = new Thickness(0, 1, 0, 1)
		};

		table.Columns.Add(new TableColumn());
		table.Columns.Add(new TableColumn());
		table.Columns.Add(new TableColumn());
		table.Columns.Add(new TableColumn());

		table.RowGroups.Add(new TableRowGroup());


		// Название участка 
		table.RowGroups[0].Rows.Add(new TableRow()
		{
			Background = Brushes.Silver,
			FontSize = 16,
			FontWeight = System.Windows.FontWeights.Bold
		});
		TableRow currentRow = table.RowGroups[0].Rows[0];

		currentRow.Cells.Add(new TableCell(new Paragraph(new Run(district.Title)))
		{
			BorderThickness = new Thickness(1, 0, 1, 1),
			BorderBrush = Brushes.Black,
			Padding = new Thickness(4, 0, 0, 0),
			ColumnSpan = 4
		});


		// Обслуживающий почтальон
		table.RowGroups[0].Rows.Add(new TableRow()
		{
			Background = Brushes.White,
			FontSize = 15,
			FontWeight = System.Windows.FontWeights.Bold
		});
		currentRow = table.RowGroups[0].Rows[1];

		var p = new Paragraph();
		p.Inlines.AddRange(new[]
		{
			new Run("Почтальон: "),
			new Run(district.Postman.Person.ShortName()){ FontWeight = FontWeights.Normal }
		});

		currentRow.Cells
			.Add(new TableCell(p)
			{
				BorderThickness = new Thickness(1, 0, 1, 1),
				BorderBrush = Brushes.Black,
				Padding = new Thickness(4, 0, 0, 0),
				ColumnSpan = 4
			});


		// Хедер таблицы 
		table.RowGroups[0].Rows.Add(new TableRow()
		{
			FontWeight = FontWeights.Bold,
			FontSize = 14
		});
		currentRow = table.RowGroups[0].Rows[2];

		currentRow.Cells.Add(MakeLeftCell("Индекс", header: true));
		currentRow.Cells.Add(MakeMidCell("Название издания", header: true));
		currentRow.Cells.Add(MakeMidCell("Адрес доставки", header: true));
		currentRow.Cells.Add(MakeRightCell("Срок подписки", header: true));


		// Данные таблицы

		int nRow = 3; // индекс строки с которой начинаются данные
		foreach (var subscribe in subscribes)
		{
			table.RowGroups[0].Rows.Add(new TableRow()
			{
				FontSize = 14,
				FontWeight = FontWeights.Normal
			});

			currentRow = table.RowGroups[0].Rows[nRow++];

			currentRow.Cells.Add(MakeLeftCell(subscribe.Publication.PubIndex));

			currentRow.Cells.Add(MakeMidCell(subscribe.Publication.Title));

			currentRow.Cells.Add(MakeMidCell(
				$"{subscribe.Subscriber.Address.Street.Name}, " +
				$"д.{subscribe.Subscriber.Address.Building}, " +
				$"кв.{subscribe.Subscriber.SubAddress}"));

			currentRow.Cells.Add(MakeRightCell(subscribe.Duration.ToString(), numeric: true));
		}

		FitWidthToContent(table, 2);

		return table;
	}


	// Настройка ширины столбцов по длине максимальной ячейки
	private static void FitWidthToContent(Table table, int skipLines = 0)
	{
		if (table.RowGroups[0].Rows.Count <= skipLines)
			return;

		for (var i = 0; i < table.Columns.Count; i++)
		{
			table.Columns[i].Width = new GridLength(
				table.RowGroups[0].Rows.Skip(skipLines)
					.Select(r => r.Cells[i])
					.Max(cell => GetDesiredWidth(new TextRange(cell.ContentStart, cell.ContentEnd)
					)) + 10);
		}
	}

	// Получить длину в пикселях из TextRange (для определения ширины колонок)
	private static double GetDesiredWidth(TextRange textRange) =>
		new FormattedText(
			textToFormat: textRange.Text,
			culture: CultureInfo.CurrentCulture,
			flowDirection: FlowDirection.LeftToRight,
			typeface: new Typeface(
				textRange.GetPropertyValue(TextElement.FontFamilyProperty) as FontFamily,
				(FontStyle)textRange.GetPropertyValue(TextElement.FontStyleProperty),
				(FontWeight)textRange.GetPropertyValue(TextElement.FontWeightProperty),
				FontStretches.Normal),
			emSize: (double)textRange.GetPropertyValue(TextElement.FontSizeProperty),
			foreground: Brushes.Black,
			pixelsPerDip: VisualTreeHelper.GetDpi(Application.Current.MainWindow).PixelsPerDip).Width;

	// Создание параграфа с текстом
	private static Paragraph TextLine(string text) => new(new Run(text)) { Margin = new Thickness(0) };

		
	// Формирование ячеек для таблицы
	private static TableCell MakeLeftCell(string text, bool header = false, bool numeric = false) =>
		new (new Paragraph(new Run(text)))
		{
			BorderThickness = new Thickness(1, 0, 1, header ? 1 : 0),
			BorderBrush = Brushes.Black,
			Padding = new Thickness(4, 0, 4, header ? 8 : 0),
			TextAlignment = numeric ? TextAlignment.Right : TextAlignment.Left
		};

	private static TableCell MakeMidCell(string text, bool header = false, bool numeric = false) =>
		new (new Paragraph(new Run(text)))
		{
			BorderThickness = new Thickness(0, 0, 1, header ? 1 : 0),
			BorderBrush = Brushes.Black,
			Padding = new Thickness(4, 0, 4, header ? 8 : 00),
			TextAlignment = numeric ? TextAlignment.Right : TextAlignment.Left
		};

	private static TableCell MakeRightCell(string text, bool header = false, bool numeric = false) =>
		new (new Paragraph(new Run(text)))
		{
			BorderThickness = new Thickness(0, 0, 1, header ? 1 : header ? 8 : 0),
			BorderBrush = Brushes.Black,
			Padding = new Thickness(4, 0, 4, 0),
			TextAlignment = numeric ? TextAlignment.Right : TextAlignment.Left
		};

	// Вспомогательный класс 
	private class PostalInfo
	{
		public int Postmans { get; set; }
		public int Districts { get; set; }
		public int Publications { get; set; }
	}
}