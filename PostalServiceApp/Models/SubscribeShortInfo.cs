using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostalServiceApp.Models;

// Краткая информация о подписке
public class SubscribeShortInfo
{
	// Фамилия И.О.
	public string ShortName { get; set; }

	// Квартира
	public int Apartment { get; set; }

	// Тип издания
	public string PubType { get; set; }
		
	// Название издания
	public string PubTitle { get; set; }
}