using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileUtilityTests.FileUtilityLibraryTests
{
    [TestClass]
    public class StreamsWithExcelAutomationService
    {
        [TestMethod]
        public void Test_GetSheetStreamsFromDocument_ReturnsReadableStreamData()
        {
            var logMock = new Mock<ILog>();
            var excelService = new StreamsWithExcelAutomationService(
                FileUtilityLibraryConstants.CONSTDirectoryToScan + "/" + FileUtilityLibraryConstants.CONSTExcelFileWithNoError,
                '|',
                logMock.Object);
            var streams = excelService.GetSheetStreamsFromDocument();
            TextReader reader = new StreamReader(streams[0]);
            var streamData = reader.ReadLine();
            reader.Close();

            Assert.AreNotEqual(0, streamData.Length);
        }
    }
}
