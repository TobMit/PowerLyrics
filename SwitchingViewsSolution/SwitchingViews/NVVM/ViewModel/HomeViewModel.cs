using SwitchingViews.Core;
using SwitchingViews.Services;

namespace SwitchingViews.NVVM.ViewModel;

public class HomeViewModel : Core.ViewModel
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

    public RealyCommand NavigateToSettingsViewCommand { get; set; }

    public HomeViewModel(INavigationService navigation)
    {
        Navigation = navigation;
        NavigateToSettingsViewCommand =
            new RealyCommand(o => { Navigation.NavigateTo<SettingsViewModel>(); }, o => true);

    }
    
}