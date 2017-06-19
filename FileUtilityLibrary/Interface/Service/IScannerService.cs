using FileUtilityLibrary.Interface.Model;
using FileUtilityLibrary.Model;
using System.Collections.Generic;
using System.IO;

namespace FileUtilityLibrary.Interface.Service
{
    public interface IScannerService
    {
        /// <summary>
        /// Determine if directory has files based on the input mask
        /// </summary>
        /// <param name="fileMask">Inputmask to determine search pattern</param>
        /// <returns></returns>
        bool DirectoryHasFile(string fileMask);
        ScannerFileCollection<IScannerFile> GetFilesToScan(IFileMaskToScannerFile<IScannerFile> fileMask);
    }
}
