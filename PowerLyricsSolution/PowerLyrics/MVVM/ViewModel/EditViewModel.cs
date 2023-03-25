using PowerLyrics.Core;
using PowerLyrics.Core.TextParser;
using PowerLyrics.MVVM.Model;
using PowerLyrics.MVVM.View;
using PowerLyrics.Windows;
using System;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Windows;
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
        get { return _openSong; }
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
        get { return _openSongSlides; }
        set
        {
            _openSongSlides = value;
            OnPropertyChanged();
        }
    }

    private int _selectedSlideNumber = -1;

    private int selectedSlideNumber
    {
        get { return _selectedSlideNumber; }
        set
        {
            if (value != -1)
            {
                if (isSelectedSlide())
                {
                    openSongSlides[_selectedSlideNumber].isSelected = false;
                }

                _selectedSlideNumber = value;
                openSongSlides[_selectedSlideNumber].isSelected = true;
                openSongSlides =
                    new ObservableCollection<Slide>(
                        openSongSlides); //! takto to je aby som forsol aktualizaciu obrazovky
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
        get { return _lyricContent; }
        set
        {
            _lyricContent = value;
            OnPropertyChanged();
        }
    }

    /**
     * Select song from playlist
     */
    public RelayCommand SelectSlideCommand { get; set; }
    public RelayCommand IncreaseFontCommand { get; set; }
    public RelayCommand DecreaseFontCommand { get; set; }
    public RelayCommand SetTextAligmentCommand { get; set; }


    public EditViewModel()
    {
        textParser = new TextParser();
        openSongSlides = new ObservableCollection<Slide>();
        inicialiseButtons();
        timer = new DispatcherTimer();
        timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
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

        IncreaseFontCommand = new RelayCommand(o =>
        {
            if (isSelectedSlide())
            {
                LyricContent.fontSize += 2;
            }
        });

        DecreaseFontCommand = new RelayCommand(o =>
        {
            // zmeny sa môžu aplikovať iba keď je niečo vybraté
            if (isSelectedSlide())
            {
                LyricContent.fontSize -= 2;
            }
        });

        SetTextAligmentCommand = new RelayCommand(o =>
        {
            // zmeny sa môžu aplikovať iba keď je niečo vybraté
            if (isSelectedSlide())
            {
                LyricContent.textAligment = (TextAlignment)Enum.Parse(typeof(TextAlignment), o.ToString());
            }
        });
    }

    private void applyChanges()
    {
        // zmeny sa môžu aplikovať iba keď je niečo vybraté
        if (isSelectedSlide())
        {
            // prenesenie zmien z view do modelu
            openSong.LyricModels[selectedSlideNumber].text = LyricContent.text;
            openSong.LyricModels[selectedSlideNumber].fontSize = (int)LyricContent.fontSize;
            openSong.LyricModels[selectedSlideNumber].fontFamily = LyricContent.fontFamily;
            openSong.LyricModels[selectedSlideNumber].textAligment = LyricContent.textAligment;
            ObservableCollection<Slide> tmp = textParser.getSlidesFromOpenSong(openSong.LyricModels);
            tmp[selectedSlideNumber].isSelected = true;
            openSongSlides = tmp;

            LyricContent = new LyricViewTemplate1(LyricContent);// to force update
        }
    }

    private bool isSelectedSlide()
    {
        return selectedSlideNumber != -1;
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
        return openSong != null ? new SongModel(openSong) : new SongModel();
    }
}