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
using System.Windows.Media;
using System.Windows.Threading;

namespace PowerLyrics.MVVM.ViewModel;

public class EditViewModel : ObservableObjects
{
    private TextParser textParser;
    private bool loadingForEdit = true;


    private SongModel _openSong;

    public SongModel openSong
    {
        get { return _openSong; }
        set
        {
            _openSong = new SongModel(value);
            selectedSlideNumber = -1;
            openSongSlides = textParser.getSlidesFromOpenSong(_openSong.LyricModels);

            loadingForEdit = true;
            Name = openSong.name;
            Number = openSong.number; 
            loadingForEdit = false;
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
            _lyricContent = new LyricViewTemplate1(value);
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
    public RelayCommand AddSlideCommand { get; set; }
    public RelayCommand RemoveSlidetCommand { get; set; }
    public RelayCommand DuplicateSlideCommand { get; set; }

    private string _text;

    public string Text
    {
        get { return _text; }
        set
        {
            _text = value;
            if (!loadingForEdit)
            {
                applyChanges();
            }

            OnPropertyChanged();
        }
    }

    private int _fontSize;

    public int FontSize
    {
        get { return _fontSize; }
        set
        {
            _fontSize = value;
            if (!loadingForEdit)
            {
                applyChanges();
            }

            OnPropertyChanged();
        }
    }

    private FontFamily _fontFamily;

    public FontFamily Fontfamily
    {
        get { return _fontFamily; }
        set
        {
            _fontFamily = value;
            if (!loadingForEdit)
            {
                applyChanges();
            }

            OnPropertyChanged();
        }
    }

    private TextAlignment _textAlignment;

    public TextAlignment TextAlignment
    {
        get { return _textAlignment; }
        set
        {
            _textAlignment = value;
            if (!loadingForEdit)
            {
                applyChanges();
            }

            OnPropertyChanged();
        }
    }

    private LyricType _lyricType;

    public LyricType LyricType
    {
        get { return _lyricType; }
        set
        {
            _lyricType = value;
            if (!loadingForEdit)
            {
                applyChanges();
            }

            OnPropertyChanged();
        }
    }

    private int _number;
    public int Number
    {
        get { return _number; }
        set
        {
            _number = value;
            if (!loadingForEdit)
            {
                openSong.number = value;
            }
            OnPropertyChanged();
        }
    }

    private string _name;
    public string Name
    {
        get { return _name; }
        set
        {
            _name = value;
            if (!loadingForEdit)
            {
                openSong.name = value;
            }
            OnPropertyChanged();
        }
    }


    public EditViewModel()
    {
        textParser = new TextParser();
        openSongSlides = new ObservableCollection<Slide>();
        openSong = new SongModel();
        this._fontFamily = constants.DEFAULT_FONT_FAMILY;
        inicialiseButtons();
    }

    private void inicialiseButtons()
    {
        SelectSlideCommand = new RelayCommand(o => { SelectSlide(Int32.Parse(o.ToString())); });

        IncreaseFontCommand = new RelayCommand(o =>
        {
            if (isSelectedSlide())
            {
                this.FontSize += 2;
            }
        });

        DecreaseFontCommand = new RelayCommand(o =>
        {
            // zmeny sa môžu aplikovať iba keď je niečo vybraté
            if (isSelectedSlide())
            {
                this.FontSize -= 2;
            }
        });

        SetTextAligmentCommand = new RelayCommand(o =>
        {
            // zmeny sa môžu aplikovať iba keď je niečo vybraté
            if (isSelectedSlide())
            {
                this.TextAlignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), o.ToString());
            }
        });

        AddSlideCommand = new RelayCommand(o =>
        {
            openSong.LyricModels.Insert(selectedSlideNumber + 1, new LyricModel());
            openSongSlides = textParser.getSlidesFromOpenSong(openSong.LyricModels);
            SelectSlide(selectedSlideNumber + 1);
        });

        RemoveSlidetCommand = new RelayCommand(o =>
        {
            // zmeny sa môžu aplikovať iba keď je niečo vybraté
            if (isSelectedSlide())
            {
                openSong.LyricModels.RemoveAt(selectedSlideNumber);
                openSongSlides = textParser.getSlidesFromOpenSong(openSong.LyricModels);
                SelectSlide(selectedSlideNumber, false);
            }
        });

        DuplicateSlideCommand = new RelayCommand(o =>
        {
            // zmeny sa môžu aplikovať iba keď je niečo vybraté
            if (isSelectedSlide())
            {
                openSong.LyricModels.Insert(selectedSlideNumber + 1, openSong.LyricModels[selectedSlideNumber]);
                openSongSlides = textParser.getSlidesFromOpenSong(openSong.LyricModels);
                SelectSlide(selectedSlideNumber + 1);
            }
        });
    }

    private void SelectSlide(int selectedSlide)
    {
        this.SelectSlide(selectedSlide, true);
    }

    private void SelectSlide(int selectedSlide, bool applyEdit)
    {
        if (applyEdit)
        {
            applyChanges();
        }

        selectedSlideNumber = selectedSlide;
        LyricContent = (LyricViewTemplate1)openSongSlides[selectedSlideNumber].UserControl;

        loadingForEdit = true; // toto je tu kvoli tomu aby som sa nezaciklyl ked nacitavam data
        this.Text = openSong.LyricModels[selectedSlideNumber].text;
        this.Fontfamily = openSong.LyricModels[selectedSlideNumber].fontFamily;
        this.FontSize = openSong.LyricModels[selectedSlideNumber].fontSize;
        this.TextAlignment = openSong.LyricModels[selectedSlideNumber].textAligment;
        this.LyricType = openSong.LyricModels[selectedSlideNumber].LyricType;
        loadingForEdit = false;
    }

    private void applyChanges()
    {
        // zmeny sa môžu aplikovať iba keď je niečo vybraté
        if (isSelectedSlide())
        {
            // prenesenie zmien z view do modelu
            openSong.LyricModels[selectedSlideNumber].text = this.Text;
            openSong.LyricModels[selectedSlideNumber].fontSize = (int)this.FontSize;
            openSong.LyricModels[selectedSlideNumber].fontFamily = this.Fontfamily;
            openSong.LyricModels[selectedSlideNumber].textAligment = this.TextAlignment;
            openSong.LyricModels[selectedSlideNumber].LyricType = this.LyricType;
            ObservableCollection<Slide> tmp = textParser.getSlidesFromOpenSong(openSong.LyricModels);
            tmp[selectedSlideNumber].isSelected = true;
            openSongSlides = tmp;

            // to force update
            LyricContent = (LyricViewTemplate1)openSongSlides[selectedSlideNumber].UserControl;
        }
    }

    private bool isSelectedSlide()
    {
        return selectedSlideNumber != -1;
    }


    public SongModel getEditedSong()
    {
        applyChanges();
        return openSong != null ? new SongModel(openSong) : new SongModel();
    }
}