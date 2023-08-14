using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using PowerLyrics.Core;
using PowerLyrics.MVVM.Model.SlideContentModels;

namespace PowerLyrics.MVVM.View;

/// <summary>
///     Interaction logic for LyricViewTemplateVideo.xaml
/// </summary>
public partial class LyricViewTemplateVideo : LyricViewTemplate
{
    public LyricViewTemplateVideo()
    {
        InitializeComponent();
        IsMuted = true;
        //this.prepareForPreview();
    }

    public LyricViewTemplateVideo(LyricViewTemplateVideo copy)
    {
        if (copy != null)
        {
            InitializeComponent();
            IsMuted = copy.IsMuted;
            Source = copy.Source;
            //this.prepareForPreview();
        }
    }

    public LyricViewTemplateVideo(VideoModel video)
    {
        InitializeComponent();
        Source = video.SourceAdress;
        IsMuted = true;
        //this.prepareForPreview();
    }

    private void prepareForPreview()
    {
        //this.videoPlayer.Position = TimeSpan.FromSeconds(1);
       // this.videoPlayer.Play();
        //this.videoPlayer.Stop();
    }

    public string Source
    {
        get => videoPlayer.Source.AbsolutePath;
        set
        {
            if (value != null)
            {
                videoPlayer.Source = new Uri(value);
            }
        }
    }

    public bool IsMuted
    {
        get => videoPlayer.IsMuted;
        set => videoPlayer.IsMuted = value;
    }

    public void videoPlayerPlay()
    {
        this.videoPlayer.Position = TimeSpan.Zero;
        this.videoPlayer.Play();
    }

    public void videoPlayerStop()
    {
        this.videoPlayer.Stop();
    }

    public override LyricViewTemplate Clone()
    {
        return new LyricViewTemplateVideo(this);
    }

    public override SlideContentType GetType()
    {
        return SlideContentType.Video;
    }
}