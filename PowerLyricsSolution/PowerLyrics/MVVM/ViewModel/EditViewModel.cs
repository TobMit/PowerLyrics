using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using PowerLyrics.Core;
using PowerLyrics.Core.DataHandler;
using PowerLyrics.Core.TextParser;
using PowerLyrics.MVVM.Model;
using PowerLyrics.MVVM.Model.SlideContentModels;
using PowerLyrics.MVVM.View;

namespace PowerLyrics.MVVM.ViewModel;

/**
 * Slúži pre správu edit-page
 */
public class EditViewModel : ObservableObjects
{
    private readonly DataLoader songsLoader;
    private readonly DataSaver songsSaver;
    private readonly TextParser textParser;
    private FontFamily _fontFamily;

    private int _fontSize;

    private LyricViewTemplate _lyricContent;

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
    private Visibility textSlideVisibility;
    private Visibility videoSlideVisibility;


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
     * Nástroje pomocou ktorých sa edituje pieseň
     */
    public Visibility TextSlideVisibility
    {
        get => textSlideVisibility;
        set
        {
            textSlideVisibility = value;
            OnPropertyChanged();
        }
    }

    /**
     * Nástroje pomocou ktorých sa edituje pieseň
     */
    public Visibility VideoSlideVisibility
    {
        get => videoSlideVisibility;
        set
        {
            videoSlideVisibility = value;
            OnPropertyChanged();
        }
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
            openSongSlides = textParser.getSlidesFromOpenSong(_openSong.ContentModels);

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
    public LyricViewTemplate LyricContent
    {
        get => _lyricContent;
        set
        {
            _lyricContent = value == null ? new LyricViewTemplateText() : (LyricViewTemplate)value.Clone();
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
    public RelayCommand AddVideoSlideCommand { get; set; }
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
            openSong.ContentModels.Insert(selectedSlideNumber + 1, new LyricModel(SlideContentType.Text));
            openSongSlides = textParser.getSlidesFromOpenSong(openSong.ContentModels);
            SelectSlide(selectedSlideNumber + 1);
        });

        AddVideoSlideCommand = new RelayCommand(o =>
        {
            openSong.ContentModels.Insert(selectedSlideNumber + 1, new LyricModel(SlideContentType.Video));
            openSongSlides = textParser.getSlidesFromOpenSong(openSong.ContentModels);
            SelectSlide(selectedSlideNumber + 1);
        });

        RemoveSlidetCommand = new RelayCommand(o =>
        {
            // zmeny sa môžu aplikovať iba keď je niečo vybraté
            if (isSelectedSlide())
            {
                openSong.ContentModels.RemoveAt(selectedSlideNumber);
                openSongSlides = textParser.getSlidesFromOpenSong(openSong.ContentModels);
                SelectSlide(selectedSlideNumber, false);
            }
        });

        DuplicateSlideCommand = new RelayCommand(o =>
        {
            // zmeny sa môžu aplikovať iba keď je niečo vybraté
            if (isSelectedSlide())
            {
                openSong.ContentModels.Insert(selectedSlideNumber + 1,
                    openSong.ContentModels[selectedSlideNumber].Clone());
                openSongSlides = textParser.getSlidesFromOpenSong(openSong.ContentModels);
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
        LyricContent = (LyricViewTemplate)openSongSlides[selectedSlideNumber].UserControl;

        loadingForEdit = true; // toto je tu kvoli tomu aby som sa nezaciklyl ked nacitavam data
        if (openSong.ContentModels[selectedSlideNumber].GetType() == typeof(LyricModel))
        {
            LyricModel lyricModel = (LyricModel)openSong.ContentModels[selectedSlideNumber];
            Text = lyricModel.text;
            Fontfamily = lyricModel.fontFamily;
            FontSize = lyricModel.fontSize;
            TextAlignment = lyricModel.textAligment;
            LyricType = lyricModel.LyricType;
            SerialNuber = lyricModel.serialNuber;
        }
        
        setEditTools(openSong.ContentModels[selectedSlideNumber].slideContentType);
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
            if (openSong.ContentModels[selectedSlideNumber].slideContentType == SlideContentType.Text)
            {
                // prenesenie zmien z view do modelu
                LyricModel lyricModel = new LyricModel();
                lyricModel.text = Text;
                lyricModel.fontSize = FontSize;
                lyricModel.fontFamily = Fontfamily;
                lyricModel.textAligment = TextAlignment;
                lyricModel.LyricType = LyricType;
                lyricModel.serialNuber = SerialNuber;

                openSong.ContentModels[selectedSlideNumber] = lyricModel;
            }
            else
            {
                //todo add impl
            }


            var tmp = textParser.getSlidesFromOpenSong(openSong.ContentModels);
            tmp[selectedSlideNumber].isSelected = true;
            openSongSlides = tmp;

            // to force update
            LyricContent = (LyricViewTemplate)openSongSlides[selectedSlideNumber].UserControl;
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
                openSongSlides = textParser.getSlidesFromOpenSong(openSong.ContentModels);
                break;
            case FileType.PlayList:
                MessageBox.Show("You are trying open playlist in edit-page!\nIn edit-page zou can open only SONGS.",
                    "Open song", MessageBoxButton.OK, MessageBoxImage.Warning);
                break;
        }
    }

    /**
     * Vyberie správne nástroje na edit podľa typu slide
     */
    private void setEditTools(SlideContentType type)
    {
        VideoSlideVisibility = Visibility.Collapsed;
        TextSlideVisibility = Visibility.Collapsed;

        switch (type)
        {
            case SlideContentType.Text:
                TextSlideVisibility = Visibility.Visible;
                break;
            case SlideContentType.Video:
                VideoSlideVisibility = Visibility.Visible;
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