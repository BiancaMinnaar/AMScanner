using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileUtilityLibrary.Interface.Service;
using FileUtilityLibrary.Interface.Repository;
using FileUtilityLibrary.Reposetory;
using System.Collections.Generic;
using FileUtilityLibrary.Interface.Model;
using Moq;
using System.IO;
using FileUtilityLibrary.ExpetionOccurrences;
using FileUtilityLibrary.Model.ScannerFile.Excel;
using FileUtilityLibrary.Model.ScannerFile;

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
            _ScannerRepository = new ScannerRepository(_MockMoverService.Object, fileMaskToScannerFile.Object, null);
            _ScannerRepository.MoveFileAfterScan(scannerFileMock.Object);

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
            fileMaskToScannerFile.Setup(m => m.GetScannerFileInstance(It.IsAny<FileInfo>()))
                .Returns(() => { return new CSVScannerFile(
                    FileUtilityLibraryConstants.CONSTScanFile1,
                    FileUtilityLibraryConstants.CONSTDirectoryToScan,
                    FileUtilityLibraryConstants.CONSTDelimiter,
                    true); });
            _ScannerRepository = new ScannerRepository(_MockMoverService.Object, fileMaskToScannerFile.Object, exeptionList);

            IList<string> exceptionList;
            _ScannerRepository.ScanForExceptions(new FileInfo(
                FileUtilityLibraryConstants.CONSTDirectoryToScan + @"\" + FileUtilityLibraryConstants.CONSTTestFile1),
                out exceptionList);

            someExceptionRule.Verify(t => t.ScanFile(It.IsAny<IScannerFile>()));
        }

        [TestMethod]
        public void Test_ScanForExceptions_CorrectlyScansMultipleSheetExcelFile()
        {
            _MockMoverService = new Moq.Mock<IMoverService>();
            _FileMaskToScannerFile = new Mock<IFileMaskToScannerFile>();
            _FileMaskToScannerFile.Setup(m => m.GetScannerFileInstance(It.IsAny<FileInfo>()))
               .Returns(() => {
                   return new ExcelScannerFile(
                       FileUtilityLibraryConstants.CONSTExcelFileWithError,
                       FileUtilityLibraryConstants.CONSTDirectoryToScan,
                       FileUtilityLibraryConstants.CONSCommaDelimiter,
                       true);
               });
            var PipeError = new HeaderColumnLineCountExceptionOccurrence("Pipe Error Found");
            _ExeptionList = new List<IExceptionOccurrence>() { PipeError };
            _ScannerRepository = new ScannerRepository(
                _MockMoverService.Object, 
                _FileMaskToScannerFile.Object, 
                _ExeptionList);

            IList<string> exceptionList;
            var hasErrors = _ScannerRepository.ScanForExceptions(new FileInfo(
                FileUtilityLibraryConstants.CONSTDirectoryToScan + @"\" + FileUtilityLibraryConstants.CONSTExcelFileWithError),
                out exceptionList);

            Assert.AreEqual(true, hasErrors);
        }

        [TestMethod]
        public void Test_ScanFileForException_CorrectlyScansMultipleSheetExcelFileWithNoErrors()
        {
            _MockMoverService = new Moq.Mock<IMoverService>();
            _FileMaskToScannerFile = new Mock<IFileMaskToScannerFile>();
            _FileMaskToScannerFile.Setup(m => m.GetScannerFileInstance(It.IsAny<FileInfo>()))
               .Returns(() => {
                   return new ExcelScannerFile(
                       FileUtilityLibraryConstants.CONSTExcelFileWithError,
                       FileUtilityLibraryConstants.CONSTDirectoryToScan,
                       FileUtilityLibraryConstants.CONSCommaDelimiter,
                       true);
               });
            var PipeError = new HeaderColumnLineCountExceptionOccurrence("Pipe Error Found");
            _ExeptionList = new List<IExceptionOccurrence>() { PipeError };
            _ScannerRepository = new ScannerRepository(
                _MockMoverService.Object,
                _FileMaskToScannerFile.Object,
                _ExeptionList);

            IList<string> exceptionList;
            var hasErrors = _ScannerRepository.ScanForExceptions(new FileInfo(
                FileUtilityLibraryConstants.CONSTDirectoryToScan + @"\" + FileUtilityLibraryConstants.CONSTExcelFileWithNoError),
                out exceptionList);

            Assert.AreEqual(true, hasErrors);
        }

        [TestMethod]
        public void Test_ScanForExceptions_ReturnsPropperErrors()
        {
            _MockMoverService = new Moq.Mock<IMoverService>();
            _FileMaskToScannerFile = new Mock<IFileMaskToScannerFile>();
            _FileMaskToScannerFile.Setup(m => m.GetScannerFileInstance(It.IsAny<FileInfo>()))
               .Returns(() => {
                   return new ExcelScannerFile(
                       FileUtilityLibraryConstants.CONSTExcelFileWithError,
                       FileUtilityLibraryConstants.CONSTDirectoryToScan,
                       FileUtilityLibraryConstants.CONSCommaDelimiter,
                       true);
               });
            var PipeError = new HeaderColumnLineCountExceptionOccurrence(FileUtilityLibraryConstants.CONSTHeaderErrorErrorMessage);
            _ExeptionList = new List<IExceptionOccurrence>() { PipeError };
            _ScannerRepository = new ScannerRepository(
                _MockMoverService.Object,
                _FileMaskToScannerFile.Object,
                _ExeptionList);

            IList<string> exceptionList;
            var hasErrors = _ScannerRepository.ScanForExceptions(new FileInfo(
                FileUtilityLibraryConstants.CONSTDirectoryToScan + @"\" + FileUtilityLibraryConstants.CONSTExcelFileWithError),
                out exceptionList);

            Assert.AreNotEqual(null, exceptionList);
            Assert.IsTrue(exceptionList[0].Contains(FileUtilityLibraryConstants.CONSTHeaderErrorErrorMessage));
        }

        [TestMethod]
        public void Test_ScanForExceptions_ReturnsNoErrorsInCollectionOnPropperFile()
        {
            _MockMoverService = new Moq.Mock<IMoverService>();
            _FileMaskToScannerFile = new Mock<IFileMaskToScannerFile>();
            _FileMaskToScannerFile.Setup(m => m.GetScannerFileInstance(It.IsAny<FileInfo>()))
               .Returns(() => {
                   return new ExcelScannerFile(
                       FileUtilityLibraryConstants.CONSTExcelFileWithNoError,
                       FileUtilityLibraryConstants.CONSTDirectoryToScan,
                       FileUtilityLibraryConstants.CONSCommaDelimiter,
                       true);
               });
            var PipeError = new HeaderColumnLineCountExceptionOccurrence(FileUtilityLibraryConstants.CONSTHeaderErrorErrorMessage);
            _ExeptionList = new List<IExceptionOccurrence>() { PipeError };
            _ScannerRepository = new ScannerRepository(
                _MockMoverService.Object,
                _FileMaskToScannerFile.Object,
                _ExeptionList);

            IList<string> exceptionList;
            var hasErrors = _ScannerRepository.ScanForExceptions(new FileInfo(
                FileUtilityLibraryConstants.CONSTDirectoryToScan + @"\" + FileUtilityLibraryConstants.CONSTExcelFileWithNoError),
                out exceptionList);

            Assert.AreEqual(0, exceptionList.Count);
        }
    }
}
