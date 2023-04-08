using System.Windows.Controls;
using PowerLyrics.Core;
using PowerLyrics.MVVM.View;

namespace PowerLyrics.MVVM.ViewModel
{
    public class MainViewModel : ObservableObjects
    {
        private PresentingViewModel _presentingViewModel;
        private PresentingView _presentingView;
        private EditViewModel _editViewModel;
        private EditView _editView;
        private bool presenting = true;
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

        private UserControl _userControl;

        public UserControl _UserControl
        {
            get
            {
                return _userControl;
            }
            set
            {
                _userControl = value;
                OnPropertyChanged();
            }
        }
        
        public MainViewModel()
        {
            inicialiseButtons();
            _presentingView = new PresentingView();
            _presentingViewModel = _presentingView.getDataContext();
            _presentingViewModel.PresentingView = _presentingView;
            _editView = new EditView();
            _editViewModel = _editView.getDataContext();
            
           _userControl = _presentingView;
        }

        private void inicialiseButtons()
        {
            
            SetAudenceFullScreanCommand = new RelayCommand(o =>
            {
                _presentingViewModel.SetAudienceFullScreanCommand();
            });

            GoLiveCommand = new RelayCommand(o =>
            {
                _presentingViewModel.GoLive((bool)o);
            });
            
            AddSongToPlayListCommand = new RelayCommand(o =>
            {
                if (presenting)
                {
                    _presentingViewModel.AddSongToPlayList();
                }
            });
            RemoveSongFromPlayListCommand = new RelayCommand(o =>
            {
                if (presenting)
                {
                    _presentingViewModel.RemoveSongFromPlayList();
                }
            });
            NextSongInPlaylistCommand = new RelayCommand(o =>
            {
                if (presenting)
                {
                    _presentingViewModel.NextSongInPlaylist();
                }
            });
            PrewSongInPlaylistCommand = new RelayCommand(o =>
            {
                if (presenting)
                {
                    _presentingViewModel.PrevSongInPlaylist();
                }
            });
            SetPresentingPageCommand = new RelayCommand(o =>
            {
                _UserControl = _presentingView;
                presenting = true;
                _presentingViewModel.applayEdit(_editViewModel.getEditedSong());
            });
            SetEditPageCommand = new RelayCommand(o =>
            {
                _UserControl = _editView;
                _presentingViewModel.selectedSlide = -1; // aby ked sa preklikne do edit page a zrusi zdielanie tak aby nenabehol slide
                presenting = false;
                _editViewModel.openSong = _presentingViewModel.getOpenSong();
            });
            OpenSongCommand = new RelayCommand(o =>
            {
                if (presenting)
                {
                    _presentingViewModel.OpenSong(null);
                }
                else
                {
                    _editViewModel.OpenSong(null);
                }
            });
            SaveSongCommand = new RelayCommand(o =>
            {
                if (presenting)
                {
                    _presentingViewModel.SaveSong();
                }
                else
                {
                    _editViewModel.SaveSong();
                }
            });
            SavePlaylistCommand = new RelayCommand(o =>
            {
                if (presenting)
                {
                    _presentingViewModel.SavePlaylist();
                }
            });

        }

        public void openSongOnStartup(string path)
        {
            _presentingViewModel.OpenSong(path);
        }

        public void closeWindow()
        {
            _presentingViewModel.closeWindow();
        }
    }
}
