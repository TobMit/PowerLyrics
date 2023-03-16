using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using PowerLyrics.Core;

namespace PowerLyrics.MVVM.Model
{
    public class LyricModel
    {
        public string text { get; set; }
        public int fontSize { get; set; }
        public LyricType LyricType { get; set; }
    }
}
