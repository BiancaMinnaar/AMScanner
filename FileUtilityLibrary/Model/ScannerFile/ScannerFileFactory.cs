using FileUtilityLibrary.Interface.Model;
using FileUtilityLibrary.Model.ScannerFile.Excel;
using FileUtilityLibrary.Service;
using log4net;

namespace FileUtilityLibrary.Model.ScannerFile
{
    public class ScannerFileFactory
    {
        private ILog logHandler;

        public ScannerFileFactory(ILog logHandler)
        {
            this.logHandler = logHandler;
        }

        public IScannerFile GetScannerFile(string fileName, string filePath, char delimiter, bool hasHeader, string maskType)
        {
            switch (maskType)
            {
                case "EXCEL":
                    return new ExcelScannerFile(fileName, filePath, delimiter, hasHeader, 
                        new CSVWithExcelAutomationService(filePath + @"\" + fileName, this.logHandler));
                case "CSV":
                    return new CSVScannerFile(fileName, filePath, delimiter, hasHeader);
            }

            return null;
        }
    }
}
