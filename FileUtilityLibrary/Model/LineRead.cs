using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUtilityLibrary.Model
{
    public class LineRead : ReadEvent
    {
        public char Delimiter { get; set; }
        public int HeaderColumnCount { get; set; }
        public string LineText { get; set; }
        public int LineNumber { get; set; }
    }
}
