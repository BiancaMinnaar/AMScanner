using FileUtilityLibrary.Interface.Model;

namespace FileUtilityLibrary.Interface.Repository
{
    public interface IScannerRepository
    {
        void DeleteFaultyFile(IScannerFile fileToDelete);
        void DeleteOrphanedFile(string fullFileName);
        void MoveFileAfterScan(IScannerFile fileToScan);
        bool ScanForExceptions(IScannerFile fileToScan);
    }
}
