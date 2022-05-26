using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using PostalServiceApp.Infrastructure;
using PostalServiceApp.Models;
using PostalServiceApp.Services;
using PostalServiceApp.ViewModels.DataForms;
using PostalServiceApp.Views;
using PostalServiceApp.Views.DataForms;
using PostalServiceClassLibrary.DataAccess;
using PostalServiceClassLibrary.Models;

namespace PostalServiceApp.ViewModels.Pages;

// Модель представления для работы с подписками
public class SubscribesPageViewModel : ViewModelBase
{
	// Ссылка на сервис данных
	private readonly DataService _dataService;
	public DataService DataService => _dataService;

	// Текст строки состояния
	private string _statusInfo;
	public string StatusInfo
	{
		get => _statusInfo;
		set
		{
			_statusInfo = value;
			OnPropertyChanged(nameof(StatusInfo));
		}
	}

	// Представление коллекции ICollectionView, привязываемая к DataGrid
	private ICollectionView _subscribesColView;
	public ICollectionView SubscribesColView
	{
		get => _subscribesColView;
		set => Set(ref _subscribesColView, value);
	}

	// Текущая выбранная подпсика
	private Subscribe _selectedSubscribe;
	public Subscribe SelectedSubscribe
	{
		get => _selectedSubscribe;
		set => Set(ref _selectedSubscribe, value);
	}

	// Команда прерывания подписки
	private RelayCommand _deleteSubscribeCommand;
	public RelayCommand DeleteSubscribePubCommand => _deleteSubscribeCommand ??= new RelayCommand(_ =>
	{
		_dataService.DeleteSubscribe(SelectedSubscribe.Id);
	}, _ => SelectedSubscribe != null);

	// Команда оформления новой подписки
	private RelayCommand _newSubscribeCommand;
	public RelayCommand NewSubscribeCommand => _newSubscribeCommand ??= new RelayCommand(_ =>
	{

		// Объект добавляемого элемента
		var newSub = new Subscribe();

		InputDataService ids = new();

		if (ids.InputNewSubscribe(newSub, _dataService) == false)
			return;

			
		_dataService.AddSubscribe(newSub);

		// Установка добавленной записи как текущего элемента таблицы
		SelectedSubscribe = _dataService.Subscribes.FirstOrDefault(s => s.Id == newSub.Id);
	});

	// Комманда формирования и отображения квитанции выбранной подписки
	private RelayCommand _displayReceiptCommand;

	public RelayCommand DisplayReceiptCommand => _displayReceiptCommand ??= new RelayCommand(_ =>
	{
		Receipt receipt = new ()
		{
			PubType = SelectedSubscribe.Publication.PubType.Name,
			PubIndex = SelectedSubscribe.Publication.PubIndex,
			PubTitle = SelectedSubscribe.Publication.Title,
			Cost = SelectedSubscribe.TotalCost ?? 0,
			Customer = SelectedSubscribe.Subscriber.Person.ShortName,
			Duration = SelectedSubscribe.Duration,
			Number = SelectedSubscribe.Id.ToString()
		};

		var vm = new DocumentViewModel(DocumentBuilder.CreateReceiptDoc(receipt));
		var wnd = new ReceiptDocView {DataContext = vm};

		wnd.ShowDialog();

	}, _ => SelectedSubscribe != null);

	// Конструктор
	public SubscribesPageViewModel(DataService dataService)
	{

		_dataService = dataService;

		_dataService.Subscribes.CollectionChanged += (_, _) => Update();
		_dataService.OnUpdateSubscribes += (_, _) => Update();

		// Выбранный элемент фильтрации по типу изданий
		SelectedType = PubTypes.First();
		SelectedDuration = DurationsList.First();

		Update();
	}

	public void Update()
	{
		SubscribesColView = CollectionViewSource.GetDefaultView(_dataService.Subscribes);
		SubscribesColView.Filter = OnFilterTriggered;

		StatusInfo = $"Подписок: {_dataService.Subscribes.Count}";
	}

	

	#region Фильтрация

	private const string NoneFilter = "";

	// Список типов изданий
	public List<string> PubTypes
	{
		get
		{
			using var repo = new UnitOfWork();

			List<string> names = 
				new List<string> { NoneFilter }
					.Concat(repo.PubTypeRepository.Get().Select(t => t.Name))
					.ToList();

			return names;
		}
	}

	// Выбранный для фильтрации тип издания 
	private string _selectedType;
	public string SelectedType
	{
		get => _selectedType;
		set => Set(ref _selectedType, value);
	}

	// Варианты сроков подписки
	public List<string> DurationsList => new List<string> { NoneFilter }
		.Concat(Constants.DurationsList)
		.ToList();

	// Выбранный срок подписки
	private string _selectedDuration;
	public string SelectedDuration
	{
		get => _selectedDuration;
		set => Set(ref _selectedDuration, value);
	}


	// Фильтр фамилии подписчика
	private string _surnameFilter;
	public string SurnameFilter
	{
		get => _surnameFilter;
		set
		{
			Set(ref _surnameFilter, value);
			FilterData.Execute(null);
		}
	}

	// Фильтр названия
	private string _titleFilter;
	public string TitleFilter
	{
		get => _titleFilter;
		set
		{
			Set(ref _titleFilter, value);
			FilterData.Execute(null);

		}
	}

	// Фильтр индекса
	private string _indexFilter;
	public string IndexFilter
	{
		get => _indexFilter;
		set
		{
			Set(ref _indexFilter, value);
			FilterData.Execute(null);
		}
	}

	// Фильтр цены
	private string _priceFilter;
	public string PriceFilter
	{
		get => _priceFilter;
		set
		{
			Set(ref _priceFilter, value);
			FilterData.Execute(null);
		}
	}

	// Фильтр начальной даты
	private DateTime? _dateFromFilter;
	public DateTime? DateFromFilter
	{
		get => _dateFromFilter;
		set
		{
			Set(ref _dateFromFilter, value);
			FilterData.Execute(null);
		}
	}


	// Фильтр конечной даты
	private DateTime? _dateToFilter;
	public DateTime? DateToFilter
	{
		get => _dateToFilter;
		set
		{
			Set(ref _dateToFilter, value);
			FilterData.Execute(null);
		}
	}


	// Команда выпонения фильтрации
	private RelayCommand _filterData;
	public RelayCommand FilterData => _filterData ??= new RelayCommand((_) =>
		CollectionViewSource.GetDefaultView(_dataService.Subscribes).Refresh());

	// Метод проверок условия при фильтрации
	private bool OnFilterTriggered(object item) =>
		item is Subscribe sub
		&& (SelectedType == NoneFilter 
		    || SelectedType == sub.Publication.PubType.Name)
		&& (SelectedDuration == NoneFilter 
		    || SelectedDuration == sub.Duration.ToString())
		&& (string.IsNullOrEmpty(SurnameFilter) 
		    || sub.Subscriber.Person.Surname.StartsWith(SurnameFilter, true, CultureInfo.InvariantCulture))
		&& (string.IsNullOrEmpty(TitleFilter) 
		    || sub.Publication.Title.StartsWith(TitleFilter, true, CultureInfo.InvariantCulture))
		&& (string.IsNullOrEmpty(IndexFilter)
		    || sub.Publication.PubIndex.StartsWith(IndexFilter, true, CultureInfo.InvariantCulture))
		&& (string.IsNullOrEmpty(PriceFilter) 
		    || !int.TryParse(PriceFilter, out var price)
		    || sub.TotalCost == price)
		&& (DateFromFilter == null
		    || sub.StartDate >= DateFromFilter)
		&& (DateToFilter == null 
		    || sub.StartDate <= DateToFilter);

	// Команда сброса фильтра
	private RelayCommand _clearFilterCommand;
	public RelayCommand ClearFilterCommand => _clearFilterCommand ??= new RelayCommand((o) =>
	{
		switch ((string)o)
		{
			case "Surname":
				SurnameFilter = string.Empty;
				break;
			case "Title":
				TitleFilter = string.Empty;
				break;
			case "Index":
				IndexFilter = string.Empty;
				break;
			case "Price":
				PriceFilter = string.Empty;
				break;
			case "Date":
				DateFromFilter = null;
				DateToFilter = null;
				break;
		}
	}, o => (string)o switch
	{
		"Surname" => !SurnameFilter.IsNullOrEmpty(),
		"Title" => !TitleFilter.IsNullOrEmpty(),
		"Index" => !IndexFilter.IsNullOrEmpty(),
		"Price" => !PriceFilter.IsNullOrEmpty(),
		"Date" => DateFromFilter != null || DateToFilter != null,
		_ => false
	});

	#endregion

}