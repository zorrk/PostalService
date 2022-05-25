using System.Linq;
using PostalServiceApp.Infrastructure;
using PostalServiceApp.Services;
using PostalServiceApp.Views;

namespace PostalServiceApp.ViewModels.Pages;

public class ReportPageViewModel : ViewModelBase
{
	// Ссылка на сервис данных
	private readonly DataService _dataService;
	public DataService DataService => _dataService;


	private string _subscribersAmount;
	public string SubscribersAmount
	{
		get => _subscribersAmount;
		set => Set(ref _subscribersAmount , value);
	}

	private string _subscribesAmount;
	public string SubscribesAmount
	{
		get => _subscribesAmount;
		set => Set(ref _subscribesAmount, value);
	}

	private string _magazineSubscriptions;
	public string MagazineSubscriptions
	{
		get => _magazineSubscriptions;
		set => Set(ref _magazineSubscriptions, value);
	}

	private string _newspaperSubscriptions;
	public string NewspaperSubscriptions
	{
		get => _newspaperSubscriptions;
		set => Set(ref _newspaperSubscriptions, value);
	}


	private string _publicationsAmount;
	public string PublicationsAmount
	{
		get => _publicationsAmount;
		set => Set(ref _publicationsAmount, value);
	}

	private string _magazinesAmount;
	public string MagazinesAmount
	{
		get => _magazinesAmount;
		set => Set(ref _magazinesAmount, value);
	}

	private string _newspapersAmount;
	public string NewspapersAmount
	{
		get => _newspapersAmount;
		set => Set(ref _newspapersAmount, value);
	}


	private string _districtsAmount;
	public string DistrictsAmount
	{
		get => _districtsAmount;
		set => Set(ref _districtsAmount, value);
	}

	private string _addressesSubscribed;
	public string AddressesSubscribed
	{
		get => _addressesSubscribed;
		set => Set(ref _addressesSubscribed, value);
	}


	private string _postmansAmount;
	public string PostmansAmount
	{
		get => _postmansAmount;
		set => Set(ref _postmansAmount, value);
	}



	private RelayCommand _buildReportCommand;

	public RelayCommand BuildReportCommand => _buildReportCommand ??= new RelayCommand(_ =>
	{
		var vm = new DocumentViewModel(DocumentBuilder.CreateReportDoc());
		var wnd = new ReportDocView { DataContext = vm };

		wnd.ShowDialog();
	});

	public ReportPageViewModel(DataService dataService)
	{
		_dataService = dataService;
		_dataService.OnUpdateDistricts += (_, _) => Update();
		_dataService.OnUpdatePostmans += (_, _) => Update();
		_dataService.OnUpdatePublications += (_, _) => Update();
		_dataService.OnUpdateSubscribers += (_, _) => Update();
		_dataService.OnUpdateSubscribes += (_, _) => Update();

		Update();
	}

	private void Update()
	{
		SubscribersAmount = _dataService.Subscribers.Count(s => s.SubscribesCount > 0).ToString();

		SubscribesAmount = _dataService.Subscribes.Count.ToString();

		MagazineSubscriptions = _dataService.Subscribes.Count(s => s.Publication.PubType.Name == "Журнал").ToString();

		NewspaperSubscriptions = _dataService.Subscribes.Count(s => s.Publication.PubType.Name == "Газета").ToString();

		PublicationsAmount = _dataService.Publications.Count().ToString();

		NewspapersAmount = _dataService.Publications.Count(p => p.PubType.Name == "Газета").ToString();
		
		MagazinesAmount = _dataService.Publications.Count(p => p.PubType.Name == "Журнал").ToString();

		DistrictsAmount = _dataService.Districts.Count().ToString();

		PostmansAmount = _dataService.Postmans.Count().ToString();

		AddressesSubscribed = _dataService.Subscribes.Select(s => new {s.Subscriber.SubAddress, s.Subscriber.Address})
			.Distinct().Count().ToString();

	}
}