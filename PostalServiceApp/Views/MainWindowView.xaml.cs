using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PostalServiceApp.Infrastructure;

namespace PostalServiceApp.Views;

/// <summary>
/// Логика взаимодействия для MainWindowView.xaml
/// </summary>
public partial class MainWindowView : Window
{
	public MainWindowView()
	{
		InitializeComponent();
	}

	protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
	{
		base.OnRenderSizeChanged(sizeInfo);

		if (sizeInfo.HeightChanged)
			Top += (sizeInfo.PreviousSize.Height - sizeInfo.NewSize.Height) / 2;

		if (sizeInfo.WidthChanged)
			Left += (sizeInfo.PreviousSize.Width - sizeInfo.NewSize.Width) / 2;
	}
	
}