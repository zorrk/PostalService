using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Win32;
using PostalServiceApp.Infrastructure;

namespace PostalServiceApp.ViewModels;

public class DocumentViewModel : ViewModelBase
{
	// Объект документа
	private FlowDocument _document;

	public FlowDocument Document
	{
		get => _document;
		set => Set(ref _document, value);
	}

	// Команда сохранения документа
	private RelayCommand _saveAsCommand;
	public RelayCommand SaveAsCommand => _saveAsCommand ??= new RelayCommand(_ =>
	{
		SaveFileDialog sfd = new()
		{
			Filter = "Файл RTF (*.rtf)|*.rtf"
		};

		if (sfd.ShowDialog() != true)
			return;

		using FileStream fs = new(sfd.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);

		var doc = Document.Clone();
		doc.FontFamily = new FontFamily("Tahoma");
		doc.FontSize = 12;
		TextRange textRange = new (doc.ContentStart, doc.ContentEnd);

			
		textRange.Save(fs, DataFormats.Rtf);
	}, _ => _document != null);

	private RelayCommand _printCommand;

	public RelayCommand PrintCommand => _printCommand ??= new RelayCommand(_ =>
	{

		var copy = Document.Clone();

		/* // Способ с помощью PrintDialog
		PrintDialog printDlg = new PrintDialog();
		copy.PageHeight = printDlg.PrintableAreaHeight;
		copy.PageWidth = printDlg.PrintableAreaWidth;
		copy.PagePadding = new Thickness(50);

		IDocumentPaginatorSource idpSource = copy;
		printDlg.PrintDocument(idpSource.DocumentPaginator, "Печать");
		*/


		// Create a XpsDocumentWriter object, implicitly opening a Windows common print dialog,
		// and allowing the user to select a printer.

		// get information about the dimensions of the seleted printer+media.
		System.Printing.PrintDocumentImageableArea ia = null;
		System.Windows.Xps.XpsDocumentWriter docWriter = System.Printing.PrintQueue.CreateXpsDocumentWriter(ref ia);

		if (docWriter != null && ia != null)
		{
			DocumentPaginator paginator = ((IDocumentPaginatorSource)copy).DocumentPaginator;

			// Change the PageSize and PagePadding for the document to match the CanvasSize for the printer device.
			paginator.PageSize = new Size(ia.MediaSizeWidth, ia.MediaSizeHeight);
			Thickness t = new (42);  // copy.PagePadding;
			copy.PagePadding = new Thickness(
				Math.Max(ia.OriginWidth, t.Left),
				Math.Max(ia.OriginHeight, t.Top),
				Math.Max(ia.MediaSizeWidth - (ia.OriginWidth + ia.ExtentWidth), t.Right),
				Math.Max(ia.MediaSizeHeight - (ia.OriginHeight + ia.ExtentHeight), t.Bottom));

			copy.ColumnWidth = double.PositiveInfinity;
			//copy.PageWidth = 528; // allow the page to be the natural with of the output device

			// Send content to the printer.
			docWriter.Write(paginator);
		}

	}, _ => Document != null);

	public DocumentViewModel(FlowDocument document)
	{
		Document = document;
	}
}