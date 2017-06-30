using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AMCustomerImportInspector.Interface;
using AMCustomerImportInspector.Model;
using System.Collections.Generic;
using AMCustomerImportInspector.Reposetory;
using AMCustomerImportInspector.Service;

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
                    FailureEmailList = "bminnaar@gmail.com",
                    ImportFormat = "EXCEL",
                    ImportName = "First Import",
                    ImportPath = "C:/ImportFileDirecory"
                }
            });
            Mock<IEmailService> mockEMailService = new Mock<IEmailService>();
            var mockEMailTemplateService = new Mock<EMailTemplateService>();

            var repo = new CustomerImportReposetory(mockDataService.Object, mockEMailService.Object, mockEMailTemplateService.Object);
            var importConfigItem = repo.GetImportDefinitionsFromDatabase()[0];

            Assert.AreEqual("EXCEL", importConfigItem.ImportFormat);
        }

        [TestMethod]
        public void Test_EmailFaultyFile_EmailsFile()
        {
            Mock<ICustomerImportRetrievalService> mockDataService = new Mock<ICustomerImportRetrievalService>();
            Mock<IEmailService> mockEMailService = new Mock<IEmailService>();
            var mockEMailTemplateService = new Mock<IEMailTemplateService>();
            mockEMailTemplateService.Setup(m => m.GetSubjectText(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => FileUtilityLibraryConstants.CONSTGoodEMailSubject);
            mockEMailTemplateService.Setup(m => m.GetWholeEmailBodyWithErrors(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()))
               .Returns(() => FileUtilityLibraryConstants.CONSTGoodEMailBody);
            var repo = new CustomerImportReposetory(mockDataService.Object, mockEMailService.Object, 
                mockEMailTemplateService.Object);
            var definitionList = new List<ImportDefinision>()
            {
                new ImportDefinision()
                {
                    Delimiter = "|",
                    FailureEmailList = "bminnaar@gmail.com",
                    ImportFormat = "EXCEL",
                    ImportName = "First Import",
                    ImportPath = FileUtilityLibraryConstants.CONSTDirectoryToScan
                }
            };

            var errorFile = FileUtilityLibraryConstants.CONSTDirectoryToScan
                + "\\" + FileUtilityLibraryConstants.CONSTExcelFileWithError;
            repo.EmailFaultyFile(errorFile, definitionList[0],
                new string[] { "Error On line 1", "Error on Line 2"});

            mockEMailService.Verify(m => m.SendEmailToRecipient(
                It.Is<string>(s => s == definitionList[0].FailureEmailAddresses[0]),
                It.Is<string>(s => s == FileUtilityLibraryConstants.CONSTGoodEMailSubject),
                It.Is<string>(s => s == FileUtilityLibraryConstants.CONSTGoodEMailBody),
                It.Is<string>(s => s == errorFile)));
        }

        [TestMethod]
        public void Test_EmailOrphanedFile_EmailsFile()
        {
            Mock<ICustomerImportRetrievalService> mockDataService = new Mock<ICustomerImportRetrievalService>();
            Mock<IEmailService> mockEMailService = new Mock<IEmailService>();
            var mockEMailTemplateService = new Mock<IEMailTemplateService>();
            mockEMailTemplateService.Setup(m => m.GetSubjectText(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => FileUtilityLibraryConstants.CONSTGoodEMailSubject);
            mockEMailTemplateService.Setup(m => m.GetWholeEmailBody(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
               .Returns(() => FileUtilityLibraryConstants.CONSTGoodEMailBody);
            var repo = new CustomerImportReposetory(mockDataService.Object, mockEMailService.Object,
                mockEMailTemplateService.Object);
            var definitionList = new List<ImportDefinision>()
            {
                new ImportDefinision()
                {
                    Delimiter = "|",
                    FailureEmailList = "bminnaar@gmail.com",
                    ImportFormat = "EXCEL",
                    ImportName = "First Import",
                    ImportPath = FileUtilityLibraryConstants.CONSTDirectoryToScan
                }
            };

            var errorFile = FileUtilityLibraryConstants.CONSTDirectoryToScan
                + "\\" + FileUtilityLibraryConstants.CONSTExcelFileWithError;
            repo.EMailOrphenedFileToSupport(errorFile, new string[] 
            {
                CustomerImportInspectorConstants.CONSTEmailAddress
            });

            mockEMailService.Verify(m => m.SendEmailToRecipient(
                It.Is<string>(s => s == CustomerImportInspectorConstants.CONSTEmailAddress),
                It.Is<string>(s => s == FileUtilityLibraryConstants.CONSTGoodEMailSubject),
                It.Is<string>(s => s == FileUtilityLibraryConstants.CONSTGoodEMailBody),
                It.Is<string>(s => s == errorFile)));
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
                           FailureEmailList = "bminnaar@gmail.com",
                            ImportFormat = "EXCEL",
                            ImportName = "First Import",
                            ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient1
                        },
                         new ImportDefinision()
                        {
                            Delimiter = "|",
                            FailureEmailList = "bminnaar@gmail.com",
                            ImportFormat = "EXCEL",
                            ImportName = "Second Import",
                            ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient2
                        }
                    };
            mockDataService.Setup(m => m.GetCustomerImports())
                .Returns(() => importDefinisions);
            Mock<IEmailService> mockEMailService = new Mock<IEmailService>();
            var mockEMailTemplateService = new Mock<EMailTemplateService>();
            var repo = new CustomerImportReposetory(mockDataService.Object, mockEMailService.Object
                , mockEMailTemplateService.Object);

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
                            FailureEmailList = "bminnaar@gmail.com",
                            ImportFormat = "EXCEL",
                            ImportName = "First Import",
                            ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient1
                        },
                         new ImportDefinision()
                        {
                            Delimiter = "|",
                            FailureEmailList = "bminnaar@gmail.com",
                            ImportFormat = "EXCEL",
                            ImportName = "Second Import",
                            ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient2
                        }
                    };
            mockDataService.Setup(m => m.GetCustomerImports())
                .Returns(() => importDefinisions);
            Mock<IEmailService> mockEMailService = new Mock<IEmailService>();
            var mockEMailTemplateService = new Mock<EMailTemplateService>();
            var repo = new CustomerImportReposetory(mockDataService.Object, mockEMailService.Object
                , mockEMailTemplateService.Object);

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
                            ID=0,
                            Delimiter = FileUtilityLibraryConstants.CONSTDelimiter.ToString(),
                            FailureEmailList = "bminnaar@gmail.com",
                            ImportFormat = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeExcel,
                            ImportName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName1,
                            ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient1 + "\\*.*"
                        },
                         new ImportDefinision()
                        {
                             ID=1,
                            Delimiter = FileUtilityLibraryConstants.CONSTDelimiter.ToString(),
                            FailureEmailList = "bminnaar@gmail.com",
                            ImportFormat = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeExcel,
                            ImportName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName2,
                            ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient2 + "\\*.*"
                        }
                    };
            mockDataService.Setup(m => m.GetCustomerImports())
                .Returns(() => importDefinisions);
            Mock<IEmailService> mockEMailService = new Mock<IEmailService>();
            var mockEMailTemplateService = new Mock<EMailTemplateService>();
            var repo = new CustomerImportReposetory(mockDataService.Object, mockEMailService.Object,
                mockEMailTemplateService.Object);

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
                            FailureEmailList = "bminnaar@gmail.com",
                            ImportFormat = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeExcel,
                            ImportName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName1,
                            ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient1
                        },
                         new ImportDefinision()
                        {
                            Delimiter = FileUtilityLibraryConstants.CONSTDelimiter.ToString(),
                            FailureEmailList = "bminnaar@gmail.com",
                            ImportFormat = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeExcel,
                            ImportName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName2,
                            ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient2
                        }
                    };
            mockDataService.Setup(m => m.GetCustomerImports())
                .Returns(() => importDefinisions);
            Mock<IEmailService> mockEMailService = new Mock<IEmailService>();
            var mockEMailTemplateService = new Mock<EMailTemplateService>();
            var repo = new CustomerImportReposetory(mockDataService.Object, mockEMailService.Object,
                mockEMailTemplateService.Object);

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
                            ID=0,
                            Delimiter = FileUtilityLibraryConstants.CONSTDelimiter.ToString(),
                            FailureEmailList = "bminnaar@gmail.com",
                            ImportFormat = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeExcel,
                            ImportName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName1,
                            ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient1
                        },
                         new ImportDefinision()
                        {
                             ID=1,
                             Delimiter = FileUtilityLibraryConstants.CONSTDelimiter.ToString(),
                             FailureEmailList = "bminnaar@gmail.com",
                             ImportFormat = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeCSV,
                             ImportName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName2,
                             ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient2
                        }
                    };
            mockDataService.Setup(m => m.GetCustomerImports())
                .Returns(() => importDefinisions);
            Mock<IEmailService> mockEMailService = new Mock<IEmailService>();
            var mockEMailTemplateService = new Mock<EMailTemplateService>();
            var repo = new CustomerImportReposetory(mockDataService.Object, mockEMailService.Object,
                mockEMailTemplateService.Object);

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
            var mockEMailTemplateService = new Mock<EMailTemplateService>();
            var repo = new CustomerImportReposetory(mockDataService.Object, mockEMailService.Object,
                mockEMailTemplateService.Object);

            var directory = repo.GetMoveToDirecotry(
                FileUtilityLibraryConstants.CONSTDirectoryToScan,
                FileUtilityLibraryConstants.CONSTPartDirecotryToScan,
                FileUtilityLibraryConstants.CONSTPartDirecotryToMoveTo);

            Assert.AreEqual(FileUtilityLibraryConstants.CONSTDirecoryToMoveTo, directory);
        }

        [TestMethod]
        public void Test_GetImportDefinisionFromFileName_ReturnsDefinisionsFromDBForCorrectFile()
        {
            var mockEMailService = new Mock<IEmailService>();
            var mockEMailTemplateService = new Mock<IEMailTemplateService>();
            var ImportRepo = new CustomerImportReposetory(
               new CustomerImportRetrievalService(), mockEMailService.Object, mockEMailTemplateService.Object);
            var fileToScan = FileUtilityLibraryConstants.CONSTScannerSetupDirecoryToWatch + "\\" +
                FileUtilityLibraryConstants.CONSTScannerSetupFileToDump;

            var definision = ImportRepo.GetImportDefinisionFromFileName(fileToScan);

            Assert.AreNotEqual(null, definision);
        }

        [TestMethod]
        public void Test_GetImportDefinisionFromFileName_ReturnsNullForTextFile()
        {
            var mockEMailService = new Mock<IEmailService>();
            var mockEMailTemplateService = new Mock<IEMailTemplateService>();
            var ImportRepo = new CustomerImportReposetory(
               new CustomerImportRetrievalService(), mockEMailService.Object, mockEMailTemplateService.Object);
            var fileToScan = FileUtilityLibraryConstants.CONSTScannerSetupDirecoryToWatch + "\\" +
                FileUtilityLibraryConstants.CONSTScannerSetupFileToDumpStoreSalesText;

            var definision = ImportRepo.GetImportDefinisionFromFileName(fileToScan);

            Assert.AreEqual(null, definision);
        }

        [TestMethod]
        public void Test_GetImportDefinisionFromFileName_ReturnsDeffinisionForExcel()
        {
            var mockEMailService = new Mock<IEmailService>();
            var mockEMailTemplateService = new Mock<IEMailTemplateService>();
            var ImportRepo = new CustomerImportReposetory(
               new CustomerImportRetrievalService(), mockEMailService.Object, mockEMailTemplateService.Object);
            var fileToScan = FileUtilityLibraryConstants.CONSTScannerSetupDirecoryToWatch + "\\" +
                FileUtilityLibraryConstants.CONSTScannerSetupFileToDumpStoreSalesExcel;

            var definision = ImportRepo.GetImportDefinisionFromFileName(fileToScan);

            Assert.AreNotEqual(null, definision);
        }
    }
}
