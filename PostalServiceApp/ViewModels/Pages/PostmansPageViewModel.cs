using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using PostalServiceApp.Infrastructure;
using PostalServiceApp.Models;
using PostalServiceApp.Services;
using PostalServiceApp.ViewModels.DataForms;
using PostalServiceApp.Views.DataForms;
using PostalServiceClassLibrary.Models;

namespace PostalServiceApp.ViewModels.Pages;

// Модель представления для работы с почтальонами
public class PostmansPageViewModel : ViewModelBase
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

	// Коллекция объектов информации о почтальонах
	private ObservableCollection<PostmanInfo> _postmans;
	public ObservableCollection<PostmanInfo> Postmans
	{
		get => _postmans;
		set => Set(ref _postmans, value);
	}

	// Текущий выбранный почтальон
	private PostmanInfo _selectedPostman;
	public PostmanInfo SelectedPostman
	{
		get => _selectedPostman;
		set => Set(ref _selectedPostman, value);
	}

	// Команда внесения данных о новом почтальоне
	private RelayCommand _newPostmanCommand;
	public RelayCommand NewPostmanCommand => _newPostmanCommand ??= new RelayCommand(_ =>
	{
		// Объект добавляемого элемента
		var newPostman = new Postman();

		InputDataService ids = new();

		if (ids.InputNewPostman(newPostman) != true)
			return;

		_dataService.AddPostman(newPostman);

		StatusInfo = $"Почтальон нанят. Почтальонов: {_dataService.Postmans.Count}";

		// Установка добавленной записи как текущего элемента таблицы
		SelectedPostman = DataService.Postmans.FirstOrDefault(p => p.Id == newPostman.Id);
	});

	// Команда удаления почтальона
	private RelayCommand _deletePostmanCommand;
	public RelayCommand DeletePostmanCommand => _deletePostmanCommand ??= 
		new RelayCommand(_ =>
			{
				_dataService?.DeletePostman(SelectedPostman.Id);
				
				StatusInfo = $"Почтальон уволен. Почтальонов: {_dataService.Postmans.Count}";
			},
			_ => SelectedPostman != null);


	public PostmansPageViewModel(DataService dataService)
	{
		_dataService = dataService;

		StatusInfo = $"Почтальонов: {_dataService.Postmans.Count}";
	}
}