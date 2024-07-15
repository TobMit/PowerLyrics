using PowerLyrics.MVVM.ViewModel;

namespace PowerLyrics.Core.PresentingCore.PlaylistCore;

/// <summary>
/// Playlist core handle all the playlist operations
/// </summary>
public class PlayListCore
{
    private PresentingViewModel _pPresentingViewModel;

    public PlayListCore(PresentingViewModel pPresentingViewModel)
    {
        _pPresentingViewModel = pPresentingViewModel;
    }
}