using SwitchingViews.Core;
using SwitchingViews.Services;

namespace SwitchingViews.NVVM.ViewModel;

public class MainViewModel : Core.ViewModel
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

    public RealyCommand NavigateToHomeCommand { get; set; }
    public RealyCommand NavigateToSettingsCommand { get; set; }

    public MainViewModel(INavigationService navService)
    {
        Navigation = navService;
        NavigateToHomeCommand = new RealyCommand(o => { Navigation.NavigateTo<HomeViewModel>(); }, o => true);
        NavigateToSettingsCommand = new RealyCommand(o => { Navigation.NavigateTo<SettingsViewModel>(); }, o => true);
    }
}