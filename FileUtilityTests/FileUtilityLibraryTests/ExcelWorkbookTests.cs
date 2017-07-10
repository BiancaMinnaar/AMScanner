using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileUtilityLibrary.Model.ScannerFile.Excel;
using System.IO;
using Moq;
using log4net;
using FileUtilityLibrary.Service;

namespace FileUtilityTests
{
    [TestClass]
    public class CSVWithExcelAutomationServiceTests
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
        public void Test_GetSheetStreamsFromDocument_ReturnsReadableStreamData()
        {
            var logMock = new Mock<ILog>();
            var excelService = new CSVWithExcelAutomationService(
                FileUtilityLibraryConstants.CONSTDirectoryToScan + "/" + FileUtilityLibraryConstants.CONSTExcelFileWithNoError,
                logMock.Object);
            var streams = excelService.GetSheetStreamsFromDocument();
            TextReader reader = new StreamReader(streams[0]);
            var streamData = reader.ReadLine();
            reader.Close();

            Assert.AreNotEqual(0, streamData.Length);
        }
    }
}
