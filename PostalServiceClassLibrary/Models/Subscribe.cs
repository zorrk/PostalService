namespace PostalServiceClassLibrary.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

public partial class Subscribe
{
	public int Id { get; set; }

	public int IdSubscriber { get; set; }

	public int IdPublication { get; set; }

	[Column(TypeName = "date")]
	public DateTime StartDate { get; set; }

	public int Duration { get; set; }

	public virtual Publication Publication { get; set; }

	public virtual Subscriber Subscriber { get; set; }

	[NotMapped] public int? TotalCost => Publication?.Price * Duration;
}