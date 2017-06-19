using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileUtilityLibrary.Service;
using System.IO;

namespace FileUtilityTests
{
    [TestClass]
    public class MoverServiceTests
    {
        [TestMethod]
        public void TestThatMoverInitialisesMoveToDirectory()
        {
            var mover = new MoverService(Constants.CONSTDirecoryToMoveTo);

            Assert.AreEqual(Constants.CONSTDirecoryToMoveTo, mover.DirectoryToMoveTo, "The move to directory was incorrectly initialised");
        }

        [TestMethod]
        public void TestMoverMovesFilesInList()
        {
            var scanDirectory = new DirectoryInfo(Constants.CONSTDirectoryToScan);
            var moveDirectory = new DirectoryInfo(Constants.CONSTDirecoryToMoveTo);
            var mover = new MoverService(Constants.CONSTDirecoryToMoveTo);

            var startCount = scanDirectory.GetFiles(Constants.CONSTMoveFileMask).Length;
            var startDestinationCount = moveDirectory.GetFiles(Constants.CONSTMoveFileMask).Length;
            mover.MoveFilesInList(scanDirectory.GetFiles(Constants.CONSTMoveFileMask));
            var endCount = scanDirectory.GetFiles(Constants.CONSTMoveFileMask).Length;
            var endDestinationCount = moveDirectory.GetFiles(Constants.CONSTMoveFileMask).Length;

            Assert.AreNotEqual(startCount, endCount, "The Files are ether still there or the directory started empty");
            Assert.AreEqual(0, endCount, "Some or all files weren't deleted");
            Assert.AreEqual(startCount, endDestinationCount, "The correct amount of files wern't moved");
            Assert.AreNotEqual(startDestinationCount, endDestinationCount, "The files never Arived or we started empty");
        }

        [TestMethod]
        public void TestMoverDeletesMovedFilesInList()
        {
            var scanDirectory = new DirectoryInfo(Constants.CONSTDirectoryToScan);
            var mover = new MoverService(Constants.CONSTDirectoryToScan);

            var startCount = scanDirectory.GetFiles(Constants.CONSTDeleteFileMask).Length;
            mover.DeleteFilesInList(scanDirectory.GetFiles(Constants.CONSTDeleteFileMask));
            var endCount = scanDirectory.GetFiles(Constants.CONSTDeleteFileMask).Length;

            Assert.AreNotEqual(startCount, endCount, "The correct number of files weren't deleted");
        }
    }
}
