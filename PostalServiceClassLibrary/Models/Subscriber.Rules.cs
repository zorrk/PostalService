using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostalServiceClassLibrary.Infrastructure;

namespace PostalServiceClassLibrary.Models;

// Правила валидации для модели
public partial class Subscriber : ValidableBase, IDataErrorInfo
{
	public string this[string columnName]
	{
		get
		{
			bool hasError = false;
			switch (columnName)
			{
				case nameof(SubAddress):
					if (SubAddress < 0)
					{
						AddError(nameof(SubAddress), "Недопустимый номер квартиры");
						hasError = true;
					}

					if (!hasError)
						ClearErrors(nameof(SubAddress));

					break;
			}

			return string.Empty;
		}
	}
	public string Error { get; }
}