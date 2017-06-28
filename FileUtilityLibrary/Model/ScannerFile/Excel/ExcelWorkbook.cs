using FileUtilityLibrary.Interface.Model.ScannerFIle.Excel;
using Microsoft.Office.Interop.Excel;
using System;
using System.IO;

namespace FileUtilityLibrary.Model.ScannerFile.Excel
{
    public class ExcelWorkbook : IExcelWorkBook, IDisposable
    {
        public Workbook CurrentWorkBook { get; set; }
        public int WorkbookSheetCount { get; set; }
        private Application _ExcelApp;
        private string _FullFileName;

        public ExcelWorkbook(string fullFileName)
        {
            _FullFileName = fullFileName;
        }

        private void setExcelApplication()
        {
            _ExcelApp = new Application();
            _ExcelApp.DisplayAlerts = false;
            CurrentWorkBook = _ExcelApp.Workbooks.Open(_FullFileName);
            WorkbookSheetCount = CurrentWorkBook.Sheets.Count;
        }

        public MemoryStream[] GetScannableData()
        {
            Type officeType = Type.GetTypeFromProgID("Excel.Application");
            if (officeType == null)
            {
                throw new Exception("Excel is not installed");
            }
            setExcelApplication();
            var csvStreamsToScan = WorkbookSheetCount > -1 ? new MemoryStream[WorkbookSheetCount] : null;

            try
            {
                for (int sheetCount = 1; sheetCount <= WorkbookSheetCount; sheetCount++)
                {
                    MemoryStream csvStream = new MemoryStream();
                    Worksheet sheet = CurrentWorkBook.Sheets[sheetCount];
                    var tempFileName = _FullFileName + ".Temp.xlsx";
                    sheet.SaveAs(tempFileName, XlFileFormat.xlCSV);
                    closeWorkBook();
                    setExcelApplication();
                    var tempFile = new FileInfo(tempFileName);
                    FileStream fileStream = tempFile.OpenRead();
                    fileStream.CopyTo(csvStream);
                    fileStream.Flush();
                    fileStream.Close();
                    tempFile.Delete();
                    csvStream.Flush();
                    csvStream.Position = 0;
                    csvStreamsToScan[sheetCount - 1] = csvStream;
                    
                }
            }
            catch (Exception excp)
            {
                throw excp;
            }
            finally
            {
                closeWorkBook();
            }

            return csvStreamsToScan;
        }

        private void closeWorkBook()
        {
            if (CurrentWorkBook != null)
                CurrentWorkBook.Close();
            if (_ExcelApp != null)
                _ExcelApp.Quit();
        }

        public void Dispose()
        {
            closeWorkBook();
        }
    }
}
