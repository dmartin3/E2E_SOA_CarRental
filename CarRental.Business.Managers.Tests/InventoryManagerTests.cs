using System.Security.Principal;
using System.Threading;
using CarRental.Business.Entities;
using CarRental.Business.Managers.Managers;
using CarRental.Data.Contracts;
using Core.Common.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CarRental.Business.Managers.Tests {
  [TestClass]
  public class InventoryManagerTests {
    [TestInitialize]
    public void Initialize() {
      var principal = new GenericPrincipal(
        new GenericIdentity("Fake"), new[] {"CarRentalAdmin"});

      Thread.CurrentPrincipal = principal;
    }

    [TestMethod]
    public void UpdateCar_add_new() {
      var newCar = new Car();
      var addedCar = new Car { CarId = 1 };

      var mockRepoFactory = new Mock<IDataRepositoryFactory>();
      mockRepoFactory.Setup(x => x.GetDataRepository<ICarRepository>().Add(newCar)).Returns(addedCar);

      var manager = new InventoryManager(mockRepoFactory.Object);
      var results = manager.UpdateCar(newCar);

      Assert.IsTrue(results == addedCar);
    }

    [TestMethod]
    public void UpdateCar_update_existing() {
      var existingCar = new Car { CarId = 1 };
      var updatedCar = new Car { CarId = 1 };

      var mockRepoFactory = new Mock<IDataRepositoryFactory>();
      mockRepoFactory.Setup(x => x.GetDataRepository<ICarRepository>().Update(existingCar)).Returns(updatedCar);

      var manager = new InventoryManager(mockRepoFactory.Object);
      var results = manager.UpdateCar(existingCar);

      Assert.IsTrue(results == updatedCar);
    }
  }
}
