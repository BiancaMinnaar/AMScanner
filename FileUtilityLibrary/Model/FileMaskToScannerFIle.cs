using FileUtilityLibrary.Interface.Model;
using FileUtilityLibrary.Model.ScannerFile;
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

        public FileMaskToScannerFile(
            string fileMask, char delimiter, bool hasHeader, string importFormat)
        {
            ImportFormat = importFormat;
            FileMask = fileMask;
            Delimiter = delimiter;
            HasHeader = hasHeader;
        }

        public IScannerFile GetScannerFileInstance(FileInfo file)
        {
            return new ScannerFileFactory().GetScannerFile(
                file.Name, file.Directory.FullName, Delimiter, HasHeader, ImportFormat);
        }
    }
}
