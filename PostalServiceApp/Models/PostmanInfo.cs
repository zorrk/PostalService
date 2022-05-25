using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostalServiceClassLibrary.Models;

namespace PostalServiceApp.Models;

// Модель информации о почтальоне
public class PostmanInfo
{
	public int Id { get; set; }
	public string Surname { get; set; }
	public string Name { get; set; }
	public string Patronymic { get; set; }
	public int DistrictsCount { get; set; }
	public int DeliveriesAmount { get; set; }

	public ICollection<District> Districts { get; set; }

	public PostmanInfo() { }
}