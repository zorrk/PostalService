using System.Linq;

namespace PostalServiceClassLibrary.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

[Table("Persons")]
public partial class Person
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
	public Person()
	{
		Postmans = new HashSet<Postman>();
		Subscribers = new HashSet<Subscriber>();
	}

	public int Id { get; set; }

	[Required]
	[StringLength(60)]
	public string Name { get; set; }

	[Required]
	[StringLength(60)]
	public string Surname { get; set; }

	[Required]
	[StringLength(60)]
	public string Patronymic { get; set; }

	[NotMapped]
	public string ShortName => $"{Surname} {Name.First()}.{Patronymic.First()}.";

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
	public virtual ICollection<Postman> Postmans { get; set; }

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
	public virtual ICollection<Subscriber> Subscribers { get; set; }
}