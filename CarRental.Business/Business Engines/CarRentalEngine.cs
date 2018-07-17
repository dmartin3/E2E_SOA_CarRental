using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using CarRental.Business.Common;
using CarRental.Business.Entities;

namespace CarRental.Business.Business_Engines {
  [Export(typeof(ICarRentalEngine))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class CarRentalEngine : ICarRentalEngine {
    public bool IsCarAvailableForRental(int carId, DateTime pickupDate, DateTime returnDate,
      IEnumerable<Rental> rentedCars, IEnumerable<Reservation> reservedCars) {

      var reservation = reservedCars.FirstOrDefault(x => x.CarId == carId);
      if (reservation != null && (
        (pickupDate >= reservation.RentalDate && pickupDate <= reservation.ReturnDate) ||
        (returnDate >= reservation.RentalDate && returnDate <= reservation.ReturnDate))) {

        return false;
      }

      var rental = rentedCars.FirstOrDefault(x => x.CarId == carId);
      if (rental != null && pickupDate <= rental.DateDue) {
        return false;
      }

      return true;
    }
  }
}
