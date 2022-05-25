using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostalServiceClassLibrary.Infrastructure;

namespace PostalServiceClassLibrary.Models;

public partial class Publication : ValidableBase, IDataErrorInfo
{
	public string this[string columnName]
	{
		get
		{
			bool hasError = false;
			switch (columnName)
			{
				case nameof(Price):
					if (Price < 0)
					{
						AddError(nameof(Price), "Недопустимая цена");
						hasError = true;
					}

					if (!hasError) 
						ClearErrors(nameof(Price));

					break;
				case nameof(PubIndex):
					if (string.IsNullOrWhiteSpace(PubIndex))
					{
						AddError(nameof(PubIndex), "Не может быть пустым");
						hasError = true;
					}

					if (!hasError) 
						ClearErrors(nameof(PubIndex));

					break;
				case nameof(Title):
					if (string.IsNullOrWhiteSpace(Title))
					{
						AddError(nameof(Title), "Не может быть пустым");
						hasError = true;
					}

					if (!hasError) 
						ClearErrors(nameof(Title));

					break;
			}

			return string.Empty;
		}
	}
	public string Error { get; }
}