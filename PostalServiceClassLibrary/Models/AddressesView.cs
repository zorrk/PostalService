namespace PostalServiceClassLibrary.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

[Table("AddressesView")]
public partial class AddressesView
{
	[Key]
	[Column(Order = 0)]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int Id { get; set; }

	[Key]
	[Column(Order = 1)]
	[StringLength(50)]
	public string Street { get; set; }

	[Key]
	[Column(Order = 2)]
	[StringLength(5)]
	public string Building { get; set; }

	[Key]
	[Column(Order = 3)]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int IdDistrict { get; set; }

	[Key]
	[Column(Order = 4)]
	[StringLength(50)]
	public string DistrictTitle { get; set; }

	[StringLength(66)]
	public string Postman { get; set; }
}