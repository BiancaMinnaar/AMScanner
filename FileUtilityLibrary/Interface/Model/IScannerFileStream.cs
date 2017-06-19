using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUtilityLibrary.Interface.Model
{
    public interface IScannerFileStream
    {
        int Peek();
        string ReadLine();
        int Read();
        void Close();
    }
}
