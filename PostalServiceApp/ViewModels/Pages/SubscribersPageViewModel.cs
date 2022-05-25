using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using PostalServiceApp.Infrastructure;
using PostalServiceApp.Services;
using PostalServiceApp.ViewModels.DataForms;
using PostalServiceApp.Views.DataForms;
using PostalServiceClassLibrary.Models;

namespace PostalServiceApp.ViewModels.Pages;

// Модель представления для работы с подписчиками
public class SubscribersPageViewModel : ViewModelBase
{
	// Ссылка на сервис данных
	private readonly DataService _dataService;
	public DataService DataService => _dataService;

	// Текст строки состояния
	private string _statusInfo;
	public string StatusInfo
	{
		get => _statusInfo;
		set => Set(ref _statusInfo, value);
	}


	// Представление коллекции ICollectionView, привязываемая к DataGrid'у для удобства фильтрации
	private ICollectionView _subscribersColView;
	public ICollectionView SubscribersColView
	{
		get => _subscribersColView;
		set => Set(ref _subscribersColView, value);
	}

	// Текущий выбранный подписчик
	private Subscriber _selectedItem;
	public Subscriber SelectedItem
	{
		get => _selectedItem;
		set => Set(ref _selectedItem, value);
	}


	// Команда внесения данных о новом подписчике
	private RelayCommand _newSubscriberCommand;
	public RelayCommand NewSubscriberCommand => _newSubscriberCommand ??= new RelayCommand(_ =>
	{

		// Объект добавляемого элемента
		var newSub = new Subscriber
		{
			Address = new Address
			{
				Street = new Street()
			}
		};

		InputDataService ids = new();

		if (ids.InputNewSubscriber(newSub) == false)
			return;

		_dataService.AddSubscriber(newSub);

		StatusInfo = $"Подписчик добавлен. Подписчиков: {_dataService.Subscribers.Count}";


		// Установка добавленной записи как текущего элемента таблицы
		SelectedItem = _dataService.Subscribers.FirstOrDefault(p => p.Id == newSub.Id);
	});


	// Команда изменения данных о подписчике
	private RelayCommand _editSubscriberCommand;
	public RelayCommand EditSubscriberCommand => _editSubscriberCommand ??= new RelayCommand(_ =>
	{
		// Изменяемый элемент
		Subscriber editing = _dataService.Subscribers.FirstOrDefault(s => s.Id == SelectedItem.Id);

		InputDataService ids = new();

		if (ids.EditSubscriber(editing) == false)
			return;

		_dataService.UpdateSubscriber(editing);

		// Установка измененной записи как текущего выбранного элемента
		SelectedItem = _dataService.Subscribers.FirstOrDefault(p => p.Id == editing!.Id);
	}, _ => SelectedItem != null);


	// Команда удаления подписчика
	private RelayCommand _deleteSubscriberCommand;
	public RelayCommand DeleteSubscriberCommand => _deleteSubscriberCommand ??= new RelayCommand(_ =>
	{
		_dataService.DeleteSubscriber(SelectedItem.Id);
	}, _ => SelectedItem != null);


	// Команда прокрутки до текущего выбранного элемента
	private RelayCommand _scrollToSelected;
	public RelayCommand ScrollToSelected =>
		_scrollToSelected ??= new RelayCommand(o =>
				((DataGrid)o).ScrollIntoView(SelectedItem),
			_ => SelectedItem != null);

	// Конструктор
	public SubscribersPageViewModel(DataService dataService)
	{
		_dataService = dataService;

		Update();
		_dataService.Subscribers.CollectionChanged += (_, _) => Update();
		_dataService.OnUpdateSubscribers += (_, _) => Update();
	}

	// Обновление данных коллекций 
	private void Update()
	{
		SubscribersColView = CollectionViewSource.GetDefaultView(_dataService.Subscribers);
		SubscribersColView.Filter = OnFilterTriggered;
		FilterData.Execute(null);
		StatusInfo = $"Подписчиков: {_dataService.Subscribers.Count}";
	}

	#region Фильтрация

	// Фильтр ФИО
	private string _nameFilter;
	public string NameFilter
	{
		get => _nameFilter;
		set
		{
			Set(ref _nameFilter, value);
			FilterData.Execute(null);
		}
	}

	// Фильтр улицы
	private string _streetFilter;
	public string StreetFilter
	{
		get => _streetFilter;
		set
		{
			Set(ref _streetFilter, value);
			FilterData.Execute(null);
		}
	}

	// Фильтр дома
	private string _buildingFilter;
	public string BuildingFilter
	{
		get => _buildingFilter;
		set
		{
			Set(ref _buildingFilter, value);
			FilterData.Execute(null);
		}
	}

	// Команда выпонения фильтрации
	private RelayCommand _filterData;
	public RelayCommand FilterData => _filterData ??= new RelayCommand((_) =>
		CollectionViewSource.GetDefaultView(_dataService.Subscribers).Refresh());

	private bool OnFilterTriggered(object item)
	{
		if (item is not Subscriber subscriber) return false;

		if (!string.IsNullOrEmpty(BuildingFilter) && subscriber.Address.Building != BuildingFilter ||
		    !string.IsNullOrEmpty(StreetFilter) && !subscriber.Address.Street.Name.ToUpperInvariant().Contains(StreetFilter.ToUpperInvariant()))
			return false;

		if (!string.IsNullOrEmpty(NameFilter))
		{
			var snp = NameFilter.ToUpperInvariant().Split(new[] { ' ' }, StringSplitOptions.None);

			string name = subscriber.Person.Name.ToUpperInvariant();
			string surname = subscriber.Person.Surname.ToUpperInvariant();
			string patronymic = subscriber.Person.Patronymic.ToUpperInvariant();

			switch (snp.Length)
			{
				case 1 when !surname.StartsWith(snp[0]):
				case 2 when snp[0] != surname || !name.StartsWith(snp[1]):
				case 3 when snp[0] != surname || snp[1] != name || !patronymic.StartsWith(snp[2]):
					return false;
				default:
					return true;
			}
		}
		return true;
	}

	// Команда сброса фильтра
	private RelayCommand _clearFilterCommand;
	public RelayCommand ClearFilterCommand => _clearFilterCommand ??= new RelayCommand((o) =>
	{
		switch ((string)o)
		{
			case "Name":
				NameFilter = string.Empty;
				break;
			case "Street":
				StreetFilter = string.Empty;
				break;
			case "Building":
				BuildingFilter = string.Empty;
				break;
		}
	}, o => (string)o switch
	{
		"Name" => !NameFilter.IsNullOrEmpty(),
		"Street" => !StreetFilter.IsNullOrEmpty(),
		"Building" => !BuildingFilter.IsNullOrEmpty(),
		_ => false
	});

	#endregion
}