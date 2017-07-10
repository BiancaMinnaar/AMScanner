using FileUtilityLibrary.Interface.Service;
using System;
using System.IO;

namespace FileUtilityLibrary.Model.ScannerFile.Excel
{
    public class ExcelScannerFile : BaseScannerFile, IDisposable
    {
        private ICSVWithExcelAutomationService excelService;
        private int currentStructureNumber;
        private MemoryStream[] streamForSheets;

        public ExcelScannerFile(string fileName, string filePath, char delimiter, bool hasHeader, 
            ICSVWithExcelAutomationService excelService) 
            : base(fileName, filePath, delimiter, hasHeader)
        {
            this.Delimiter = ',';
            this.currentStructureNumber = 0;
            this.excelService = excelService;
            setCurrentReader(currentStructureNumber);
        }

        public override bool HasSubStructures()
        {
            bool hasSubStructures = false;
            if (streamForSheets != null && streamForSheets.Length - 1 > currentStructureNumber)
            {
                hasSubStructures = true;
                currentStructureNumber++;
                setCurrentReader(currentStructureNumber);
            }
            return hasSubStructures;
        }


        private void setCurrentReader(int arrayItem)
        {
            if (streamForSheets == null)
            {
                streamForSheets = excelService.GetSheetStreamsFromDocument();
            }
            _StreamReader = new StreamReader(streamForSheets[arrayItem]);
        }

        public override void Dispose()
        {
            base.Dispose();
            var log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Debug("Excel ScannerFile being disposed.");
            foreach (MemoryStream stream in streamForSheets)
            {
                log.Debug("stream being flused");
                stream.Flush();
                log.Debug("stream being closed");
                stream.Close();
                log.Debug("stream closed");
                stream.Dispose();
                log.Debug("stream Disposed");
            }
            log.Debug("Excel ScannerFile disposed");
        }
    }
}
