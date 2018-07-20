using System.ComponentModel.Composition;
using System.Web.Mvc;
using CarRental.Web.Core;
using CarRental.Web.Models;

namespace CarRental.Web.Controllers.MVC {
  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  [RoutePrefix("account")]
  public class AccountController : ViewControllerBase {
    private readonly ISecurityAdapter _securityAdapter;

    [ImportingConstructor]
    public AccountController(ISecurityAdapter securityAdapter) {
      _securityAdapter = securityAdapter;
    }

    [HttpGet]
    [Route("login")]
    public ActionResult Login(string returnUrl) {
      _securityAdapter.Initialize();
      return View(new AccountLoginModel{ReturnUrl = returnUrl});
    }
  }
}