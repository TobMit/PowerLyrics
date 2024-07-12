using System.Windows;
using System.Windows.Controls;
using PowerLyrics.Core;
using PowerLyrics.Core.PresentingCore;
using PowerLyrics.MVVM.View;

namespace PowerLyrics.MVVM.ViewModel;

public class MainViewModel : ObservableObjects
{
    private readonly PresentingCore _core;

    private Visibility _editingButtons;
    private readonly EditView _editView;
    private readonly EditViewModel _editViewModel;

    private bool _presenting;

    private Visibility _presentingButtons;
    private readonly PresentingView _presentingView;
    private readonly PresentingViewModel _presentingViewModel;

    private UserControl _userControl;

    public MainViewModel()
    {
        inicialiseButtons();
        _core = new(this, _presentingViewModel);
        _presentingView = new PresentingView();
        _presentingViewModel = _presentingView.getDataContext();
        _presentingViewModel.PresentingView = _presentingView;
        _editView = new EditView();
        _editViewModel = _editView.getDataContext();
        presenting = true;

        _userControl = _presentingView;
    }

    private bool presenting
    {
        get => _presenting;
        set
        {
            _presenting = value;
            if (_presenting)
            {
                PresentingButtons = Visibility.Visible;
                EditingButtons = Visibility.Hidden;
            }
            else
            {
                PresentingButtons = Visibility.Hidden;
                EditingButtons = Visibility.Visible;
            }
        }
    }

    public Visibility PresentingButtons
    {
        get => _presentingButtons;
        set
        {
            _presentingButtons = value;
            OnPropertyChanged();
        }
    }

    public Visibility EditingButtons
    {
        get => _editingButtons;
        set
        {
            _editingButtons = value;
            OnPropertyChanged();
        }
    }

    /**
         * Set audience window full screan
         */
    public RelayCommand SetAudenceFullScreanCommand { get; set; }

    /**
         * Set status to live
         */
    public RelayCommand GoLiveCommand { get; set; }

    /**
         * Add song from library to playlist
         */
    public RelayCommand AddSongToPlayListCommand { get; set; }

    /**
         * Remove song from playlist
         */
    public RelayCommand RemoveSongFromPlayListCommand { get; set; }

    /**
         * Fast forward selected slide to first slide on next song
         */
    public RelayCommand NextSongInPlaylistCommand { get; set; }

    /**
         * Fast rewind selected slide to first slide previous song
         */
    public RelayCommand PrewSongInPlaylistCommand { get; set; }

    public RelayCommand SetPresentingPageCommand { get; set; }

    public RelayCommand SetEditPageCommand { get; set; }
    public RelayCommand OpenSongCommand { get; set; }
    public RelayCommand SaveSongCommand { get; set; }
    public RelayCommand SavePlaylistCommand { get; set; }

    public UserControl _UserControl
    {
        get => _userControl;
        set
        {
            _userControl = value;
            OnPropertyChanged();
        }
    }

    /**
         * Inicializácia tlačidiel
         */
    private void inicialiseButtons()
    {
        SetAudenceFullScreanCommand = new RelayCommand(o => { _presentingViewModel.SetAudienceFullScreanCommand(); });

        GoLiveCommand = new RelayCommand(o => { _core.GoLive((bool)o); });

        AddSongToPlayListCommand = new RelayCommand(o =>
        {
            if (buttonCheck(presenting))
                _core.AddSongToPlayList();
        });
        RemoveSongFromPlayListCommand = new RelayCommand(o =>
        {
            if (buttonCheck(presenting))
                _core.RemoveSongFromPlayList();
        });
        NextSongInPlaylistCommand = new RelayCommand(o =>
        {
            if (buttonCheck(presenting))
                _core.NextSongInPlaylist();
        });
        PrewSongInPlaylistCommand = new RelayCommand(o =>
        {
            if (buttonCheck(presenting))
                _core.PrevSongInPlaylist();
        });
        SetPresentingPageCommand = new RelayCommand(o =>
        {
            _UserControl = _presentingView;
            presenting = true;
            _core.applayEdit(_editViewModel.getEditedSong());
        });
        SetEditPageCommand = new RelayCommand(o =>
        {
            _UserControl = _editView;
            //todo fix
            _presentingViewModel.selectedSlide =
                -1; // aby ked sa preklikne do edit page a zrusi zdielanie tak aby nenabehol slide
            presenting = false;
            _editViewModel.openSong = _core.getOpenSong();
        });
        OpenSongCommand = new RelayCommand(o =>
        {
            if (presenting)
                _core.OpenSong(null);
            else
                _editViewModel.OpenSong(null);
        });
        SaveSongCommand = new RelayCommand(o =>
        {
            if (presenting)
                _core.SaveSong();
            else
                _editViewModel.SaveSong();
        });
        SavePlaylistCommand = new RelayCommand(o =>
        {
            if (buttonCheck(presenting))
                _core.SavePlaylist();
        });
    }

    /// <summary>
    /// If we can use button in this view it throws error
    /// </summary>
    /// <param name="pPresenting"></param>
    /// <returns></returns>
    private bool buttonCheck(bool pPresenting){
        if (!pPresenting)
        {
            MessageBox.Show("You can use this button only in SHOW-PAGE", "Button", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Open song from start up
    /// </summary>
    /// <param name="path"></param>
    public void openSongOnStartup(string path)
    {
        _presentingViewModel.OpenSong(path);
    }

    /// <summary>
    /// Close audience window
    /// </summary>
    public void closeWindow()
    {
        _presentingViewModel.closeWindow();
    }
}