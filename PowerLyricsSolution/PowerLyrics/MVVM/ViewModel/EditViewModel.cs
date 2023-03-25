using PowerLyrics.Core;
using PowerLyrics.Core.TextParser;
using PowerLyrics.MVVM.Model;
using PowerLyrics.MVVM.View;
using PowerLyrics.Windows;
using System;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PowerLyrics.MVVM.ViewModel;

public class EditViewModel : ObservableObjects
{
    private TextParser textParser;
    private DispatcherTimer timer;


    private SongModel _openSong;
    public SongModel openSong
    {
        get
        {
            return _openSong;
        }
        set
        {
            _openSong = value;
            selectedSlideNumber = -1;
            openSongSlides = textParser.getSlidesFromOpenSong(_openSong.LyricModels);
        }
    }

    private ObservableCollection<Slide> _openSongSlides;
    public ObservableCollection<Slide> openSongSlides
    {
        get
        {
            return _openSongSlides;
        }
        set
        {
            _openSongSlides = value;
            OnPropertyChanged();
        }
    }

    private int _selectedSlideNumber = -1;
    private int selectedSlideNumber
    {
        get
        {
            return _selectedSlideNumber;
        }
        set
        {
            if (value != -1)
            {
                if (_selectedSlideNumber != -1)
                {
                    openSongSlides[_selectedSlideNumber].isSelected = false;
                }

                _selectedSlideNumber = value;
                openSongSlides[_selectedSlideNumber].isSelected = true;
                openSongSlides =
                    new ObservableCollection<Slide>(openSongSlides); //! takto to je aby som forsol aktualizaciu obrazovky
            }
            else
            {
                _selectedSlideNumber = value;
                LyricContent = null;
            }
        }

    }
    
    private LyricViewTemplate1 _lyricContent;
    public LyricViewTemplate1 LyricContent
    {
        get
        {
            return _lyricContent;
        }
        set
        {
            _lyricContent = value;
            OnPropertyChanged();
        }
    }


    
    public RelayCommand SelectSlideCommand { get; set; }




    public EditViewModel()
    {
        textParser = new TextParser();
        openSongSlides = new ObservableCollection<Slide>();
        inicialiseButtons();
        timer = new DispatcherTimer();
        timer.Interval = new TimeSpan(0, 0, 1);
        timer.Tick += new EventHandler(timer_Tick);
    }

    private void timer_Tick(object? sender, EventArgs e)
    {
        this.applyChanges();
    }

    private void inicialiseButtons()
    {

        SelectSlideCommand = new RelayCommand(o =>
        {
            applyChanges();
            selectedSlideNumber = Int32.Parse(o.ToString());
            LyricContent = new LyricViewTemplate1((LyricViewTemplate1)openSongSlides[selectedSlideNumber].UserControl);

        });
        
    }

    private void applyChanges()
    {
        // zmeny sa môžu aplikovať iba keď je niečo vybraté
        if (selectedSlideNumber != -1)
        {
            openSong.LyricModels[selectedSlideNumber].text = LyricContent.text;
            openSong.LyricModels[selectedSlideNumber].fontSize = (int)LyricContent.fontSize;
            ObservableCollection<Slide> tmp = textParser.getSlidesFromOpenSong(openSong.LyricModels);
            tmp[selectedSlideNumber].isSelected = true;
            openSongSlides = tmp;
        }
    }

    public void startTimer()
    {
        timer.Start();
    }

    public void stopTimer() 
    {
        timer.Stop();
    }

    public SongModel getEditedSong()
    {
        applyChanges();
        return new SongModel(openSong); ;
    }

}