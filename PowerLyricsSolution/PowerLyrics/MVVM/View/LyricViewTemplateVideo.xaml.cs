using PowerLyrics.MVVM.Model.SlideContentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PowerLyrics.MVVM.View
{
    /// <summary>
    /// Interaction logic for LyricViewTemplateVideo.xaml
    /// </summary>
    public partial class LyricViewTemplateVideo : LyricViewTemplate
    {
        public LyricViewTemplateVideo()
        {
            InitializeComponent();
        }

        public LyricViewTemplateVideo(LyricViewTemplateVideo copy)
        {
            if (copy != null)
            {
                InitializeComponent();
                
            }
        }

        public LyricViewTemplateVideo(VideoModel video)
        {
            InitializeComponent();
            
        }

        public LyricViewTemplateVideo(string text)
        {
            InitializeComponent();
        }

        public override object Clone()
        {
            return new LyricViewTemplateVideo(this);
        }
    }
}
