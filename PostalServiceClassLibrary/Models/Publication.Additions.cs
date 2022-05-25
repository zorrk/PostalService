using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostalServiceClassLibrary.Models;

public partial class Publication
{
	// Количество выписанных экземпляров
	public int SubscribesAmount => Subscribes.Count;

	// Средний срок подписки
	public double? AverageSubscribeDuration => Subscribes.Count != 0 ? Math.Round(Subscribes.Average(s => s.Duration),1) : 0;
}