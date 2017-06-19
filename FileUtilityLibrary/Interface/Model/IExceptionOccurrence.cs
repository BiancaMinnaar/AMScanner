using FileUtilityLibrary.Model;

namespace FileUtilityLibrary.Interface.Model
{
    public interface IExceptionOccurrence
    {
        string ExceptionMessage { get; set; }
        int CountDelimiterInString(string stringToCount, char delimiter);
        void ScanFile(IScannerFile scannerFile);
        void CharacterReadEventHandler(object sender, CharacterRead e);
        void LineReadEventHandler(object sender, LineRead e);
        void HeaderReadEventHandler(object sender, HeaderRead e);
    }
}
