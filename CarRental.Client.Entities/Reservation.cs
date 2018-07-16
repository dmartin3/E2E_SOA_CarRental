using System;
using Core.Common.Core;

namespace CarRental.Client.Entities {
  public class Reservation : ObjectBase {
    private int _reservationId;
    private int _accountId;
    private int _carId;
    private DateTime _returnDate;
    private DateTime _rentalDate;

    public int ReservationId {
      get { return _reservationId; }
      set {
        if (_reservationId != value) {
          _reservationId = value;
          OnPropertyChanged(() => ReservationId);
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

    public DateTime RentalDate {
      get { return _rentalDate; }
      set {
        if (_rentalDate != value) {
          _rentalDate = value;
          OnPropertyChanged(() => RentalDate);
        }
      }
    }

    public DateTime ReturnDate {
      get { return _returnDate; }
      set {
        if (_returnDate != value) {
          _returnDate = value;
          OnPropertyChanged(() => ReturnDate);
        }
      }
    }
  }
}
