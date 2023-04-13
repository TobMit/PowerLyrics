using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using PowerLyrics.Core;
using PowerLyrics.Core.DataHandler;
using PowerLyrics.Core.TextParser;
using PowerLyrics.MVVM.Model;
using PowerLyrics.MVVM.View;

namespace PowerLyrics.MVVM.ViewModel;

/**
 * Slúži pre správu edit-page
 */
public class EditViewModel : ObservableObjects
{
    private FontFamily _fontFamily;

    private int _fontSize;

    private LyricViewTemplate1 _lyricContent;

    private LyricType _lyricType;

    private string _name;

    private int _number;


    private SongModel _openSong;

    private ObservableCollection<Slide> _openSongSlides;

    private int _selectedSlideNumber = -1;

    private int _serialNuber;

    private string _text;

    private TextAlignment _textAlignment;
    private bool loadingForEdit = true;
    private readonly DataLoader songsLoader;
    private readonly DataSaver songsSaver;
    private readonly TextParser textParser;


    public EditViewModel()
    {
        textParser = new TextParser();
        openSongSlides = new ObservableCollection<Slide>();
        openSong = new SongModel();
        _fontFamily = constants.DEFAULT_FONT_FAMILY;
        songsLoader = new DataLoader();
        songsSaver = new DataSaver();
        inicialiseButtons();
    }

    /**
     * otvorená pieseň ktorá sa edituje
     */
    public SongModel openSong
    {
        get => _openSong;
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

    /**
     * Slide otvorenej piesne
     */
    public ObservableCollection<Slide> openSongSlides
    {
        get => _openSongSlides;
        set
        {
            _openSongSlides = value;
            OnPropertyChanged();
        }
    }

    /**
     * Vybratý slide
     */
    private int selectedSlideNumber
    {
        get => _selectedSlideNumber;
        set
        {
            if (value != -1)
            {
                if (isSelectedSlide()) openSongSlides[_selectedSlideNumber].isSelected = false;

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

    /**
     * otvorený slide v prieview
     */
    public LyricViewTemplate1 LyricContent
    {
        get => _lyricContent;
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

    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            if (!loadingForEdit) applyChanges();

            OnPropertyChanged();
        }
    }

    public int FontSize
    {
        get => _fontSize;
        set
        {
            _fontSize = value;
            if (!loadingForEdit) applyChanges();

            OnPropertyChanged();
        }
    }

    public FontFamily Fontfamily
    {
        get => _fontFamily;
        set
        {
            _fontFamily = value;
            if (!loadingForEdit) applyChanges();

            OnPropertyChanged();
        }
    }

    public TextAlignment TextAlignment
    {
        get => _textAlignment;
        set
        {
            _textAlignment = value;
            if (!loadingForEdit) applyChanges();

            OnPropertyChanged();
        }
    }

    public LyricType LyricType
    {
        get => _lyricType;
        set
        {
            _lyricType = value;
            if (!loadingForEdit) applyChanges();

            OnPropertyChanged();
        }
    }

    public int Number
    {
        get => _number;
        set
        {
            _number = value;
            if (!loadingForEdit) openSong.number = value;
            OnPropertyChanged();
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            if (!loadingForEdit) openSong.name = value;
            OnPropertyChanged();
        }
    }

    public int SerialNuber
    {
        get => _serialNuber;
        set
        {
            _serialNuber = value;
            if (!loadingForEdit) applyChanges();
            OnPropertyChanged();
        }
    }

    /**
     * Inicializuje ovaldanie pomocu tlačidiel
     */
    private void inicialiseButtons()
    {
        SelectSlideCommand = new RelayCommand(o => { SelectSlide(int.Parse(o.ToString())); });

        IncreaseFontCommand = new RelayCommand(o =>
        {
            if (isSelectedSlide()) FontSize += 2;
        });

        DecreaseFontCommand = new RelayCommand(o =>
        {
            // zmeny sa môžu aplikovať iba keď je niečo vybraté
            if (isSelectedSlide()) FontSize -= 2;
        });

        SetTextAligmentCommand = new RelayCommand(o =>
        {
            // zmeny sa môžu aplikovať iba keď je niečo vybraté
            if (isSelectedSlide()) TextAlignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), o.ToString());
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
                openSong.LyricModels.Insert(selectedSlideNumber + 1,
                    new LyricModel(openSong.LyricModels[selectedSlideNumber]));
                openSongSlides = textParser.getSlidesFromOpenSong(openSong.LyricModels);
                SelectSlide(selectedSlideNumber + 1);
            }
        });
    }

    /**
     * Výber slide
     */
    private void SelectSlide(int selectedSlide)
    {
        SelectSlide(selectedSlide, true);
    }

    /**
     * Výber slide a apply edit
     */
    private void SelectSlide(int selectedSlide, bool applyEdit)
    {
        if (applyEdit) applyChanges();

        selectedSlideNumber = selectedSlide;
        LyricContent = (LyricViewTemplate1)openSongSlides[selectedSlideNumber].UserControl;

        loadingForEdit = true; // toto je tu kvoli tomu aby som sa nezaciklyl ked nacitavam data
        Text = openSong.LyricModels[selectedSlideNumber].text;
        Fontfamily = openSong.LyricModels[selectedSlideNumber].fontFamily;
        FontSize = openSong.LyricModels[selectedSlideNumber].fontSize;
        TextAlignment = openSong.LyricModels[selectedSlideNumber].textAligment;
        LyricType = openSong.LyricModels[selectedSlideNumber].LyricType;
        SerialNuber = openSong.LyricModels[selectedSlideNumber].serialNuber;
        loadingForEdit = false;
    }

    /**
     * uloží zmeny z prop naspäť do LyricModelu
     */
    private void applyChanges()
    {
        // zmeny sa môžu aplikovať iba keď je niečo vybraté
        if (isSelectedSlide())
        {
            // prenesenie zmien z view do modelu
            openSong.LyricModels[selectedSlideNumber].text = Text;
            openSong.LyricModels[selectedSlideNumber].fontSize = FontSize;
            openSong.LyricModels[selectedSlideNumber].fontFamily = Fontfamily;
            openSong.LyricModels[selectedSlideNumber].textAligment = TextAlignment;
            openSong.LyricModels[selectedSlideNumber].LyricType = LyricType;
            openSong.LyricModels[selectedSlideNumber].serialNuber = SerialNuber;
            var tmp = textParser.getSlidesFromOpenSong(openSong.LyricModels);
            tmp[selectedSlideNumber].isSelected = true;
            openSongSlides = tmp;

            // to force update
            LyricContent = (LyricViewTemplate1)openSongSlides[selectedSlideNumber].UserControl;
        }
    }

    /**
     * Slide je vybratý ak je číslo íné ako -1
     */
    private bool isSelectedSlide()
    {
        return selectedSlideNumber != -1;
    }

    /**
     * Metóda na otvorenie piesene zo súboru
     */
    public void OpenSong(string? path)
    {
        if (openSongSlides.Count != 0)
        {
            MessageBox.Show("You are trying override song.", "Open song", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (path != null)
            songsLoader.loadFileStartUp(path);
        else
            songsLoader.loadFile();
        switch (songsLoader.openedFileType)
        {
            case FileType.Song:
                var oldId = openSong.id;
                openSong = songsLoader.getSongModel();
                openSong.id = oldId;
                openSongSlides = textParser.getSlidesFromOpenSong(openSong.LyricModels);
                break;
            case FileType.PlayList:
                MessageBox.Show("You are trying open playlist in edit-page!\nIn edit-page zou can open only SONGS.",
                    "Open song", MessageBoxButton.OK, MessageBoxImage.Warning);
                break;
            default:
                break;
        }
    }

    /**
     * Uloží pieseň zo súboru
     */
    public void SaveSong()
    {
        if (openSong != null) songsSaver.saveSong(openSong);
    }

    /**
     * vráti edit v piesne
     */
    public SongModel getEditedSong()
    {
        applyChanges();
        return openSong != null ? new SongModel(openSong) : new SongModel();
    }
}