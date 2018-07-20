using CarRental.Client.Bootstrapper;
using CarRental.Web.Core;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CarRental.Web {
  public class MvcApplication : System.Web.HttpApplication {
    protected void Application_Start() {
      AreaRegistration.RegisterAllAreas();
      GlobalConfiguration.Configure(WebApiConfig.Register);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);

      var catalog = new AggregateCatalog();
      catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
      var container = MEFLoader.Init(catalog.Catalogs);

      DependencyResolver.SetResolver(new MefDependencyResolver(container)); // view controllers
      GlobalConfiguration.Configuration.DependencyResolver = new MefAPIDependencyResolver(container); // web api controllers
    }
  }
}
