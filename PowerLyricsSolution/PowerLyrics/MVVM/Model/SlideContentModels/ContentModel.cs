using PowerLyrics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace PowerLyrics.MVVM.Model.SlideContentModels
{
    public abstract class ContentModel
    {
        abstract public ContentModel Clone();
        public LyricType LyricType { get; set; }
        public int serialNuber { get; set; } // in case there are more verses or bridges...
        public SlideContentType slideContentType { get; set; }
    }
}
