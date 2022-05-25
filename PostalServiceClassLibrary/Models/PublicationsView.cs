namespace PostalServiceClassLibrary.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

[Table("PublicationsView")]
public partial class PublicationsView
{
	[Key]
	[Column(Order = 0)]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int Id { get; set; }

	[Key]
	[Column(Order = 1)]
	[StringLength(150)]
	public string PubTitle { get; set; }

	[Key]
	[Column(Order = 2)]
	[StringLength(10)]
	public string PubType { get; set; }

	[Key]
	[Column(Order = 3)]
	[StringLength(15)]
	public string PubIndex { get; set; }

	[Key]
	[Column(Order = 4)]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int Price { get; set; }
}