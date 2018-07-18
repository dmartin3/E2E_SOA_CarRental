using CarRental.Business.Business_Engines;
using CarRental.Business.Entities;
using CarRental.Data.Contracts;
using Core.Common.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CarRental.Business.Tests {
  [TestClass]
  public class CarRentalEngineTests {
    [TestMethod]
    public void IsCarCurrentlyRented_any_account() {
      var rental = new Rental {CarId = 1};

      var mockRentalRepo = new Mock<IRentalRepository>();
      mockRentalRepo.Setup(x => x.GetCurrentRentalByCar(1)).Returns(rental);

      var mockRepoFactory = new Mock<IDataRepositoryFactory>();
      mockRepoFactory.Setup(x => x.GetDataRepository<IRentalRepository>()).Returns(mockRentalRepo.Object);

      var engine = new CarRentalEngine(mockRepoFactory.Object);

      var try1 = engine.IsCarCurrentlyRented(2);
      var try2 = engine.IsCarCurrentlyRented(1);

      Assert.IsFalse(try1);
      Assert.IsTrue(try2);
    }
  }
}
