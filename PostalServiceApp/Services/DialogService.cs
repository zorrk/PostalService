using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PostalServiceApp.ViewModels;

namespace PostalServiceApp.Services;

public class DialogService : IDialogService
{
	public bool? ShowDialog<T>(ViewModelBase viewModel) where T : Window, new()
	{
		T view = new() { DataContext = viewModel };
		return view.ShowDialog();
	}
}