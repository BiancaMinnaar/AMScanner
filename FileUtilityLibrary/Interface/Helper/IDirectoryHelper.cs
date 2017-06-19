using System.IO;

namespace FileUtilityLibrary.Interface.Helper
{
    public interface IDirectoryHelper
    {
        FileInfo[] GetFiles(string fileMask, SearchOption searchOption);
    }
}
