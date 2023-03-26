﻿using PowerLyrics.Core;
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


    public EditViewModel()
    {
        textParser = new TextParser();
        openSongSlides = new ObservableCollection<Slide>();
        this._fontFamily = constants.DEFAULT_FONT_FAMILY;
        inicialiseButtons();
    }

    private void inicialiseButtons()
    {
        SelectSlideCommand = new RelayCommand(o =>
        {
            applyChanges();
            selectedSlideNumber = Int32.Parse(o.ToString());
            LyricContent = (LyricViewTemplate1)openSongSlides[selectedSlideNumber].UserControl;

            loadingForEdit = true; // toto je tu kvoli tomu aby som sa nezaciklyl ked nacitavam data
            this.Text = openSong.LyricModels[selectedSlideNumber].text;
            this.Fontfamily = openSong.LyricModels[selectedSlideNumber].fontFamily;
            this.FontSize = openSong.LyricModels[selectedSlideNumber].fontSize;
            this.TextAlignment = openSong.LyricModels[selectedSlideNumber].textAligment;
            loadingForEdit = false;
        });

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