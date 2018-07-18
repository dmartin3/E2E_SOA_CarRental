using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using CarRental.Business.Common;
using CarRental.Business.Entities;
using CarRental.Common;
using CarRental.Data.Contracts;
using CarRental.Data.Contracts.Repository_Interfaces;
using Core.Common.Contracts;
using Core.Common.Exceptions;

namespace CarRental.Business.Business_Engines {
  [Export(typeof(ICarRentalEngine))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class CarRentalEngine : ICarRentalEngine {
    private IDataRepositoryFactory _dataRepositoryFactory;

    [ImportingConstructor]
    public CarRentalEngine(IDataRepositoryFactory dataRepositoryFactory) {
      _dataRepositoryFactory = dataRepositoryFactory;
    }


    public bool IsCarCurrentlyRented(int carId, int accountId) {
      var rentalRepository = _dataRepositoryFactory.GetDataRepository<IRentalRepository>();
      var currentRental = rentalRepository.GetCurrentRentalByCar(carId);

      return currentRental != null && currentRental.AccountId == accountId;
    }

    public bool IsCarCurrentlyRented(int carId) {
      var rentalRepository = _dataRepositoryFactory.GetDataRepository<IRentalRepository>();
      var currentRental = rentalRepository.GetCurrentRentalByCar(carId);

      return currentRental != null;
    }

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

    public Rental RentCarToCustomer(string loginEmail, int carId, DateTime rentalDate, DateTime dateDueBack) {
      if (rentalDate > DateTime.Now) {
        throw new UnableToRentForDateException($"Cannot rent for date {rentalDate.ToShortDateString()} yet.");
      }
        
      var accountRepository = _dataRepositoryFactory.GetDataRepository<IAccountRepository>();
      var rentalRepository = _dataRepositoryFactory.GetDataRepository<IRentalRepository>();

      if (IsCarCurrentlyRented(carId))
        throw new CarCurrentlyRentedException($"Car {carId} is already rented.");

      var account = accountRepository.GetByLogin(loginEmail);
      if (account == null)
        throw new NotFoundException($"No account found for login '{loginEmail}'.");

      var rental = new Rental {
        AccountId = account.AccountId,
        CarId = carId,
        DateRented = rentalDate,
        DateDue = dateDueBack
      };

      var savedEntity = rentalRepository.Add(rental);

      return savedEntity;
    }
  }
}
