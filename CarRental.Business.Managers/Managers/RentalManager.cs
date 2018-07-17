using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using CarRental.Business.Common;
using CarRental.Business.Contracts;
using CarRental.Business.Entities;
using CarRental.Common;
using CarRental.Data.Contracts;
using Core.Common.Contracts;
using Core.Common.Exceptions;
using System.Security.Permissions;
using CarRental.Business.Contracts.Service_Contracts;
using CarRental.Data.Contracts.Repository_Interfaces;

namespace CarRental.Business.Managers.Managers {

  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class RentalManager : ManagerBase, IRentalService{
    [Import] private IDataRepositoryFactory _dataRepositoryFactory;
    [Import] private IBusinessEngineFactory _businessEngineFactory;

    public RentalManager() { }
    public RentalManager(IDataRepositoryFactory dataRepositoryFactory) {
      _dataRepositoryFactory = dataRepositoryFactory;
    }

    public RentalManager(IBusinessEngineFactory businessEngineFactory) {
      _businessEngineFactory = businessEngineFactory;
    }

    public RentalManager(IDataRepositoryFactory dataRepositoryFactory, IBusinessEngineFactory businessEngineFactory) {
      _dataRepositoryFactory = dataRepositoryFactory;
      _businessEngineFactory = businessEngineFactory;
    }

    protected override Account LoadAuthorizationValidationAccount(string loginName) {
      var accountRepo = _dataRepositoryFactory.GetDataRepository<IAccountRepository>();
      var authAccount = accountRepo.GetByLogin(loginName);

      if (authAccount == null) {
        throw new NotFoundException($"Cannot find account for login name {loginName} to use for security trimming.");
      }

      return authAccount;
    }

    [PrincipalPermission(SecurityAction.Demand, Role = Security.CarRentalAdminRole)]
    [PrincipalPermission(SecurityAction.Demand, Name = Security.CarRentalUser)]
    public IEnumerable<Rental> GetRentalHistory(string loginEmail) {
      return ExecuteFaultHandledOperation(() => {
        var accountRepo = _dataRepositoryFactory.GetDataRepository<IAccountRepository>();
        var rentalRepo = _dataRepositoryFactory.GetDataRepository<IRentalRepository>();

        var account = accountRepo.GetByLogin(loginEmail);
        if (account == null) {
          var ex = new NotFoundException($"No account found for login '{loginEmail}'");
          throw new FaultException<NotFoundException>(ex, ex.Message);
        }

        ValidateAuthorization(account);
        return rentalRepo.GetRentalHistoryByAccount(account.AccountId);
      });
    }
  }
}
