using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using PostalServiceApp.Infrastructure;
using PostalServiceApp.Models;
using PostalServiceApp.Services;
using PostalServiceClassLibrary.Models;

namespace PostalServiceApp.ViewModels.DataForms;

// Модель представления для формы данных оформления новой подписки
public class SubscribeFormViewModel : ViewModelBase
{
	// Объект новой подписки
	private Subscribe _subscribe;

	// Ссылка на сервис данных
	private DataService _dataService;

	// Заголовок окна
	private string _title;
	public string Title
	{
		get => _title;
		set => Set(ref _title, value);
	}

	// Строка для поиска подписчика
	private string _searchSubscriber;
	public string SearchSubscriber
	{
		get => _searchSubscriber;
		set
		{
			Set(ref _searchSubscriber, value);
			// При изменении фильтра - обновить отображение для коллекции
			CollectionViewSource.GetDefaultView(SubscribersList).Refresh();
		}
	}

	// Строка для поиска издания
	private string _searchPublication;
	public string SearchPublication
	{
		get => _searchPublication;
		set
		{
			Set(ref _searchPublication, value);
			// При изменении фильтра - обновить отображение для коллекции
			CollectionViewSource.GetDefaultView(PublicationsList).Refresh();
		}
	}

	// Объект выбранного подписчика
	private Subscriber _selectedSubscriber;
	public Subscriber SelectedSubscriber
	{
		get => _selectedSubscriber;
		set => Set(ref _selectedSubscriber, value);
	}

	// Объект выбранного издания
	private Publication _selectedPublication;
	public Publication SelectedPublication
	{
		get => _selectedPublication;
		set
		{
			Set(ref _selectedPublication, value);
			if (!string.IsNullOrEmpty(SelectedDuration) && SelectedPublication != null)
				TotalPrice = int.Parse(SelectedDuration) * SelectedPublication.Price;
			else TotalPrice = 0;
		}
	}

	// Список подписчиков
	private List<Subscriber> _subscribersList;
	public List<Subscriber> SubscribersList
	{
		get => _subscribersList;
		set => Set(ref _subscribersList, value);
	}

	// Отображение для списка подписчиков
	private ICollectionView _subscribersView;

	public ICollectionView SubscribersView
	{
		get=> _subscribersView;
		set => Set(ref _subscribersView, value);
	}

	// Список изданий
	private List<Publication> _publicationsList;

	public List<Publication> PublicationsList
	{
		get => _publicationsList;
		set => Set(ref _publicationsList, value);
	}

	// Отображение для списка изданий
	private ICollectionView _publicationsView;
	public ICollectionView PublicationsView
	{
		get => _publicationsView;
		set => Set(ref _publicationsView, value);
	}

	// Варианты сроков подписки
	public List<string> DurationsList => Constants.DurationsList;

	// Выбранный срок подписки
	private string _selectedDuration;
	public string SelectedDuration
	{
		get => _selectedDuration;
		set
		{
			Set(ref _selectedDuration, value);
			TotalPrice = int.Parse(SelectedDuration) * SelectedPublication.Price;
		}
	}

	// Итоговая стоимость подписки
	private int _totalPrice;
	public int TotalPrice
	{
		get => _totalPrice;
		set => Set(ref _totalPrice, value);
	}

	// Команда подтверждающего закрытия диалогового окна, сборка объекта подписки
	private RelayCommand _okCommand;
	public RelayCommand OkCommand => _okCommand ??= new RelayCommand(o =>
	{
		_subscribe.IdSubscriber = _dataService.Subscribers.First(s =>
			s.Person.Surname == SelectedSubscriber.Person.Surname &&
			s.Person.Name == SelectedSubscriber.Person.Name &&
			s.Person.Patronymic == SelectedSubscriber.Person.Patronymic).Id;

		_subscribe.IdPublication =
			_dataService.Publications.First(p => p.PubIndex == SelectedPublication.PubIndex).Id;

		_subscribe.Duration = int.Parse(SelectedDuration);

		_subscribe.StartDate = DateTime.Now;

		((Window) o).DialogResult = true;
	}, (o) => 
		SelectedPublication is not null && 
		SelectedSubscriber is not null &&
		!SelectedSubscriber.Person.Surname.IsNullOrEmpty() &&
		!SelectedPublication.Title.IsNullOrEmpty());
		
	// Конструктор
	public SubscribeFormViewModel(Subscribe subscribe, DataService dataService)
	{
		Title = "Оформление подписки";

		_subscribe = subscribe;

		_dataService = dataService;

		_publicationsList = _dataService.Publications.ToList();
		_subscribersList = _dataService.Subscribers.ToList();

		SelectedPublication = new Publication();
		SelectedSubscriber = new Subscriber { Person = new Person() };

		PublicationsView = CollectionViewSource.GetDefaultView(PublicationsList); 
		PublicationsView.Filter = OnPubsFilterTriggered;
			
		SubscribersView = CollectionViewSource.GetDefaultView(SubscribersList);
		SubscribersView.Filter = OnSubersFilterTriggered;

		SelectedDuration = DurationsList.First();
	}

	private bool OnSubersFilterTriggered(object item)
	{
		if (item is not Subscriber subscriber)
			return false;

		if (!string.IsNullOrEmpty(SearchSubscriber))
		{
			var snp = SearchSubscriber.ToUpperInvariant().Split(new[] { ' ' }, StringSplitOptions.None);

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

	private bool OnPubsFilterTriggered(object item) =>
		item is Publication pub && (string.IsNullOrEmpty(SearchPublication) || pub.Title.StartsWith(
			SearchPublication, true, CultureInfo.InvariantCulture));
}