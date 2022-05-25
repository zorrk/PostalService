using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using PostalServiceApp.Models;
using PostalServiceClassLibrary.Models;

namespace PostalServiceApp.Services;

// Создание объектов PostmanInfo
public static class PostmanInfoBuilder
{
	public static IEnumerable<PostmanInfo> CreateCollection(IEnumerable<Postman> postmans, IEnumerable<Subscribe> subscribes) => 
		postmans.Select(p => CreateOne(p, subscribes
			.Where(s => s.Subscriber.Address.District.IdPostman == p.Id)));

	public static PostmanInfo CreateOne(Postman postman, IEnumerable<Subscribe> postmanDeliveries) => 
		new()
		{
			Id = postman.Id,
			Name = postman.Person.Name,
			Surname = postman.Person.Surname,
			Patronymic = postman.Person.Patronymic,
			Districts = new List<District>(postman.Districts), 
			DistrictsCount = postman.DistrictsCount,
			DeliveriesAmount = postmanDeliveries
				.Select(d => new { d.Subscriber.Address, d.Subscriber.SubAddress })
				.Distinct()
				.Count()
		};
		
}