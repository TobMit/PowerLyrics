using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using PowerLyrics.Core;

namespace PowerLyrics.MVVM.View
{
    public abstract class LyricViewTemplate : UserControl
    {
         abstract public LyricViewTemplate Clone();

         abstract public SlideContentType GetType();
    }
}
