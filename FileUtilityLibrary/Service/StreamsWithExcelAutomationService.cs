using System;
using System.IO;
using FileUtilityLibrary.Interface.Service;
using log4net;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;

namespace FileUtilityLibrary.Service
{
    public class StreamsWithExcelAutomationService : ICSVFromExcelService
    {
        private ILog _LogInterface;
        private string _ExcelFileName;
        private bool _ExcelAvailable;
        private Application _ExcelApp;
        private Workbook _CurrentWorkBook;
        private Sheets _AllSheets;
        private bool _AutomationIsSet;
        private char _Delimiter;

        public StreamsWithExcelAutomationService(string fullFileName, char delimiter, ILog log)
        {
            _ExcelFileName = fullFileName;
            _LogInterface = log;
            _ExcelAvailable = hasExcel();
            _AutomationIsSet = false;
            _Delimiter = delimiter;
        }

        private bool hasExcel()
        {
            var officeType = Type.GetTypeFromProgID("Excel.Application");
            var excelIsInstalled = officeType != null;
            if (!excelIsInstalled)
            {
                _LogInterface.Fatal("Excel is not installed!!");
            }
            return excelIsInstalled;
        }

        private void setExcelAutomation()
        {
            _ExcelApp = new Application();
            _ExcelApp.DisplayAlerts = false;
            _CurrentWorkBook = _ExcelApp.Workbooks.Open(_ExcelFileName);
            _AllSheets = _CurrentWorkBook.Sheets;
            _AutomationIsSet = true;
        }

        private void closeExcelAutomation()
        {
            if (_AllSheets != null)
            {
                Marshal.FinalReleaseComObject(_AllSheets);
                _AllSheets = null;
            }
            if (_CurrentWorkBook != null)
            {
                _CurrentWorkBook.Close();
                Marshal.FinalReleaseComObject(_CurrentWorkBook);
                _CurrentWorkBook = null;
            }

            if (_ExcelApp != null)
            {
                _ExcelApp.Quit();
                Marshal.FinalReleaseComObject(_ExcelApp);
                _ExcelApp = null;
            }

            _AutomationIsSet = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public MemoryStream[] GetSheetStreamsFromDocument()
        {
            MemoryStream[] csvStreamsToScan = null;
            if (_ExcelAvailable)
            {
                try
                {
                    setExcelAutomation();
                    var numberOfSheets = _AllSheets.Count;
                    csvStreamsToScan = numberOfSheets > -1 ? new MemoryStream[numberOfSheets] : null;

                    for (int sheetCounter = 1; sheetCounter <= numberOfSheets; sheetCounter++)
                    {
                        if (!_AutomationIsSet)
                        {
                            setExcelAutomation();
                        }
                        var sheet = (_Worksheet)_AllSheets[sheetCounter];
                        //var tempFileName = excelFileName + ".Temp.xlsx";
                        //sheet.SaveAs(tempFileName, XlFileFormat.xlCSV);
                        csvStreamsToScan[sheetCounter - 1] = getSheetStream(sheet);
                        Marshal.FinalReleaseComObject(sheet);
                        closeExcelAutomation();

                    }
                }
                catch (Exception excp)
                {
                    _LogInterface.Fatal("excel Automation error");
                    _LogInterface.Fatal(excp.Message);
                    _LogInterface.Fatal(excp.StackTrace);
                }
                finally
                {
                    closeExcelAutomation();
                }
            }
            return csvStreamsToScan;
        }

        private MemoryStream getSheetStream(_Worksheet sheet)
        {
            var usedRange = sheet.UsedRange;
            MemoryStream excelStream = new MemoryStream();
            StreamWriter writeExcel = new StreamWriter(excelStream);
            List<object> disposeChain = new List<object>();
            var rows = usedRange.Rows;
            
            for (var rowCount = 1; rowCount <= rows.Count; rowCount++)
            {
                var row = rows[rowCount];
                for (int i = 0; i < row.Columns.Count; i++)
                {
                    var cell = row.Cells[1, i + 1];
                    if (cell.Value2 != null)
                    {
                        writeExcel.Write(cell.Value2.ToString());
                    }
                    if (rowCount < rows.Count)
                    {
                        writeExcel.Write(_Delimiter);
                    }

                    disposeChain.Add(cell);
                }
                writeExcel.Write("\n\r");
                disposeChain.Add(row);              
            }
            disposeChain.Add(rows);
            disposeChain.Add(usedRange);
            disposeComObjects(disposeChain.ToArray());

            writeExcel.Flush();
            writeExcel.Close();
            excelStream.Flush();
            excelStream.Position = 0;

            return excelStream;
        }

        private void disposeComObjects(params object[] toDispose)
        {
            for(var counter = 0; counter < toDispose.Length; counter++)
            {
                Marshal.FinalReleaseComObject(toDispose);
            }
        }

        ~StreamsWithExcelAutomationService()
        {
            closeExcelAutomation();
        }
    }
}
