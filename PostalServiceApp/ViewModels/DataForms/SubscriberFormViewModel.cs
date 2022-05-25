using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PostalServiceApp.Infrastructure;
using PostalServiceApp.Models;
using PostalServiceApp.Services;
using PostalServiceClassLibrary.DataAccess;
using PostalServiceClassLibrary.Models;

namespace PostalServiceApp.ViewModels.DataForms;

// Модель представления для ввода данных о подписчике
public class SubscriberFormViewModel : ViewModelBase
{
	// Заголовок окна
	private string _title;
	public string Title
	{
		get => _title;
		set => Set(ref _title, value);
	}

	// Объект для работы
	private Subscriber _subscriber;

	public Subscriber Subscriber 
	{
		get => _subscriber;
		set => Set(ref _subscriber, value);
	}

	// Объект персоны для ввода ФИО
	private Person _person;
	public Person Person {
		get => _person;
		set => Set(ref _person, value);

	}


	// Список адресов
	private readonly List<Address> _addresses;

	// Список улиц
	public List<string> Streets => _addresses
		.Select(a => a.Street.Name)
		.Distinct()
		.ToList(); 

	// Выбранная улица
	private string _selectedStreet;

	public string SelectedStreet
	{
		get => _selectedStreet;
		set
		{
			Set(ref _selectedStreet, value);

			Buildings = _addresses.Where(a => a.Street.Name == value)
				.Select(a => a.Building)
				.ToList();

			SelectedBuilding = Buildings.First();
		}
	}

	// Список домов улицы
	private List<string> _buildings;
	public List<string> Buildings
	{
		get => _buildings;
		set => Set(ref _buildings, value);
	}

	// Выбранный дом
	private string _selectedBuilding;
	public string SelectedBuilding
	{
		get => _selectedBuilding;
		set => Set(ref _selectedBuilding, value);
	}

	// Команда подтверждающего закрытия диалогового окна - сборка объекта
	private RelayCommand _okCommand;
	public RelayCommand OkCommand => _okCommand ??= new RelayCommand(o =>
	{
		using var repo = new UnitOfWork();


		_subscriber.Person = Person;
		_subscriber.Address.Street.Name = SelectedStreet;
		_subscriber.Address.Building = SelectedBuilding;



		((Window)o).DialogResult = true;
	});

	public SubscriberFormViewModel(Subscriber subscriber, string title = "Добавить нового подписчика")
	{
		Title = title;

		Subscriber = subscriber;

		// Копия данных ФИО для заполнения формы
		Person = new Person
		{
			Surname = subscriber.Person?.Surname,
			Name = subscriber.Person?.Name,
			Patronymic = subscriber.Person?.Patronymic
		};

		_addresses = DataService.GetAddresses();

		// Выставить текущие 
		SelectedStreet = string.IsNullOrEmpty(Subscriber?.Address?.Street?.Name) ? Streets.FirstOrDefault() 
			: Subscriber.Address.Street.Name;
		SelectedBuilding = string.IsNullOrEmpty(Subscriber?.Address?.Building) ? Buildings.FirstOrDefault() 
			: Subscriber.Address.Building;
	}
}