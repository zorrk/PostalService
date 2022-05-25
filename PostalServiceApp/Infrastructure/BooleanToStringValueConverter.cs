using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PostalServiceApp.Infrastructure
{
	public class BooleanToStringValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return System.Convert.ToString(value).Equals(System.Convert.ToString(parameter));
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => System.Convert.ToBoolean(value) ? parameter : null;
	}
}
