using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileUtilityLibrary.Model.ScannerFile.Excel;
using System.IO;

namespace FileUtilityTests
{
    [TestClass]
    public class ExcelWorkbookTests
    {
        //[TestMethod]
        //public void TestExcelWorkbookGetScannableDataReturnsMultipleStreamsFOrMultipleWorksheets()
        //{
        //    var workBook = new ExcelWorkbook(FileUtilityLibraryConstants.CONSTDirectoryToScan + "/" + FileUtilityLibraryConstants.CONSTExcelFileWithError);
        //    workBook.GetScannableData();
        //    var streamCount = workBook.WorkbookSheetCount;
        //    Assert.AreEqual(2, streamCount);
        //}

        [TestMethod]
        public void TestExcelWorkbookGetScannableDataReturnsReadableStreamData()
        {
            var workBook = new ExcelWorkbook(FileUtilityLibraryConstants.CONSTDirectoryToScan + "/" + FileUtilityLibraryConstants.CONSTExcelFileWithError);
            var streams = workBook.GetScannableData();
            TextReader reader = new StreamReader(streams[0]);
            var streamData = reader.ReadLine();
            reader.Close();

            Assert.AreNotEqual(0, streamData.Length);
        }
    }
}
