using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestUI.Core;

namespace TestUI.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {

        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand DiscoveryViewCommand { get; set; }

        public HomeViewModel HomeVm { get; set; }     
        public DiscoveryViewModel DiscoveryVM { get; set; }     

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value; 
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            HomeVm = new HomeViewModel();
            DiscoveryVM = new DiscoveryViewModel();

            CurrentView = HomeVm;

            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = HomeVm;
            });

            DiscoveryViewCommand = new RelayCommand(o =>
            {
                CurrentView = DiscoveryVM;
            });
        }

    }
}

