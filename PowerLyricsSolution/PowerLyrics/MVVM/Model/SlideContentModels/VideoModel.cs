using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using PowerLyrics.Core;

namespace PowerLyrics.MVVM.Model.SlideContentModels
{
    public class VideoModel : ContentModel
    {

        public VideoModel()
        {
            slideContentType = SlideContentType.Video;
            SourceAdress = "D:\\WinSubory\\Videos\\HG-meme-Je mi to tak luto.mp4";
        }

        public VideoModel(VideoModel clone)
        {
            slideContentType = SlideContentType.Video;
            SourceAdress = clone.SourceAdress;
            return;
        }

        public override ContentModel Clone()
        {
            return new VideoModel(this);
        }

        public string SourceAdress { get; set; }
    }
}
