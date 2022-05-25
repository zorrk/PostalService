using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostalServiceApp.Models;

// Модель квитанции
public class Receipt
{
	// Номер квитанции
	public string Number { get; set; }

	// Фамилия И.О. клиента
	public string Customer { get; set; }

	// Индекс выписываемого издания
	public string PubIndex { get; set; }

	// Тип выписываемого издания
	public string PubType { get; set; }

	// Название выписываемого издания
	public string PubTitle { get; set; }
		
	// Срок подписки
	public int Duration { get; set; }

	// Общая стоимость подписки
	public int Cost { get; set; }

}