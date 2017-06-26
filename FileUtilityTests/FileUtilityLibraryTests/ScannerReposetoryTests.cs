﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileUtilityLibrary.Interface.Service;
using FileUtilityLibrary.Interface.Repository;
using FileUtilityLibrary.Reposetory;
using System.Collections.Generic;
using FileUtilityLibrary.Model;
using FileUtilityLibrary.Interface.Model;
using Moq;
using System.IO;
using FileUtilityLibrary.Model.ScannerFile;
using FileUtilityLibrary.ExpetionOccurrences;
using FileUtilityLibrary.Model.ScannerFile.Excel;

namespace FileUtilityTests
{
    [TestClass]
    public class ScannerReposetoryTests
    {
        private Moq.Mock<IScannerService> _MockScannerService;
        private Moq.Mock<IMoverService> _MockMoverService;
        private IScannerRepository _ScannerRepository;
        private Mock<IFileMaskToScannerFile<IScannerFile>> _FileMaskToScannerFile;
        private Mock<IExceptionOccurrence> _SomeExceptionRule;
        private IList<IExceptionOccurrence> _ExeptionList;

        [TestMethod]
        public void TestThatFindFilesToScanReturnsCallsDirecotryHasFile()
        {
            _MockScannerService = new Moq.Mock<IScannerService>();
            _MockMoverService = new Moq.Mock<IMoverService>();
            Mock<IFileMaskToScannerFile<IScannerFile>> fileMaskToScannerFile = new Mock<IFileMaskToScannerFile<IScannerFile>>();
            fileMaskToScannerFile.Setup(t => t.GetFileMask()).Returns(FileUtilityLibraryConstants.CONSTCorrectFileMask);
            _ScannerRepository = new ScannerRepository(
                _MockScannerService.Object, _MockMoverService.Object, fileMaskToScannerFile.Object,  null);

            _ScannerRepository.FindFilesToScan();
            _MockScannerService.Verify(t => t.DirectoryHasFile(FileUtilityLibraryConstants.CONSTCorrectFileMask));
        }

        [TestMethod]
        public void TestThatFindFilesToScanReturnsCallsGetFilesToScan()
        {
            _MockScannerService = new Moq.Mock<IScannerService>();
            _MockScannerService.Setup(t => t.DirectoryHasFile(FileUtilityLibraryConstants.CONSTCorrectFileMask)).Returns(true);
            _MockMoverService = new Moq.Mock<IMoverService>();
            Mock<IFileMaskToScannerFile<IScannerFile>> fileMaskToScannerFile = new Mock<IFileMaskToScannerFile<IScannerFile>>();
            fileMaskToScannerFile.Setup(t => t.GetFileMask()).Returns(FileUtilityLibraryConstants.CONSTCorrectFileMask);
            _ScannerRepository = new ScannerRepository(_MockScannerService.Object, _MockMoverService.Object, fileMaskToScannerFile.Object, null);

            _ScannerRepository.FindFilesToScan();
            _MockScannerService.Verify(t => t.GetFilesToScan(fileMaskToScannerFile.Object));
        }

        [TestMethod]
        public void TestThatGetFilesToScanReturnsCorrectScannerFileList()
        {
            _MockScannerService = new Moq.Mock<IScannerService>();
            _MockScannerService.Setup(t => t.DirectoryHasFile(FileUtilityLibraryConstants.CONSTCorrectFileMask)).Returns(true);
            var scanFilesToReturn = new ScannerFileCollection<IScannerFile>()
            {
                new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile1, FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader),
                new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile2, FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader)
            };
            Mock<IFileMaskToScannerFile<IScannerFile>> fileMaskToScannerFile = new Mock<IFileMaskToScannerFile<IScannerFile>>();
            fileMaskToScannerFile.Setup(t => t.GetFileMask()).Returns(FileUtilityLibraryConstants.CONSTCorrectFileMask);
            _MockScannerService.Setup(t => t.GetFilesToScan(fileMaskToScannerFile.Object)).Returns(scanFilesToReturn);
            _MockMoverService = new Moq.Mock<IMoverService>();
            _ScannerRepository = new ScannerRepository(_MockScannerService.Object, _MockMoverService.Object, fileMaskToScannerFile.Object, null);

            var returnedFiles = _ScannerRepository.FindFilesToScan();

            Assert.AreEqual(scanFilesToReturn, returnedFiles, "The Returned files weren't the same");
        }

        [TestMethod]
        public void TestThatRepoMovesFiles()
        {
            _MockScannerService = new Moq.Mock<IScannerService>();
            _MockMoverService = new Moq.Mock<IMoverService>();
            Mock<IScannerFile> scannerFileMock = new Mock<IScannerFile>();
            scannerFileMock.Setup(t => t.HasException).Returns(false);
            Mock<IFileMaskToScannerFile<IScannerFile>> fileMaskToScannerFile = new Mock<IFileMaskToScannerFile<IScannerFile>>();
            fileMaskToScannerFile.Setup(t => t.GetFileMask()).Returns(FileUtilityLibraryConstants.CONSTCorrectFileMask);
            _ScannerRepository = new ScannerRepository(_MockScannerService.Object, _MockMoverService.Object, fileMaskToScannerFile.Object, null);
            _ScannerRepository.MoveFileAfterScan(scannerFileMock.Object);

            _MockMoverService.Verify(t => t.MoveFilesInList(It.IsAny<FileInfo[]>()));
        }

        [TestMethod]
        public void TestThatRepoScansFileForException()
        {
            _MockScannerService = new Moq.Mock<IScannerService>();
            _MockMoverService = new Moq.Mock<IMoverService>();
            var someExceptionRule = new Mock<IExceptionOccurrence>();
            IList<IExceptionOccurrence> exeptionList = new List<IExceptionOccurrence>()
            {
                someExceptionRule.Object
            };
            Mock<IScannerFile> scannerFileMock = new Mock<IScannerFile>();
            Mock<IFileMaskToScannerFile<IScannerFile>> fileMaskToScannerFile = new Mock<IFileMaskToScannerFile<IScannerFile>>();
            fileMaskToScannerFile.Setup(t => t.GetFileMask()).Returns(FileUtilityLibraryConstants.CONSTCorrectFileMask);
            _ScannerRepository = new ScannerRepository(_MockScannerService.Object, _MockMoverService.Object, fileMaskToScannerFile.Object, exeptionList);

            _ScannerRepository.ScanFileForException(scannerFileMock.Object);

            someExceptionRule.Verify(t => t.ScanFile(It.IsAny<IScannerFile>()));
        }

        [TestMethod]
        public void TestScannerReposetoryScanFileForExceptionCorrectlyScansMultipleSheetExcelFile()
        {
            _MockScannerService = new Moq.Mock<IScannerService>();
            //SetupFilesToScan
            _MockScannerService.Setup(t => t.GetFilesToScan(It.IsAny<IFileMaskToScannerFile<IScannerFile>>()))
                .Returns(() =>
                {
                    return new ScannerFileCollection<IScannerFile>()
                    {
                        new ExcelScannerFile(FileUtilityLibraryConstants.CONSTExcelFileWithError, FileUtilityLibraryConstants.CONSTDirectoryToScan, ';', true)
                    };
                });
            _MockMoverService = new Moq.Mock<IMoverService>();
            _FileMaskToScannerFile = new Mock<IFileMaskToScannerFile<IScannerFile>>();
            var PipeError = new HeaderColumnLineCountExceptionOccurrence("Pipe Error Found");
            _ExeptionList = new List<IExceptionOccurrence>() { PipeError };
            _ScannerRepository = new ScannerRepository(
                _MockScannerService.Object, 
                _MockMoverService.Object, 
                _FileMaskToScannerFile.Object, 
                _ExeptionList);

            var scanFile = new ExcelScannerFile(FileUtilityLibraryConstants.CONSTExcelFileWithError, FileUtilityLibraryConstants.CONSTDirectoryToScan, ';', true);
            _ScannerRepository.ScanFileForException(scanFile);

            Assert.AreEqual(true, scanFile.HasException);
        }

        [TestMethod]
        public void TestScannerReposetoryScanFileForExceptionCorrectlyScansMultipleSheetExcelFileWithNoErrors()
        {
            _MockScannerService = new Moq.Mock<IScannerService>();
            //SetupFilesToScan
            _MockScannerService.Setup(t => t.GetFilesToScan(It.IsAny<IFileMaskToScannerFile<IScannerFile>>()))
                .Returns(() =>
                {
                    return new ScannerFileCollection<IScannerFile>()
                    {
                        new ExcelScannerFile(FileUtilityLibraryConstants.CONSTExcelFileWithNoError, FileUtilityLibraryConstants.CONSTDirectoryToScan, ';', true)
                    };
                });
            _MockMoverService = new Moq.Mock<IMoverService>();
            _FileMaskToScannerFile = new Mock<IFileMaskToScannerFile<IScannerFile>>();
            var PipeError = new HeaderColumnLineCountExceptionOccurrence("Pipe Error Found");
            _ExeptionList = new List<IExceptionOccurrence>() { PipeError };
            _ScannerRepository = new ScannerRepository(
                _MockScannerService.Object,
                _MockMoverService.Object,
                _FileMaskToScannerFile.Object,
                _ExeptionList);

            var scanFile = new ExcelScannerFile(FileUtilityLibraryConstants.CONSTExcelFileWithNoError, FileUtilityLibraryConstants.CONSTDirectoryToScan, ';', true);
            _ScannerRepository.ScanFileForException(scanFile);

            Assert.AreEqual(false, scanFile.HasException);
        }
    }
}
