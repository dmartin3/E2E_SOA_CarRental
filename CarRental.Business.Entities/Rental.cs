using System;
using System.Runtime.Serialization;
using Core.Common.Contracts;
using Core.Common.Core;

namespace CarRental.Business.Entities {
  [DataContract]
  public class Rental : EntityBase, IIdentifiableEntity, IAccountOwnedEntity {
    [DataMember]
    public int RentalId { get; set; }

    [DataMember]
    public int AccountId { get; set; }

    [DataMember]
    public int CarId { get; set; }

    [DataMember]
    public DateTime DateRented { get; set; }

    [DataMember]
    public DateTime DateDue { get; set; }

    [DataMember]
    public DateTime? DateReturned { get; set; }

    public int EntityId {
      get => RentalId;
      set => RentalId = value;
    }

    int IAccountOwnedEntity.OwnerAccountId => AccountId;
  }
}
