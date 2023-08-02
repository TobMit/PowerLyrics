using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PowerLyrics.MVVM.View
{
    public abstract class LyricViewTemplate : UserControl
    {
         abstract public object Clone();
    }
}
