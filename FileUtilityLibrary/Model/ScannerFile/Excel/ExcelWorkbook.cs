using FileUtilityLibrary.Interface.Model.ScannerFIle.Excel;
using log4net;
using Microsoft.Office.Interop.Excel;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace FileUtilityLibrary.Model.ScannerFile.Excel
{
    public class ExcelWorkbook : IExcelWorkBook, IDisposable
    {
        private Application ExcelApp;
        private Workbook CurrentWorkBook;
        private string _FullFileName;
        ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ExcelWorkbook(string fullFileName)
        {
            _FullFileName = fullFileName;
        }

        private int setExcelApplication()
        {
            try
            {
                log.Debug("Scanner set Excel spreadsheet");
                ExcelApp = new Application();
                ExcelApp.DisplayAlerts = false;
                log.Debug("ExcelApp setup");
                CurrentWorkBook = ExcelApp.Workbooks.Open(_FullFileName);
            }
            catch
            {
                log.Debug("ExcelApp Exception");
                GC.Collect();
                GC.WaitForPendingFinalizers();

                //cleanup
                Marshal.ReleaseComObject(ExcelApp);
                Marshal.ReleaseComObject(CurrentWorkBook);
            }

            return CurrentWorkBook != null ? CurrentWorkBook.Sheets.Count : 0;
        }

        public MemoryStream[] GetScannableData()
        {
            var officeType = Type.GetTypeFromProgID("Excel.Application");
            if (officeType == null)
            {
                throw new Exception("Excel is not installed");
            }
            var workbookSheetCount = setExcelApplication();
            var csvStreamsToScan = workbookSheetCount > -1 ? new MemoryStream[workbookSheetCount] : null;

            try
            {
                for (int sheetCount = 1; sheetCount <= workbookSheetCount; sheetCount++)
                {
                    MemoryStream csvStream = new MemoryStream();
                    var sheet = CurrentWorkBook.Sheets[sheetCount];
                    var tempFileName = _FullFileName + ".Temp.xlsx";
                    sheet.SaveAs(tempFileName, XlFileFormat.xlCSV);
                    closeWorkBook(sheet);
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

        private void closeWorkBook(params object[] cleanups)
        {
            if (CurrentWorkBook != null)
            {
                CurrentWorkBook.Close();
                log.Debug("Closing Workbook - Close");
            }
            if (ExcelApp != null)
            {
                ExcelApp.Quit();
                log.Debug("Closing Workbook - Quit");
            }
            log.Debug("Closing Workbook - done");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            foreach (object obj in cleanups)
            {
                Marshal.ReleaseComObject(obj);
            }
            Marshal.ReleaseComObject(CurrentWorkBook);
            Marshal.ReleaseComObject(ExcelApp);
            log.Debug("Marshaled all exel objects");
        }

        public void Dispose()
        {
            closeWorkBook();
        }
    }
}