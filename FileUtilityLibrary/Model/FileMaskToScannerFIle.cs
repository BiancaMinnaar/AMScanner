using FileUtilityLibrary.Interface.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUtilityLibrary.Model
{
    public class FileMaskToScannerFile<T> : IFileMaskToScannerFile<T> where T : IScannerFile
    {
        private string _FileMask;
        private char _Delimiter;
        private bool _HasHeader;
        private Func<string, string, char, bool, T> _Constructor;

        public FileMaskToScannerFile(string fileMask, char delimiter, bool hasHeader, Func<string, string, char, bool, T> constructor)
        {
            _Constructor = constructor;
            _FileMask = fileMask;
            _Delimiter = delimiter;
            _HasHeader = hasHeader;
        }

        public string GetFileMask()
        {
            return _FileMask;
        }

        public char GetDelimiter()
        {
            return _Delimiter;
        }

        public T GetScannerFileInstance(FileInfo file)
        {
            return _Constructor(file.Name, file.DirectoryName, _Delimiter, _HasHeader);
        }
    }
}
