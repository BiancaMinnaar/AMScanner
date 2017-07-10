using System.IO;

namespace FileUtilityLibrary.Interface.Model
{
    public interface IFileMaskToScannerFile
    {
        string FileMask { get; }
        char Delimiter { get; }
        bool HasHeader { get; }
        string ImportFormat { get; }
        IScannerFile GetScannerFileInstance(FileInfo file);
    }
}
