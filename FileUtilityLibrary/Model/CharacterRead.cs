using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUtilityLibrary.Model
{
    public class CharacterRead : ReadEvent
    {
        public char Character { get; set; }
        public int CharacterCount { get; set; }
    }
}
