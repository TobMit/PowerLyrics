using PowerLyrics.Core.DataHandler;
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
        _presentingViewModel.ListOfSongs = pLoader.GetSongs();
    }
}