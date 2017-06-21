using System;
using FileUtilityLibrary.Model;

namespace FileUtilityLibrary.ExpetionOccurrences
{
    public class HeaderColumnLineCountExceptionOccurrence : ExceptionOccurrence
    {        
        public HeaderColumnLineCountExceptionOccurrence(string exceptionMessage) : base(exceptionMessage)
        {
            OnCharacterRead -= CharacterReadEventHandler;
            OnHeaderRead -= HeaderReadEventHandler;
        }

        public override void CharacterReadEventHandler(object sender, CharacterRead e)
        {
            throw new NotImplementedException();
        }

        public override void HeaderReadEventHandler(object sender, HeaderRead e)
        {
            throw new NotImplementedException();
        }

        public override void LineReadEventHandler(object sender, LineRead e)
        {
            int lineColumnCount = CountDelimiterInString(e.LineText, e.Delimiter);
            if (e.ScannerFile.HasHeader && e.HeaderColumnCount != lineColumnCount)
            {
                OnExceptionFound(this, new ScannerException()
                {
                    ExceptionDescription = ExceptionMessage,
                    ExceptionLineNumber = e.LineNumber.ToString(),
                    ScannerFile = e.ScannerFile
                });
            }
        }
    }
}
