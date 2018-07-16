using Core.Common.Core;
using FluentValidation;

namespace Core.Common.Tests {
   internal class TestClass : ObjectBase {
    private string _cleanProp = string.Empty;
    private string _dirtyProp = string.Empty;
    private string _stringProp = string.Empty;
    private TestChild _child = new TestChild();
    private TestChild _notNavigableChild = new TestChild();

    public string CleanProp {
      get { return _cleanProp; }
      set {
        if (_cleanProp == value)
          return;

        _cleanProp = value;
        OnPropertyChanged(() => CleanProp, false);
      }
    }

    public string DirtyProp {
      get { return _dirtyProp; }
      set {
        if (_dirtyProp == value)
          return;

        _dirtyProp = value;
        OnPropertyChanged(() => DirtyProp);
      }
    }

    public string StringProp {
      get { return _stringProp; }
      set {
        if (_stringProp == value)
          return;

        _stringProp = value;
        OnPropertyChanged("StringProp", false);
      }
    }

    public TestChild Child {
      get { return _child; }
    }

    [NotNavigable]
    public TestChild NotNavigableChild {
      get { return _notNavigableChild; }
    }

    private class TestClassValidator : AbstractValidator<TestClass> {
      public TestClassValidator() {
        RuleFor(obj => obj.StringProp).NotEmpty();
      }
    }

    protected override IValidator GetValidator() {
      return new TestClassValidator();
    }
  }
}
