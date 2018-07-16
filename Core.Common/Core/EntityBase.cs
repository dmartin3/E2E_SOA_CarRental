using System.Runtime.Serialization;

namespace Core.Common.Core {
  [DataContract]
  public abstract class EntityBase : IExtensibleDataObject {
    public ExtensionDataObject ExtensionData { get; set; }

  }

}
