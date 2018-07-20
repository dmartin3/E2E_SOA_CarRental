appMainModule.controller("AccountLoginViewModel",
  function ($scope, $http, viewModelHelper, validator) {
    $scope.viewModelHelper = viewModelHelper;
    $scope.accountModel = new CarRental.AccountLoginModel();
    $scope.returnUrl = '';

    var accountModelRules = [];

    var setupRules = function () {
      accountModelRules.push(new validator.PropertyRule("LoginEmail",
        {
          required: { message: "Login is required" },
          email: { message: "Login must be in email format" }
        }));
      accountModelRules.push(new validator.PropertyRule("Password",
        {
          required: { message: "Password is required" },
          minLength: { message: "Password must be at least 6 characters", params: 6 }
        }));
    }

    $scope.login = function () {
      validator.ValidateModel($scope.accountModel, accountModelRules);
      viewModelHelper.modelIsValue = $scope.accountModel.isValid;
      viewModelHelper.modelErrors = $scope.accountModel.errors;
      if (viewModelHelper.modelIsValid) {

      }
    }

    setupRules();
  });