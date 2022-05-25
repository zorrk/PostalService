namespace PostalServiceClassLibrary.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

public partial class Address
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
	public Address()
	{
		Subscribers = new HashSet<Subscriber>();
	}

	public int Id { get; set; }

	public int IdStreet { get; set; }

	[Required]
	[StringLength(5)]
	public string Building { get; set; }

	public int IdDistrict { get; set; }

	public virtual District District { get; set; }

	public virtual Street Street { get; set; }

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
	public virtual ICollection<Subscriber> Subscribers { get; set; }
}