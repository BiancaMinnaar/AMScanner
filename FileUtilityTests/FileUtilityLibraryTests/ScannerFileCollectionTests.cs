using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileUtilityLibrary.Model;
using FileUtilityLibrary.Model.ScannerFile;

namespace FileUtilityTests
{
    [TestClass]
    public class ScannerFileCollectionTests
    {
        [TestMethod]
        public void TestGetScannerFilesWithExceptionsReturnsFilesWithExeptions()
        {
            //assign
            ScannerFileCollection<CSVScannerFile> testList = 
                new ScannerFileCollection<CSVScannerFile>() {
                    new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile1,FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader) { HasException = false },
                    new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile2,FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader) { HasException = false },
                    new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile3,FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader) { HasException = true }
                };

            //action
            var testedAction = testList.GetScannerFilesWithExceptions();

            //assert
            Assert.AreEqual(testedAction.Count, 1, "The correct number of items didn't come through");
        }

        [TestMethod]
        public void TestGetScannerFilesWithExceptionsReturnsFilesWithExeptionsWHileSupplyingNone()
        {
            //assign
            ScannerFileCollection<CSVScannerFile> testList =
                new ScannerFileCollection<CSVScannerFile>() {
                    new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile1,FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader) { HasException = false },
                    new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile2,FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader) { HasException = false },
                    new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile3,FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader) { HasException = false }
                };

            //action
            var testedAction = testList.GetScannerFilesWithExceptions();

            //assert
            Assert.AreEqual(testedAction.Count, 0, "The correct number of items didn't come through");
        }

        [TestMethod]
        public void TestGetScannerFilesWithExceptionsReturnsFilesWithExeptionsWHileSupplyingAll()
        {
            //assign
            var testList =
                new ScannerFileCollection<CSVScannerFile>() {
                    new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile1,FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader) { HasException = true },
                    new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile2,FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader) { HasException = true },
                    new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile3,FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader) { HasException = true }
                };

            //action
            var testedAction = testList.GetScannerFilesWithExceptions();

            //assert
            Assert.AreEqual(testedAction.Count, 3, "The correct number of items didn't come through");
        }

        [TestMethod]
        public void TestGetScannerFilesWithNoExceptionsReturnsFilesWithNoExeptions()
        {
            //assign
            var testList =
                new ScannerFileCollection<CSVScannerFile>() {
                    new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile1,FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader) { HasException = false },
                    new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile2,FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader) { HasException = false },
                    new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile3,FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader) { HasException = true }
                };

            //action
            var testedAction = testList.GetScannerFilesWithNoExceptions();

            //assert
            Assert.AreEqual(testedAction.Count, 2, "The correct number of items didn't come through");
        }

        [TestMethod]
        public void TestGetScannerFilesWithNoExceptionsReturnsFilesWithNoExeptionsWHileSupplyingNone()
        {
            //assign
            var testList =
                new ScannerFileCollection<CSVScannerFile>() {
                    new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile1,FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader) { HasException = false },
                    new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile2,FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader) { HasException = false },
                    new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile3,FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader) { HasException = false }
                };

            //action
            var testedAction = testList.GetScannerFilesWithNoExceptions();

            //assert
            Assert.AreEqual(testedAction.Count, 3, "The correct number of items didn't come through");
        }

        [TestMethod]
        public void TestGetScannerFilesWithNoExceptionsReturnsFilesWithNoExeptionsWHileSupplyingAll()
        {
            //assign
            var testList =
                new ScannerFileCollection<CSVScannerFile>() {
                    new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile1,FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader) { HasException = true },
                    new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile2,FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader) { HasException = true },
                    new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile3,FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader) { HasException = true }
                };

            //action
            var testedAction = testList.GetScannerFilesWithNoExceptions();

            //assert
            Assert.AreEqual(testedAction.Count, 0, "The correct number of items didn't come through");
        }

        [TestMethod]
        public void TestGetScannerFilesWithNoExceptionsReturnsFilesWithOutExeptionsWhileSupplyingUnAssignedBooleans()
        {
            //assign
            var testList =
                new ScannerFileCollection<CSVScannerFile>() {
                    new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile1,FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader),
                    new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile2,FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader),
                    new CSVScannerFile(FileUtilityLibraryConstants.CONSTTestFile3,FileUtilityLibraryConstants.CONSTDirectoryToScan, FileUtilityLibraryConstants.CONSTDelimiter, FileUtilityLibraryConstants.CONSTHasHeader)
                };

            //action
            var testedAction = testList.GetScannerFilesWithNoExceptions();

            //assert
            Assert.AreEqual(3, testedAction.Count, "The correct number of items didn't come through");
        }
    }
}
