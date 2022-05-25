using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostalServiceClassLibrary.Infrastructure;

namespace PostalServiceClassLibrary.Models;

// Правила валидации для модели
public partial class Person : ValidableBase, IDataErrorInfo
{
	public string this[string columnName]
	{
		get
		{
			bool hasError = false;
			switch (columnName)
			{
				case nameof(Surname):
					if (string.IsNullOrWhiteSpace(Surname))
					{
						AddError(nameof(Surname), "Не может быть пустым");
						hasError = true;
					}else if (Surname.Any(c => !char.IsLetter(c)))
					{
						AddError(nameof(Surname), "Может состоять только из букв");
						hasError = true;
					}

					if (!hasError)
						ClearErrors(nameof(Surname));

					break;
				case nameof(Name):
					if (string.IsNullOrWhiteSpace(Name))
					{
						AddError(nameof(Name), "Не может быть пустым");
						hasError = true;
					}else if (Name.Any(c => !char.IsLetter(c)))
					{
						AddError(nameof(Name), "Может состоять только из букв");
						hasError = true;
					}

					if (!hasError)
						ClearErrors(nameof(Name));

					break;

				case nameof(Patronymic):
					if (string.IsNullOrWhiteSpace(Patronymic))
					{
						AddError(nameof(Patronymic), "Не может быть пустым");
						hasError = true;
					}else if (Patronymic.Any(c => !char.IsLetter(c)))
					{
						AddError(nameof(Patronymic), "Может состоять только из букв");
						hasError = true;
					}

					if (!hasError)
						ClearErrors(nameof(Patronymic));

					break;
			}

			return string.Empty;
		}
	}
	public string Error { get; }
}