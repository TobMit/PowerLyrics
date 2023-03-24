using PowerLyrics.Core;
using PowerLyrics.Core.TextParser;
using PowerLyrics.MVVM.Model;
using PowerLyrics.MVVM.View;
using PowerLyrics.Windows;
using System;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;

namespace PowerLyrics.MVVM.ViewModel;

public class EditViewModel : ObservableObjects
{
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
            }
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

    private object _lyricContent;
    public object LyricContent
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
        openSongSlides = new ObservableCollection<Slide>();
        inicialiseButtons();
    }

    private void inicialiseButtons()
    {

        SelectSlideCommand = new RelayCommand(o =>
        {
            selectedSlideNumber = Int32.Parse(o.ToString());
            LyricContent = new LyricViewTemplate1((LyricViewTemplate1)openSongSlides[selectedSlideNumber].UserControl);
        });
        
    }

    public ObservableCollection<Slide> getEditedSong()
    {
        // to je preto aby sa mi vybraty slide neobjavil aj v presenting mode
        ObservableCollection<Slide> tmp = new ObservableCollection<Slide>(openSongSlides);
        foreach (Slide slide in tmp)
        {
            slide.isSelected = false;
        }
        return tmp;
    }
}