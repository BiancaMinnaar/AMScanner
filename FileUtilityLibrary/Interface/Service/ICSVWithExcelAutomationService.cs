using System.IO;

namespace FileUtilityLibrary.Interface.Service
{
    public interface ICSVFromExcelService
    {
        MemoryStream[] GetSheetStreamsFromDocument();
    }
}
