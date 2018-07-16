using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Runtime.Serialization;
using CarRental.Business.Entities;
using Core.Common.Contracts;

namespace CarRental.Data {
  public class CarRentalContext : DbContext {
    public CarRentalContext() : base("name=CarRental") {
      Database.SetInitializer<CarRentalContext>(null);
    }

    public DbSet<Account> AccountSet { get; set; }
    public DbSet<Car> CarSet { get; set; }
    public DbSet<Rental> RentalSet { get; set; }
    public DbSet<Reservation> ReservationSet { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder) {
      modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
      modelBuilder.Ignore<ExtensionDataObject>();
      modelBuilder.Ignore<IIdentifiableEntity>();

      modelBuilder.Entity<Account>().HasKey(e => e.AccountId).Ignore(e => e.EntityId);
      modelBuilder.Entity<Car>().HasKey(e => e.CarId).Ignore(e => e.EntityId);
      modelBuilder.Entity<Car>().Ignore(e => e.CurrentlyRented);
      modelBuilder.Entity<Rental>().HasKey(e => e.RentalId).Ignore(e => e.EntityId);
      modelBuilder.Entity<Reservation>().HasKey(e => e.ReservationId).Ignore(e => e.EntityId);
    }
  }
}
