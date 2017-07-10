using System.IO;

namespace FileUtilityLibrary.Interface.Service
{
    public interface ICSVWithExcelAutomationService
    {
        MemoryStream[] GetSheetStreamsFromDocument();
    }
}
