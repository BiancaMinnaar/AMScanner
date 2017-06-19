using FileUtilityLibrary.Interface.Helper;
using System.IO;

namespace FileUtilityLibrary.Service.Helper
{
    public class DirecotryHelper : IDirectoryHelper
    {
        private DirectoryInfo _DirecotryInfo;

        public DirecotryHelper(string directoryPath)
        {
            _DirecotryInfo = new DirectoryInfo(directoryPath);
        }
        public FileInfo[] GetFiles(string fileMask, SearchOption searchOption)
        {
            return _DirecotryInfo.GetFiles(fileMask, searchOption);
        }
    }
}
