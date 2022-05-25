using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PostalServiceApp.Authentication;
using PostalServiceApp.Infrastructure;
using PostalServiceApp.Services;
using PostalServiceApp.ViewModels.Pages;
using PostalServiceApp.Views;

namespace PostalServiceApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
	private readonly DataService _dataService = new();
	private readonly AuthenticationService _authService = new();
	public IPrincipal Principal => Thread.CurrentPrincipal;

	private ViewModelBase _subscribesPageViewModel;
	private ViewModelBase _publicationsPageViewModel;
	private ViewModelBase _addressesPageViewModel;
	private ViewModelBase _postmansPageViewModel;
	private ViewModelBase _subscribersPageViewModel;
	private ViewModelBase _reportPageViewModel;
	private ViewModelBase _authenticationPageViewModel;


	// Текущий контент рабочей области окна
	private ViewModelBase _currentContent;
	public ViewModelBase CurrentContent
	{
		get => _currentContent;
		set => Set(ref _currentContent, value);
	}

	private RelayCommand _loadSubscribesPageCommand;
	public RelayCommand LoadSubscribesPageCommand =>
		_loadSubscribesPageCommand ??= new RelayCommand(_ => CurrentContent = _subscribesPageViewModel);

	private RelayCommand _loadPublicationsPageCommand;
	public RelayCommand LoadPublicationsPageCommand =>
		_loadPublicationsPageCommand ??= new RelayCommand(_ => CurrentContent = _publicationsPageViewModel);
	private RelayCommand _loadAddressesPageCommand;
	public RelayCommand LoadAddressesPageCommand =>
		_loadAddressesPageCommand ??= new RelayCommand(_ => CurrentContent = _addressesPageViewModel);

	private RelayCommand _loadPostmansPageCommand;
	public RelayCommand LoadPostmansPageCommand =>
		_loadPostmansPageCommand ??= new RelayCommand(_ => CurrentContent = _postmansPageViewModel);

	private RelayCommand _loadSubscribersPageCommand;
	public RelayCommand LoadSubscribersPageCommand =>
		_loadSubscribersPageCommand ??= new RelayCommand(_ => CurrentContent = _subscribersPageViewModel);

	private RelayCommand _loadReportPageCommand;
	public RelayCommand LoadReportPageCommand =>
		_loadReportPageCommand ??= new RelayCommand(_ => CurrentContent = _reportPageViewModel);


	private RelayCommand _aboutCommand;

	public RelayCommand AboutCommand => _aboutCommand ??= new RelayCommand(_ => new AboutWindowView().ShowDialog());


	// Команда выход из учетной записи
	private RelayCommand _logOut;
	public RelayCommand LogOutCommand => _logOut ??= new RelayCommand(_ =>
	{
		_authService.LogOut();
	}, _ => _authService.IsAuthenticated);

	public void UpdateAuth()
	{
		OnPropertyChanged(nameof(Principal));
		CurrentContent = _authService.IsAuthenticated ? _subscribesPageViewModel : _authenticationPageViewModel;

		CreateViewModels();
	}

	public MainWindowViewModel()
	{
		_authService.OnAuthenticate += (_, _) => UpdateAuth();
		_authenticationPageViewModel = new AuthenticationPageViewModel(_authService);

		_currentContent = _authenticationPageViewModel;

		CreateViewModels();
	}

	public void CreateViewModels()
	{
		_subscribesPageViewModel = new SubscribesPageViewModel(_dataService);
		_publicationsPageViewModel = new PublicationsPageViewModel(_dataService);
		_addressesPageViewModel = new AddressesPageViewModel(_dataService);
		_postmansPageViewModel = new PostmansPageViewModel(_dataService);
		_subscribersPageViewModel = new SubscribersPageViewModel(_dataService);
		_reportPageViewModel = new ReportPageViewModel(_dataService);
	}


}