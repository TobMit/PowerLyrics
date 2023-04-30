using System.Windows.Controls;
using PowerLyrics.Core;

namespace PowerLyrics.MVVM.Model;

/**
     * Model priamo Slide
     */
public class Slide : ObservableObjects
{
    private bool _isSelected;

    public Slide()
    {
        UserControl = null;
        SlideType = SlideType.Slide;
        LyricType = LyricType.Undefined;
        id = 0;
        labelText = "";
        isSelected = false;
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

    public UserControl? UserControl { get; set; }
    public SlideType SlideType { get; set; }
    public LyricType LyricType { get; set; }
    public int id { get; set; }
    public string labelText { get; set; }

    public bool isSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged();
        }
    }
}