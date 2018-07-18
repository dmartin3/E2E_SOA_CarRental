using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using CarRental.Business.Common;
using CarRental.Business.Contracts;
using CarRental.Business.Entities;
using CarRental.Common;
using CarRental.Data.Contracts;
using Core.Common.Contracts;
using Core.Common.Exceptions;
using System.Security.Permissions;
using CarRental.Business.Contracts.Service_Contracts;
using CarRental.Data.Contracts.Repository_Interfaces;

namespace CarRental.Business.Managers.Managers {

  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
                  ConcurrencyMode = ConcurrencyMode.Multiple,
                  ReleaseServiceInstanceOnTransactionComplete = false)]
  public class RentalManager : ManagerBase, IRentalService {
    [Import] private IDataRepositoryFactory _dataRepositoryFactory;
    [Import] private IBusinessEngineFactory _businessEngineFactory;

    public RentalManager() { }

    public RentalManager(IDataRepositoryFactory dataRepositoryFactory) {
      _dataRepositoryFactory = dataRepositoryFactory;
    }

    public RentalManager(IBusinessEngineFactory businessEngineFactory) {
      _businessEngineFactory = businessEngineFactory;
    }

    public RentalManager(IDataRepositoryFactory dataRepositoryFactory, IBusinessEngineFactory businessEngineFactory) {
      _dataRepositoryFactory = dataRepositoryFactory;
      _businessEngineFactory = businessEngineFactory;
    }

    protected override Account LoadAuthorizationValidationAccount(string loginName) {
      var accountRepo = _dataRepositoryFactory.GetDataRepository<IAccountRepository>();
      var authAccount = accountRepo.GetByLogin(loginName);

      if (authAccount == null) {
        var ex = new NotFoundException($"Cannot find account for login name {loginName} to use for security trimming.");
        throw new FaultException<NotFoundException>(ex, ex.Message);
      }

      return authAccount;
    }

    [OperationBehavior(TransactionScopeRequired = true)]
    [PrincipalPermission(SecurityAction.Demand, Role = Security.CarRentalAdminRole)]
    public Rental RentCarToCustomer(string loginEmail, int carId, DateTime dateDueBack) {
      return ExecuteFaultHandledOperation(() => {
        var carRentalEngine = _businessEngineFactory.GetBusinessEngine<ICarRentalEngine>();

        try {
          var rental = carRentalEngine.RentCarToCustomer(loginEmail, carId, DateTime.Now, dateDueBack);
          return rental;
        }
        catch (UnableToRentForDateException ex) {
          throw new FaultException<UnableToRentForDateException>(ex, ex.Message);
        }
        catch (CarCurrentlyRentedException ex) {
          throw new FaultException<CarCurrentlyRentedException>(ex, ex.Message);
        }
        catch (NotFoundException ex) {
          throw new FaultException<NotFoundException>(ex, ex.Message);
        }
      });
    }

    [OperationBehavior(TransactionScopeRequired = true)]
    [PrincipalPermission(SecurityAction.Demand, Role = Security.CarRentalAdminRole)]
    public Rental RentCarToCustomer(string loginEmail, int carId, DateTime rentalDate, DateTime dateDueBack) {
      return ExecuteFaultHandledOperation(() => {
        var carRentalEngine = _businessEngineFactory.GetBusinessEngine<ICarRentalEngine>();

        try {
          var rental = carRentalEngine.RentCarToCustomer(loginEmail, carId, rentalDate, dateDueBack);
          return rental;
        }
        catch (UnableToRentForDateException ex) {
          throw new FaultException<UnableToRentForDateException>(ex, ex.Message);
        }
        catch (CarCurrentlyRentedException ex) {
          throw new FaultException<CarCurrentlyRentedException>(ex, ex.Message);
        }
        catch (NotFoundException ex) {
          throw new FaultException<NotFoundException>(ex, ex.Message);
        }
      });
    }

    [OperationBehavior(TransactionScopeRequired = true)]
    [PrincipalPermission(SecurityAction.Demand, Role = Security.CarRentalAdminRole)]
    public void AcceptCarReturn(int carId) {
      ExecuteFaultHandledOperation(() => {
        var rentalRepository = _dataRepositoryFactory.GetDataRepository<IRentalRepository>();
        var rental = rentalRepository.GetCurrentRentalByCar(carId);

        if (rental == null) {
          var ex = new CarNotRentedException($"Car {carId} is not currently rented.");
          throw new FaultException<CarNotRentedException>(ex, ex.Message);
        }

        rental.DateReturned = DateTime.Now;
        rentalRepository.Update(rental);
      });
    }

    [PrincipalPermission(SecurityAction.Demand, Role = Security.CarRentalAdminRole)]
    [PrincipalPermission(SecurityAction.Demand, Name = Security.CarRentalUser)]
    public IEnumerable<Rental> GetRentalHistory(string loginEmail) {
      return ExecuteFaultHandledOperation(() => {
        var accountRepo = _dataRepositoryFactory.GetDataRepository<IAccountRepository>();
        var rentalRepo = _dataRepositoryFactory.GetDataRepository<IRentalRepository>();

        var account = accountRepo.GetByLogin(loginEmail);
        if (account == null) {
          var ex = new NotFoundException($"No account found for login '{loginEmail}'");
          throw new FaultException<NotFoundException>(ex, ex.Message);
        }

        ValidateAuthorization(account);
        return rentalRepo.GetRentalHistoryByAccount(account.AccountId);
      });
    }

    [PrincipalPermission(SecurityAction.Demand, Role = Security.CarRentalAdminRole)]
    [PrincipalPermission(SecurityAction.Demand, Name = Security.CarRentalUser)]
    public Reservation GetReservation(int reservationId) {
      return ExecuteFaultHandledOperation(() => {
        var reservationRepository = _dataRepositoryFactory.GetDataRepository<IReservationRepository>();

        var reservation = reservationRepository.Get(reservationId);
        if (reservation == null) {
          var ex = new NotFoundException($"No reservation found for id '{reservationId}'.");
          throw new FaultException<NotFoundException>(ex, ex.Message);
        }

        ValidateAuthorization(reservation);

        return reservation;
      });
    }

    [OperationBehavior(TransactionScopeRequired = true)]
    [PrincipalPermission(SecurityAction.Demand, Role = Security.CarRentalAdminRole)]
    [PrincipalPermission(SecurityAction.Demand, Name = Security.CarRentalUser)]
    public Reservation MakeReservation(string loginEmail, int carId, DateTime rentalDate, DateTime returnDate) {
      return ExecuteFaultHandledOperation(() => {
        var accountRepository = _dataRepositoryFactory.GetDataRepository<IAccountRepository>();
        var reservationRepository = _dataRepositoryFactory.GetDataRepository<IReservationRepository>();

        var account = accountRepository.GetByLogin(loginEmail);
        if (account == null) {
          var ex = new NotFoundException($"No account found for login '{loginEmail}'.");
          throw new FaultException<NotFoundException>(ex, ex.Message);
        }

        ValidateAuthorization(account);

        var reservation = new Reservation {
          AccountId = account.AccountId,
          CarId = carId,
          RentalDate = rentalDate,
          ReturnDate = returnDate
        };

        var savedEntity = reservationRepository.Add(reservation);

        return savedEntity;
      });
    }

    [OperationBehavior(TransactionScopeRequired = true)]
    [PrincipalPermission(SecurityAction.Demand, Role = Security.CarRentalAdminRole)]
    public void ExecuteRentalFromReservation(int reservationId) {
      ExecuteFaultHandledOperation(() => {
        var accountRepository = _dataRepositoryFactory.GetDataRepository<IAccountRepository>();
        var reservationRepository = _dataRepositoryFactory.GetDataRepository<IReservationRepository>();
        var carRentalEngine = _businessEngineFactory.GetBusinessEngine<ICarRentalEngine>();

        var reservation = reservationRepository.Get(reservationId);
        if (reservation == null) {
          NotFoundException ex = new NotFoundException($"Reservation {reservationId} is not found.");
          throw new FaultException<NotFoundException>(ex, ex.Message);
        }

        var account = accountRepository.Get(reservation.AccountId);
        if (account == null) {
          NotFoundException ex = new NotFoundException($"No account found for account ID '{reservation.AccountId}'.");
          throw new FaultException<NotFoundException>(ex, ex.Message);
        }

        try {
          var rental = carRentalEngine.RentCarToCustomer(account.LoginEmail, reservation.CarId, reservation.RentalDate, reservation.ReturnDate);
        }
        catch (UnableToRentForDateException ex) {
          throw new FaultException<UnableToRentForDateException>(ex, ex.Message);
        }
        catch (CarCurrentlyRentedException ex) {
          throw new FaultException<CarCurrentlyRentedException>(ex, ex.Message);
        }
        catch (NotFoundException ex) {
          throw new FaultException<NotFoundException>(ex, ex.Message);
        }

        reservationRepository.Remove(reservation);
      });
    }

    [OperationBehavior(TransactionScopeRequired = true)]
    [PrincipalPermission(SecurityAction.Demand, Role = Security.CarRentalAdminRole)]
    [PrincipalPermission(SecurityAction.Demand, Name = Security.CarRentalUser)]
    public void CancelReservation(int reservationId) {
      ExecuteFaultHandledOperation(() => {
        var reservationRepository = _dataRepositoryFactory.GetDataRepository<IReservationRepository>();
        var reservation = reservationRepository.Get(reservationId);

        if (reservation == null) {
          var ex = new NotFoundException($"No reservation found for ID '{reservationId}'.");
          throw new FaultException<NotFoundException>(ex, ex.Message);
        }

        ValidateAuthorization(reservation);

        reservationRepository.Remove(reservationId);
      });
    }

    [PrincipalPermission(SecurityAction.Demand, Role = Security.CarRentalAdminRole)]
    public CustomerReservationData[] GetCurrentReservations() {
      return ExecuteFaultHandledOperation(() => {
        var reservationRepository = _dataRepositoryFactory.GetDataRepository<IReservationRepository>();
        var reservationData = new List<CustomerReservationData>();

        var reservationInfoSet = reservationRepository.GetCurrentCustomerReservationInfo();
        foreach (var reservationInfo in reservationInfoSet) {
          reservationData.Add(new CustomerReservationData() {
            ReservationId = reservationInfo.Reservation.ReservationId,
            Car = reservationInfo.Car.Color + " " + reservationInfo.Car.Year + " " + reservationInfo.Car.Description,
            CustomerName = reservationInfo.Customer.FirstName + " " + reservationInfo.Customer.LastName,
            RentalDate = reservationInfo.Reservation.RentalDate,
            ReturnDate = reservationInfo.Reservation.ReturnDate
          });
        }

        return reservationData.ToArray();
      });
    }

    [PrincipalPermission(SecurityAction.Demand, Role = Security.CarRentalAdminRole)]
    [PrincipalPermission(SecurityAction.Demand, Name = Security.CarRentalUser)]
    public CustomerReservationData[] GetCustomerReservations(string loginEmail) {
      return ExecuteFaultHandledOperation(() => {
        var accountRepository = _dataRepositoryFactory.GetDataRepository<IAccountRepository>();
        var reservationRepository = _dataRepositoryFactory.GetDataRepository<IReservationRepository>();
        var account = accountRepository.GetByLogin(loginEmail);

        if (account == null) {
          var ex = new NotFoundException($"No account found for login '{loginEmail}'.");
          throw new FaultException<NotFoundException>(ex, ex.Message);
        }

        ValidateAuthorization(account);

        var reservationData = new List<CustomerReservationData>();

        var reservationInfoSet = reservationRepository.GetCustomerOpenReservationInfo(account.AccountId);
        foreach (var reservationInfo in reservationInfoSet) {
          reservationData.Add(new CustomerReservationData() {
            ReservationId = reservationInfo.Reservation.ReservationId,
            Car = reservationInfo.Car.Color + " " + reservationInfo.Car.Year + " " + reservationInfo.Car.Description,
            CustomerName = reservationInfo.Customer.FirstName + " " + reservationInfo.Customer.LastName,
            RentalDate = reservationInfo.Reservation.RentalDate,
            ReturnDate = reservationInfo.Reservation.ReturnDate
          });
        }

        return reservationData.ToArray();
      });
    }

    [PrincipalPermission(SecurityAction.Demand, Role = Security.CarRentalAdminRole)]
    [PrincipalPermission(SecurityAction.Demand, Name = Security.CarRentalUser)]
    public Rental GetRental(int rentalId) {
      return ExecuteFaultHandledOperation(() => {
        var rentalRepository = _dataRepositoryFactory.GetDataRepository<IRentalRepository>();
        var rental = rentalRepository.Get(rentalId);

        if (rental == null) {
          var ex = new NotFoundException($"No rental record found for id '{rentalId}'.");
          throw new FaultException<NotFoundException>(ex, ex.Message);
        }

        ValidateAuthorization(rental);

        return rental;
      });
    }

    [PrincipalPermission(SecurityAction.Demand, Role = Security.CarRentalAdminRole)]
    public CustomerRentalData[] GetCurrentRentals() {
      return ExecuteFaultHandledOperation(() => {
        var rentalRepository = _dataRepositoryFactory.GetDataRepository<IRentalRepository>();
        var rentalData = new List<CustomerRentalData>();

        var rentalInfoSet = rentalRepository.GetCurrentCustomerRentalInfo();
        foreach (var rentalInfo in rentalInfoSet) {
          rentalData.Add(new CustomerRentalData {
            RentalId = rentalInfo.Rental.RentalId,
            Car = rentalInfo.Car.Color + " " + rentalInfo.Car.Year + " " + rentalInfo.Car.Description,
            CustomerName = rentalInfo.Customer.FirstName + " " + rentalInfo.Customer.LastName,
            DateRented = rentalInfo.Rental.DateRented,
            ExpectedReturn = rentalInfo.Rental.DateDue
          });
        }

        return rentalData.ToArray();
      });
    }

    [PrincipalPermission(SecurityAction.Demand, Role = Security.CarRentalAdminRole)]
    public Reservation[] GetDeadReservations() {
      return ExecuteFaultHandledOperation(() => {
        var reservationRepository = _dataRepositoryFactory.GetDataRepository<IReservationRepository>();
        var reservations = reservationRepository.GetReservationsByPickupDate(DateTime.Now.AddDays(-1));

        return reservations?.ToArray();
      });
    }

    [PrincipalPermission(SecurityAction.Demand, Role = Security.CarRentalAdminRole)]
    public bool IsCarCurrentlyRented(int carId) {
      return ExecuteFaultHandledOperation(() => {
        var carRentalEngine = _businessEngineFactory.GetBusinessEngine<ICarRentalEngine>();
        return carRentalEngine.IsCarCurrentlyRented(carId);
      });
    }
  }
}
