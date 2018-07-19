using System;
using System.ServiceModel;
using System.Threading.Tasks;
using CarRental.Client.Entities;
using Core.Common.Exceptions;

namespace CarRental.Client.Contracts.Service_Contracts {
  [ServiceContract]
  public interface IInventoryService {
    [OperationContract]
    [FaultContract(typeof(NotFoundException))]
    Car GetCar(int carId);

    [OperationContract]
    Car[] GetAllCars();

    [OperationContract]
    [TransactionFlow(TransactionFlowOption.Allowed)]
    Car UpdateCar(Car car);

    [OperationContract]
    [TransactionFlow(TransactionFlowOption.Allowed)]
    void DeleteCar(int carId);

    [OperationContract]
    Car[] GetAvailableCars(DateTime pickupDate, DateTime returnDate);

    [OperationContract]
    Task<Car> UpdateCarAsync(Car car);

    [OperationContract]
    Task DeleteCarAsync(int carId);

    [OperationContract]
    Task<Car> GetCarAsync(int carId);

    [OperationContract]
    Task<Car[]> GetAllCarsAsync();

    [OperationContract]
    Task<Car[]> GetAvailableCarsAsync(DateTime pickupDate, DateTime returnDate);
  }
}
