using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileUtilityLibrary.Service;
using FileUtilityLibrary.Interface.Service;
using FileUtilityLibrary.Service.Helper;
using Moq;
using FileUtilityLibrary.Interface.Helper;
using System.IO;
using FileUtilityLibrary.Interface.Model;
using FileUtilityLibrary.Model;
using FileUtilityLibrary.Model.ScannerFile;

namespace FileUtilityTests
{
    [TestClass]
    public class ScannerServiceTests
    {
        [TestMethod]
        public void TestScannerFindsFileInDirectory()
        {
            var direcotryToScan = Constants.CONSTDirectoryToScan;
            IScannerService scanner = new ScannerService(new DirecotryHelper(direcotryToScan));

            var hasFiles = scanner.DirectoryHasFile("*.*");

            Assert.IsTrue(hasFiles, "No Files were found");
        }

        [TestMethod]
        public void TestScannerOnlyFindsFilesInMask()
        {
            var direcotryToScan = Constants.CONSTDirectoryToScan;
            IScannerService scanner = new ScannerService(new DirecotryHelper(direcotryToScan));

            var hasFiles = scanner.DirectoryHasFile(Constants.CONSTCorrectFileMask);

            Assert.IsTrue(hasFiles, "No Files were found");
        }

        [TestMethod]
        public void TestScannerDoesNotFindsFilesNotInMask()
        {
            var direcotryToScan = Constants.CONSTDirectoryToScan;
            IScannerService scanner = new ScannerService(new DirecotryHelper(direcotryToScan));

            var hasFiles = scanner.DirectoryHasFile(Constants.CONSTInCorrectFileMask);

            Assert.IsFalse(hasFiles, "Some Files were found");
        }

        [TestMethod]
        public void TestThatScannerReturnsListOfFilesInMask()
        {
            var direcotryToScan = Constants.CONSTDirectoryToScan;
            IScannerService scanner = new ScannerService(new DirecotryHelper(direcotryToScan));
            var FileMaskToScannerFiles = new FileMaskToScannerFile<IScannerFile>(Constants.CONSTCorrectFileMask, Constants.CONSTDelimiter, Constants.CONSTHasHeader, (n, p, d, h) => new CSVScannerFile(n, p, d, h));

            var filesToScan = scanner.GetFilesToScan(FileMaskToScannerFiles);

            Assert.AreNotEqual(0, filesToScan.Count, "No Files were found");
        }

        [TestMethod]
        public void TestThatGetFilesToScanPopulatesFileNameCorrectly()
        {
            var factory = new MockRepository(MockBehavior.Loose);
            var directoryHelper = factory.Create<IDirectoryHelper>();
            FileInfo[] fileInfos = new FileInfo[2] { new FileInfo(Constants.CONSTDirectoryToScan + @"\" + Constants.CONSTScanFile1), new FileInfo(Constants.CONSTDirectoryToScan + @"\" + Constants.CONSTScanFile1) };
            directoryHelper.Setup(t => t.GetFiles(It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(fileInfos);
            IScannerService scanner = new ScannerService(directoryHelper.Object);
            var FileMaskToScannerFiles = new FileMaskToScannerFile<IScannerFile>(Constants.CONSTCorrectFileMask, Constants.CONSTDelimiter, Constants.CONSTHasHeader, (n, p, d, h) => new CSVScannerFile(n, p, d, h));

            var filesToScan = scanner.GetFilesToScan(FileMaskToScannerFiles);

            Assert.AreEqual(filesToScan[0].FileName, Constants.CONSTScanFile1, "GetFilesToScan incorrectly populated the FileName");
        }

        [TestMethod]
        public void TestThatGetFilesToScanPopulatesFilePathCorrectly()
        {
            var factory = new MockRepository(MockBehavior.Loose);
            var directoryHelper = factory.Create<IDirectoryHelper>();
            FileInfo[] fileInfos = new FileInfo[2] { new FileInfo(Constants.CONSTDirectoryToScan + @"\" + Constants.CONSTScanFile1), new FileInfo(Constants.CONSTDirectoryToScan + @"\" + Constants.CONSTScanFile1) };
            directoryHelper.Setup(t => t.GetFiles(It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(fileInfos);
            IScannerService scanner = new ScannerService(directoryHelper.Object);
            var FileMaskToScannerFiles = new FileMaskToScannerFile<IScannerFile>(Constants.CONSTCorrectFileMask, Constants.CONSTDelimiter, Constants.CONSTHasHeader, (n, p, d, h) => new CSVScannerFile(n, p, d, h));

            var filesToScan = scanner.GetFilesToScan(FileMaskToScannerFiles);

            Assert.AreEqual(filesToScan[0].FilePath, Constants.CONSTDirectoryToScan, "GetFilesToScan incorrectly populated the FilePath");
        }
    }
}
