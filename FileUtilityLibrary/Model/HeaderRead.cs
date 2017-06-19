using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUtilityLibrary.Model
{
    public class HeaderRead : ReadEvent
    {
        public string HeaderText { get; set; }
        public IList<string> HeaderColumns { get; set; }
    }
}
