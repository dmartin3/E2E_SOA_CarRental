using System.ComponentModel.Composition;
using Core.Common.Contracts;
using Core.Common.Core;

namespace CarRental.Client.Proxies {
  [Export(typeof(IServiceFactory))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class ServiceFactory : IServiceFactory {
    public T CreateClient<T>() where T : IServiceContract {
      return ObjectBase.Container.GetExportedValue<T>();
    }
  }
}
