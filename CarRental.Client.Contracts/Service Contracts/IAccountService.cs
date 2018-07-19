using System.ServiceModel;
using System.Threading.Tasks;
using CarRental.Client.Entities;
using CarRental.Common;
using Core.Common.Exceptions;

namespace CarRental.Client.Contracts.Service_Contracts {
  [ServiceContract]
  public interface IAccountService {
    [OperationContract]
    [FaultContract(typeof(NotFoundException))]
    [FaultContract(typeof(AuthorizationValidationException))]
    Account GetCustomerAccountInfo(string loginEmail);

    [OperationContract]
    [FaultContract(typeof(AuthorizationValidationException))]
    [TransactionFlow(TransactionFlowOption.Allowed)]
    void UpdateCustomerAccountInfo(Account account);

    [OperationContract]
    Task<Account> GetCustomerAccountInfoAsync(string loginEmail);

    [OperationContract]
    Task UpdateCustomerAccountInfoAsync(Account account);
  }
}
