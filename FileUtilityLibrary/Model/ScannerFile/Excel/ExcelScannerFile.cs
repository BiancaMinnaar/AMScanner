using System;
using System.IO;

namespace FileUtilityLibrary.Model.ScannerFile.Excel
{
    public class ExcelScannerFile : BaseScannerFile, IDisposable
    {
        private int _CurrentStructureNumber;
        private MemoryStream[] _StreamForSheets;

        public ExcelScannerFile(string fileName, string filePath, char delimiter, bool hasHeader) 
            : base(fileName, filePath, delimiter, hasHeader)
        {
            this.Delimiter = ',';
            _CurrentStructureNumber = 0;
            setCurrentReader(_CurrentStructureNumber);
        }

        public override bool HasSubStructures()
        {
            bool hasSubStructures = false;
            if (_StreamForSheets != null && _StreamForSheets.Length - 1 > _CurrentStructureNumber)
            {
                hasSubStructures = true;
                _CurrentStructureNumber++;
                setCurrentReader(_CurrentStructureNumber);
            }
            return hasSubStructures;
        }


        private void setCurrentReader(int arrayItem)
        {
            if (_StreamForSheets == null)
            {
                _StreamForSheets = getExcelStreamsForSheets(FilePath + @"\" + FileName);
            }
            _StreamReader = new StreamReader(_StreamForSheets[arrayItem]);
        }

        private MemoryStream[] getExcelStreamsForSheets(string fullFileName)
        {
            ExcelWorkbook book = new ExcelWorkbook(fullFileName);
            var csvStreamsToScan = book.GetScannableData();
            
            return csvStreamsToScan;
        }

        public override void Dispose()
        {
            base.Dispose();
            var log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Debug("Excel ScannerFile being disposed.");
            foreach (MemoryStream stream in _StreamForSheets)
            {
                log.Debug("stream being flused");
                stream.Flush();
                log.Debug("stream being closed");
                stream.Close();
                log.Debug("stream closed");
            }
            log.Debug("Excel ScannerFile disposed");
        }
    }
}
