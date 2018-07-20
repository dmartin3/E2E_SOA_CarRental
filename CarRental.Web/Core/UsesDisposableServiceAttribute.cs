using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace CarRental.Web.Core {
  public class UsesDisposableServiceAttribute : ActionFilterAttribute {
    public override void OnActionExecuting(HttpActionContext actionContext) {
      // pre-processing

      var controller = actionContext.ControllerContext.Controller as IServiceAwareController;
      controller?.RegisterDisposableServices(((IServiceAwareController)controller).DisposableServices);
    }

    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext) {
      //post-processing

      var controller = actionExecutedContext.ActionContext.ControllerContext.Controller as IServiceAwareController;
      if (controller != null) {
        foreach (var service in controller.DisposableServices) {
          if (service != null && service is IDisposable)
            (service as IDisposable).Dispose();
        }
      }
    }
  }
}
