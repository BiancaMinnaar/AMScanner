using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AMCustomerImportInspector.Service;

namespace FileUtilityTests.CustomerImportInspectorTests
{
    [TestClass]
    public class CustomerImportRetrievalServiceTests
    {
        [TestMethod]
        public void TestGetCustomerImportsReturnsRowsFromDB()
        {
            var service = new CustomerImportRetrievalService();
            var returnobj = service.GetCustomerImports();
            Assert.AreNotEqual(0, returnobj.Count);
        }
    }
}
