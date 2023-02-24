using SwitchingViews.Core;
using SwitchingViews.Services;

namespace SwitchingViews.NVVM.ViewModel;

public class SettingsViewModel : Core.ViewModel
{
    private INavigationService _navigation;

    public INavigationService Navigation
    {
        get => _navigation;
        set
        {
            _navigation = value;
            OnPropertyChanged();
        }
    }

    public RealyCommand NavigateToHomeViewCommand { get; set; }

    public SettingsViewModel(INavigationService navigation)
    {
        Navigation = navigation;
        NavigateToHomeViewCommand =
            new RealyCommand(o => { Navigation.NavigateTo<HomeViewModel>(); }, o => true);

    }
    
}