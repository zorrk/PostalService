namespace PostalServiceClassLibrary.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

public partial class Subscriber
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
	public Subscriber()
	{
		Subscribes = new HashSet<Subscribe>();
	}

	public int Id { get; set; }

	public int IdPerson { get; set; }

	public int IdAddress { get; set; }

	public int SubAddress { get; set; }

	public virtual Address Address { get; set; }

	public virtual Person Person { get; set; }

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
	public virtual ICollection<Subscribe> Subscribes { get; set; }

	[NotMapped] public string FullAddress => $"{Address?.Street?.Name}, д.{Address?.Building}, кв.{SubAddress}";

	[NotMapped] public int SubscribesCount => Subscribes.Count;
}