using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostalServiceClassLibrary.Models;

public partial class Postman
{
	// Количество обслуживаемых участков
	public int DistrictsCount => Districts.Count;

	// Количество адресов с доставками
	public int AddressesSubscribed => Districts.Sum(district => district.Addresses
		.SelectMany(districtAddress => districtAddress.Subscribers)
		.Count(districtAddressSubscriber => districtAddressSubscriber.Subscribes
			.Any()));

}