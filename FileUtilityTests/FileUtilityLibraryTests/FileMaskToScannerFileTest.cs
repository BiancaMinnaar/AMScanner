using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileUtilityLibrary.Model;
using System.IO;

namespace FileUtilityTests.FileUtilityLibraryTests
{
    [TestClass]
    public class FileMaskToScannerFileTest
    {
        [TestMethod]
        public void Test_GetScannerFileInstance_ReturnsCorrectScannerFile()
        {
            var FileMastT = new FileMaskToScannerFile(
                FileUtilityLibraryConstants.CONSTCorrectFileMask,
                FileUtilityLibraryConstants.CONSTDelimiter,
                true, FileUtilityLibraryConstants.CONSTCustomerImportDefinitionTypeExcel);

            var scannerFile = FileMastT.GetScannerFileInstance(new FileInfo(
                FileUtilityLibraryConstants.CONSTDirectoryToScan + @"\" + FileUtilityLibraryConstants.CONSTTestFile1));

            Assert.AreNotEqual(null, scannerFile);
        }
    }
}
