using PowerLyrics.Core.DataHandler;
using PowerLyrics.MVVM.Model;
using PowerLyrics.MVVM.ViewModel;

namespace PowerLyrics.Core.PresentingCore.LibraryCore;

/// <summary>
/// Library core handle all the library operations
/// </summary>
public class LibraryCore
{
    private PresentingViewModel _presentingViewModel;
    public LibraryCore(PresentingViewModel pPresentingViewModel)
    {
        _presentingViewModel = pPresentingViewModel;
    }

    /// <summary>
    /// Loads library from the Song folder and process them with data loader
    /// </summary>
    /// <param name="pLoader">Load the library</param>
    public void InitLibrary(DataLoader pLoader)
    {
        _presentingViewModel.ListOfLibrarySongs = pLoader.GetSongs();
    }

    //todo vyriešiť selected from library aby sa zrušilo vyznačenie
    /// <summary>
    /// When song is no longer selected from library is needed to by deselect
    /// </summary>
    public void DeselectSong()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Get selected song from library
    /// </summary>
    /// <param name="index"></param>
    /// <returns>Selected Song</returns>
    public SongModel GetSong(int index)
    {
        return new(_presentingViewModel.ListOfLibrarySongs[index]);
    }
}