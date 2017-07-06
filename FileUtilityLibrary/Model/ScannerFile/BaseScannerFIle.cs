using FileUtilityLibrary.Interface.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUtilityLibrary.Model.ScannerFile
{
    public abstract class BaseScannerFile : IScannerFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public char Delimiter { get; set; }
        public bool HasHeader { get; set; }
        public bool HasException { get; set; }
        public IList<string> ExceptionList { get; set; }
        protected StreamReader _StreamReader;

        public BaseScannerFile(string fileName, string filePath, char delimiter, bool hasHeader)
        {
            FileName = fileName;
            FilePath = filePath;
            Delimiter = delimiter;
            HasHeader = hasHeader;
            var fullFileName = FilePath + @"\" + FileName;
            _StreamReader = new StreamReader(fullFileName);
            ExceptionList = new List<string>();
        }

        public abstract bool HasSubStructures();

        public int Peek()
        {
            return _StreamReader.Peek();
        }

        public string ReadLine()
        {
            return _StreamReader.ReadLine();
        }

        public int Read()
        {
            return _StreamReader.Read();
        }

        public void Close()
        {
            var log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Debug("Base Scanner Close");
            _StreamReader.Close();
            log.Debug("Base Scanner _StreamReader.Close");
            _StreamReader.Dispose();
            log.Debug("Base Scanner _StreamReader.Disposed");
        }

        public virtual void Dispose()
        {
            var log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Debug("Base Scanner destructor");
            if (_StreamReader != null)
            {
                Close();
            }
            log.Debug("Base Scanner finished destruction");
        }
        
    }
}
