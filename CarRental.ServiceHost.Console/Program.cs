using System;
using CarRental.Business.Managers.Managers;
using SM = System.ServiceModel;

namespace CarRental.ServiceHost {
  class Program {
    static void Main(string[] args) {
      Console.WriteLine("Starting up services...");
      Console.WriteLine("");

      var host = new SM.ServiceHost(typeof(InventoryManager));
      host.Open();

      Console.WriteLine("");
      Console.WriteLine("Press [Enter] to exit.");
      Console.ReadLine();
    }
  }
}
