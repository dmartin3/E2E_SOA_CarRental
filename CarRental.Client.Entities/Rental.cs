using System;
using Core.Common.Core;

namespace CarRental.Client.Entities {
  public class Rental : ObjectBase {
    private int _rentalId;
    private int _accountId;
    private int _carId;
    private DateTime _dateRented;
    private DateTime _dateDue;
    private DateTime? _dateReturned;

    public int RentalId {
      get { return _rentalId; }
      set {
        if (_rentalId != value) {
          _rentalId = value;
          OnPropertyChanged(() => RentalId);
        }
      }
    }

    public int AccountId {
      get { return _accountId; }
      set {
        if (_accountId != value) {
          _accountId = value;
          OnPropertyChanged(() => AccountId);
        }
      }
    }

    public int CarId {
      get { return _carId; }
      set {
        if (_carId != value) {
          _carId = value;
          OnPropertyChanged(() => CarId);
        }
      }
    }

    public DateTime DateRented {
      get { return _dateRented; }
      set {
        if (_dateRented != value) {
          _dateRented = value;
          OnPropertyChanged(() => DateRented);
        }
      }
    }

    public DateTime DateDue {
      get { return _dateDue; }
      set {
        if (_dateDue != value) {
          _dateDue = value;
          OnPropertyChanged(() => DateDue);
        }
      }
    }

    public DateTime? DateReturned {
      get { return _dateReturned; }
      set {
        if (_dateReturned != value) {
          _dateReturned = value;
          OnPropertyChanged(() => DateReturned);
        }
      }
    }
  }
}
