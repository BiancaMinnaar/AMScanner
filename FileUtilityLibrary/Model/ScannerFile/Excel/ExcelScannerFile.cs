using FileUtilityLibrary.Interface.Model;
using System.Collections.Generic;
using System.IO;

namespace FileUtilityLibrary.Model.ScannerFile.Excel
{
    public class ExcelScannerFile : BaseScannerFile
    {
        private int _CurrentStructureNumber;
        private MemoryStream[] _StreamForSheets;

        public ExcelScannerFile(string fileName, string filePath, char delimiter, bool hasHeader) 
            : base(fileName, filePath, delimiter, hasHeader)
        {
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
    }
}
