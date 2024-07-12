using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerLyrics.MVVM.Model;
using PowerLyrics.MVVM.ViewModel;

namespace PowerLyrics.Core.PresentingCore;
public class PresentingCore
{
    private MainViewModel _mainViewModel;
    private PresentingViewModel _presentingViewModel;

    private int _selectedSlide;

    public int SelectedSlide
    {
        get => _selectedSlide;
        set => _selectedSlide = value;
    }

    public PresentingCore(MainViewModel pMainViewModel, PresentingViewModel pPresentingViewModel)
    {
        _mainViewModel = pMainViewModel;
        _presentingViewModel = pPresentingViewModel;
        
        _selectedSlide = -1;
    }

    /// <summary>
    /// Ak true tak sa premieta ak false tak sa nepremieta
    /// </summary>
    /// <param name="pLive"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void GoLive(bool pLive)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Pridá pieseň do playlistu aj z Library ale aj keď je pieseň otvorená zo súboru
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void AddSongToPlayList()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Odstráni pieseň z playlistu
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void RemoveSongFromPlayList()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Daľšia pieseň z playlistu
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void NextSongInPlaylist()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Predcházdajúca pieseň z playlistu
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void PrevSongInPlaylist()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Aplykuje edit danej piesne
    /// </summary>
    /// <param name="getEditedSong">Editovaná pieseň</param>
    /// <exception cref="NotImplementedException"></exception>
    public void applayEdit(SongModel getEditedSong)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Vráti aktuálne otvorenú pieseň
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public SongModel getOpenSong()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Otvorý pieseň
    /// </summary>
    /// <param name="o"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void OpenSong(object o)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Uloží pieseň do súboru
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void SaveSong()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Uloží celý playlist
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void SavePlaylist()
    {
        throw new NotImplementedException();
    }
}
