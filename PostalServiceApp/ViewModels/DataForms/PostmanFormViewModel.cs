using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PostalServiceApp.Infrastructure;
using PostalServiceApp.Models;
using PostalServiceClassLibrary.Models;

namespace PostalServiceApp.ViewModels.DataForms;

public class PostmanFormViewModel : ViewModelBase
{
	// Заголовок окна
	private string _title;
	public string Title
	{
		get => _title;
		set => Set(ref _title, value);
	}

	// Объект для работы
	private Postman _postman;

	public Postman Postman
	{
		get => _postman;
		set => Set(ref _postman, value);
	}

	// Объект персоны для ввода ФИО
	private Person _person;
	public Person Person { get => _person; set => Set(ref _person, value); }


	// Команда подтверждающего закрытия диалогового окна - сборка объекта для сохранения в базе 
	private RelayCommand _okCommand;
	public RelayCommand OkCommand => _okCommand ??= new RelayCommand(o =>
	{
		_postman.Person = Person;

		((Window)o).DialogResult = true;
	});

	public PostmanFormViewModel(Postman postman, string title = "Добавить данные нового почтальона")
	{
		Title = title;

		Postman = postman;

		// Копия данных ФИО для заполнения формы
		Person = new Person
		{
			Surname = postman.Person?.Surname,
			Name = postman.Person?.Name,
			Patronymic = postman.Person?.Patronymic
		};

	}
}