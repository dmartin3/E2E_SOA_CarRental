using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using CarRental.Business.Bootstrapper;
using CarRental.Business.Entities;
using CarRental.Data.Contracts;
using Core.Common.Contracts;
using Core.Common.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CarRental.Data.Tests {
  [TestClass]
  public class DataLayerTests {

    [TestInitialize]
    public void Initialize() {
      ObjectBase.Container = MEFLoader.Init();
    }

    [TestMethod]
    public void test_repository_usage() {
      var repositoryTest = new RepositoryTestClass();
      var cars = repositoryTest.GetCars();
      Assert.IsNotNull(cars);
    }

    [TestMethod]
    public void test_repository_factory_usage() {
      var factoryTest = new RepositoryFactoryTestClass();
      var cars = factoryTest.GetCars();
      Assert.IsNotNull(cars);
    }

    [TestMethod]
    public void test_repository_mocking() {
      var cars = new List<Car> {
        new Car {CarId = 1, Description = "Mustang"},
        new Car {CarId = 2, Description = "Corvette"}
      };

      var mockCarRepository = new Mock<ICarRepository>();
      mockCarRepository.Setup(x => x.Get()).Returns(cars);

      var repositoryTest = new RepositoryTestClass(mockCarRepository.Object);
      var result = repositoryTest.GetCars();
      Assert.IsTrue(Equals(result, cars));
    }

    [TestMethod]
    public void test_repository_factory_mocking() {
      var cars = new List<Car> {
        new Car {CarId = 1, Description = "Mustang"},
        new Car {CarId = 2, Description = "Corvette"}
      };

      var mockDataRepository = new Mock<IDataRepositoryFactory>();
      mockDataRepository.Setup(x => x.GetDataRepository<ICarRepository>().Get()).Returns(cars);

      var factory = new RepositoryFactoryTestClass(mockDataRepository.Object);
      var result = factory.GetCars();
      Assert.IsTrue(Equals(result, cars));
    }

    [TestMethod]
    public void test_factory_mocking2() {
      var cars = new List<Car> {
        new Car { CarId = 1, Description = "Mustang" },
        new Car { CarId = 2, Description = "Corvette" }
      };

      var mockCarRepository = new Mock<ICarRepository>();
      mockCarRepository.Setup(obj => obj.Get()).Returns(cars);

      var mockDataRepository = new Mock<IDataRepositoryFactory>();
      mockDataRepository.Setup(obj => obj.GetDataRepository<ICarRepository>()).Returns(mockCarRepository.Object);

      var factoryTest = new RepositoryFactoryTestClass(mockDataRepository.Object);

      var ret = factoryTest.GetCars();

      Assert.IsTrue(ret == cars);
    }
  }

  public class RepositoryTestClass {
    public RepositoryTestClass() {
      ObjectBase.Container.SatisfyImportsOnce(this);
    }

    public RepositoryTestClass(ICarRepository carRepository) {
      _carRepository = carRepository;
    }

    [Import] private ICarRepository _carRepository;

    public IEnumerable<Car> GetCars() {
      return _carRepository.Get();
    }
  }

  public class RepositoryFactoryTestClass {
    public RepositoryFactoryTestClass() {
      ObjectBase.Container.SatisfyImportsOnce(this);
    }

    public RepositoryFactoryTestClass(IDataRepositoryFactory factory) {
      _dataRepositoryFactory = factory;
    }

    [Import] private IDataRepositoryFactory _dataRepositoryFactory;

    public IEnumerable<Car> GetCars() {
      var carRepository = _dataRepositoryFactory.GetDataRepository<ICarRepository>();
      return carRepository.Get();
    }
  }
}
