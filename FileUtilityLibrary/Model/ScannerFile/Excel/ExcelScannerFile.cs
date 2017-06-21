using FileUtilityLibrary.Interface.Model;
using System.Collections.Generic;
using System.IO;

namespace FileUtilityLibrary.Model.ScannerFile.Excel
{
    public class ExcelScannerFile : IScannerFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public char Delimiter { get; set; }
        public bool HasHeader { get; set; }
        public bool HasException { get; set; }
        public IList<string> ExceptionList { get; set; }
        public bool HasSubStructures()
        {
            bool hasSubStructures = false;
            if (_StreamForSheets != null && _StreamForSheets.Length-1 > _CurrentStructureNumber)
            {
                hasSubStructures = true;
                _CurrentStructureNumber++;
                setCurrentReader(_CurrentStructureNumber);
            }
            return hasSubStructures;
        }
        private int _CurrentStructureNumber;
        private StreamReader _StreamReader;
        private MemoryStream[] _StreamForSheets;

        public ExcelScannerFile(string fileName, string filePath, char delimiter, bool hasHeader)
        {
            FileName = fileName;
            FilePath = filePath;
            Delimiter = delimiter;
            HasHeader = hasHeader;
            ExceptionList = new List<string>();
            _CurrentStructureNumber = 0;
            setCurrentReader(_CurrentStructureNumber);
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

        public void Close()
        {
            _StreamReader.Close();
        }

        public FileInfo GetFileInfo()
        {
            return new FileInfo(FilePath + @"\" + FileName);
        }

        public int Peek()
        {
            return _StreamReader.Peek();
        }

        public int Read()
        {
            return _StreamReader.Read();
        }

        public string ReadLine()
        {
            return _StreamReader.ReadLine();
        }
    }
}
