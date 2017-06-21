using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUtilityLibrary.Interface.Model.ScannerFIle.Excel
{
    public interface IExcelWorkBook
    {
        MemoryStream[] GetScannableData();
    }
}
