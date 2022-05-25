using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using PostalServiceClassLibrary.Models;
using PostalServiceClassLibrary.Models.QueriesResults;

namespace PostalServiceClassLibrary.Context;


// Класс контекста базы данных
public partial class PostalDbContext : DbContext
{
	public PostalDbContext()
		: base("name=PostalDb")
	{
	}

	public virtual DbSet<Address> Addresses { get; set; }
	public virtual DbSet<District> Districts { get; set; }
	public virtual DbSet<Person> Persons { get; set; }
	public virtual DbSet<Postman> Postmans { get; set; }
	public virtual DbSet<Publication> Publications { get; set; }
	public virtual DbSet<PubType> PubTypes { get; set; }
	public virtual DbSet<Street> Streets { get; set; }
	public virtual DbSet<Subscriber> Subscribers { get; set; }
	public virtual DbSet<Subscribe> Subscribes { get; set; }
	public virtual DbSet<AddressesView> AddressesViews { get; set; }
	public virtual DbSet<PublicationsView> PublicationsViews { get; set; }
	public virtual DbSet<SubscribersView> SubscribersViews { get; set; }
	public virtual DbSet<SubscribesView> SubscribesViews { get; set; }


	protected override void OnModelCreating(DbModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Address>()
			.HasMany(e => e.Subscribers)
			.WithRequired(e => e.Address)
			.HasForeignKey(e => e.IdAddress)
			.WillCascadeOnDelete(false);

		modelBuilder.Entity<District>()
			.HasMany(e => e.Addresses)
			.WithRequired(e => e.District)
			.HasForeignKey(e => e.IdDistrict)
			.WillCascadeOnDelete(false);

		modelBuilder.Entity<Person>()
			.HasMany(e => e.Postmans)
			.WithRequired(e => e.Person)
			.HasForeignKey(e => e.IdPerson)
			.WillCascadeOnDelete(false);

		modelBuilder.Entity<Person>()
			.HasMany(e => e.Subscribers)
			.WithRequired(e => e.Person)
			.HasForeignKey(e => e.IdPerson)
			.WillCascadeOnDelete(false);

		modelBuilder.Entity<Postman>()
			.HasMany(e => e.Districts)
			.WithRequired(e => e.Postman)
			.HasForeignKey(e => e.IdPostman)
			.WillCascadeOnDelete(false);

		modelBuilder.Entity<Publication>()
			.HasMany(e => e.Subscribes)
			.WithRequired(e => e.Publication)
			.HasForeignKey(e => e.IdPublication)
			.WillCascadeOnDelete(false);

		modelBuilder.Entity<PubType>()
			.HasMany(e => e.Publications)
			.WithRequired(e => e.PubType)
			.HasForeignKey(e => e.IdPubType)
			.WillCascadeOnDelete(false);

		modelBuilder.Entity<Street>()
			.HasMany(e => e.Addresses)
			.WithRequired(e => e.Street)
			.HasForeignKey(e => e.IdStreet)
			.WillCascadeOnDelete(false);

		modelBuilder.Entity<Subscriber>()
			.HasMany(e => e.Subscribes)
			.WithRequired(e => e.Subscriber)
			.HasForeignKey(e => e.IdSubscriber)
			.WillCascadeOnDelete(false);

	}
}