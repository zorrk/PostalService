using System;
using System.Threading;
using System.Windows.Controls;
using PostalServiceApp.Authentication;
using PostalServiceApp.Infrastructure;

namespace PostalServiceApp.ViewModels.Pages;

public class AuthenticationPageViewModel : ViewModelBase
{
	private readonly IAuthenticationService _authenticationService;

	private string _username;
	public string Username
	{
		get => _username;
		set => Set(ref _username, value);
	}

	private string _status;
	public string Status
	{
		get => _status;
		set => Set(ref _status, value);
	}

	public AuthenticationPageViewModel(IAuthenticationService authenticationService)
	{
		_authenticationService = authenticationService;

		Username = "manager"; 
	}

	private RelayCommand _loginCommand;
	public RelayCommand LoginCommand => _loginCommand ??= new RelayCommand(o =>
	{
		PasswordBox passwordBox = o as PasswordBox;
		string clearTextPassword = passwordBox?.Password;

		if (clearTextPassword.IsNullOrEmpty())  // devmode
			clearTextPassword = "manager";

		try
		{
			_authenticationService.LogIn(Username, clearTextPassword);

			Username = string.Empty; 
			passwordBox.Password = string.Empty; 
			Status = string.Empty;
		}
		catch (UnauthorizedAccessException)
		{
			Status = "Неверное имя пользователя или пароль.";
		}
		catch (Exception ex)
		{
			Status = $"Ошибка: {ex.Message}";
		}

	}, _ => !Username.IsNullOrEmpty());
}