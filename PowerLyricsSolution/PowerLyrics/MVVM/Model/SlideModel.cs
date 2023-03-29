using PowerLyrics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PowerLyrics.MVVM.Model
{
    public class Slide : ObservableObjects
    {
        public UserControl? UserControl { get; set; }
        public SlideType SlideType { get; set; }
        public LyricType LyricType { get; set; }
        public int id { get; set; }
        public string labelText { get; set; }
        private bool _isSelected;
        public bool isSelected {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public Slide()
        {
            
        }
        //copy constructor for Slide
        public Slide(Slide copy)
        {
            UserControl = copy.UserControl;
            SlideType = copy.SlideType;
            id = copy.id;
            labelText = copy.labelText;
            LyricType = copy.LyricType;
            isSelected = false;
        }
    }
}
