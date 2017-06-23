using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AMCustomerImportInspector.Interface;
using AMCustomerImportInspector.Model;
using System.Collections.Generic;
using AMCustomerImportInspector.Reposetory;

namespace FileUtilityTests.CustomerImportInspectorTests
{
    [TestClass]
    public class CustomerImportReposetoryTest
    {
        [TestMethod]
        public void TestGetImportDefinitionsFromDatabaseReturnsCorrectListFromRepo()
        {
            Mock<ICustomerImportRetrievalService> mockDataService = new Mock<ICustomerImportRetrievalService>();
            mockDataService.Setup(a => a.GetCustomerImports()).Returns(() => new List<ImportDefinision>()
            {
                new ImportDefinision()
                {
                    Delimiter = "|",
                    FailureEmailAddresses = new string[] { "bminnaar@gmail.com" },
                    FileMask = "*.xls",
                    ImportFormat = "EXCEL",
                    ImportName = "First Import",
                    ImportPath = "C:/ImportFileDirecory"
                }
            });

            var repo = new CustomerImportReposetory(mockDataService.Object);
            var importConfigItem = repo.GetImportDefinitionsFromDatabase()[0];

            Assert.AreEqual("EXCEL", importConfigItem.ImportFormat);
        }
    }
}
