using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileUtilityLibrary.Interface.Service;
using FileUtilityLibrary.Interface.Repository;
using FileUtilityLibrary.Reposetory;
using System.Collections.Generic;
using FileUtilityLibrary.Interface.Model;
using Moq;
using System.IO;
using FileUtilityLibrary.ExpetionOccurrences;
using FileUtilityLibrary.Model.ScannerFile;
using log4net;
using FileUtilityLibrary.Model.ScannerFile.Excel;

namespace FileUtilityTests
{
    [TestClass]
    public class ScannerReposetoryTests
    {
        private Moq.Mock<IMoverService> _MockMoverService;
        private IScannerRepository _ScannerRepository;
        private Mock<IFileMaskToScannerFile> _FileMaskToScannerFile;
        private IList<IExceptionOccurrence> _ExeptionList;

        [TestMethod]
        public void Test_MoveFileAfterScan_MovesFiles()
        {
            _MockMoverService = new Moq.Mock<IMoverService>();
            Mock<IScannerFile> scannerFileMock = new Mock<IScannerFile>();
            scannerFileMock.Setup(t => t.HasException).Returns(false);
            Mock<IFileMaskToScannerFile> fileMaskToScannerFile = new Mock<IFileMaskToScannerFile>();
            fileMaskToScannerFile.Setup(t => t.FileMask).Returns(FileUtilityLibraryConstants.CONSTCorrectFileMask);
            var logMock = new Mock<ILog>();
            _ScannerRepository = new ScannerRepository(_MockMoverService.Object, fileMaskToScannerFile.Object, null, logMock.Object);
            var fullFileName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient1 + @"\" +
                FileUtilityLibraryConstants.CONSTScannerSetupFileToDump;
            _ScannerRepository.MoveFileAfterScan(fullFileName);

            _MockMoverService.Verify(t => t.MoveFilesInList(It.IsAny<FileInfo[]>()));
        }

        [TestMethod]
        public void Test_ScansFileForException_CallsScanFile()
        {
            _MockMoverService = new Moq.Mock<IMoverService>();
            var someExceptionRule = new Mock<IExceptionOccurrence>();
            IList<IExceptionOccurrence> exeptionList = new List<IExceptionOccurrence>()
            {
                someExceptionRule.Object
            };
            Mock<IScannerFile> scannerFileMock = new Mock<IScannerFile>();
            Mock<IFileMaskToScannerFile> fileMaskToScannerFile = new Mock<IFileMaskToScannerFile>();
            fileMaskToScannerFile.Setup(t => t.FileMask).Returns(FileUtilityLibraryConstants.CONSTCorrectFileMask);
            fileMaskToScannerFile.Setup(m => m.ImportFormat).Returns(FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeCSV);
            var scannerFile = new CSVScannerFile(
                    FileUtilityLibraryConstants.CONSTScanFile1,
                    FileUtilityLibraryConstants.CONSTDirectoryToScan,
                    FileUtilityLibraryConstants.CONSTDelimiter,
                    true);
            var logMock = new Mock<ILog>();
            _ScannerRepository = new ScannerRepository(_MockMoverService.Object, fileMaskToScannerFile.Object, exeptionList, 
                logMock.Object);

            _ScannerRepository.ScanForExceptions(scannerFile);

            someExceptionRule.Verify(t => t.ScanFile(It.IsAny<IScannerFile>()));
        }

        [TestMethod]
        public void Test_ScanForExceptions_CorrectlyScansMultipleSheetExcelFile()
        {
            _MockMoverService = new Moq.Mock<IMoverService>();
            _FileMaskToScannerFile = new Mock<IFileMaskToScannerFile>();
            var scannerFileMock = new Mock<IScannerFile>();
            scannerFileMock.Setup(p => p.HasException).Returns(true);
            scannerFileMock.Setup(p => p.ExceptionList).Returns(new List<string>());
            scannerFileMock.Setup(p => p.HasHeader).Returns(true);

            var scannerFile = new ExcelScannerFile(
                    FileUtilityLibraryConstants.CONSTExcelFileWithError,
                    FileUtilityLibraryConstants.CONSTDirectoryToScan,
                    //FileUtilityLibraryConstants.CONSCommaDelimiter,
                    FileUtilityLibraryConstants.CONSTDelimiter,
                    true);
            var PipeError = new HeaderColumnLineCountExceptionOccurrence("Delimiter Error found");
            _ExeptionList = new List<IExceptionOccurrence>() { PipeError };
            var exceptionListMock = new Mock<List<IExceptionOccurrence>>();
            var logMock = new Mock<ILog>();
            _ScannerRepository = new ScannerRepository(
                _MockMoverService.Object, 
                _FileMaskToScannerFile.Object,
                _ExeptionList,
                logMock.Object);

            var hasErrors = _ScannerRepository.ScanForExceptions(scannerFile);

            Assert.AreEqual(true, hasErrors);
        }

        [TestMethod]
        public void Test_ScanFileForException_CorrectlyScansMultipleSheetExcelFileWithNoErrors()
        {
            _MockMoverService = new Moq.Mock<IMoverService>();
            _FileMaskToScannerFile = new Mock<IFileMaskToScannerFile>();
            var scannerFile = new CSVScannerFile(
                     FileUtilityLibraryConstants.CONSTExcelFileWithNoError,
                     FileUtilityLibraryConstants.CONSTDirectoryToScan,
                     FileUtilityLibraryConstants.CONSCommaDelimiter,
                     true);
            var PipeError = new HeaderColumnLineCountExceptionOccurrence("Pipe Error Found");
            _ExeptionList = new List<IExceptionOccurrence>() { PipeError };
            var logMock = new Mock<ILog>();
            _ScannerRepository = new ScannerRepository(
                _MockMoverService.Object,
                _FileMaskToScannerFile.Object,
                _ExeptionList,
                logMock.Object);

            var hasErrors = _ScannerRepository.ScanForExceptions(scannerFile);

            Assert.AreEqual(true, hasErrors);
        }

        [TestMethod]
        public void Test_ScanForExceptions_ReturnsPropperErrors()
        {
            _MockMoverService = new Moq.Mock<IMoverService>();
            _FileMaskToScannerFile = new Mock<IFileMaskToScannerFile>();
            var scannerFile = new CSVScannerFile(
                     FileUtilityLibraryConstants.CONSTExcelFileWithError,
                     FileUtilityLibraryConstants.CONSTDirectoryToScan,
                     FileUtilityLibraryConstants.CONSCommaDelimiter,
                     true);
            var PipeError = new HeaderColumnLineCountExceptionOccurrence(FileUtilityLibraryConstants.CONSTHeaderErrorErrorMessage);
            _ExeptionList = new List<IExceptionOccurrence>() { PipeError };
            var logMock = new Mock<ILog>();
            _ScannerRepository = new ScannerRepository(
                _MockMoverService.Object,
                _FileMaskToScannerFile.Object,
                _ExeptionList,
                logMock.Object);

            var hasErrors = _ScannerRepository.ScanForExceptions(scannerFile);

            Assert.AreNotEqual(null, scannerFile.ExceptionList);
            Assert.IsTrue(scannerFile.ExceptionList[0].Contains(FileUtilityLibraryConstants.CONSTHeaderErrorErrorMessage));
        }

        [TestMethod]
        public void Test_ScanForExceptions_ReturnsNoErrorsInCollectionOnPropperFile()
        {
            _MockMoverService = new Moq.Mock<IMoverService>();
            _FileMaskToScannerFile = new Mock<IFileMaskToScannerFile>();
            var scannerFile = new CSVScannerFile(
                     FileUtilityLibraryConstants.CONSTScanFile1,
                     FileUtilityLibraryConstants.CONSTDirectoryToScan,
                     FileUtilityLibraryConstants.CONSTDelimiter,
                     true);
            var PipeError = new HeaderColumnLineCountExceptionOccurrence(FileUtilityLibraryConstants.CONSTHeaderErrorErrorMessage);
            _ExeptionList = new List<IExceptionOccurrence>() { PipeError };
            var logMock = new Mock<ILog>();
            _ScannerRepository = new ScannerRepository(
                _MockMoverService.Object,
                _FileMaskToScannerFile.Object,
                _ExeptionList,
                logMock.Object);

            var hasErrors = _ScannerRepository.ScanForExceptions(scannerFile);

            Assert.AreEqual(0, scannerFile.ExceptionList.Count);
        }

        [TestMethod]
        public void Test_DeleteFaultyFile_DeletesScannerFileWithErrors()
        {
            _MockMoverService = new Moq.Mock<IMoverService>();
            Mock<IScannerFile> scannerFileMock = new Mock<IScannerFile>();
            scannerFileMock.Setup(t => t.HasException).Returns(true);
            Mock<IFileMaskToScannerFile> fileMaskToScannerFile = new Mock<IFileMaskToScannerFile>();
            fileMaskToScannerFile.Setup(t => t.FileMask).Returns(FileUtilityLibraryConstants.CONSTCorrectFileMask);
            var logMock = new Mock<ILog>();
            _ScannerRepository = new ScannerRepository(_MockMoverService.Object, fileMaskToScannerFile.Object, null, logMock.Object);
            var fullFileName = FileUtilityLibraryConstants.CONSTCustomerImportDefinitionPathClient1 + @"\" +
                FileUtilityLibraryConstants.CONSTScannerSetupFileToDump;
            _ScannerRepository.DeleteFaultyFile(fullFileName);

            _MockMoverService.Verify(t => t.DeleteFilesInList(It.IsAny<FileInfo[]>()));
        }

        [TestMethod]
        public void Test_DeleteOrphanedFile_DeletesFile()
        {
            _MockMoverService = new Moq.Mock<IMoverService>();
            Mock<IScannerFile> scannerFileMock = new Mock<IScannerFile>();
            Mock<IFileMaskToScannerFile> fileMaskToScannerFile = new Mock<IFileMaskToScannerFile>();
            var logMock = new Mock<ILog>();
            _ScannerRepository = new ScannerRepository(_MockMoverService.Object, fileMaskToScannerFile.Object, null, logMock.Object);
            _ScannerRepository.DeleteOrphanedFile(
                FileUtilityLibraryConstants.CONSTDirectoryToScan + @"\" + FileUtilityLibraryConstants.CONSTTestFile3);

            _MockMoverService.Verify(t => t.DeleteFilesInList(It.IsAny<FileInfo[]>()));
        }
    }
}
