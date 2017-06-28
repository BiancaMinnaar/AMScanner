﻿namespace FileUtilityTests
{
    public class FileUtilityLibraryConstants
    {
        public const string CONSTDirectoryToScan = @"C:\Client\Britehouse\AMDevelopment\FileScannerConsole\ScanDirectory";
        public const string CONSTDirecoryToMoveTo = @"C:\Client\Britehouse\AMDevelopment\FileScannerConsole\MoveDirectory";
        public const string CONSTPartDirecotryToScan = "ScanDirectory";
        public const string CONSTPartDirecotryToMoveTo = "MoveDirectory";
        public const string CONSTCorrectFileMask = "*.txt";
        public const string CONSTInCorrectFileMask = "*.exe";
        public const string CONSTMoveFileMask = "MoveFile*.txt";
        public const string CONSTDeleteFileMask = "DeleteFile*.txt";
        public const string CONSTScanFile1 = "ScanFile1 - Copy (4).txt";
        public const string CONSTScanFile2 = "ScanFile1 - Copy (5).txt";
        public const string CONSTTestFile1 = "TestFile1.txt";
        public const string CONSTTestFile2 = "TestFile2.txt";
        public const string CONSTTestFile3 = "TestFile3.txt";
        public const string CONSTTestFile4 = "TestFile1.csv";
        public const char CONSTDelimiter = '|';
        public const char CONSSemiColonTDelimiter = ';';
        public const char CONSCommaDelimiter = ',';
        public const bool CONSTHasHeader = true;
        public const bool CONSTHasNoHeader = false;
        public const string CONSTHeaderLine = "Header1|Header2|Header3";
        public const string CONSTCorrectFileSctructure = "Header1|Header2|Header3\n\rLine1Column1|Line1COlumn2|Line1Column3\n\rLine2Column1|Line2Column2|Line2Column3\n\rLine3Column1|Line3Column2|Line3Column3";
        public const string CONSTInCorrectFileSctructureLine2Column2 = "Header1|Header2|Header3\n\rLine1Column1|Line1COlumn2|Line1Column3\n\rLine2Column1|Line2\nColumn2|Line2Column3\n\rLine3Column1|Line3Column2|Line3Column3";
        public const string CONSTPipeCountLineEndingErrorMessage = "The Ammount of columns returned from the line was incorrect.";
        public const string CONSTExcelFileWithError = "Master1.xlsb";
        public const string CONSTExcelFileWithNoError = "VolumePerPeriod_20170519.xls";
        public const string CONSTCustomerImportDefinitionPathClient1 = @"C:\test1\Client1\Import";
        public const string CONSTCustomerImportDefinitionPathClient2 = @"C:\test1\Client2\Import";
        public const string CONSTCustomerImportDefinitionName1 = "First Import";
        public const string CONSTCustomerImportDefinitionName2 = "Second Import";
        public const string CONSTCustomerImportDefinitionTypeExcel = "EXCEL";
        public const string CONSTCustomerImportDefinitionTypeCSV = "CSV";
        public const string CONSTHeaderErrorErrorMessage = "Header Error Found";
    }
}
