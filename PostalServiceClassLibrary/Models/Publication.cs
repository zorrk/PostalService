namespace PostalServiceClassLibrary.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

public partial class Publication
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
	public Publication()
	{
		Subscribes = new HashSet<Subscribe>();
	}

	public int Id { get; set; }

	public int IdPubType { get; set; }

	[Required]
	[StringLength(150)]
	public string Title { get; set; }

	[Required]
	[StringLength(15)]
	public string PubIndex { get; set; }

	public int Price { get; set; }

	public virtual PubType PubType { get; set; }

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
	public virtual ICollection<Subscribe> Subscribes { get; set; }
}