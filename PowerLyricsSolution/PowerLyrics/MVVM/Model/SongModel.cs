using System.Collections;
using System.Collections.Generic;
using System.Windows.Documents;
using PowerLyrics.Core;

namespace PowerLyrics.MVVM.Model;

public class SongModel
{
    public int number { get; set; }
    
    public int id { get; set; }

    public string name { get; set; }
    public ArrayList verse { get; set; }

    public ArrayList chorus { get; set; }
    
    public ArrayList bridge { get; set; }

    public List<LyricType> lyricTypeQueue { get; set; }
    
    public bool isSelected { get; set; }

    public List<LyricModel> LyricModels { get; set; }

    public SongModel()
    {
        verse = new ArrayList();
        chorus = new ArrayList();
        bridge = new ArrayList();
        lyricTypeQueue = new List<LyricType>();
        LyricModels = new List<LyricModel>();
        isSelected = false;
    }

    public SongModel(SongModel copy)
    {
        if (copy != null)
        {
            verse = copy.verse != null ? new ArrayList(copy.verse) : new ArrayList();
            chorus = copy.chorus != null ? new ArrayList(copy.chorus) : new ArrayList();
            bridge = copy.bridge != null ? new ArrayList(copy.bridge) : new ArrayList();
            lyricTypeQueue = copy.lyricTypeQueue != null ? new List<LyricType>(copy.lyricTypeQueue) : new List<LyricType>();
            LyricModels = copy.LyricModels != null ? new List<LyricModel>(copy.LyricModels) : new List<LyricModel>();
            id = copy.id;
            number = copy.number;
            name = copy.name;
            isSelected = false;
        }
    }
}