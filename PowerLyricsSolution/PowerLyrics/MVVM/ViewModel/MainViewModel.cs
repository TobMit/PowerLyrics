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
                _presentingViewModel.AddSongToPlayList();
            });
            RemoveSongFromPlayListCommand = new RelayCommand(o =>
            {
                _presentingViewModel.RemoveSongFromPlayList();
            });
            NextSongInPlaylistCommand = new RelayCommand(o =>
            {
                _presentingViewModel.NextSongInPlaylist();
            });
            PrewSongInPlaylistCommand = new RelayCommand(o =>
            {
                _presentingViewModel.PrevSongInPlaylist();
            });
            SetPresentingPageCommand = new RelayCommand(o =>
            {
                _UserControl = _presentingView;
            });
            SetEditPageCommand = new RelayCommand(o =>
            {
                _UserControl = _editView;
            });
            
        }

        public void closeWindow()
        {
            _presentingViewModel.closeWindow();
        }
    }
}
