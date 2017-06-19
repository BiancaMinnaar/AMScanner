using FileUtilityLibrary.Interface.Helper;
using FileUtilityLibrary.Interface.Service;
using FileUtilityLibrary.Model;
using FileUtilityLibrary.Service.Helper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using FileUtilityLibrary.Interface.Model;

namespace FileUtilityLibrary.Service
{
    public class ScannerService : IScannerService
    {
        public string DirectoryToScan { get; }
        private IDirectoryHelper _DircecoryHelper;

        public ScannerService(IDirectoryHelper directoryHelper)
        {
            _DircecoryHelper = directoryHelper;
        }
        public bool DirectoryHasFile(string fileMask)
        {
            var files = _DircecoryHelper.GetFiles(fileMask, SearchOption.TopDirectoryOnly);
            return files.Count() > 0;
        }

        public ScannerFileCollection<IScannerFile> GetFilesToScan(IFileMaskToScannerFile<IScannerFile> fileMask)
        {
            var scannerFiles = new ScannerFileCollection<IScannerFile>();
            var files = _DircecoryHelper.GetFiles(fileMask.GetFileMask(), SearchOption.TopDirectoryOnly);
            foreach(FileInfo info in files)
            {
                scannerFiles.Add(fileMask.GetScannerFileInstance(info));
            }

            return scannerFiles;
        }
    }
}
