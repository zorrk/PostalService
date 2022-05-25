namespace PostalServiceClassLibrary.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

[Table("SubscribesView")]
public partial class SubscribesView
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

	[Key]
	[Column(Order = 9)]
	[StringLength(10)]
	public string PubType { get; set; }

	[Key]
	[Column(Order = 10)]
	[StringLength(15)]
	public string PubIndex { get; set; }

	[Key]
	[Column(Order = 11)]
	[StringLength(150)]
	public string PubTitle { get; set; }

	[Key]
	[Column(Order = 12, TypeName = "date")]
	public DateTime StartDate { get; set; }

	[Key]
	[Column(Order = 13)]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int Duration { get; set; }

	public int? TotalPrice { get; set; }
}