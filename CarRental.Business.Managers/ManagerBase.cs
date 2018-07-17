using System;
using Core.Common.Core;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using CarRental.Business.Entities;
using CarRental.Common;
using Core.Common.Contracts;

namespace CarRental.Business.Managers {
  public class ManagerBase {
    protected string LoginName;
    protected Account AuthorizationAccount = null; 

    public ManagerBase() {
      var context = OperationContext.Current;
      if (context != null) {
        LoginName = context.IncomingMessageHeaders.GetHeader<string>("String", "System");

        if (LoginName.IndexOf(@"\") > 1) {
          LoginName = string.Empty;
        }
      }

      if (ObjectBase.Container != null) {
        ObjectBase.Container.SatisfyImportsOnce(this);
      }

      if (!string.IsNullOrWhiteSpace(LoginName)) {
        AuthorizationAccount = LoadAuthorizationValidationAccount(LoginName);
      }
    }

    protected virtual Account LoadAuthorizationValidationAccount(string loginName) {
      return null;
    }

    protected void ValidateAuthorization(IAccountOwnedEntity entity) {
      if (!Thread.CurrentPrincipal.IsInRole(Security.CarRentalAdminRole)) {
        if (LoginName != string.Empty && entity.OwnerAccountId != AuthorizationAccount.AccountId) {
          var ex = new AuthorizationValidationException("Attempt to access a secure record with improper user authorization validation.");
          throw new FaultException<AuthorizationValidationException>(ex, ex.Message);
        }
      }
    }

    protected T ExecuteFaultHandledOperation<T>(Func<T> codeToExecute) {
      try {
        return codeToExecute.Invoke();
      }
      catch (FaultException ex) {
        throw ex;
      }
      catch (Exception ex) {
        throw new FaultException(ex.Message);
      }
    }

    protected void ExecuteFaultHandledOperation(Action codeToExecute) {
      try {
        codeToExecute.Invoke();
      }
      catch (FaultException ex) {
        throw ex;
      }
      catch (Exception ex) {
        throw new FaultException(ex.Message);
      }
    }
  }
}
