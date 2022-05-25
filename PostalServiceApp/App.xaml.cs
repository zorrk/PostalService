using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using PostalServiceApp.Authentication;
using PostalServiceApp.Infrastructure;
using PostalServiceApp.ViewModels;
using PostalServiceApp.Views;

namespace PostalServiceApp;

/// <summary>
/// Логика взаимодействия для App.xaml
/// </summary>
public partial class App : Application
{
	protected override void OnStartup(StartupEventArgs e)
	{
		AppDomain.CurrentDomain.SetThreadPrincipal(new CustomPrincipal());

		base.OnStartup(e);

		var mainWindow = new MainWindowView {DataContext = new MainWindowViewModel() };

		mainWindow.Show();
	}

}