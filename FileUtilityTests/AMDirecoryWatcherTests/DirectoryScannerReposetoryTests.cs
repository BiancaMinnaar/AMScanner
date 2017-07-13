using AMCustomerImportInspector.Interface;
using AMCustomerImportInspector.Model;
using AMCustomerImportInspector.Reposetory;
using AMCustomerImportInspector.Service;
using AMDirectoryWatcher.Reposetory;
using FileUtilityLibrary.ExpetionOccurrences;
using FileUtilityLibrary.Interface.Model;
using FileUtilityLibrary.Interface.Repository;
using FileUtilityLibrary.Interface.Service;
using FileUtilityLibrary.Model.ScannerFile.Excel;
using FileUtilityLibrary.Reposetory;
using FileUtilityLibrary.Service;
using FileUtilityTests.CustomerImportInspectorTests;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.IO;

namespace FileUtilityTests.AMDirecoryWatcherTests
{
    [TestClass]
    public class DirectoryScannerReposetoryTests
    {
        [TestMethod]
        public void Test_ScannCreatedFile_SendsOrphanedEmailIfNoImportDeffinisionIsFound()
        {
            Mock<ICustomerImportReposetory> importRepo = new Mock<ICustomerImportReposetory>();
            importRepo.Setup(m => m.GetImportDefinisionFromFileName(It.IsAny<string>()))
                .Returns(() => null);
            Mock<IScannerRepository> scannerRepo = new Mock<IScannerRepository>();
            Mock<ILog> logHandler = new Mock<ILog>();
            var directoryRepo = new DirectoryScannerReposetory(
                importRepo.Object, scannerRepo.Object, logHandler.Object,
                FileUtilityLibraryConstants.CONSTPartDirecotryToScan,
                FileUtilityLibraryConstants.CONSTPartDirecotryToMoveTo,
                CustomerImportInspectorConstants.CONSTEmailAddress.Split(';'),
                true);

            directoryRepo.ScannCreatedFile("");

            importRepo.Verify(m => m.EMailOrphenedFileToSupport(It.IsAny<string>(), It.IsAny<string[]>()), "Email wasn't sent");
            scannerRepo.Verify(m => m.DeleteOrphanedFile(It.IsAny<string>()), "The File wasn't deleted");
        }

        private void setupReposForTestWithImportDefinision(
            out IScannerFile scannerFile, 
            out Mock<ICustomerImportReposetory> importRepo, 
            out Mock<IScannerRepository> scannerRepo,
            ImportDefinision importDefinision,
            IScannerFile sendScannerFile)
        {
            Mock<IMoverService> moverService = new Mock<IMoverService>();

            
            Mock<IFileMaskToScannerFile> maskToScannerFile = new Mock<IFileMaskToScannerFile>();
            maskToScannerFile.Setup(m => m.GetScannerFileInstance(It.IsAny<FileInfo>()))
                .Returns(() => sendScannerFile);
            scannerFile = sendScannerFile;
            importRepo = new Mock<ICustomerImportReposetory>();
            importRepo.Setup(m => m.GetImportDefinisionFromFileName(It.IsAny<string>()))
                .Returns(() => importDefinision);
            importRepo.Setup(m => m.GetMoveToDirecotry(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(() =>
                    FileUtilityLibraryConstants.CONSTDirecoryToMoveTo);

            scannerRepo = new Mock<IScannerRepository>();
            scannerRepo.SetupGet(p => p.MoverService).Returns(moverService.Object);
            scannerRepo.SetupGet(p => p.FileMaskToScannerFile).Returns(maskToScannerFile.Object);
            scannerRepo.SetupGet(p => p.ExceptionsToScanFor).Returns(() => new List<IExceptionOccurrence>()
            {
                new HeaderColumnLineCountExceptionOccurrence(
                    FileUtilityLibraryConstants.CONSTActualErrorMessageForHeaderError)
            });
        }

        [TestMethod]
        public void Test_ScannCreatedFile_SetsUpCorrectlyFromCustomerDefinision()
        {
            IScannerFile scannerFile;
            Mock<ICustomerImportReposetory> importRepo;
            Mock<IScannerRepository> scannerRepo;
            Mock<ILog> logHandler = new Mock<ILog>();
            Mock<ICSVFromExcelService> csvExcelService = new Mock<ICSVFromExcelService>();
            var importDefinision = new ImportDefinision()
            {
                ClientDatabase = FileUtilityLibraryConstants.CONSTClientName,
                Delimiter = FileUtilityLibraryConstants.CONSCommaDelimiter.ToString(),
                FailureEmailList = CustomerImportInspectorConstants.CONSTEmailAddress,
                HasHeader = FileUtilityLibraryConstants.CONSTHasHeader,
                ID = FileUtilityLibraryConstants.CONSTClientRecordID,
                ImportFormat = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeExcel,
                ImportName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName1,
                ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathWithMaskClient3,
                IsEnabled = FileUtilityLibraryConstants.CONSTImportIsEnabled
            };
            var sendScannerFile = new ExcelScannerFile(
                    FileUtilityLibraryConstants.CONSTScannerSetupFileToDump,
                    FileUtilityLibraryConstants.CONSTDirectoryToScan,
                    FileUtilityLibraryConstants.CONSCommaDelimiter,
                    FileUtilityLibraryConstants.CONSTHasHeader,
                    csvExcelService.Object);
            setupReposForTestWithImportDefinision(out scannerFile, out importRepo, out scannerRepo, importDefinision, sendScannerFile);
            var directoryRepo = new DirectoryScannerReposetory(
                importRepo.Object, scannerRepo.Object, logHandler.Object,
                FileUtilityLibraryConstants.CONSTPartDirecotryToScan,
                FileUtilityLibraryConstants.CONSTPartDirecotryToMoveTo,
                CustomerImportInspectorConstants.CONSTEmailAddress.Split(';'),
                true);
            var fullFileName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient1 + @"\" +
                FileUtilityLibraryConstants.CONSTScannerSetupFileToDump;

            directoryRepo.ScannCreatedFile(fullFileName);

            importRepo.Verify(m => m.GetMoveToDirecotry(
                It.Is<string>(p => p == FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient1),
                It.Is<string>(p => p == FileUtilityLibraryConstants.CONSTPartDirecotryToScan),
                It.Is<string>(p => p == FileUtilityLibraryConstants.CONSTPartDirecotryToMoveTo)),
                "Email wasn't sent");
            scannerRepo.Verify(m => m.ScanForExceptions(It.Is<IScannerFile>(p => p == scannerFile)));
        }

        [TestMethod]
        public void Test_ScannCreatedFile_MovesSuccessfullFile()
        {
            IScannerFile scannerFile;
            Mock<ICustomerImportReposetory> importRepo;
            Mock<IScannerRepository> scannerRepo;
            Mock<ICSVFromExcelService> csvExcelService = new Mock<ICSVFromExcelService>();
            var importDefinision = new ImportDefinision()
            {
                ClientDatabase = FileUtilityLibraryConstants.CONSTClientName,
                Delimiter = FileUtilityLibraryConstants.CONSCommaDelimiter.ToString(),
                FailureEmailList = CustomerImportInspectorConstants.CONSTEmailAddress,
                HasHeader = FileUtilityLibraryConstants.CONSTHasHeader,
                ID = FileUtilityLibraryConstants.CONSTClientRecordID,
                ImportFormat = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeExcel,
                ImportName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName1,
                ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathWithMaskClient3,
                IsEnabled = FileUtilityLibraryConstants.CONSTImportIsEnabled
            };
            var sendScannerFile = new ExcelScannerFile(
                                FileUtilityLibraryConstants.CONSTScannerSetupFileToDump,
                                FileUtilityLibraryConstants.CONSTDirectoryToScan,
                                FileUtilityLibraryConstants.CONSCommaDelimiter,
                                FileUtilityLibraryConstants.CONSTHasHeader,
                                csvExcelService.Object);
            setupReposForTestWithImportDefinision(out scannerFile, out importRepo, out scannerRepo, importDefinision, sendScannerFile);
            scannerRepo.Setup(m => m.ScanForExceptions(It.IsAny<IScannerFile>())).Returns(false);
            Mock<ILog> logHandler = new Mock<ILog>();
            var directoryRepo = new DirectoryScannerReposetory(
                importRepo.Object, scannerRepo.Object, logHandler.Object,
                FileUtilityLibraryConstants.CONSTPartDirecotryToScan,
                FileUtilityLibraryConstants.CONSTPartDirecotryToMoveTo,
                CustomerImportInspectorConstants.CONSTEmailAddress.Split(';'),
                true);
            var fullFileName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient1 + @"\" +
                FileUtilityLibraryConstants.CONSTScannerSetupFileToDump;

            directoryRepo.ScannCreatedFile(fullFileName);

            importRepo.Verify(m => m.GetMoveToDirecotry(
                It.Is<string>(p => p == FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient1),
                It.Is<string>(p => p == FileUtilityLibraryConstants.CONSTPartDirecotryToScan),
                It.Is<string>(p => p == FileUtilityLibraryConstants.CONSTPartDirecotryToMoveTo)),
                "Email wasn't sent");
            scannerRepo.Verify(m => m.ScanForExceptions(It.Is<IScannerFile>(p => p == scannerFile)));
            scannerRepo.Verify(m => m.MoveFileAfterScan(It.Is<string>(p => p == fullFileName)));
        }

        [TestMethod]
        public void Test_ScannCreatedFile_EmailsCustomerAndDeletesFileOnUnsuccessfullScan()
        {
            var fullFileName = FileUtilityLibraryConstants.CONSTDirectoryToScan + @"\" +
                FileUtilityLibraryConstants.CONSTExcelFileWithError;
            IScannerFile scannerFile;
            Mock<ICustomerImportReposetory> importRepo;
            Mock<IScannerRepository> scannerRepo;
            Mock<ILog> logHandler = new Mock<ILog>();
            var excelAutomation = new CSVWithExcelAutomationService(fullFileName, logHandler.Object);
            var importDefinision = new ImportDefinision()
            {
                ClientDatabase = FileUtilityLibraryConstants.CONSTClientName,
                Delimiter = FileUtilityLibraryConstants.CONSCommaDelimiter.ToString(),
                FailureEmailList = CustomerImportInspectorConstants.CONSTEmailAddress,
                HasHeader = FileUtilityLibraryConstants.CONSTHasHeader,
                ID = FileUtilityLibraryConstants.CONSTClientRecordID,
                ImportFormat = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeExcel,
                ImportName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName1,
                ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathWithMaskClient3,
                IsEnabled = FileUtilityLibraryConstants.CONSTImportIsEnabled
            };
            var sendScannerFile = new ExcelScannerFile(
                    FileUtilityLibraryConstants.CONSTScannerSetupFileToDump,
                    FileUtilityLibraryConstants.CONSTDirectoryToScan,
                    FileUtilityLibraryConstants.CONSCommaDelimiter,
                    FileUtilityLibraryConstants.CONSTHasHeader,
                    excelAutomation);
            setupReposForTestWithImportDefinision(out scannerFile, out importRepo, out scannerRepo, importDefinision, sendScannerFile);
            scannerRepo.Setup(m => m.ScanForExceptions(It.IsAny<IScannerFile>())).Returns(true);
            var directoryRepo = new DirectoryScannerReposetory(
                importRepo.Object, scannerRepo.Object, logHandler.Object,
                FileUtilityLibraryConstants.CONSTPartDirecotryToScan,
                FileUtilityLibraryConstants.CONSTPartDirecotryToMoveTo,
                CustomerImportInspectorConstants.CONSTEmailAddress.Split(';'),
                true);

            directoryRepo.ScannCreatedFile(fullFileName);

            importRepo.Verify(m => m.GetMoveToDirecotry(
                It.Is<string>(p => p == FileUtilityLibraryConstants.CONSTDirectoryToScan),
                It.Is<string>(p => p == FileUtilityLibraryConstants.CONSTPartDirecotryToScan),
                It.Is<string>(p => p == FileUtilityLibraryConstants.CONSTPartDirecotryToMoveTo)),
                "Email wasn't sent");
            scannerRepo.Verify(m => m.ScanForExceptions(It.Is<IScannerFile>(p => p == scannerFile)));
            importRepo.Verify(m => m.EmailFaultyFile(
                It.Is<string>(p => p == fullFileName),
                It.Is<ImportDefinision>(p => p == importDefinision),
                It.IsAny<string[]>()));
            scannerRepo.Verify(m => m.DeleteFaultyFile(It.Is<string>(p => p == fullFileName)));
        }

        [TestMethod]
        public void Test_ScannCreatedFile_EmailsSupportAndDeletesOrphanedFile()
        {
            IScannerFile scannerFile;
            Mock<ICustomerImportReposetory> importRepo;
            Mock<IScannerRepository> scannerRepo;
            Mock<ICSVFromExcelService> csvExcelService = new Mock<ICSVFromExcelService>();
            ImportDefinision importDefinision = null;
            var sendScannerFile = new ExcelScannerFile(
                    FileUtilityLibraryConstants.CONSTScannerSetupFileToDump,
                    FileUtilityLibraryConstants.CONSTDirectoryToScan,
                    FileUtilityLibraryConstants.CONSCommaDelimiter,
                    FileUtilityLibraryConstants.CONSTHasHeader,
                    csvExcelService.Object);
            setupReposForTestWithImportDefinision(out scannerFile, out importRepo, out scannerRepo, importDefinision, sendScannerFile);
            scannerRepo.Setup(m => m.ScanForExceptions(It.IsAny<IScannerFile>())).Returns(false);
            Mock<ILog> logHandler = new Mock<ILog>();
            var directoryRepo = new DirectoryScannerReposetory(
                importRepo.Object, scannerRepo.Object, logHandler.Object,
                FileUtilityLibraryConstants.CONSTPartDirecotryToScan,
                FileUtilityLibraryConstants.CONSTPartDirecotryToMoveTo,
                CustomerImportInspectorConstants.CONSTEmailAddress.Split(';'),
                true);
            var fullFileName = FileUtilityLibraryConstants.CONSTDirectoryToScan + @"\" +
                FileUtilityLibraryConstants.CONSTScannerSetupFileToDump;

            directoryRepo.ScannCreatedFile(fullFileName);

            var emails = CustomerImportInspectorConstants.CONSTEmailAddress.Split(';');
            importRepo.Verify(m => m.EMailOrphenedFileToSupport(
                It.Is<string>(p => p == fullFileName),
                emails));
            scannerRepo.Verify(m => m.DeleteOrphanedFile(It.Is<string>(p => p == fullFileName)));
        }

        [TestMethod]
        public void Test_ScannCreatedFile_SendsOrphanedEmailIfNoImportDeffinisionIsFoundWithLiveSetup()
        {
            Mock<ILog> logHandler = new Mock<ILog>();
            var emailTempMock = new Mock<IEMailTemplateService>();
            var emailService = new Mock<IEmailService>();
            var retrevalMock = new Mock<ICustomerImportRetrievalService>();
            retrevalMock.Setup(m => m.GetCustomerImports())
                .Returns(() =>
                {
                    return new List<ImportDefinision>() {
                        new ImportDefinision()
                        {
                            ClientDatabase = FileUtilityLibraryConstants.CONSTClientName,
                            Delimiter = FileUtilityLibraryConstants.CONSCommaDelimiter.ToString(),
                            FailureEmailList = CustomerImportInspectorConstants.CONSTEmailAddress,
                            HasHeader = FileUtilityLibraryConstants.CONSTHasHeader,
                            ID = FileUtilityLibraryConstants.CONSTClientRecordID,
                            ImportFormat = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeExcel,
                            ImportName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName1,
                            ImportPath = @"\\amftp\ftp sites\rgbc\uploads\Profiles*.*",
                            IsEnabled = FileUtilityLibraryConstants.CONSTImportIsEnabled
                        }
                    };
                });
            var importRepo = new CustomerImportReposetory(
                FileUtilityLibraryConstants.CONSTDirectoryToScan,
                retrevalMock.Object,
                emailService.Object,
                emailTempMock.Object, logHandler.Object);
            Mock<IScannerRepository> scannerRepo = new Mock<IScannerRepository>();
            var directoryRepo = new DirectoryScannerReposetory(
                importRepo, scannerRepo.Object, logHandler.Object,
                FileUtilityLibraryConstants.CONSTPartDirecotryToScan,
                FileUtilityLibraryConstants.CONSTPartDirecotryToMoveTo,
                CustomerImportInspectorConstants.CONSTEmailAddress.Split(';'),
                true);

            directoryRepo.ScannCreatedFile(@"C:\Client\Britehouse\AMDevelopment\FileScannerConsole\DirectoryToWatch\RGBC\Upload\profiles20170704.xlsx");

            scannerRepo.Verify(m => m.DeleteOrphanedFile(It.IsAny<string>()));
        }

        [TestMethod]
        public void Test_ScannCreatedFile_ClosesAllReferences()
        {
            var importDefinision = new ImportDefinision()
            {
                ClientDatabase = FileUtilityLibraryConstants.CONSTClientName,
                Delimiter = FileUtilityLibraryConstants.CONSCommaDelimiter.ToString(),
                FailureEmailList = CustomerImportInspectorConstants.CONSTEmailAddress,
                HasHeader = FileUtilityLibraryConstants.CONSTHasHeader,
                ID = FileUtilityLibraryConstants.CONSTClientRecordID,
                ImportFormat = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeExcel,
                ImportName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName1,
                ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathBrokenOneSheetBinaryFile,
                IsEnabled = FileUtilityLibraryConstants.CONSTImportIsEnabled
            };
            Mock<ICustomerImportReposetory> importRepo;
            importRepo = new Mock<ICustomerImportReposetory>();
            importRepo.Setup(m => m.GetImportDefinisionFromFileName(It.IsAny<string>()))
                .Returns(() => importDefinision);
            importRepo.Setup(m => m.GetMoveToDirecotry(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(() =>
                    FileUtilityLibraryConstants.CONSTDirecoryToMoveTo);
            var mockMover = new Mock<IMoverService>();
            ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            IScannerRepository scannerRepo = new ScannerRepository(mockMover.Object, log);
            var directoryRepo = new DirectoryScannerReposetory(
                importRepo.Object, scannerRepo, log,
                FileUtilityLibraryConstants.CONSTPartDirecotryToScan,
                FileUtilityLibraryConstants.CONSTPartDirecotryToMoveTo,
                CustomerImportInspectorConstants.CONSTEmailAddress.Split(';'),
                true);
            var fullFileName = FileUtilityLibraryConstants.CONSTDirectoryToScan + @"\" +
                FileUtilityLibraryConstants.CONSTBrokenOneSheetBinaryFile;

            directoryRepo.ScannCreatedFile(fullFileName);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Test_ScannCreatesFile_SendsEmailToSupport()
        {
            var fullFileName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient2 + @"\" +
                FileUtilityLibraryConstants.CONSTScannerSetupFileToDump;
            var supportEmailAddresa = CustomerImportInspectorConstants.CONSTEmailAddress.Split(';')[0];
            var importDefinisionIn = new ImportDefinision()
            {
                ClientDatabase = FileUtilityLibraryConstants.CONSTClientName,
                Delimiter = FileUtilityLibraryConstants.CONSCommaDelimiter.ToString(),
                FailureEmailList = CustomerImportInspectorConstants.CONSTClientEmailAddress,
                HasHeader = FileUtilityLibraryConstants.CONSTHasHeader,
                ID = FileUtilityLibraryConstants.CONSTClientRecordID,
                ImportFormat = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeExcel,
                ImportName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName1,
                ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathWithMaskClient3,
                IsEnabled = FileUtilityLibraryConstants.CONSTImportIsEnabled
            };
            var logMock = new Mock<ILog>();
            var emailServiceMock = new Mock<IEmailService>();
            var emailTemplateServiceMock = new Mock<IEMailTemplateService>();
            emailTemplateServiceMock.Setup(m => m.GetSubjectText(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("Subject");
            emailTemplateServiceMock.Setup(m => m.GetWholeEmailBodyWithErrors(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()))
                .Returns("Whole body");
            var customerImportRetrivalMock = new Mock<ICustomerImportRetrievalService>();
            customerImportRetrivalMock.Setup(m => m.GetCustomerImports())
                .Returns(new List<ImportDefinision> { importDefinisionIn });
            var customerImportReposetory = new CustomerImportReposetory(
                FileUtilityLibraryConstants.CONSTCustomerImportDefinisionScanPath, customerImportRetrivalMock.Object, emailServiceMock.Object, emailTemplateServiceMock.Object,
                logMock.Object);
            var scannerFileMock = new Mock<IScannerFile>();
            scannerFileMock.Setup(p => p.ExceptionList)
                .Returns(new List<string> { });
            var fileMaskToScannerFile = new Mock<IFileMaskToScannerFile>();
            fileMaskToScannerFile.Setup(m => m.GetScannerFileInstance(It.IsAny<FileInfo>()))
                .Returns(scannerFileMock.Object);
            var scannerRepoMock = new Mock<IScannerRepository>();
            scannerRepoMock.Setup(p => p.FileMaskToScannerFile)
                .Returns(fileMaskToScannerFile.Object);
            scannerRepoMock.Setup(m => m.ScanForExceptions(It.IsAny<IScannerFile>()))
                .Returns(true);
            var directoryRepo = new DirectoryScannerReposetory(
               customerImportReposetory, scannerRepoMock.Object, logMock.Object,
               FileUtilityLibraryConstants.CONSTPartDirecotryToScan,
               FileUtilityLibraryConstants.CONSTPartDirecotryToMoveTo,
               CustomerImportInspectorConstants.CONSTEmailAddress.Split(';'),
               true);

            directoryRepo.ScannCreatedFile(fullFileName);

            emailServiceMock.Verify(m => m.SendEmailToRecipient(It.Is<string>(v => v == supportEmailAddresa), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
        }

        [TestMethod]
        public void Test_ScannCreatesFile_SendsEmailToCustomer()
        {
            var fullFileName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient2 + @"\" +
                FileUtilityLibraryConstants.CONSTScannerSetupFileToDump;
            var supportEmailAddresa = CustomerImportInspectorConstants.CONSTEmailAddress.Split(';')[0];
            var importDefinisionIn = new ImportDefinision()
            {
                ClientDatabase = FileUtilityLibraryConstants.CONSTClientName,
                Delimiter = FileUtilityLibraryConstants.CONSCommaDelimiter.ToString(),
                FailureEmailList = CustomerImportInspectorConstants.CONSTClientEmailAddress,
                HasHeader = FileUtilityLibraryConstants.CONSTHasHeader,
                ID = FileUtilityLibraryConstants.CONSTClientRecordID,
                ImportFormat = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeExcel,
                ImportName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName1,
                ImportPath = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathWithMaskClient3,
                IsEnabled = FileUtilityLibraryConstants.CONSTImportIsEnabled
            };
            var logMock = new Mock<ILog>();
            var emailServiceMock = new Mock<IEmailService>();
            var emailTemplateServiceMock = new Mock<IEMailTemplateService>();
            emailTemplateServiceMock.Setup(m => m.GetSubjectText(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("Subject");
            emailTemplateServiceMock.Setup(m => m.GetWholeEmailBodyWithErrors(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()))
                .Returns("Whole body");
            var customerImportRetrivalMock = new Mock<ICustomerImportRetrievalService>();
            customerImportRetrivalMock.Setup(m => m.GetCustomerImports())
                .Returns(new List<ImportDefinision> { importDefinisionIn });
            var customerImportReposetory = new CustomerImportReposetory(
                FileUtilityLibraryConstants.CONSTCustomerImportDefinisionScanPath, customerImportRetrivalMock.Object, emailServiceMock.Object, emailTemplateServiceMock.Object,
                logMock.Object);
            var scannerFileMock = new Mock<IScannerFile>();
            scannerFileMock.Setup(p => p.ExceptionList)
                .Returns(new List<string> { });
            var fileMaskToScannerFile = new Mock<IFileMaskToScannerFile>();
            fileMaskToScannerFile.Setup(m => m.GetScannerFileInstance(It.IsAny<FileInfo>()))
                .Returns(scannerFileMock.Object);
            var scannerRepoMock = new Mock<IScannerRepository>();
            scannerRepoMock.Setup(p => p.FileMaskToScannerFile)
                .Returns(fileMaskToScannerFile.Object);
            scannerRepoMock.Setup(m => m.ScanForExceptions(It.IsAny<IScannerFile>()))
                .Returns(true);
            var directoryRepo = new DirectoryScannerReposetory(
               customerImportReposetory, scannerRepoMock.Object, logMock.Object,
               FileUtilityLibraryConstants.CONSTPartDirecotryToScan,
               FileUtilityLibraryConstants.CONSTPartDirecotryToMoveTo,
               CustomerImportInspectorConstants.CONSTEmailAddress.Split(';'),
               false);

            directoryRepo.ScannCreatedFile(fullFileName);

            emailServiceMock.Verify(m => m.SendEmailToRecipient(It.Is<string>(v => v == CustomerImportInspectorConstants.CONSTClientEmailAddress), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
        }
    }
}
