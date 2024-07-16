using System.Collections.Generic;
using System.Collections.ObjectModel;
using PowerLyrics.MVVM.Model;
using PowerLyrics.MVVM.ViewModel;

namespace PowerLyrics.Core.PresentingCore.PlaylistCore;

/// <summary>
/// Playlist core handle all the playlist operations
/// </summary>
public class PlayListCore
{
    private PresentingViewModel _pPresentingViewModel;
    /// <summary>
    /// Index array of start slide and end slide number of each song used to find the selected song in the playlist
    /// </summary>
    private List<SlideSongIndexingModel> _listOfSongSlideIndex;

    public PlayListCore(PresentingViewModel pPresentingViewModel)
    {
        _pPresentingViewModel = pPresentingViewModel;
        _listOfSongSlideIndex = new();
    }

    /// <summary>
    /// Add song to the playlist
    /// </summary>
    /// <param name="openedSongModel">Song added to the playlist</param>
    public ObservableCollection<Slide> AddSong(SongModel openedSongModel)
    {
        _pPresentingViewModel.ListOfPlaylistSongs.Add(new(openedSongModel));
        _pPresentingViewModel.ListOfPlaylistSongs[_pPresentingViewModel.ListOfPlaylistSongs.Count - 1].id = 
            _pPresentingViewModel.ListOfPlaylistSongs.Count - 1;
        
        _listOfSongSlideIndex.Clear();
        return TextParser.TextParser.getSlidesFromOpenSong(_pPresentingViewModel.ListOfPlaylistSongs,
            _listOfSongSlideIndex);
    }
}