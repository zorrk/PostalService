using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PostalServiceApp.Infrastructure;
using PostalServiceApp.Models;
using PostalServiceApp.Services;
using PostalServiceClassLibrary.Models;

namespace PostalServiceApp.ViewModels.DataForms;

// Модель представления для ввода данных об издании
public class PublicationFormViewModel : ViewModelBase
{

	// Заголовок окна
	private string _title;
	public string Title
	{
		get => _title;
		set => Set(ref _title, value);
	}
		
	// Объект с данными
	private Publication _pub;
	public Publication Pub
	{
		get => _pub;
		set => Set(ref _pub, value);
	}

	// Список типов изданий
	public List<string> Types => DataService.PubTypeList;//GetPubTypes().Select(t => t.Name).ToList();

	// Выбранный тип издания
	private string _selectedType;
	public string SelectedType
	{
		get => _selectedType;
		set
		{
			Set(ref _selectedType, value);
			_pub.IdPubType = DataService.GetPubTypes().FirstOrDefault(t => t.Name == value)!.Id;
		}
	}

	// Команда подтверждающего закрытия диалогового окна
	private RelayCommand _okCommand;
	public RelayCommand OkCommand => _okCommand ??= new RelayCommand(o =>
		((Window)o).DialogResult = true);

	// Конструктор
	public PublicationFormViewModel(Publication pub, string title = "Добавить новое издание")
	{
		_pub = pub;
		SelectedType = string.IsNullOrEmpty(pub?.PubType?.Name) ? Types.First() : pub.PubType.Name;
		Title = title;
	}
}