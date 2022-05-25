using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using PostalServiceApp.Infrastructure;
using PostalServiceApp.Services;
using PostalServiceApp.ViewModels.DataForms;
using PostalServiceApp.Views.DataForms;
using PostalServiceClassLibrary.Models;

namespace PostalServiceApp.ViewModels.Pages;

// Модель представления для работы с изданиями
public class PublicationsPageViewModel : ViewModelBase
{
	// Ссылка на сервис данных
	private readonly DataService _dataService;

	// Текст строки состояния
	private string _statusInfo;
	public string StatusInfo
	{
		get => _statusInfo;
		set => Set(ref _statusInfo, value);
	}

	// Представление коллекции ICollectionView, привязываемая к DataGrid'у для фильтрации
	private ICollectionView _publicationsColView;
	public ICollectionView PublicationsColView
	{
		get => _publicationsColView;
		set => Set(ref _publicationsColView, value);
	}

	// Текущее выбранное издание
	private Publication _selectedItem;
	public Publication SelectedItem
	{
		get => _selectedItem;
		set => Set(ref _selectedItem, value);
	}
	

	// Команда внесения данных о новом издании
	private RelayCommand _newPubCommand;
	public RelayCommand NewPubCommand => _newPubCommand ??= new RelayCommand(_ =>
	{
		// Объект добавляемого элемента
		var newPub = new Publication { IdPubType = 1 };

		InputDataService ids = new();

		if(ids.InputNewPublication(newPub) == false)
			return;

		_dataService.AddPublication(newPub);

		StatusInfo = $"Издание добавлено. Изданий: {_dataService.Publications.Count}";

		// Установка добавленной записи как текущего выбранного элемента
		SelectedItem = _dataService.Publications.FirstOrDefault(p => p.Id == newPub.Id);
	});

	// Команда изменения данных об издании
	private RelayCommand _editPubCommand;
	public RelayCommand EditPubCommand => _editPubCommand ??= new RelayCommand(_ =>
	{

		InputDataService ids = new();

		if (ids.EditPublication(SelectedItem) == false)
			return;
	
		_dataService.UpdatePublication(SelectedItem);

		StatusInfo = $"Издание отредактировано. Изданий: {_dataService.Publications.Count}";


	}, _ => SelectedItem != null);


	// Команда удаления издания
	private RelayCommand _deletePubCommand;
	public RelayCommand DeletePubCommand => _deletePubCommand ??= new RelayCommand(_ =>
	{
		_dataService.DeletePublication(SelectedItem.Id);
	}, _ => SelectedItem != null);


	// Команда прокрутки до текущего выбранного элемента
	private RelayCommand _scrollToSelected;
	public RelayCommand ScrollToSelected =>
		_scrollToSelected ??= new RelayCommand(o => 
			((DataGrid)o).ScrollIntoView(SelectedItem),
			_ => SelectedItem != null);

	// Конструктор
	public PublicationsPageViewModel(DataService dataService)
	{
		_dataService = dataService;

		// Начальный выбранный элемент фильтрации по типу изданий
		SelectedType = PubTypes.First();

		Update();

		_dataService.OnUpdatePublications += (sender, args) => Update();
	}

	// Обновление данных коллекций 
	public void Update()
	{
		PublicationsColView = CollectionViewSource.GetDefaultView(_dataService.Publications);
		PublicationsColView.Filter = OnFilterTriggered;

		StatusInfo = $"Изданий: {_dataService.Publications.Count}";
	}

	#region Фильтрация

	private const string NoneFilter = "";

	// Список типов изданий
	public List<string> PubTypes =>
		new List<string>{NoneFilter}
			.Concat(DataService.PubTypeList)
			.ToList();

	// Выбранный для фильтрации тип издания 
	private string _selectedType;
	public string SelectedType
	{
		get => _selectedType;
		set => Set(ref _selectedType, value);
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
	
	// Команда выпонения фильтрации
	private RelayCommand _filterData;
	public RelayCommand FilterData => _filterData ??= new RelayCommand((_) => 
		CollectionViewSource.GetDefaultView(_dataService.Publications).Refresh());
		
	// Метод проверок условия при фильтрации
	private bool OnFilterTriggered(object item) =>
		item is Publication pub 
		&& (SelectedType == NoneFilter || SelectedType == pub.PubType.Name)
		&& (string.IsNullOrEmpty(TitleFilter) || pub.Title.StartsWith(TitleFilter,
			true,CultureInfo.InvariantCulture)) 
		&& (string.IsNullOrEmpty(IndexFilter) || pub.PubIndex.StartsWith(IndexFilter,
			true, CultureInfo.InvariantCulture)) 
		&& (string.IsNullOrEmpty(PriceFilter) || !int.TryParse(PriceFilter, out var price) || pub.Price == price);

	// Команда сброса фильтра
	private RelayCommand _clearFilterCommand;
	public RelayCommand ClearFilterCommand => _clearFilterCommand ??= new RelayCommand((o) =>
	{
		switch ((string)o)
		{
			case "Title": 
				TitleFilter = string.Empty;
				break;
			case "Index":
				IndexFilter = string.Empty;
				break;
			case "Price":
				PriceFilter = string.Empty;
				break;
		}
	}, o => (string) o switch
	{
		"Title" => !TitleFilter.IsNullOrEmpty(),
		"Index" => !IndexFilter.IsNullOrEmpty(),
		"Price" => !PriceFilter.IsNullOrEmpty(),
		_ => false
	});

	#endregion
}