using FileUtilityLibrary.Interface.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileUtilityLibrary.Model
{
    public abstract class ExceptionOccurrence : IExceptionOccurrence
    {
        public event EventHandler<CharacterRead> OnCharacterRead;
        public event EventHandler<LineRead> OnLineRead;
        public event EventHandler<HeaderRead> OnHeaderRead;
        public string ExceptionMessage { get;set; }
        private IList<string> _ReaderLines;

        public ExceptionOccurrence(string exceptionMessage)
        {
            OnCharacterRead += CharacterReadEventHandler;
            OnLineRead += LineReadEventHandler;
            OnHeaderRead += HeaderReadEventHandler;
            ExceptionMessage = exceptionMessage;
            _ReaderLines = new List<string>();
        }

        protected void OnExceptionFound(object sender, ScannerException scannerException)
        {
            scannerException.ScannerFile.ExceptionList.Add(scannerException.ExceptionDescription + " On Line " + scannerException.ExceptionLineNumber);
            scannerException.ScannerFile.HasException = true;
        } 

        public int CountDelimiterInString(string stringToCount, char delimiter)
        {
            int columnCount = 1;
            bool doubleQuoteOverride = false;
            bool hasDoubleQuote = false;
            bool hasDelimiter = false;
            for (int count = 0; count < stringToCount.Length; count++)
            {
                if (hasDoubleQuote && doubleQuoteOverride)
                {
                    hasDoubleQuote = stringToCount[count] == '"';
                    if (hasDoubleQuote)
                    {
                        doubleQuoteOverride = false;
                        hasDoubleQuote = false;
                    }
                }
                else if (!hasDoubleQuote)
                {
                    hasDoubleQuote = stringToCount[count] == '"';
                }
                if (hasDoubleQuote && !doubleQuoteOverride)
                {
                    doubleQuoteOverride = true;
                }
                else if (hasDoubleQuote && doubleQuoteOverride)
                {
                    doubleQuoteOverride = false;
                    hasDoubleQuote = false;
                }

                hasDelimiter = stringToCount[count] == delimiter;
                if (hasDelimiter && !doubleQuoteOverride)
                {
                    columnCount++;
                }
            }

            return columnCount;
        }

        private string GetLineString(IScannerFile scannerFile)
        {
            StringBuilder lineStringBuilder = new StringBuilder();
            int characterCount = 0;
            while (scannerFile.Peek() > -1)
            {
                char nextChar = (char)scannerFile.Read();
                characterCount++;
                OnCharacterRead?.Invoke(this, new CharacterRead()
                {
                    Character = nextChar,
                    CharacterCount = characterCount,
                    ScannerFile = scannerFile
                });
                lineStringBuilder.Append(nextChar);
                if (nextChar == '\n')
                {
                    return lineStringBuilder.ToString();
                }
            }

            return lineStringBuilder.ToString();
        }

        public void ScanFile(IScannerFile scannerFile)
        {
            try
            {
                int lineCount = 0;
                int columnHeaderCount = readHeaderLine(scannerFile, ref lineCount);
                lineCount = readItemLines(scannerFile, lineCount, columnHeaderCount);
            }
            catch (Exception excp)
            {
                scannerFile.HasException = true;
                scannerFile.ExceptionList.Add(excp.Message);
            }
            finally
            {
                scannerFile.Close();
            }

            while (scannerFile.HasSubStructures())
            {
                ScanFile(scannerFile);
            }
        }

        private int readHeaderLine(IScannerFile scannerFile, ref int lineCount)
        {
            int columnHeaderCount = 0;
            if (scannerFile.HasHeader)
            {
                var headerLine = GetLineString(scannerFile);
                _ReaderLines.Add(headerLine);
                lineCount++;
                OnHeaderRead?.Invoke(this, new HeaderRead()
                {
                    ScannerFile = scannerFile,
                    HeaderText = headerLine
                });
                columnHeaderCount = CountDelimiterInString(headerLine, scannerFile.Delimiter);
            }

            return columnHeaderCount;
        }

        private int readItemLines(IScannerFile scannerFile, int lineCount, int columnHeaderCount)
        {
            while (scannerFile.Peek() > -1)
            {
                var lineRead = GetLineString(scannerFile);
                _ReaderLines.Add(lineRead);
                lineCount++;
                OnLineRead?.Invoke(this, new LineRead()
                {
                    Delimiter = scannerFile.Delimiter,
                    HeaderColumnCount = columnHeaderCount,
                    LineNumber = lineCount,
                    LineText = _ReaderLines[lineCount - 1],
                    ScannerFile = scannerFile
                });
            }

            return lineCount;
        }

        public abstract void CharacterReadEventHandler(object sender, CharacterRead e);
        public abstract void LineReadEventHandler(object sender, LineRead e);
        public abstract void HeaderReadEventHandler(object sender, HeaderRead e);
    }
}
