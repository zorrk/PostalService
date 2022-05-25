namespace PostalServiceClassLibrary.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

[Table("SubscribersView")]
public partial class SubscribersView
{
	[Key]
	[Column(Order = 0)]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int Id { get; set; }

	[Key]
	[Column(Order = 1)]
	[StringLength(60)]
	public string Surname { get; set; }

	[Key]
	[Column(Order = 2)]
	[StringLength(60)]
	public string Name { get; set; }

	[Key]
	[Column(Order = 3)]
	[StringLength(60)]
	public string Patronymic { get; set; }

	[Key]
	[Column(Order = 4)]
	[StringLength(50)]
	public string Street { get; set; }

	[Key]
	[Column(Order = 5)]
	[StringLength(5)]
	public string Building { get; set; }

	[Key]
	[Column(Order = 6)]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int Apartment { get; set; }

	[Key]
	[Column(Order = 7)]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int IdDistrict { get; set; }

	[Key]
	[Column(Order = 8)]
	[StringLength(50)]
	public string DistrictTitle { get; set; }

	[StringLength(66)]
	public string Postman { get; set; }
}