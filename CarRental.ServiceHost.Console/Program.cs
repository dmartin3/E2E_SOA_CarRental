using System;
using System.Security.Principal;
using System.Threading;
using System.Timers;
using System.Transactions;
using CarRental.Business.Bootstrapper;
using CarRental.Business.Managers.Managers;
using Core.Common.Core;
using SM = System.ServiceModel;
using Timer = System.Timers.Timer;

namespace CarRental.ServiceHost.Console {
  internal class Program {
    private static void Main() {
      var principal = new GenericPrincipal(
        new GenericIdentity("FakeUser"), new [] {"CarRentalAdmin"});
      Thread.CurrentPrincipal = principal;

      ObjectBase.Container = MEFLoader.Init();

      System.Console.WriteLine("Starting up services...");
      System.Console.WriteLine("");

      var hostInventoryManager = new SM.ServiceHost(typeof(InventoryManager));
      var hostRentalManager = new SM.ServiceHost(typeof(RentalManager));
      var hostAccountManager = new SM.ServiceHost(typeof(AccountManager));
      
      StartService(hostInventoryManager, "InventoryManager");
      StartService(hostRentalManager, "RentalManager");
      StartService(hostAccountManager, "AccountManager");

      var timer = new Timer(10000);
      timer.Elapsed += OnTimerElapsed;
      timer.Start();
      System.Console.WriteLine("Reservation monitor started.");

      System.Console.WriteLine("");
      System.Console.WriteLine("Press [Enter] to exit.");
      System.Console.ReadLine();

      timer.Stop();
      System.Console.WriteLine("Reservation monitor stopped.");

      StopService(hostInventoryManager, "InventoryManager");
      StopService(hostRentalManager, "RentalManager");
      StopService(hostAccountManager, "AccountManager");
    }

    private static void StartService(SM.ServiceHost host, string serviceDescription) {
      host.Open();
      System.Console.WriteLine($"Service {serviceDescription} started.");

      foreach (var endpoint in host.Description.Endpoints) {
        System.Console.WriteLine("Listening on endpoint:");
        System.Console.WriteLine($"Address: {endpoint.Address.Uri}");
        System.Console.WriteLine($"Binding: {endpoint.Binding.Name}");
        System.Console.WriteLine($"Contract: {endpoint.Contract.ConfigurationName}");
      }

      System.Console.WriteLine();
    }

    private static void StopService(SM.ServiceHost host, string serviceDescription) {
      host.Close();
      System.Console.WriteLine($"Service {serviceDescription} stopped.");
    }

    private static void OnTimerElapsed(object sender, ElapsedEventArgs e) {
      System.Console.WriteLine($"Looking for dead reservations at {DateTime.Now}");

      var rentalManager = new RentalManager();
      var reservations = rentalManager.GetDeadReservations();

      foreach (var reservation in reservations) {
        using (var scope = new TransactionScope() ) {
          try {
            rentalManager.CancelReservation(reservation.ReservationId);
            System.Console.WriteLine($"Canceling reservation '{reservation.ReservationId}'.");
            scope.Complete();
          }
          catch (Exception) {
            System.Console.WriteLine($"There was an exception when attempting to cancel reservation '{reservation.ReservationId}'.");
          }
        }
      }
    }
  }
}
