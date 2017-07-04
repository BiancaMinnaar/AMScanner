using Microsoft.VisualStudio.TestTools.UnitTesting;
using AMCustomerImportInspector.Service;
using log4net;
using Moq;

namespace FileUtilityTests.CustomerImportInspectorTests
{
    [TestClass]
    public class CustomerImportRetrievalServiceTests
    {
        [TestMethod]
        public void Test_GetCustomerImports_ReturnsRowsFromDB()
        {
            var logMock = new Mock<ILog>();
            var service = new CustomerImportRetrievalService(logMock.Object);
            var returnobj = service.GetCustomerImports();
            Assert.AreNotEqual(0, returnobj.Count);
        }
    }
}
