using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileUtilityLibrary.Model;
using System.IO;
using Moq;
using log4net;

namespace FileUtilityTests.FileUtilityLibraryTests
{
    [TestClass]
    public class FileMaskToScannerFileTest
    {
        [TestMethod]
        public void Test_GetScannerFileInstance_ReturnsCorrectScannerFile()
        {
            var logHandler = new Mock<ILog>();
            var FileMastT = new FileMaskToScannerFile(
                FileUtilityLibraryConstants.CONSTCorrectFileMask,
                FileUtilityLibraryConstants.CONSTDelimiter,
                true, FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeExcel,
                logHandler.Object);

            var scannerFile = FileMastT.GetScannerFileInstance(new FileInfo(
                FileUtilityLibraryConstants.CONSTDirectoryToScan + @"\" + FileUtilityLibraryConstants.CONSTTestFile1));

            Assert.AreNotEqual(null, scannerFile);
        }
    }
}
