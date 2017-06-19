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
    public class PipeCountLineEndingExceptionOccurrenceTests
    {
        /*
         string ExceptionMessage { get; set; }
         TestThatExceptionMessageIsInitialised
        int CountDelimiterInString(string stringToCount);
        TestCountDelimiterInStringReturnsCorrectNumerOfDelimiters
        void ScanFile(ScannerFile scannerFile);
        TestThatScanFileRaiseHeaderScanEvent
        TestThatScanFileRaiseLineReadEvent
        TestThatScanFileRaiseCharacterReadEvent
        TestThatScanFileScansWholeFile
        void LineReadEventHandler(object sender, LineRead e);
        TestLineReadEventHandlerRaisesOnExceptionFoundWithColumnNumberMissmatch
        */

        //[TestMethod]
        //public void TestExceptionOccurrenceInitialisesDelimiter()
        //{
        //    //Assign
        //    //Action
        //    PipeCountLineEndingExceptionOccurrence exceptionTest = new PipeCountLineEndingExceptionOccurrence(
        //        Constants.CONSTDelimiter, "");

        //    //Assert
        //    Assert.AreEqual(Constants.CONSTDelimiter, exceptionTest.Delimiter);
        //}

        [TestMethod]
        public void TestCountDelimiterInStringReturnsCorrectNumerOfDelimiters()
        {
            //Assign
            PipeCountLineEndingExceptionOccurrence exceptionTest = new PipeCountLineEndingExceptionOccurrence(
                "");

            //Action
            var columnCount = exceptionTest.CountDelimiterInString(Constants.CONSTHeaderLine, Constants.CONSTDelimiter);

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
            PipeCountLineEndingExceptionOccurrence exceptionTest = new PipeCountLineEndingExceptionOccurrence(
                "");
            var scannerFileMock = getScannerMockSetup(Constants.CONSTCorrectFileSctructure);
            scannerFileMock.Setup(t => t.HasHeader).Returns(() => Constants.CONSTHasHeader);

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
            PipeCountLineEndingExceptionOccurrence exceptionTest = new PipeCountLineEndingExceptionOccurrence(
                "");
            var scannerFileMock = getScannerMockSetup(Constants.CONSTCorrectFileSctructure);
            scannerFileMock.Setup(t => t.HasHeader).Returns(() => Constants.CONSTHasHeader);

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
            PipeCountLineEndingExceptionOccurrence exceptionTest = new PipeCountLineEndingExceptionOccurrence(
                "");
            var scannerFileMock = getScannerMockSetup(Constants.CONSTCorrectFileSctructure);
            scannerFileMock.Setup(t => t.HasHeader).Returns(() => Constants.CONSTHasHeader);

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
            PipeCountLineEndingExceptionOccurrence exceptionTest = new PipeCountLineEndingExceptionOccurrence(
                Constants.CONSTPipeCountLineEndingErrorMessage);
            var scannerFileMock = getScannerMockSetup(Constants.CONSTInCorrectFileSctructureLine2Column2);
            scannerFileMock.Setup(t => t.ExceptionList).Returns(() => new List<string>());
            scannerFileMock.Setup(t => t.Delimiter).Returns(() => Constants.CONSTDelimiter);
            scannerFileMock.Setup(t => t.HasHeader).Returns(() => Constants.CONSTHasHeader);

            exceptionTest.ScanFile(scannerFileMock.Object);

            scannerFileMock.VerifySet(v =>v.HasException = It.Is<bool>(t => t == true));
        }

        [TestMethod]
        public void TestLineReadEventHandlerRaisesOnExceptionFoundWithColumnNumberMissmatchWithGoodErrorFeedback()
        {
            PipeCountLineEndingExceptionOccurrence exceptionTest = new PipeCountLineEndingExceptionOccurrence(
                Constants.CONSTPipeCountLineEndingErrorMessage);
            var scannerFileMock = getScannerMockSetup(Constants.CONSTInCorrectFileSctructureLine2Column2);
            var errorList = new List<string>();
            scannerFileMock.Setup(t => t.ExceptionList).Returns(() => errorList);
            scannerFileMock.Setup(t => t.Delimiter).Returns(() => Constants.CONSTDelimiter);
            scannerFileMock.Setup(t => t.HasHeader).Returns(() => Constants.CONSTHasHeader);

            exceptionTest.ScanFile(scannerFileMock.Object);

            Assert.AreEqual(2, errorList.Count);
        }

        [TestMethod]
        public void TestLineReadEventHandlerRaisesNoExceptionWithoutHeaders()
        {
            PipeCountLineEndingExceptionOccurrence exceptionTest = new PipeCountLineEndingExceptionOccurrence(
                Constants.CONSTPipeCountLineEndingErrorMessage);
            var scannerFileMock = getScannerMockSetup(Constants.CONSTInCorrectFileSctructureLine2Column2);
            var errorList = new List<string>();
            scannerFileMock.Setup(t => t.ExceptionList).Returns(() => errorList);
            scannerFileMock.Setup(t => t.Delimiter).Returns(() => Constants.CONSTDelimiter);
            scannerFileMock.Setup(t => t.HasHeader).Returns(() => Constants.CONSTHasNoHeader);

            exceptionTest.ScanFile(scannerFileMock.Object);

            Assert.AreEqual(0, errorList.Count);
        }
    }
}
