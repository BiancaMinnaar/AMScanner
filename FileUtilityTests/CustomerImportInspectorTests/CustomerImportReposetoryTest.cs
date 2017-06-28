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

        [TestMethod]
        public void Test_GetImportDefinisionFromFileName_ReturnsImportDefinition()
        {
            Mock<ICustomerImportRetrievalService> mockDataService = new Mock<ICustomerImportRetrievalService>();
            var importDefinisions = new List<ImportDefinision>()
                    {
                        new ImportDefinision()
                        {
                            Delimiter = "|",
                            FailureEmailAddresses = new string[] { "bminnaar@gmail.com" },
                            ImportFormat = "EXCEL",
                            ImportName = "First Import",
                            ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient1
                        },
                         new ImportDefinision()
                        {
                            Delimiter = "|",
                            FailureEmailAddresses = new string[] { "bminnaar@gmail.com" },
                            ImportFormat = "EXCEL",
                            ImportName = "Second Import",
                            ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient2
                        }
                    };
            mockDataService.Setup(m => m.GetCustomerImports())
                .Returns(() => importDefinisions);
            Mock<IEmailService> mockEMailService = new Mock<IEmailService>();
            var repo = new CustomerImportReposetory(mockDataService.Object, mockEMailService.Object);

            var importDefinision = repo.GetImportDefinisionFromFileName(
                FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient1 + @"\" 
                    + FileUtilityLibraryConstants.CONSTExcelFileWithError);

            Assert.AreNotEqual(null, importDefinision);
        }

        [TestMethod]
        public void Test_GetImportDefinisionFromFileName_ReturnsOnlyOneImportDefinition()
        {
            Mock<ICustomerImportRetrievalService> mockDataService = new Mock<ICustomerImportRetrievalService>();
            var importDefinisions = new List<ImportDefinision>()
                    {
                        new ImportDefinision()
                        {
                            Delimiter = "|",
                            FailureEmailAddresses = new string[] { "bminnaar@gmail.com" },
                            ImportFormat = "EXCEL",
                            ImportName = "First Import",
                            ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient1
                        },
                         new ImportDefinision()
                        {
                            Delimiter = "|",
                            FailureEmailAddresses = new string[] { "bminnaar@gmail.com" },
                            ImportFormat = "EXCEL",
                            ImportName = "Second Import",
                            ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient2
                        }
                    };
            mockDataService.Setup(m => m.GetCustomerImports())
                .Returns(() => importDefinisions);
            Mock<IEmailService> mockEMailService = new Mock<IEmailService>();
            var repo = new CustomerImportReposetory(mockDataService.Object, mockEMailService.Object);

            var importDefinision = repo.GetImportDefinisionFromFileName(
                FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient1 + @"\"
                    + FileUtilityLibraryConstants.CONSTExcelFileWithError);

            Assert.AreEqual("First Import", importDefinision.ImportName);
        }

        [TestMethod]
        public void Test_GetImportDefinisionFromFileName_ReturnsCorrectImportDefinition()
        {
            Mock<ICustomerImportRetrievalService> mockDataService = new Mock<ICustomerImportRetrievalService>();
            var importDefinisions = new List<ImportDefinision>()
                    {
                        new ImportDefinision()
                        {
                            Delimiter = FileUtilityLibraryConstants.CONSTDelimiter.ToString(),
                            FailureEmailAddresses = new string[] { "bminnaar@gmail.com" },
                            ImportFormat = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeExcel,
                            ImportName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName1,
                            ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient1
                        },
                         new ImportDefinision()
                        {
                            Delimiter = FileUtilityLibraryConstants.CONSTDelimiter.ToString(),
                            FailureEmailAddresses = new string[] { "bminnaar@gmail.com" },
                            ImportFormat = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeExcel,
                            ImportName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName2,
                            ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient2
                        }
                    };
            mockDataService.Setup(m => m.GetCustomerImports())
                .Returns(() => importDefinisions);
            Mock<IEmailService> mockEMailService = new Mock<IEmailService>();
            var repo = new CustomerImportReposetory(mockDataService.Object, mockEMailService.Object);

            var importDefinision = repo.GetImportDefinisionFromFileName(
                FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient2 + @"\"
                    + FileUtilityLibraryConstants.CONSTExcelFileWithNoError);

            Assert.AreEqual(FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName2, importDefinision.ImportName);
        }

        [TestMethod]
        public void Test_GetImportDefinisionFromFileName_ReturnsNoImportDefinitionOnIncorrectExtension()
        {
            Mock<ICustomerImportRetrievalService> mockDataService = new Mock<ICustomerImportRetrievalService>();
            var importDefinisions = new List<ImportDefinision>()
                    {
                        new ImportDefinision()
                        {
                            Delimiter = FileUtilityLibraryConstants.CONSTDelimiter.ToString(),
                            FailureEmailAddresses = new string[] { "bminnaar@gmail.com" },
                            ImportFormat = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeExcel,
                            ImportName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName1,
                            ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient1
                        },
                         new ImportDefinision()
                        {
                            Delimiter = FileUtilityLibraryConstants.CONSTDelimiter.ToString(),
                            FailureEmailAddresses = new string[] { "bminnaar@gmail.com" },
                            ImportFormat = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeExcel,
                            ImportName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName2,
                            ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient2
                        }
                    };
            mockDataService.Setup(m => m.GetCustomerImports())
                .Returns(() => importDefinisions);
            Mock<IEmailService> mockEMailService = new Mock<IEmailService>();
            var repo = new CustomerImportReposetory(mockDataService.Object, mockEMailService.Object);

            var importDefinision = repo.GetImportDefinisionFromFileName(
                FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient2 + @"\"
                    + FileUtilityLibraryConstants.CONSTTestFile3);

            Assert.AreEqual(null, importDefinision);
        }

        [TestMethod]
        public void Test_GetImportDefinisionFromFileName_ForTextFile()
        {
            Mock<ICustomerImportRetrievalService> mockDataService = new Mock<ICustomerImportRetrievalService>();
            var importDefinisions = new List<ImportDefinision>()
                    {
                        new ImportDefinision()
                        {
                            Delimiter = FileUtilityLibraryConstants.CONSTDelimiter.ToString(),
                            FailureEmailAddresses = new string[] { "bminnaar@gmail.com" },
                            ImportFormat = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeExcel,
                            ImportName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName1,
                            ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient1
                        },
                         new ImportDefinision()
                        {
                            Delimiter = FileUtilityLibraryConstants.CONSTDelimiter.ToString(),
                            FailureEmailAddresses = new string[] { "bminnaar@gmail.com" },
                            ImportFormat = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeCSV,
                            ImportName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName2,
                            ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient2
                        }
                    };
            mockDataService.Setup(m => m.GetCustomerImports())
                .Returns(() => importDefinisions);
            Mock<IEmailService> mockEMailService = new Mock<IEmailService>();
            var repo = new CustomerImportReposetory(mockDataService.Object, mockEMailService.Object);

            var importDefinision = repo.GetImportDefinisionFromFileName(
                FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient2 + @"\"
                    + FileUtilityLibraryConstants.CONSTTestFile3);

            Assert.AreEqual(FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName2, importDefinision.ImportName);
        }

        [TestMethod]
        public void Test_GetMoveToDirecotry_ReturnsCorrectDirecotry()
        {
            Mock<ICustomerImportRetrievalService> mockDataService = new Mock<ICustomerImportRetrievalService>();
            Mock<IEmailService> mockEMailService = new Mock<IEmailService>();
            var repo = new CustomerImportReposetory(mockDataService.Object, mockEMailService.Object);

            var directory = repo.GetMoveToDirecotry(
                FileUtilityLibraryConstants.CONSTDirectoryToScan,
                FileUtilityLibraryConstants.CONSTPartDirecotryToScan,
                FileUtilityLibraryConstants.CONSTPartDirecotryToMoveTo);

            Assert.AreEqual(FileUtilityLibraryConstants.CONSTDirecoryToMoveTo, directory);
        }
    }
}
