using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostalServiceApp.ViewModels;
using PostalServiceClassLibrary.Models;
using System.Data.Services;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using PostalServiceApp.Models;
using PostalServiceClassLibrary.DataAccess;

namespace PostalServiceApp.Services;

// Класс, обеспечивающий выгрузку данных из БД в память
// и управление для общей работы с ними моделей представления
public class DataService : INotifyPropertyChanged
{
	// События при выгрузке данных
	public EventHandler OnUpdateSubscribes;
	public EventHandler OnUpdatePublications;
	public EventHandler OnUpdateDistricts;
	public EventHandler OnUpdatePostmans;
	public EventHandler OnUpdateSubscribers;



	// Коллекция объектов подписок	
	private ObservableCollection<Subscribe> _subscribes;
	public ObservableCollection<Subscribe> Subscribes
	{
		get
		{
			if(_subscribes == null)
				LoadSubscribes();
			return _subscribes;
		}
		set
		{
			Set(ref _subscribes, value);
			OnUpdateSubscribes?.Invoke(this, EventArgs.Empty);
		}
	}

	// Коллекция объектов изданий
	private ObservableCollection<Publication> _publications;
	public ObservableCollection<Publication> Publications
	{
		get
		{
			if(_publications == null)
				LoadPublications();
			return _publications;
		}
		set
		{
			Set(ref _publications, value);
			OnUpdatePublications?.Invoke(this,EventArgs.Empty);
		}
	}

	// Cписок участков
	private ObservableCollection<District> _districts;
	public ObservableCollection<District> Districts
	{
		get  
		{
			if (_districts == null)
				LoadDistricts();
			return _districts;
		}
		set
		{
			Set(ref _districts, value);
			OnUpdateDistricts?.Invoke(this, EventArgs.Empty);
		}
	}

	// Коллекция объектов информации о почтальонах
	private ObservableCollection<PostmanInfo> _postmans;
	public ObservableCollection<PostmanInfo> Postmans
	{
		get
		{
			if (_postmans == null)
				LoadPostmans();
			return _postmans;
		}
		set
		{
			Set(ref _postmans, value);
			OnUpdatePostmans?.Invoke(this, EventArgs.Empty);
		}
	}

	// Коллекция объектов подписчиков	
	private ObservableCollection<Subscriber> _subscribers;
	public ObservableCollection<Subscriber> Subscribers
	{
		get
		{
			if (_subscribers == null)
				LoadSubscribers();
			return _subscribers;
		}
		set
		{
			Set(ref _subscribers, value);
			OnUpdateSubscribers?.Invoke(this, EventArgs.Empty);
		}
	}

	#region Выгрузка данных


	private void LoadSubscribes()
	{
		using var repo = new UnitOfWork();
		Subscribes = new ObservableCollection<Subscribe>(repo.SubscribeRepository
			.Get(includeProperties: "Subscriber.Person,Subscriber.Address.Street,Subscriber.Address.District.Postman.Person,Publication.PubType"));
	}

	private void LoadPublications()
	{
		using var repo = new UnitOfWork();
		Publications = new ObservableCollection<Publication>(repo.PublicationRepository
			.Get(includeProperties: "PubType,Subscribes"));
	}

	private void LoadDistricts()
	{
		using var repo = new UnitOfWork();
		Districts = new ObservableCollection<District>(repo.DistrictRepository.Get(includeProperties: "Postman.Person,Addresses.Street").ToList());
	}

	private void LoadPostmans()
	{
		using var repo = new UnitOfWork();
		Postmans = new ObservableCollection<PostmanInfo>(PostmanInfoBuilder.CreateCollection(repo.PostmanRepository
			.Get(includeProperties: "Person"), repo.SubscribeRepository.Get(includeProperties:"Subscriber.Address.District")));
	}
	private void LoadSubscribers()
	{
		using var repo = new UnitOfWork();
		Subscribers = new ObservableCollection<Subscriber>(repo.SubscriberRepository
			.Get(includeProperties: "Person,Address,Address.Street,Subscribes"));
	}
	

	#endregion
	// Добавление подписки
	public void AddSubscribe(Subscribe newSub)
	{
		using var repo = new UnitOfWork();
		
		repo.SubscribeRepository.Insert(newSub);
		repo.Save();

		Subscribes.Add(repo.SubscribeRepository
			.Get(s => s.Id == newSub.Id,includeProperties: "Subscriber.Person,Subscriber.Address.Street,Subscriber.Address.District.Postman.Person,Publication.PubType").First());

		// Обновления связанных коллекций

		if (_districts != null)
			LoadDistricts();

		if (_postmans != null)
			LoadPostmans();

		if(_subscribers != null)
			LoadSubscribers();

		if(_publications != null)
			LoadPublications();
	}

	// Удаление подписки
	public void DeleteSubscribe(int id)
	{
		using var repo = new UnitOfWork();

		repo.SubscribeRepository.Delete(id);
		repo.Save();

		Subscribes.RemoveAt(Subscribes.IndexOf(Subscribes.First(s => s.Id == id)));
			
		// Обновления связанных коллекций

		if(_districts != null)
			LoadDistricts();
			
		if (_postmans != null)
			LoadPostmans();
	}


	// Добавление издания
	public void AddPublication(Publication newPub)
	{
		using var repo = new UnitOfWork();
			
		repo.PublicationRepository.Insert(newPub);
		repo.Save();

		Publications.Add(repo.PublicationRepository.Get(p => p.Id == newPub.Id, includeProperties: "PubType,Subscribes").First());
	}

	// Изменение издания
	public void UpdatePublication(Publication editing)
	{
		using var repo = new UnitOfWork();

		repo.PublicationRepository.Update(editing);
		repo.Save();

		Publications[Publications.IndexOf(Publications.First(p => p.Id == editing.Id))] = 
			repo.PublicationRepository.Get(p => p.Id == editing.Id, includeProperties: "PubType,Subscribes").First();

		// Обновления связанных коллекций

		if (_subscribes != null)
			LoadSubscribes();
	}

	// Удаление издания
	public void DeletePublication(int id)
	{
		using var repo = new UnitOfWork();

		repo.PublicationRepository.Delete(id);
		repo.Save();

		Publications.RemoveAt(Publications.IndexOf(Publications.First(p => p.Id == id)));

		// Обновления связанных коллекций

		if (_subscribes != null)
			LoadSubscribes();
	}

	// Смена почтальона на участке
	public void ChangeDistrictPostman(int districtId, int newPostmanId)
	{
		using var repo = new UnitOfWork();

		repo.DistrictRepository.GetById(districtId).IdPostman = newPostmanId;
		repo.Save();

		Districts[Districts.IndexOf(Districts.First(d => d.Id == districtId))] =
			repo.DistrictRepository.Get(d => d.Id == districtId, includeProperties: "Postman.Person,Addresses.Street").First();

		// Обновления связанных коллекций
		if (_postmans != null) 
			LoadPostmans();

		if(_subscribes != null)
			LoadSubscribes();
	}

	// Добавление почтальона
	public void AddPostman(Postman newPostman)
	{
		using var repo = new UnitOfWork();

		var person = repo.PersonRepository.Get(p =>
			p.Surname == newPostman.Person.Surname &&
			p.Name == newPostman.Person.Name &&
			p.Patronymic == newPostman.Person.Patronymic).FirstOrDefault();

		if (person == null)
		{
			person = repo.PersonRepository.Insert(newPostman.Person);
			repo.Save();
		}

		newPostman.IdPerson = person.Id;


		repo.PostmanRepository.Insert(newPostman);
		repo.Save();

		Postmans.Add(PostmanInfoBuilder.
			CreateOne(repo.PostmanRepository.
					Get(p => p.Id == newPostman.Id, includeProperties:"Person,Districts")
					.First(),
				Subscribes.Where(s => s.Subscriber.Address.District.IdPostman == newPostman.Id)));

		Districts =
			new ObservableCollection<District>(repo.DistrictRepository.Get(includeProperties: "Postman.Person,Addresses.Street").ToList());
	}

	// Удаление почтальона
	public void DeletePostman(int id)
	{
		using var repo = new UnitOfWork();

		repo.DismissPostman(id);

		Postmans.RemoveAt(Postmans.IndexOf(Postmans.First(p => p.Id == id)));

		Districts =
			new ObservableCollection<District>(repo.DistrictRepository.Get(includeProperties: "Postman.Person,Addresses.Street").ToList());
	}


	// Добавление подписчика
	public void AddSubscriber(Subscriber newSubscriber)
	{
		using var repo = new UnitOfWork();

		var person = repo.PersonRepository.Get(p =>
			p.Surname == newSubscriber.Person.Surname &&
			p.Name == newSubscriber.Person.Name &&
			p.Patronymic == newSubscriber.Person.Patronymic).FirstOrDefault();

		if (person == null)
		{
			person = repo.PersonRepository.Insert(newSubscriber.Person);
			repo.Save();
		}

		newSubscriber.IdPerson = person.Id;

		newSubscriber.Address = repo.AddressRepository.Get(a =>
				a.Street.Name == newSubscriber.Address.Street.Name
				&& a.Building == newSubscriber.Address.Building)
			.FirstOrDefault();
		

		repo.SubscriberRepository.Insert(newSubscriber);
		repo.Save();

		Subscribers.Add(repo.SubscriberRepository
			.Get(s => s.Id == newSubscriber.Id, includeProperties: "Person,Address,Address.Street").First());

	}

	// Изменение подписчика
	public void UpdateSubscriber(Subscriber editing)
	{
		using var repo = new UnitOfWork();

		var person = repo.PersonRepository.Get(p =>
			p.Surname == editing.Person.Surname &&
			p.Name == editing.Person.Name &&
			p.Patronymic == editing.Person.Patronymic).FirstOrDefault();

		if (person == null)
		{
			person = repo.PersonRepository.Insert(editing.Person);
			repo.Save();
		}

		editing.Person = person;
		editing.IdPerson = person.Id;

		editing.IdAddress = repo.AddressRepository.Get(a =>
				a.Street.Name == editing.Address.Street.Name
				&& a.Building == editing.Address.Building)
			.FirstOrDefault()!.Id;

		editing.Address = null;

		repo.SubscriberRepository.Update(editing);
		repo.Save();
			
		Subscribers[Subscribers.IndexOf(Subscribers.First(s => s.Id == editing.Id))] =
			repo.SubscriberRepository
				.Get(s => s.Id == editing.Id, includeProperties: "Person,Address,Address.Street").First();

		// Обновления связанных коллекций
		if (_postmans != null)
			LoadPostmans();

		if (_subscribes != null)
			LoadSubscribes();

		if(_districts != null)
			LoadDistricts();
	}

	// Удаление подписчика
	public void DeleteSubscriber(int id)
	{
		using var repo = new UnitOfWork();

		repo.SubscriberRepository.Delete(id);
		repo.Save();

		Subscribers.RemoveAt(Subscribers.IndexOf(Subscribers.First(s => s.Id == id)));
	}


	public static List<PubType> GetPubTypes()
	{
		using var repo = new UnitOfWork();
		return repo.PubTypeRepository.Get().ToList();
	}
	public static List<string> PubTypeList => GetPubTypes().Select(t => t.Name).ToList();

	public static List<Address> GetAddresses()
	{
		using var repo = new UnitOfWork();
		return repo.AddressRepository.Get(includeProperties:"Street").ToList();
	}

	public static List<string> GetStreetNamesList()
	{
		using var repo = new UnitOfWork();
		return repo.StreetRepository.Get().ToList().Select(s => s.Name).ToList();
	}



	#region INPC

	public event PropertyChangedEventHandler PropertyChanged;

	protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
	{
		if (EqualityComparer<T>.Default.Equals(field, value)) return false;
		field = value;
		OnPropertyChanged(propertyName);
		return true;
	}

	#endregion

}