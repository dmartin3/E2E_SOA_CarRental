using System;
using Core.Common.Core;
using FluentValidation;

namespace CarRental.Client.Entities {
  public class Car : ObjectBase {
    private int _carId;
    private string _description;
    private string _color;
    private int _year;
    private decimal _rentalPrice;
    private bool _currentlyRented;

    public int CarId {
      get => _carId;
      set {
        if (_carId != value) {
          _carId = value;
          OnPropertyChanged(() => CarId);
        }
      }
    }

    public string Description {
      get => _description;
      set {
        if (_description != value) {
          _description = value;
          OnPropertyChanged(() => Description);
        }
      }
    }

    public string Color {
      get => _color;
      set {
        if (_color != value) {
          _color = value;
          OnPropertyChanged(() => Color);
        }
      }
    }

    public int Year {
      get => _year;
      set {
        if (_year != value) {
          _year = value;
          OnPropertyChanged(() => Year);
        }
      }
    }

    public decimal RentalPrice {
      get => _rentalPrice;
      set {
        if (_rentalPrice != value) {
          _rentalPrice = value;
          OnPropertyChanged(() => RentalPrice);
        }
      }
    }

    public bool CurrentlyRented {
      get => _currentlyRented;
      set {
        if (_currentlyRented != value) {
          _currentlyRented = value;
          OnPropertyChanged(() => CurrentlyRented);
        }
      }
    }

    public string LongDescription => $"{_year} {_color} {_description}";

    private class CarValidator : AbstractValidator<Car> {
      public CarValidator() {
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Color).NotEmpty();
        RuleFor(x => x.RentalPrice).GreaterThan(0);
        RuleFor(x => x.Year).GreaterThan(2000).LessThanOrEqualTo(DateTime.Now.Year + 1);
      }
    }

    protected override IValidator GetValidator() {
      return new CarValidator();
    }
  }
}
