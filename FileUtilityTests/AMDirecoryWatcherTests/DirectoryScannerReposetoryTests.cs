using AMCustomerImportInspector.Interface;
using AMCustomerImportInspector.Model;
using AMDirectoryWatcher.Reposetory;
using FileUtilityLibrary.ExpetionOccurrences;
using FileUtilityLibrary.Interface.Model;
using FileUtilityLibrary.Interface.Repository;
using FileUtilityLibrary.Interface.Service;
using FileUtilityLibrary.Model.ScannerFile.Excel;
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
                CustomerImportInspectorConstants.CONSTEmailAddress.Split(';'));

            directoryRepo.ScannCreatedFile("");

            importRepo.Verify(m => m.EMailOrphenedFileToSupport(It.IsAny<string>(), It.IsAny<string[]>()), "Email wasn't sent");
            scannerRepo.Verify(m => m.DeleteOrphanedFile(It.IsAny<string>()), "The File wasn't deleted");
        }

        private void setupReposForTestWithImportDefinision(
            out IScannerFile scannerFile, 
            out Mock<ICustomerImportReposetory> importRepo, 
            out Mock<IScannerRepository> scannerRepo,
            ImportDefinision importDefinision)
        {
            Mock<IMoverService> moverService = new Mock<IMoverService>();

            var sendScannerFile = new ExcelScannerFile(
                    FileUtilityLibraryConstants.CONSTScannerSetupFileToDump,
                    FileUtilityLibraryConstants.CONSTDirectoryToScan,
                    FileUtilityLibraryConstants.CONSCommaDelimiter,
                    FileUtilityLibraryConstants.CONSTHasHeader);
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
            var importDefinision = new ImportDefinision()
            {
                ClientDatabase = FileUtilityLibraryConstants.CONSTClientName,
                Delimiter = FileUtilityLibraryConstants.CONSCommaDelimiter.ToString(),
                FailureEmailList = CustomerImportInspectorConstants.CONSTEmailAddress,
                HasHeader = FileUtilityLibraryConstants.CONSTHasHeader,
                ID = FileUtilityLibraryConstants.CONSTClientRecordID,
                ImportFormat = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeExcel,
                ImportName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName1,
                ImportPath = FileUtilityLibraryConstants.CONSTDirectoryToScan,
                IsEnabled = FileUtilityLibraryConstants.CONSTImportIsEnabled
            };
            setupReposForTestWithImportDefinision(out scannerFile, out importRepo, out scannerRepo, importDefinision);
            Mock<ILog> logHandler = new Mock<ILog>();
            var directoryRepo = new DirectoryScannerReposetory(
                importRepo.Object, scannerRepo.Object, logHandler.Object,
                FileUtilityLibraryConstants.CONSTPartDirecotryToScan,
                FileUtilityLibraryConstants.CONSTPartDirecotryToMoveTo,
                CustomerImportInspectorConstants.CONSTEmailAddress.Split(';'));
            var fullFileName = FileUtilityLibraryConstants.CONSTDirectoryToScan + @"\" +
                FileUtilityLibraryConstants.CONSTScannerSetupFileToDump;

            directoryRepo.ScannCreatedFile(fullFileName);

            importRepo.Verify(m => m.GetMoveToDirecotry(
                It.Is<string>(p => p == FileUtilityLibraryConstants.CONSTDirectoryToScan),
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
            var importDefinision = new ImportDefinision()
            {
                ClientDatabase = FileUtilityLibraryConstants.CONSTClientName,
                Delimiter = FileUtilityLibraryConstants.CONSCommaDelimiter.ToString(),
                FailureEmailList = CustomerImportInspectorConstants.CONSTEmailAddress,
                HasHeader = FileUtilityLibraryConstants.CONSTHasHeader,
                ID = FileUtilityLibraryConstants.CONSTClientRecordID,
                ImportFormat = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeExcel,
                ImportName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName1,
                ImportPath = FileUtilityLibraryConstants.CONSTDirectoryToScan,
                IsEnabled = FileUtilityLibraryConstants.CONSTImportIsEnabled
            };
            setupReposForTestWithImportDefinision(out scannerFile, out importRepo, out scannerRepo, importDefinision);
            scannerRepo.Setup(m => m.ScanForExceptions(It.IsAny<IScannerFile>())).Returns(true);
            Mock<ILog> logHandler = new Mock<ILog>();
            var directoryRepo = new DirectoryScannerReposetory(
                importRepo.Object, scannerRepo.Object, logHandler.Object,
                FileUtilityLibraryConstants.CONSTPartDirecotryToScan,
                FileUtilityLibraryConstants.CONSTPartDirecotryToMoveTo,
                CustomerImportInspectorConstants.CONSTEmailAddress.Split(';'));
            var fullFileName = FileUtilityLibraryConstants.CONSTDirectoryToScan + @"\" +
                FileUtilityLibraryConstants.CONSTScannerSetupFileToDump;

            directoryRepo.ScannCreatedFile(fullFileName);

            importRepo.Verify(m => m.GetMoveToDirecotry(
                It.Is<string>(p => p == FileUtilityLibraryConstants.CONSTDirectoryToScan),
                It.Is<string>(p => p == FileUtilityLibraryConstants.CONSTPartDirecotryToScan),
                It.Is<string>(p => p == FileUtilityLibraryConstants.CONSTPartDirecotryToMoveTo)),
                "Email wasn't sent");
            scannerRepo.Verify(m => m.ScanForExceptions(It.Is<IScannerFile>(p => p == scannerFile)));
            scannerRepo.Verify(m => m.MoveFileAfterScan(It.Is<IScannerFile>(p => p == scannerFile)));
        }

        [TestMethod]
        public void Test_ScannCreatedFile_EmailsCustomerAndDeletesFileOnUnsuccessfullScan()
        {
            IScannerFile scannerFile;
            Mock<ICustomerImportReposetory> importRepo;
            Mock<IScannerRepository> scannerRepo;
            var importDefinision = new ImportDefinision()
            {
                ClientDatabase = FileUtilityLibraryConstants.CONSTClientName,
                Delimiter = FileUtilityLibraryConstants.CONSCommaDelimiter.ToString(),
                FailureEmailList = CustomerImportInspectorConstants.CONSTEmailAddress,
                HasHeader = FileUtilityLibraryConstants.CONSTHasHeader,
                ID = FileUtilityLibraryConstants.CONSTClientRecordID,
                ImportFormat = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeExcel,
                ImportName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionName1,
                ImportPath = FileUtilityLibraryConstants.CONSTDirectoryToScan,
                IsEnabled = FileUtilityLibraryConstants.CONSTImportIsEnabled
            };
            setupReposForTestWithImportDefinision(out scannerFile, out importRepo, out scannerRepo, importDefinision);
            scannerRepo.Setup(m => m.ScanForExceptions(It.IsAny<IScannerFile>())).Returns(false);
            Mock<ILog> logHandler = new Mock<ILog>();
            var directoryRepo = new DirectoryScannerReposetory(
                importRepo.Object, scannerRepo.Object, logHandler.Object,
                FileUtilityLibraryConstants.CONSTPartDirecotryToScan,
                FileUtilityLibraryConstants.CONSTPartDirecotryToMoveTo,
                CustomerImportInspectorConstants.CONSTEmailAddress.Split(';'));
            var fullFileName = FileUtilityLibraryConstants.CONSTDirectoryToScan + @"\" +
                FileUtilityLibraryConstants.CONSTScannerSetupFileToDump;

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
            scannerRepo.Verify(m => m.DeleteFaultyFile(It.Is<IScannerFile>(p => p == scannerFile)));
        }

        [TestMethod]
        public void Test_ScannCreatedFile_EmailsSupportAndDeletesOrphanedFile()
        {
            IScannerFile scannerFile;
            Mock<ICustomerImportReposetory> importRepo;
            Mock<IScannerRepository> scannerRepo;
            ImportDefinision importDefinision = null;
            setupReposForTestWithImportDefinision(out scannerFile, out importRepo, out scannerRepo, importDefinision);
            scannerRepo.Setup(m => m.ScanForExceptions(It.IsAny<IScannerFile>())).Returns(false);
            Mock<ILog> logHandler = new Mock<ILog>();
            var directoryRepo = new DirectoryScannerReposetory(
                importRepo.Object, scannerRepo.Object, logHandler.Object,
                FileUtilityLibraryConstants.CONSTPartDirecotryToScan,
                FileUtilityLibraryConstants.CONSTPartDirecotryToMoveTo,
                CustomerImportInspectorConstants.CONSTEmailAddress.Split(';'));
            var fullFileName = FileUtilityLibraryConstants.CONSTDirectoryToScan + @"\" +
                FileUtilityLibraryConstants.CONSTScannerSetupFileToDump;

            directoryRepo.ScannCreatedFile(fullFileName);

            var emails = CustomerImportInspectorConstants.CONSTEmailAddress.Split(';');
            importRepo.Verify(m => m.EMailOrphenedFileToSupport(
                It.Is<string>(p => p == fullFileName),
                emails));
            scannerRepo.Verify(m => m.DeleteOrphanedFile(It.Is<string>(p => p == fullFileName)));
        }
    }
}
