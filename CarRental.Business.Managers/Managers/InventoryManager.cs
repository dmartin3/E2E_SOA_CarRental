using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using CarRental.Business.Contracts.Service_Contracts;
using CarRental.Business.Entities;
using CarRental.Data.Contracts;
using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.Exceptions;

namespace CarRental.Business.Managers.Managers {
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
    ConcurrencyMode = ConcurrencyMode.Multiple, ReleaseServiceInstanceOnTransactionComplete = false)]
  public class InventoryManager : IInventoryService{
    [Import]
    private IDataRepositoryFactory _dataRepositoryFactory;

    public InventoryManager() {
      ObjectBase.Container.SatisfyImportsOnce(this);
    }

    public InventoryManager(IDataRepositoryFactory dataRepositoryFactory) {
      _dataRepositoryFactory = dataRepositoryFactory;
    }

    public Car GetCar(int carId) {
      try {
        var carRepo = _dataRepositoryFactory.GetDataRepository<ICarRepository>();
        var car = carRepo.Get(carId);

        if (car == null) {
          var ex = new NotFoundException($"Car with ID of {carId} is not in the database.");
          throw new FaultException<NotFoundException>(ex, ex.Message);
        }

        return car;
      } catch (FaultException ex) {
        throw ex;
      } catch (Exception ex) {
        throw new FaultException(ex.Message);
      }
    }

    public Car[] GetAllCars() {
      try {
        var carRepo = _dataRepositoryFactory.GetDataRepository<ICarRepository>();
        var rentalRepo = _dataRepositoryFactory.GetDataRepository<IRentalRepository>();

        var cars = carRepo.Get().ToList();
        var rentedCars = rentalRepo.GetCurrentlyRentedCars().ToList();

        foreach (var car in cars) {
          var rental = rentedCars.FirstOrDefault(x => x.CarId == car.CarId);
          car.CurrentlyRented = (rental != null);
        }

        return cars.ToArray();
      } catch (Exception ex) {
        throw new FaultException(ex.Message);
      }
    }
  }
}
