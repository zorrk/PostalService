namespace PostalServiceClassLibrary.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

public partial class District
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
	public District()
	{
		Addresses = new HashSet<Address>();
	}

	public int Id { get; set; }

	[Required]
	[StringLength(50)]
	public string Title { get; set; }

	public int IdPostman { get; set; }

	public double GeoX { get; set; }
	public double GeoY { get; set; }

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
	public virtual ICollection<Address> Addresses { get; set; }

	public virtual Postman Postman { get; set; }
}