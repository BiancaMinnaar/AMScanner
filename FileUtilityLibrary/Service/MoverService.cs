using FileUtilityLibrary.Interface.Service;
using System.IO;

namespace FileUtilityLibrary.Service
{
    public class MoverService : IMoverService
    {
        public string DirectoryToMoveTo { get; }
        private DirectoryInfo direcotryHelper;

        public MoverService() { }

        public MoverService(string direcotryToMoveTo)
        {
            DirectoryToMoveTo = direcotryToMoveTo;
            direcotryHelper = new DirectoryInfo(DirectoryToMoveTo);
        }
        public void DeleteFilesInList(FileInfo[] fileListToRemove)
        {
            foreach(FileInfo info in fileListToRemove)
            {
                info.Delete();
            }
        }

        public void MoveFilesInList(FileInfo[] fileListToMove)
        {
            if (DirectoryToMoveTo != null)
            {
                foreach (FileInfo info in fileListToMove)
                {
                    info.MoveTo(DirectoryToMoveTo + "//" + info.Name);
                }
            }
        }
    }
}
