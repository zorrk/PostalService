using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostalServiceClassLibrary.Infrastructure;

namespace PostalServiceClassLibrary.Models;

// Правила валидации для модели
public partial class Address : ValidableBase, IDataErrorInfo
{
	public string this[string columnName]
	{
		get
		{
			bool hasError = false;
			switch (columnName)
			{
				case nameof(Building):
					if (string.IsNullOrWhiteSpace(Building))
					{
						AddError(nameof(Building), "Не может быть пустым");
						hasError = true;
					}

					if (!hasError)
						ClearErrors(nameof(Building));

					break;
			}

			return string.Empty;
		}
	}
	public string Error { get; }
}