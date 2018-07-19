using System.ComponentModel.Composition;
using System.Security.Permissions;
using System.ServiceModel;
using CarRental.Business.Contracts.Service_Contracts;
using CarRental.Business.Entities;
using CarRental.Common;
using CarRental.Data.Contracts.Repository_Interfaces;
using Core.Common.Contracts;
using Core.Common.Exceptions;

namespace CarRental.Business.Managers.Managers {
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
                   ConcurrencyMode = ConcurrencyMode.Multiple,
                   ReleaseServiceInstanceOnTransactionComplete = false)]
  public class AccountManager : ManagerBase, IAccountService {
    [Import] private IDataRepositoryFactory _dataRepositoryFactory;

    public AccountManager() {}

    public AccountManager(IDataRepositoryFactory dataRepositoryFactory) {
      _dataRepositoryFactory = dataRepositoryFactory;
    }

    protected override Account LoadAuthorizationValidationAccount(string loginName) {
      var accountRepo = _dataRepositoryFactory.GetDataRepository<IAccountRepository>();
      var authAccount = accountRepo.GetByLogin(loginName);

      if (authAccount == null) {
        var ex = new NotFoundException($"Cannot find account for login name {loginName} to use for security trimming.");
        throw new FaultException<NotFoundException>(ex, ex.Message);
      }

      return authAccount;
    }

    [PrincipalPermission(SecurityAction.Demand, Role = Security.CarRentalAdminRole)]
    [PrincipalPermission(SecurityAction.Demand, Name = Security.CarRentalUser)]
    public Account GetCustomerAccountInfo(string loginEmail) {
      return ExecuteFaultHandledOperation(() => {
        var accountRepository = _dataRepositoryFactory.GetDataRepository<IAccountRepository>();
        var accountEntity = accountRepository.GetByLogin(loginEmail);

        if (accountEntity == null) {
          var ex = new NotFoundException($"Account with login {loginEmail} is not in database");
          throw new FaultException<NotFoundException>(ex, ex.Message);
        }

        ValidateAuthorization(accountEntity);

        return accountEntity;
      });
    }

    [OperationBehavior(TransactionScopeRequired = true)]
    [PrincipalPermission(SecurityAction.Demand, Role = Security.CarRentalAdminRole)]
    [PrincipalPermission(SecurityAction.Demand, Name = Security.CarRentalUser)]
    public void UpdateCustomerAccountInfo(Account account) {
      ExecuteFaultHandledOperation(() => {
        var accountRepository = _dataRepositoryFactory.GetDataRepository<IAccountRepository>();

        ValidateAuthorization(account);

        var updatedAccount = accountRepository.Update(account);
      });
    }
  }
}
