using System;
using System.Collections.Generic;
using System.Linq;
using PostalServiceClassLibrary.Context;
using PostalServiceClassLibrary.Models;
using PostalServiceClassLibrary.Models.QueriesResults;

namespace PostalServiceClassLibrary.DataAccess;


// Класс, обеспечивающий работу всех репозиториев с общим контекстом
public class UnitOfWork : IDisposable
{
	private PostalDbContext _context = new ();

	private GenericRepository<Address> _addressRepository;
	private GenericRepository<District> _districtRepository;
	private GenericRepository<Person> _personRepository;
	private GenericRepository<Postman> _postmanRepository;
	private GenericRepository<Publication> _publicationRepository;
	private GenericRepository<PubType> _pubTypeRepository;
	private GenericRepository<Street> _streetRepository;
	private GenericRepository<Subscribe> _subscribeRepository;
	private GenericRepository<Subscriber> _subscriberRepository;

	public GenericRepository<Address> AddressRepository =>
		_addressRepository ??= new GenericRepository<Address>(_context);
	public GenericRepository<District> DistrictRepository
		=> _districtRepository ??= new GenericRepository<District>(_context);
	public GenericRepository<Person> PersonRepository =>
		_personRepository ??= new GenericRepository<Person>(_context);
	public GenericRepository<Postman> PostmanRepository =>
		_postmanRepository ??= new GenericRepository<Postman>(_context);
	public GenericRepository<Publication> PublicationRepository =>
		_publicationRepository ??= new GenericRepository<Publication>(_context);
	public GenericRepository<PubType> PubTypeRepository =>
		_pubTypeRepository ??= new GenericRepository<PubType>(_context);
	public GenericRepository<Street> StreetRepository =>
		_streetRepository ??= new GenericRepository<Street>(_context);
	public GenericRepository<Subscribe> SubscribeRepository =>
		_subscribeRepository ??= new GenericRepository<Subscribe>(_context);
	public GenericRepository<Subscriber> SubscriberRepository =>
		_subscriberRepository ??= new GenericRepository<Subscriber>(_context);

	public void Save() => _context.SaveChanges();

		
	// Вызов хранимой процедуры отчёта по участкам
	public List<ReportDistrictResult> ReportDistricts() =>
		_context.Database.SqlQuery<ReportDistrictResult>("ReportDistricts").ToList();

	// Вызов хранимой процедуры отчёта по изданиям
	public List<ReportPublicationResult> ReportPublications() =>
		_context.Database.SqlQuery<ReportPublicationResult>("ReportPublications").ToList();


	// Увольнение почтальона, с переназначением обслуживаемым им участков других сотрудников
	public void DismissPostman(int id)
	{
		// все участки
		var districts = DistrictRepository.Get().ToList();

		// участки, обслуживаемые увольняемым почтальоном
		var servicedDistricts = districts.Where(d => d.IdPostman == id).ToList();

		if (servicedDistricts.Any())
		{
			foreach (var servicedDistrict in servicedDistricts)
			{
				// остальные участки по близости относительно текущего
				var closest = districts.OrderBy(d =>
				{
					var xA = servicedDistrict.GeoX;
					var yA = servicedDistrict.GeoY;
					var xB = d.GeoX;
					var yB = d.GeoY;
					return Math.Sqrt((xA - xB) * (xA - xB) + (yA - yB) * (yA - yB));
				});

				// назначить почтальона с ближайшего участка (обслуживемого другим почтальоном)
				servicedDistrict.IdPostman = closest.First(d => d.IdPostman != id).IdPostman;
			}
		}

		PostmanRepository.Delete(id);
		Save();
	}



	private bool _disposed;
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed && disposing)
			_context.Dispose();

		_disposed = true;
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}