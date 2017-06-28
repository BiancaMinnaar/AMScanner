using System.Collections.Generic;
using System.IO;

namespace FileUtilityLibrary.Model.ScannerFile
{
    public class CSVScannerFile : BaseScannerFile
    {
        public CSVScannerFile(string fileName, string filePath, char delimiter, bool hasHeader)
            : base(fileName, filePath, delimiter, hasHeader)
        {
            var fullFileName = FilePath + @"\" + FileName;
            _StreamReader = new StreamReader(fullFileName);
            ExceptionList = new List<string>();
        }

        public override bool HasSubStructures()
        {
            return false;
        }

        ~CSVScannerFile()
        {
            if (_StreamReader != null)
                _StreamReader.Close();
        }
    }
}
