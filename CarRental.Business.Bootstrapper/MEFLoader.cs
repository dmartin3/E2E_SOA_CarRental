using System.ComponentModel.Composition.Hosting;
using CarRental.Data.Data_Repositories;

namespace CarRental.Business.Bootstrapper {
  public static class MEFLoader {
    public static CompositionContainer Init() {
      var catalog = new AggregateCatalog();
      catalog.Catalogs.Add(new AssemblyCatalog(typeof(AccountRepository).Assembly));
      var container = new CompositionContainer(catalog);

      return container;
    }
  }
}
