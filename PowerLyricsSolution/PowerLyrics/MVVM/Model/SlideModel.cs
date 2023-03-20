using PowerLyrics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PowerLyrics.MVVM.Model
{
    public class Slide
    {
        public UserControl? UserControl { get; set; }
        public SlideType SlideType { get; set; }
        public int id { get; set; }
        public string dividerText { get; set; }

        public bool isSelected { get; set; }
    }
}
