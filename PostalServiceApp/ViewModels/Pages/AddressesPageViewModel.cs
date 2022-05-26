using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PostalServiceApp.Infrastructure;
using PostalServiceApp.Models;
using PostalServiceApp.Services;
using PostalServiceApp.Views;
using PostalServiceClassLibrary.Models;

namespace PostalServiceApp.ViewModels.Pages;

// Модель представления для работы с адресами
public class AddressesPageViewModel : ViewModelBase
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

	// Текущий выбранный участок
	private District _selectedDistrict;
	public District SelectedDistrict
	{
		get => _selectedDistrict;
		set
		{
			Set(ref _selectedDistrict, value);
			UpdateDistrictInfo();
		}
	}

	// Почтальон выбранного участка
	private string _postmanName;
	public string PostmanName
	{
		get => _postmanName;
		set => Set(ref _postmanName, value);
	}

	// Количество адресов с подписками выбранного участка
	private int _subscribedAddressesCount;
	public int SubscribedAddressesCount
	{
		get => _subscribedAddressesCount;
		set => Set(ref _subscribedAddressesCount, value);
	}

	// Количество экземпляров подписных изданий выбранного участка
	private int _districtPublicationsCount;
	public int DistrictPublicationsCount
	{
		get => _districtPublicationsCount;
		set => Set(ref _districtPublicationsCount, value);
	}

	// Коллекция с адресами участка
	private ObservableCollection<Address> _districtAddresses;
	public ObservableCollection<Address> DistrictAddresses
	{
		get => _districtAddresses;
		set => Set(ref _districtAddresses, value);
	}

	// Выбранный адрес
	private Address _selectedAddress;
	public Address SelectedAddress
	{
		get => _selectedAddress;
		set
		{
			Set(ref _selectedAddress, value);
			UpdateAddressSubscribes();
		}
	}

	// Список подписок выбранного адреса
	private ObservableCollection<SubscribeShortInfo> _addressSubscribes;
	public ObservableCollection<SubscribeShortInfo> AddressSubscribes
	{
		get => _addressSubscribes;
		set => Set(ref _addressSubscribes, value);
	}

	// Введенная улица искомого адреса
	private string _streetFilter;
	public string StreetFilter
	{
		get => _streetFilter;
		set => Set(ref _streetFilter, value);
	}

	// Введенный дом искомого адреса
	private string _buildingFilter;
	public string BuildingFilter
	{
		get => _buildingFilter;
		set => Set(ref _buildingFilter, value);
	}

	// Команда смены обслуживающего почтальона
	private RelayCommand _changePostmanCommand;

	public RelayCommand ChangePostmanCommand => _changePostmanCommand ??= new RelayCommand(o =>
	{
		int current = SelectedDistrict.Id;

		InputDataService ids = new();

		if(ids.SelectingPostman(_dataService.Postmans) is { } newPostmanId)
			_dataService.ChangeDistrictPostman(SelectedDistrict.Id, newPostmanId);

		SelectedDistrict = _dataService.Districts.First(d => d.Id == current);


	}, _ => SelectedDistrict != null);


	// Команда найти обслуживаемый адрес
	private RelayCommand _findAddressCommand;
	public RelayCommand FindAddressCommand => _findAddressCommand ??= new RelayCommand((o) =>
	{
		var district = _dataService.Districts.FirstOrDefault(d =>
			d.Addresses.Any(a => a.Street.Name.ToUpperInvariant().Contains(StreetFilter.ToUpperInvariant()) 
			                     && a.Building == BuildingFilter));

		if (district is null)
		{
			// TODO: вынести вывзов MessageBox из модели представления
			MessageBox.Show("Адрес не найден", "Поиск адреса", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			return;
		}

		SelectedDistrict = district;
		SelectedAddress = DistrictAddresses.FirstOrDefault(a => a.Building == BuildingFilter);

	}, (o) => !StreetFilter.IsNullOrEmpty() && !BuildingFilter.IsNullOrEmpty());


	// Команда сброса фильтра
	private RelayCommand _clearFilterCommand;
	public RelayCommand ClearFilterCommand => _clearFilterCommand ??= new RelayCommand(o =>
	{
		switch (o as string)
		{
			case "Street":
				StreetFilter = string.Empty;
				break;
			case "Building":
				BuildingFilter = string.Empty;
				break;
		}
	}, o => (string)o switch
	{
		"Street" => !StreetFilter.IsNullOrEmpty(),
		"Building" => !BuildingFilter.IsNullOrEmpty(),
		_ => false
	});


	// Команда прокрутки до текущего выбранного элемента
	private RelayCommand _scrollToSelectedAddress;
	public RelayCommand ScrollToSelectedAddress =>
		_scrollToSelectedAddress ??= new RelayCommand(o =>
				((DataGrid)o).ScrollIntoView(SelectedAddress),
			_ => SelectedAddress != null);


	// Конструктор
	public AddressesPageViewModel(DataService dataService)
	{
		_dataService = dataService;

		_dataService.OnUpdateDistricts += (sender, args) => SelectedDistrict = null;

		StatusInfo = $"Участков: {_dataService.Districts.Count}";
	}

	// Обновить информацию по выбранному участку
	private void UpdateDistrictInfo()
	{
		if (SelectedDistrict == null)
		{
			PostmanName = string.Empty;
			DistrictAddresses = null;
			DistrictPublicationsCount = 0;
			SubscribedAddressesCount = 0;
			return;
		}

		// Обслуживающий почтальон
		PostmanName = _selectedDistrict?.Postman?.Person?.ShortName;

		DistrictPublicationsCount = _dataService.Subscribes.Count(s => s.Subscriber.Address.IdDistrict == SelectedDistrict.Id);

		SubscribedAddressesCount = _dataService.Subscribes.Where(s => s.Subscriber.Address.IdDistrict == SelectedDistrict.Id)
			.Select(d => new { d.Subscriber.Address, d.Subscriber.SubAddress })
			.Distinct()
			.Count();

		DistrictAddresses = new ObservableCollection<Address>(_selectedDistrict.Addresses);

		SelectedAddress = DistrictAddresses.FirstOrDefault();
	}

	// Обновить список подписок выбранного адреса
	private void UpdateAddressSubscribes()
	{
		if (_selectedAddress == null)
			AddressSubscribes = null;
		else
		{
			AddressSubscribes = new ObservableCollection<SubscribeShortInfo>(_dataService.Subscribes
				.Where(s => s.Subscriber.IdAddress == SelectedAddress.Id)
				.Select(s => new SubscribeShortInfo()
				{
					PubType = s.Publication.PubType.Name,
					PubTitle = s.Publication.Title,
					Apartment = s.Subscriber.SubAddress,
					ShortName = s.Subscriber.Person.ShortName
				}));
		}
	}
}