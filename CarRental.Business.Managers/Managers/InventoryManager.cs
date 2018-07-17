using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using CarRental.Business.Common;
using CarRental.Business.Contracts.Service_Contracts;
using CarRental.Business.Entities;
using CarRental.Data.Contracts;
using Core.Common.Contracts;
using Core.Common.Exceptions;

namespace CarRental.Business.Managers.Managers {
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
    ConcurrencyMode = ConcurrencyMode.Multiple, ReleaseServiceInstanceOnTransactionComplete = false)]
  public class InventoryManager : ManagerBase, IInventoryService{
    [Import] private IDataRepositoryFactory _dataRepositoryFactory;
    [Import] private IBusinessEngineFactory _businessEngineFactory;

    public InventoryManager() {}

    public InventoryManager(IDataRepositoryFactory dataRepositoryFactory) {
      _dataRepositoryFactory = dataRepositoryFactory;
    }

    public InventoryManager(IBusinessEngineFactory businessEngineFactory) {
      _businessEngineFactory = businessEngineFactory;
    }

    public InventoryManager(IDataRepositoryFactory dataRepositoryFactory, IBusinessEngineFactory businessEngineFactory) {
      _dataRepositoryFactory = dataRepositoryFactory;
      _businessEngineFactory = businessEngineFactory;
    }

    public Car GetCar(int carId) {
      return ExecuteFaultHandledOperation(() => {
        var carRepo = _dataRepositoryFactory.GetDataRepository<ICarRepository>();
        var car = carRepo.Get(carId);

        if (car == null) {
          var ex = new NotFoundException($"Car with ID of {carId} is not in the database.");
          throw new FaultException<NotFoundException>(ex, ex.Message);
        }

        return car;
      });
    }

    public Car[] GetAllCars() {
      return ExecuteFaultHandledOperation(() => {
        var carRepo = _dataRepositoryFactory.GetDataRepository<ICarRepository>();
        var rentalRepo = _dataRepositoryFactory.GetDataRepository<IRentalRepository>();

        var cars = carRepo.Get().ToList();
        var rentedCars = rentalRepo.GetCurrentlyRentedCars().ToList();

        foreach (var car in cars) {
          var rental = rentedCars.FirstOrDefault(x => x.CarId == car.CarId);
          car.CurrentlyRented = (rental != null);
        }

        return cars.ToArray();
      });
    }

    [OperationBehavior(TransactionScopeRequired = true)]
    public Car UpdateCar(Car car) {
      return ExecuteFaultHandledOperation(() => {
        var carRepo = _dataRepositoryFactory.GetDataRepository<ICarRepository>();

        var updatedEntity = car.CarId == 0 ? carRepo.Add(car) : carRepo.Update(car);

        return updatedEntity;
      });
    }

    [OperationBehavior(TransactionScopeRequired = true)]
    public void DeleteCar(int carId) {
      ExecuteFaultHandledOperation(() => {
        var carRepo = _dataRepositoryFactory.GetDataRepository<ICarRepository>();
        carRepo.Remove(carId);
      });
    }

    public Car[] GetAvailableCars(DateTime pickupDate, DateTime returnDate) {
      return ExecuteFaultHandledOperation(() => {
        var carRepo = _dataRepositoryFactory.GetDataRepository<ICarRepository>();
        var rentalRepo = _dataRepositoryFactory.GetDataRepository<IRentalRepository>();
        var reservationRepo = _dataRepositoryFactory.GetDataRepository<IReservationRepository>();

        var carRentalEngine = _businessEngineFactory.GetBusinessEngine<ICarRentalEngine>();

        var allCars = carRepo.Get();
        var rentedCars = rentalRepo.GetCurrentlyRentedCars().ToList();
        var reservedCars = reservationRepo.Get().ToList();
        var availableCars = new List<Car>();

        foreach (var car in allCars) {
          if (carRentalEngine.IsCarAvailableForRental(car.CarId, pickupDate, returnDate, rentedCars, reservedCars)) {
            availableCars.Add(car);
          }
        }

        return availableCars.ToArray();
      });
    }
  }
}
