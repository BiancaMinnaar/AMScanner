using FileUtilityLibrary.Interface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUtilityLibrary.Model
{
    public class ScannerException
    {
        public IScannerFile ScannerFile { get; set; }
        public string ExceptionDescription { get; set; }
        public string ExceptionLineNumber { get; set; }
    }
}
