using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PostalServiceApp.Infrastructure;
using PostalServiceApp.Models;
using PostalServiceApp.Services;
using PostalServiceClassLibrary.Models;

namespace PostalServiceApp.ViewModels;

public class SelectPostmanViewModel : ViewModelBase
{
	private IEnumerable<PostmanInfo> _postmans;
	public IEnumerable<PostmanInfo> Postmans
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

	// Команда подтверждающего закрытия диалогового окна 
	private RelayCommand _okCommand;
	public RelayCommand OkCommand => _okCommand ??= new RelayCommand(o =>
	{
		((Window)o).DialogResult = true;
	}, _ => SelectedPostman != null);


	public SelectPostmanViewModel(IEnumerable<PostmanInfo> postmans)
	{
		Postmans = postmans;
	}

}