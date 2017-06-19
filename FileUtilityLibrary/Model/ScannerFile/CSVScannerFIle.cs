using FileUtilityLibrary.Interface.Model;
using System.Collections.Generic;
using System.IO;

namespace FileUtilityLibrary.Model.ScannerFile
{
    public class CSVScannerFile : IScannerFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public char Delimiter { get; set; }
        public bool HasHeader { get; set; }
        public bool HasException { get; set; }
        public IList<string> ExceptionList { get; set; }
        private StreamReader _StreamReader;

        public CSVScannerFile(string fileName, string filePath, char delimiter, bool hasHeader)
        {
            FileName = fileName;
            FilePath = filePath;
            Delimiter = delimiter;
            HasHeader = hasHeader;
            var fullFileName = FilePath + @"\" + FileName;
            _StreamReader = new StreamReader(fullFileName);
            ExceptionList = new List<string>();
        }

        public bool HasSubStructures()
        {
            return false;
        }

        public int Peek()
        {
            return _StreamReader.Peek();
        }

        public string ReadLine()
        {
            return _StreamReader.ReadLine();
        }

        public int Read()
        {
            return _StreamReader.Read();
        }

        public void Close()
        {
            _StreamReader.Close();
        }

        public FileInfo GetFileInfo()
        {
            return new FileInfo(FilePath + @"\" + FileName);
        }

        ~CSVScannerFile()
        {
            if (_StreamReader != null)
                _StreamReader.Close();
        }
    }
}
