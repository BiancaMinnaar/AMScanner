using System.IO;

namespace FileUtilityLibrary.Interface.Service
{
    public interface IMoverService
    {
        /// <summary>
        /// Move FIles in list supplied
        /// </summary>
        /// <param name="fileListToMove">a List of files to move</param>
        void MoveFilesInList(FileInfo[] fileListToMove);
        void DeleteFilesInList(FileInfo[] fileListToRemove);
    }
}
