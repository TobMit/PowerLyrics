﻿using PowerLyrics.Core;
using PowerLyrics.Core.DataHandler;
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
/**
 * Slúži pre správu edit-page
 */
public class EditViewModel : ObservableObjects
{
    private TextParser textParser;
    private bool loadingForEdit = true;
    private DataLoader songsLoader;
    private DataSaver songsSaver;


    private SongModel _openSong;
    /**
     * otvorená pieseň ktorá sa edituje
     */
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
    /**
     * Slide otvorenej piesne
     */
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
    /**
     * Vybratý slide
     */
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
    /**
     * otvorený slide v prieview
     */
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

    private int _serialNuber;
    public int SerialNuber
    {
        get { return _serialNuber; }
        set
        {
            _serialNuber = value;
            if (!loadingForEdit)
            {
                applyChanges();
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
        songsLoader = new DataLoader();
        songsSaver = new DataSaver();
        inicialiseButtons();
    }
    /**
     * Inicializuje ovaldanie pomocu tlačidiel
     */
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
                openSong.LyricModels.Insert(selectedSlideNumber + 1, new LyricModel(openSong.LyricModels[selectedSlideNumber]));
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
        this.SelectSlide(selectedSlide, true);
    }
    /**
     * Výber slide a apply edit
     */
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
        this.SerialNuber = openSong.LyricModels[selectedSlideNumber].serialNuber;
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
            openSong.LyricModels[selectedSlideNumber].text = this.Text;
            openSong.LyricModels[selectedSlideNumber].fontSize = (int)this.FontSize;
            openSong.LyricModels[selectedSlideNumber].fontFamily = this.Fontfamily;
            openSong.LyricModels[selectedSlideNumber].textAligment = this.TextAlignment;
            openSong.LyricModels[selectedSlideNumber].LyricType = this.LyricType;
            openSong.LyricModels[selectedSlideNumber].serialNuber = this.SerialNuber;
            ObservableCollection<Slide> tmp = textParser.getSlidesFromOpenSong(openSong.LyricModels);
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
        {
            songsLoader.loadFileStartUp(path);
        }
        else
        {
            songsLoader.loadFile();
        }
        switch (songsLoader.openedFileType)
        {
            case FileType.Song:
                int oldId = openSong.id;
                openSong = songsLoader.getSongModel();
                openSong.id = oldId;
                openSongSlides= textParser.getSlidesFromOpenSong(openSong.LyricModels);
                break;
            case FileType.PlayList:
                MessageBox.Show("You are trying open playlist in edit-page!\nIn edit-page zou can open only SONGS.", "Open song", MessageBoxButton.OK, MessageBoxImage.Warning);
                break;
        }
    }
    /**
     * Uloží pieseň zo súboru
     */
    public void SaveSong()
    {
        if (openSong != null)
        {
            this.songsSaver.saveSong(openSong);
        }
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