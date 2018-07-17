using System.Collections.Generic;
using System.ServiceModel;
using CarRental.Business.Entities;
using CarRental.Common;

namespace CarRental.Business.Contracts.Service_Contracts {
  [ServiceContract]
  public interface IRentalService {
    [OperationContract]
    [FaultContract(typeof(AuthorizationValidationException))]
    IEnumerable<Rental> GetRentalHistory(string loginEmail);
  }
}
