using System.ServiceModel;
using CarRental.Business.Contracts.Service_Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CarRental.ServiceHost.Tests {
  [TestClass]
  public class ServiceAccessTests {
    [TestMethod]
    public void test_inventory_manager_as_service() {
      var channelFactory = new ChannelFactory<IInventoryService>("");
      var proxy = channelFactory.CreateChannel();
      (proxy as ICommunicationObject).Open();
      channelFactory.Close();
    }

    [TestMethod]
    public void test_rental_manager_as_service() {
      var channelFactory = new ChannelFactory<IRentalService>("");
      var proxy = channelFactory.CreateChannel();
      (proxy as ICommunicationObject).Open();
      channelFactory.Close();
    }

    [TestMethod]
    public void test_account_manager_as_service() {
      var channelFactory = new ChannelFactory<IAccountService>("");
      var proxy = channelFactory.CreateChannel();
      (proxy as ICommunicationObject).Open();
      channelFactory.Close();
    }
  }
}
