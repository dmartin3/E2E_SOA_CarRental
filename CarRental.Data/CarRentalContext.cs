using System.Data.Entity;
using CarRental.Business.Entities;

namespace CarRental.Data {
  public class CarRentalContext : DbContext {
    public CarRentalContext() : base("name=CarRental") {
      Database.SetInitializer<CarRentalContext>(null);
    }

    public DbSet<Account> AccountSet { get; set; }
    public DbSet<Car> CarSet { get; set; }
    public DbSet<Rental> RentalSet { get; set; }
    public DbSet<Reservation> ReservationSet { get; set; }
  }
}
