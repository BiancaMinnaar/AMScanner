using System;
using System.IO;
using FileUtilityLibrary.Interface.Service;
using log4net;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace FileUtilityLibrary.Service
{
    public class CSVWithExcelAutomationService : ICSVFromExcelService
    {
        private ILog logInterface;
        private string excelFileName;
        private bool excelAvailable;
        private Application excelApp;
        private Workbook currentWorkBook;
        private Sheets allSheets;
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
            var excelIsInstalled = officeType != null;
            if (!excelIsInstalled)
            {
                logInterface.Fatal("Excel is not installed!!");
            }
            return excelIsInstalled;
        }

        private void setExcelAutomation()
        {
            excelApp = new Application();
            excelApp.DisplayAlerts = false;
            currentWorkBook = excelApp.Workbooks.Open(excelFileName);
            allSheets = currentWorkBook.Sheets;
            automationIsSet = true;
        }

        private void closeExcelAutomation()
        {
            if (allSheets != null)
            {
                Marshal.FinalReleaseComObject(allSheets);
                allSheets = null;
            }
            if (currentWorkBook != null)
            {
                currentWorkBook.Close();
                Marshal.FinalReleaseComObject(currentWorkBook);
                currentWorkBook = null;
            }

            if (excelApp != null)
            {
                excelApp.Quit();
                Marshal.FinalReleaseComObject(excelApp);
                excelApp = null;
            }

            automationIsSet = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public MemoryStream[] GetSheetStreamsFromDocument()
        {
            MemoryStream[] csvStreamsToScan = null;
            if (excelAvailable)
            {
                try
                {
                    setExcelAutomation();
                    var numberOfSheets = allSheets.Count;
                    csvStreamsToScan = numberOfSheets > -1 ? new MemoryStream[numberOfSheets] : null;

                    for (int sheetCounter = 1; sheetCounter <= numberOfSheets; sheetCounter++)
                    {
                        if (!automationIsSet)
                        {
                            setExcelAutomation();
                        }
                        var sheet = allSheets[sheetCounter];
                        var tempFileName = excelFileName + ".Temp.xlsx";
                        sheet.SaveAs(tempFileName, XlFileFormat.xlCSV);
                        Marshal.FinalReleaseComObject(sheet);
                        closeExcelAutomation();

                        csvStreamsToScan[sheetCounter - 1] = getSheetStream(tempFileName);
                    }
                }
                catch(Exception excp)
                {
                    logInterface.Fatal("excel Automation error");
                    logInterface.Fatal(excp.Message);
                    logInterface.Fatal(excp.StackTrace);
                }
                finally
                {
                    closeExcelAutomation();
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

        ~CSVWithExcelAutomationService()
        {
            closeExcelAutomation();
        }
    }
}
