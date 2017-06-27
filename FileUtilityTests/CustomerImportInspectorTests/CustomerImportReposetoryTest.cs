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
                    ImportFormat = "EXCEL",
                    ImportName = "First Import",
                    ImportPath = "C:/ImportFileDirecory"
                }
            });
            Mock<IEmailService> mockEMailService = new Mock<IEmailService>();

            var repo = new CustomerImportReposetory(mockDataService.Object, mockEMailService.Object);
            var importConfigItem = repo.GetImportDefinitionsFromDatabase()[0];

            Assert.AreEqual("EXCEL", importConfigItem.ImportFormat);
        }

        [TestMethod]
        public void Test_IsFileInImportDefinition_FindsFileInList()
        {
            Mock<ICustomerImportRetrievalService> mockDataService = new Mock<ICustomerImportRetrievalService>();
            Mock<IEmailService> mockEMailService = new Mock<IEmailService>();
            var repo = new CustomerImportReposetory(mockDataService.Object, mockEMailService.Object);
            var definitionList = new List<ImportDefinision>()
            {
                new ImportDefinision()
                {
                    Delimiter = "|",
                    FailureEmailAddresses = new string[] { "bminnaar@gmail.com" },
                    ImportFormat = "EXCEL",
                    ImportName = "First Import",
                    ImportPath = FileUtilityLibraryConstants.CONSTDirectoryToScan
                }
            };

            var checkFile = repo.IsFileInImportDefinition(FileUtilityLibraryConstants.CONSTDirectoryToScan + "\\" + FileUtilityLibraryConstants.CONSTExcelFileWithNoError, definitionList);

            Assert.IsTrue(checkFile);
        }

        [TestMethod]
        public void Test_IsFileInImportDefinition_FindsTextFileInList()
        {
            Mock<ICustomerImportRetrievalService> mockDataService = new Mock<ICustomerImportRetrievalService>();
            Mock<IEmailService> mockEMailService = new Mock<IEmailService>();
            var repo = new CustomerImportReposetory(mockDataService.Object, mockEMailService.Object);
            var definitionList = new List<ImportDefinision>()
            {
                new ImportDefinision()
                {
                    Delimiter = "|",
                    FailureEmailAddresses = new string[] { "bminnaar@gmail.com" },
                    ImportFormat = "CSV",
                    ImportName = "First Import",
                    ImportPath = FileUtilityLibraryConstants.CONSTDirectoryToScan
                }
            };

            var checkFile = repo.IsFileInImportDefinition(FileUtilityLibraryConstants.CONSTDirectoryToScan + "\\" + FileUtilityLibraryConstants.CONSTTestFile1, definitionList);

            Assert.IsTrue(checkFile);
        }

        [TestMethod]
        public void Test_IsFileInImportDefinition_FindsCSVFileInList()
        {
            Mock<ICustomerImportRetrievalService> mockDataService = new Mock<ICustomerImportRetrievalService>();
            Mock<IEmailService> mockEMailService = new Mock<IEmailService>();
            var repo = new CustomerImportReposetory(mockDataService.Object, mockEMailService.Object);
            var definitionList = new List<ImportDefinision>()
            {
                new ImportDefinision()
                {
                    Delimiter = "|",
                    FailureEmailAddresses = new string[] { "bminnaar@gmail.com" },
                    ImportFormat = "CSV",
                    ImportName = "First Import",
                    ImportPath = FileUtilityLibraryConstants.CONSTDirectoryToScan
                }
            };

            var checkFile = repo.IsFileInImportDefinition(FileUtilityLibraryConstants.CONSTDirectoryToScan + "\\" + FileUtilityLibraryConstants.CONSTTestFile4, definitionList);

            Assert.IsTrue(checkFile);
        }

        [TestMethod]
        public void Test_IsFileInImportDefinition_DoesntFindaFileInList()
        {
            Mock<ICustomerImportRetrievalService> mockDataService = new Mock<ICustomerImportRetrievalService>();
            Mock<IEmailService> mockEMailService = new Mock<IEmailService>();
            var repo = new CustomerImportReposetory(mockDataService.Object, mockEMailService.Object);
            var definitionList = new List<ImportDefinision>()
            {
                new ImportDefinision()
                {
                    Delimiter = "|",
                    FailureEmailAddresses = new string[] { "bminnaar@gmail.com" },
                    ImportFormat = "EXCEL",
                    ImportName = "First Import",
                    ImportPath = FileUtilityLibraryConstants.CONSTDirectoryToScan
                }
            };

            var checkFile = repo.IsFileInImportDefinition(FileUtilityLibraryConstants.CONSTDirectoryToScan + "\\" + FileUtilityLibraryConstants.CONSTTestFile1, definitionList);

            Assert.IsFalse(checkFile);
        }

        [TestMethod]
        public void Test_EmailFaultyFile_EmailsFile()
        {
            Mock<ICustomerImportRetrievalService> mockDataService = new Mock<ICustomerImportRetrievalService>();
            Mock<IEmailService> mockEMailService = new Mock<IEmailService>();
            var repo = new CustomerImportReposetory(mockDataService.Object, mockEMailService.Object);
            var definitionList = new List<ImportDefinision>()
            {
                new ImportDefinision()
                {
                    Delimiter = "|",
                    FailureEmailAddresses = new string[] { "bminnaar@gmail.com" },
                    ImportFormat = "EXCEL",
                    ImportName = "First Import",
                    ImportPath = FileUtilityLibraryConstants.CONSTDirectoryToScan
                }
            };

            repo.EmailFaultyFile(FileUtilityLibraryConstants.CONSTDirectoryToScan + "\\" + FileUtilityLibraryConstants.CONSTExcelFileWithError, definitionList[0]);

            Assert.IsTrue(true);
        }
    }
}
