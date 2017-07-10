using FileUtilityLibrary.Interface.Model;
using FileUtilityLibrary.Model.ScannerFile;
using log4net;
using System;
using System.IO;

namespace FileUtilityLibrary.Model
{
    public class FileMaskToScannerFile : IFileMaskToScannerFile
    {
        public string FileMask { get; }
        public char Delimiter { get; }
        public bool HasHeader { get; }
        public string ImportFormat { get; }
        private ILog logHandler;

        public FileMaskToScannerFile(
            string fileMask, char delimiter, bool hasHeader, string importFormat, ILog logHandler)
        {
            ImportFormat = importFormat;
            FileMask = fileMask;
            Delimiter = delimiter;
            HasHeader = hasHeader;
            this.logHandler = logHandler;
        }

        public IScannerFile GetScannerFileInstance(FileInfo file)
        {
            return new ScannerFileFactory(logHandler).GetScannerFile(
                file.Name, file.Directory.FullName, Delimiter, HasHeader, ImportFormat);
        }
    }
}
