using System.Windows;
using System.Windows.Controls;

namespace PostalServiceApp.Views.Pages;

/// <summary>
/// Логика взаимодействия для SubscribesPageView.xaml
/// </summary>
public partial class SubscribesPageView : UserControl
{
	public SubscribesPageView()
	{
		InitializeComponent();
	}

	private void TbExpander_OnExpanded(object sender, RoutedEventArgs e)
	{
		ToolbarExt.Visibility = Visibility.Visible;
	}

	private void TbExpander_OnCollapsed(object sender, RoutedEventArgs e)
	{
		ToolbarExt.Visibility = Visibility.Collapsed;

	}

	private void DgSubscribes_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if(sender is DataGrid grid)
		{
			var selected = grid.SelectedItem;
			if (selected != null) 
				grid.ScrollIntoView(selected);
		}
	}
}