using System;
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

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
