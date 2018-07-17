using System.ComponentModel.Composition.Hosting;
using CarRental.Business.Business_Engines;
using CarRental.Data.Data_Repositories;

namespace CarRental.Business.Bootstrapper {
  public static class MEFLoader {
    public static CompositionContainer Init() {
      var catalog = new AggregateCatalog();

      catalog.Catalogs.Add(new AssemblyCatalog(typeof(AccountRepository).Assembly));
      catalog.Catalogs.Add(new AssemblyCatalog(typeof(CarRentalEngine).Assembly));

      return new CompositionContainer(catalog);
    }
  }
}
