using System.ComponentModel.Composition;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CarRental.Web.Core;
using CarRental.Web.Models;

namespace CarRental.Web.Controllers.API {
  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  [RoutePrefix("api/account")]
  public class AccountApiController : ApiControllerBase {
    private ISecurityAdapter _securityAdapter;

    [ImportingConstructor]
    public AccountApiController(ISecurityAdapter securityAdapter) {
      _securityAdapter = securityAdapter;
    }

    [HttpPost]
    [Route("login")]
    public HttpResponseMessage Login(HttpRequestMessage request, [FromBody]AccountLoginModel accountModel) {
      return GetHttpResponse(request, () => {
        HttpResponseMessage response;
        var success = _securityAdapter.Login(accountModel.LoginEmail, accountModel.Password, accountModel.RememberMe);

        if (success) {
          response = request.CreateResponse(HttpStatusCode.OK);
        }
        else {
          response = request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized login.");
        }

        return response;
      });
    }
  }
}