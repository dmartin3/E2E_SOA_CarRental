using System.ServiceModel;
using CarRental.Business.Entities;

namespace CarRental.Business.Contracts.Service_Contracts {
  [ServiceContract]
  public interface IInventoryService {
    [OperationContract]
    Car GetCar(int carId);

    [OperationContract]
    Car[] GetAllCars();
  }
}
