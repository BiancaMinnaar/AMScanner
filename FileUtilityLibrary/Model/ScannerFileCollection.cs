using FileUtilityLibrary.Interface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUtilityLibrary.Model
{
    public class ScannerFileCollection<T> : List<T>, IScannerFileCollection<T> where T : IScannerFile
    {
        public IList<T> GetScannerFilesWithExceptions()
        {
            return this.Where(t => t.HasException).ToList();
        }

        public IList<T> GetScannerFilesWithNoExceptions()
        {
            return this.Where(t => !t.HasException).ToList();
        }
    }
}
