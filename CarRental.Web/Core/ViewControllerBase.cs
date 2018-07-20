using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Core.Common.Contracts;

namespace CarRental.Web.Core {
  public class ViewControllerBase : Controller {
    private List<IServiceContract> _disposableServices;

    protected virtual void RegisterServices(List<IServiceContract> disposableServices) {
    }

    List<IServiceContract> DisposableServices => _disposableServices ?? (_disposableServices = new List<IServiceContract>());

    protected override void OnActionExecuting(ActionExecutingContext filterContext) {
      base.OnActionExecuting(filterContext);

      RegisterServices(DisposableServices);
    }

    protected override void OnActionExecuted(ActionExecutedContext filterContext) {
      base.OnActionExecuted(filterContext);

      foreach (var service in DisposableServices) {
        if (service != null && service is IDisposable)
          (service as IDisposable).Dispose();
      }
    }
  }
}
