using System.IO;

namespace FileUtilityLibrary.Interface.Model.ScannerFIle.Excel
{
    public interface IExcelWorkBook
    {
        MemoryStream[] GetScannableData();
    }
}
