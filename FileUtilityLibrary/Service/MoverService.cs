using FileUtilityLibrary.Interface.Service;
using log4net;
using System.IO;

namespace FileUtilityLibrary.Service
{
    public class MoverService : IMoverService
    {
        public string DirectoryToMoveTo { get; }
        private DirectoryInfo direcotryHelper;
        private ILog _LogHandler;

        public MoverService(ILog logHandler)
        {
            _LogHandler = logHandler;
        }

        public MoverService(string direcotryToMoveTo, ILog logHandler)
            :this(logHandler)
        {
            DirectoryToMoveTo = direcotryToMoveTo;
            direcotryHelper = new DirectoryInfo(DirectoryToMoveTo);
        }
        public void DeleteFilesInList(FileInfo[] fileListToRemove)
        {
            foreach(FileInfo info in fileListToRemove)
            {
                _LogHandler.Debug(info.FullName + " is being Deleted");
                info.Delete();
                _LogHandler.Debug(info.FullName + " is Deleted");
            }
        }

        public void MoveFilesInList(FileInfo[] fileListToMove)
        {
            if (DirectoryToMoveTo != null)
            {
                foreach (FileInfo info in fileListToMove)
                {
                    var destinationFileName = DirectoryToMoveTo + @"\" + info.Name;
                    _LogHandler.Debug(destinationFileName);
                    _LogHandler.Debug(info.FullName + " is being moved");
                    info.MoveTo(destinationFileName);
                }
            }
        }
    }
}
