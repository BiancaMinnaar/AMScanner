namespace FileUtilityTests
{
    public class FileUtilityLibraryConstants
    {
        public const string CONSTDirectoryToScan = @"C:\Client\Britehouse\AMDevelopment\FileScannerConsole\ftp sites";
        public const string CONSTDirecoryToMoveTo = @"C:\Client\Britehouse\AMDevelopment\FileScannerConsole\MoveDirectory";
        public const string CONSTPartDirecotryToScan = "ftp sites";
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
        public const int CONSTClientRecordID = 1;
        public const string CONSTClientName = "Sebenzella";
        public const string CONSTCustomerImportDefinisionScanPath = @"C:\test1";
        public const string CONSTCustomerImportDefinitionPathClient1 = @"C:\test1\Client1\Import";
        public const string CONSTCustomerImportDefinitionPathClient2 = @"C:\test1\Client2\Import";
        public const string CONSTCustomerImportDefinitionPathWithMaskClient1 = @"C:\test1\Client1\Import\Master*.*";
        public const string CONSTCustomerImportDefinitionPathWithMaskClient2 = @"C:\test1\Client2\Import\TestFile*.*";
        public const string CONSTCustomerImportDefinitionPathWithMaskClient3 = @"C:\test1\Client2\Import\POS*.*";
        public const string CONSTCustomerImportDefinitionName1 = "First Import";
        public const string CONSTCustomerImportDefinitionName2 = "Second Import";
        public const string CONSTCustomerImportDefinitionTypeExcel = "EXCEL";
        public const string CONSTCustomerImportDefinitionTypeCSV = "CSV";
        public const bool CONSTImportIsEnabled = true;
        public const bool CONSTImportIsNOTEnabled = false;
        public const string CONSTHeaderErrorErrorMessage = "Header Error Found";
        public const string CONSTGoodEMailSubject = "Good Email Subject";
        public const string CONSTGoodEMailBody = "Good Email Body";
        public const string CONSTScannerSetupDirecoryToWatchRoot = @"C:\Client\Britehouse\AMDevelopment\FileScannerConsole\ftp sites";
        public const string CONSTScannerSetupDirecoryToWatch = @"C:\Client\Britehouse\AMDevelopment\FileScannerConsole\ftp sites\danone\uploads\V7";
        public const string CONSTScannerSetupFileToDump = "POS_Additional.xlsx";
        public const string CONSTScannerSetupFileToDumpStoreSalesText = "storesales.txt";
        public const string CONSTScannerSetupFileToDumpStoreSalesExcel = "storesales.xls";
        public const string CONSTScannerSetupFileToImportCallCycles = "callcycles_alt - Phil (1).xlsx";
        public const string CONSTActualErrorMessageForHeaderError = "There is an error in the following line: ";
        public const string CONSTBrokenOneSheetBinaryFile = "ProfilesOneSheetD1.xlsb";
        public const string CONSTCustomerImportDefinitionPathBrokenOneSheetBinaryFile = @"C:\test1\Client2\Import\ProfilesOneSheet*.*";
    }
}
