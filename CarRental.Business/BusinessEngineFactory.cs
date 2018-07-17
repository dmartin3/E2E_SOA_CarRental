using Core.Common.Contracts;
using Core.Common.Core;

namespace CarRental.Business {
  public class BusinessEngineFactory : IBusinessEngineFactory {

    T IBusinessEngineFactory.GetBusinessEngine<T>() {
      return ObjectBase.Container.GetExportedValue<T>();
    }

  }
}
