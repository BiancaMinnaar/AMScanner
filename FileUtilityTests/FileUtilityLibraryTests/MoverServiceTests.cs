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
            var mover = new MoverService(FileUtilityLibraryConstants.CONSTDirecoryToMoveTo);

            Assert.AreEqual(FileUtilityLibraryConstants.CONSTDirecoryToMoveTo, mover.DirectoryToMoveTo, "The move to directory was incorrectly initialised");
        }

        [TestMethod]
        public void TestMoverMovesFilesInList()
        {
            var scanDirectory = new DirectoryInfo(FileUtilityLibraryConstants.CONSTDirectoryToScan);
            var moveDirectory = new DirectoryInfo(FileUtilityLibraryConstants.CONSTDirecoryToMoveTo);
            var mover = new MoverService(FileUtilityLibraryConstants.CONSTDirecoryToMoveTo);

            var startCount = scanDirectory.GetFiles(FileUtilityLibraryConstants.CONSTMoveFileMask).Length;
            var startDestinationCount = moveDirectory.GetFiles(FileUtilityLibraryConstants.CONSTMoveFileMask).Length;
            mover.MoveFilesInList(scanDirectory.GetFiles(FileUtilityLibraryConstants.CONSTMoveFileMask));
            var endCount = scanDirectory.GetFiles(FileUtilityLibraryConstants.CONSTMoveFileMask).Length;
            var endDestinationCount = moveDirectory.GetFiles(FileUtilityLibraryConstants.CONSTMoveFileMask).Length;

            Assert.AreNotEqual(startCount, endCount, "The Files are ether still there or the directory started empty");
            Assert.AreEqual(0, endCount, "Some or all files weren't deleted");
            Assert.AreEqual(startCount, endDestinationCount, "The correct amount of files wern't moved");
            Assert.AreNotEqual(startDestinationCount, endDestinationCount, "The files never Arived or we started empty");
        }

        [TestMethod]
        public void TestMoverDeletesMovedFilesInList()
        {
            var scanDirectory = new DirectoryInfo(FileUtilityLibraryConstants.CONSTDirectoryToScan);
            var mover = new MoverService(FileUtilityLibraryConstants.CONSTDirectoryToScan);

            var startCount = scanDirectory.GetFiles(FileUtilityLibraryConstants.CONSTDeleteFileMask).Length;
            mover.DeleteFilesInList(scanDirectory.GetFiles(FileUtilityLibraryConstants.CONSTDeleteFileMask));
            var endCount = scanDirectory.GetFiles(FileUtilityLibraryConstants.CONSTDeleteFileMask).Length;

            Assert.AreNotEqual(startCount, endCount, "The correct number of files weren't deleted");
        }
    }
}
