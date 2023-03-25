using PowerLyrics.Core;
using PowerLyrics.Core.TextParser;
using PowerLyrics.MVVM.Model;
using PowerLyrics.MVVM.View;
using PowerLyrics.Windows;
using System;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Windows.Controls;

namespace PowerLyrics.MVVM.ViewModel;

public class EditViewModel : ObservableObjects
{
    private TextParser textParser;


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
    }

    private void inicialiseButtons()
    {

        SelectSlideCommand = new RelayCommand(o =>
        {
            if (selectedSlideNumber == -1)
            {
                selectedSlideNumber = Int32.Parse(o.ToString());
                LyricContent = new LyricViewTemplate1((LyricViewTemplate1)openSongSlides[selectedSlideNumber].UserControl);
            }
            else
            {
                applyChanges();
                selectedSlideNumber = Int32.Parse(o.ToString());
                LyricContent = new LyricViewTemplate1((LyricViewTemplate1)openSongSlides[selectedSlideNumber].UserControl);
            }
            
        });
        
    }

    private void applyChanges()
    {
        openSong.LyricModels[selectedSlideNumber].text = LyricContent.text;
        openSong.LyricModels[selectedSlideNumber].fontSize = (int)LyricContent.fontSize;
        openSongSlides = textParser.getSlidesFromOpenSong(openSong.LyricModels);
    }

    public SongModel getEditedSong()
    {
        applyChanges();
        return new SongModel(openSong); ;
    }

}