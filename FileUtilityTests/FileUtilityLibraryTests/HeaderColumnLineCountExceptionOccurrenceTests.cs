using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileUtilityLibrary.Model;
using FileUtilityLibrary.ExpetionOccurrences;
using Moq;
using FileUtilityLibrary.Interface.Model;
using System.Collections.Generic;

namespace FileUtilityTests
{
    [TestClass]
    public class HeaderColumnLineCountExceptionOccurrenceTests
    {
        [TestMethod]
        public void TestCountDelimiterInStringReturnsCorrectNumerOfDelimiters()
        {
            //Assign
            HeaderColumnLineCountExceptionOccurrence exceptionTest = new HeaderColumnLineCountExceptionOccurrence(
                "");

            //Action
            var columnCount = exceptionTest.CountDelimiterInString(FileUtilityLibraryConstants.CONSTHeaderLine, FileUtilityLibraryConstants.CONSTDelimiter);

            ////Assert
            Assert.AreEqual(3, columnCount);
        }

        private Mock<IScannerFile> getScannerMockSetup(string fileText)
        {
            Mock<IScannerFile> scannerFileMock = new Mock<IScannerFile>();
            var queueCharacters = new Queue<char>();
            foreach (char charater in fileText.ToCharArray())
            {
                queueCharacters.Enqueue(charater);
            }
            scannerFileMock.Setup(t => t.Peek()).Returns(() =>
            {
                try
                {
                    return queueCharacters.Peek();
                }
                catch (Exception)
                {
                    return -1;
                }
            });
            scannerFileMock.Setup(t => t.Read()).Returns(() => queueCharacters.Dequeue());

            return scannerFileMock;
        }

        [TestMethod]
        public void TestThatScanFileRaiseOnCharacterRead()
        {
            HeaderColumnLineCountExceptionOccurrence exceptionTest = new HeaderColumnLineCountExceptionOccurrence(
                "");
            var scannerFileMock = getScannerMockSetup(FileUtilityLibraryConstants.CONSTCorrectFileSctructure);
            scannerFileMock.Setup(t => t.HasHeader).Returns(() => FileUtilityLibraryConstants.CONSTHasHeader);

            bool eventWasRecieved = false;
            exceptionTest.OnCharacterRead += delegate (object sender, CharacterRead e)
            {
                eventWasRecieved = true;
            };
            exceptionTest.ScanFile(scannerFileMock.Object);

            Assert.AreEqual(true, eventWasRecieved);
        }

        [TestMethod]
        public void TestThatScanFileRaiseOnHeaderRead()
        {
            HeaderColumnLineCountExceptionOccurrence exceptionTest = new HeaderColumnLineCountExceptionOccurrence(
                "");
            var scannerFileMock = getScannerMockSetup(FileUtilityLibraryConstants.CONSTCorrectFileSctructure);
            scannerFileMock.Setup(t => t.HasHeader).Returns(() => FileUtilityLibraryConstants.CONSTHasHeader);

            bool eventWasRecieved = false;
            exceptionTest.OnHeaderRead += delegate (object sender, HeaderRead e)
            {
                eventWasRecieved = true;
            };
            exceptionTest.ScanFile(scannerFileMock.Object);

            Assert.AreEqual(true, eventWasRecieved);
        }

        [TestMethod]
        public void TestThatScanFileRaiseOnLineRead()
        {
            HeaderColumnLineCountExceptionOccurrence exceptionTest = new HeaderColumnLineCountExceptionOccurrence(
                "");
            var scannerFileMock = getScannerMockSetup(FileUtilityLibraryConstants.CONSTCorrectFileSctructure);
            scannerFileMock.Setup(t => t.HasHeader).Returns(() => FileUtilityLibraryConstants.CONSTHasHeader);

            bool eventWasRecieved = false;
            exceptionTest.OnLineRead += delegate (object sender, LineRead e)
            {
                eventWasRecieved = true;
            };
            exceptionTest.ScanFile(scannerFileMock.Object);

            Assert.AreEqual(true, eventWasRecieved);
        }

        [TestMethod]
        public void TestLineReadEventHandlerRaisesOnExceptionFoundWithColumnNumberMissmatch()
        {
            HeaderColumnLineCountExceptionOccurrence exceptionTest = new HeaderColumnLineCountExceptionOccurrence(
                FileUtilityLibraryConstants.CONSTPipeCountLineEndingErrorMessage);
            var scannerFileMock = getScannerMockSetup(FileUtilityLibraryConstants.CONSTInCorrectFileSctructureLine2Column2);
            scannerFileMock.Setup(t => t.ExceptionList).Returns(() => new List<string>());
            scannerFileMock.Setup(t => t.Delimiter).Returns(() => FileUtilityLibraryConstants.CONSTDelimiter);
            scannerFileMock.Setup(t => t.HasHeader).Returns(() => FileUtilityLibraryConstants.CONSTHasHeader);

            exceptionTest.ScanFile(scannerFileMock.Object);

            scannerFileMock.VerifySet(v =>v.HasException = It.Is<bool>(t => t == true));
        }

        [TestMethod]
        public void TestLineReadEventHandlerRaisesOnExceptionFoundWithColumnNumberMissmatchWithGoodErrorFeedback()
        {
            HeaderColumnLineCountExceptionOccurrence exceptionTest = new HeaderColumnLineCountExceptionOccurrence(
                FileUtilityLibraryConstants.CONSTPipeCountLineEndingErrorMessage);
            var scannerFileMock = getScannerMockSetup(FileUtilityLibraryConstants.CONSTInCorrectFileSctructureLine2Column2);
            var errorList = new List<string>();
            scannerFileMock.Setup(t => t.ExceptionList).Returns(() => errorList);
            scannerFileMock.Setup(t => t.Delimiter).Returns(() => FileUtilityLibraryConstants.CONSTDelimiter);
            scannerFileMock.Setup(t => t.HasHeader).Returns(() => FileUtilityLibraryConstants.CONSTHasHeader);

            exceptionTest.ScanFile(scannerFileMock.Object);

            Assert.AreEqual(2, errorList.Count);
        }

        [TestMethod]
        public void TestLineReadEventHandlerRaisesNoExceptionWithoutHeaders()
        {
            HeaderColumnLineCountExceptionOccurrence exceptionTest = new HeaderColumnLineCountExceptionOccurrence(
                FileUtilityLibraryConstants.CONSTPipeCountLineEndingErrorMessage);
            var scannerFileMock = getScannerMockSetup(FileUtilityLibraryConstants.CONSTInCorrectFileSctructureLine2Column2);
            var errorList = new List<string>();
            scannerFileMock.Setup(t => t.ExceptionList).Returns(() => errorList);
            scannerFileMock.Setup(t => t.Delimiter).Returns(() => FileUtilityLibraryConstants.CONSTDelimiter);
            scannerFileMock.Setup(t => t.HasHeader).Returns(() => FileUtilityLibraryConstants.CONSTHasNoHeader);

            exceptionTest.ScanFile(scannerFileMock.Object);

            Assert.AreEqual(0, errorList.Count);
        }
    }
}
