using System;
using System.IO;
using FileUtilityLibrary.Interface.Service;
using log4net;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace FileUtilityLibrary.Service
{
    public class CSVWithExcelAutomationService : ICSVWithExcelAutomationService
    {
        private ILog logInterface;
        private string excelFileName;
        private bool excelAvailable;
        private Application excelApp;
        private Workbook currentWorkBook;
        private bool automationIsSet;

        public CSVWithExcelAutomationService(string fullFileName, ILog log)
        {
            excelFileName = fullFileName;
            logInterface = log;
            excelAvailable = hasExcel();
            automationIsSet = false;
        }

        private bool hasExcel()
        {
            var officeType = Type.GetTypeFromProgID("Excel.Application");
            logInterface.Fatal("Excel is not installed!!");
            return officeType != null;
        }

        private void setExcelAutomation()
        {
            excelApp = new Application();
            excelApp.DisplayAlerts = false;
            currentWorkBook = excelApp.Workbooks.Open(excelFileName);
            automationIsSet = true;
        }

        private void closeExcelAutomation(params object[] cleanups)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            foreach (object obj in cleanups)
            {
                Marshal.FinalReleaseComObject(obj);
            }
            
            currentWorkBook.Close();
            Marshal.FinalReleaseComObject(currentWorkBook);
            currentWorkBook = null;
            excelApp.Quit();
            Marshal.FinalReleaseComObject(excelApp);
            excelApp = null;
            automationIsSet = false;
        }

        

        
        //TestForExcel
        //Open Workbook
        //Return sheet streams
        //save file as CSV
        //Reset and return stream
        //close workbook

        public MemoryStream[] GetSheetStreamsFromDocument()
        {
            MemoryStream[] csvStreamsToScan = null;
            if (excelAvailable)
            {
                try
                {
                    setExcelAutomation();
                    var sheetCount = currentWorkBook.Sheets.Count;
                    csvStreamsToScan = sheetCount > -1 ? new MemoryStream[sheetCount] : null;

                    for (int sheetCounter = 1; sheetCounter <= sheetCount; sheetCounter++)
                    {
                        if (!automationIsSet)
                        {
                            setExcelAutomation();
                        }
                        MemoryStream csvStream = new MemoryStream();
                        var sheet = currentWorkBook.Sheets[sheetCount];
                        var tempFileName = excelFileName + ".Temp.xlsx";
                        sheet.SaveAs(tempFileName, XlFileFormat.xlCSV);
                        closeExcelAutomation(sheet);

                        csvStreamsToScan[sheetCount - 1] = getSheetStream(tempFileName);
                    }
                }
                catch(Exception excp)
                {
                    logInterface.Fatal("excel Automation error");
                    logInterface.Fatal(excp.Message);
                    logInterface.Fatal(excp.StackTrace);
                }
            }
            return csvStreamsToScan;
        }

        private MemoryStream getSheetStream(string tempFileName)
        {
            var tempFile = new FileInfo(tempFileName);
            MemoryStream csvStream = new MemoryStream();

            FileStream fileStream = tempFile.OpenRead();
            fileStream.CopyTo(csvStream);
            fileStream.Flush();
            fileStream.Close();
            tempFile.Delete();
            csvStream.Flush();
            csvStream.Position = 0;

            return csvStream;
        }
    }
}
