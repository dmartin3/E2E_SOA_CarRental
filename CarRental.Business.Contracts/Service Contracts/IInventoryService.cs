﻿using System;
using System.ServiceModel;
using CarRental.Business.Entities;
using Core.Common.Exceptions;

namespace CarRental.Business.Contracts.Service_Contracts {
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
  }
}
