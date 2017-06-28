using FileUtilityLibrary.Interface.Model;
using FileUtilityLibrary.Model.ScannerFile.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUtilityLibrary.Model.ScannerFile
{
    public class ScannerFileFactory
    {

        public IScannerFile GetScannerFile(string fileName, string filePath, char delimiter, bool hasHeader, string maskType)
        {
            switch (maskType)
            {
                case "EXCEL":
                    return new ExcelScannerFile(fileName, filePath, delimiter, hasHeader);
                case "CSV":
                    return new CSVScannerFile(fileName, filePath, delimiter, hasHeader);
            }

            return null;
        }
    }
}
