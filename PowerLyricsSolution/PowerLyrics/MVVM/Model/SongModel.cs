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
        verse = new ArrayList(copy.verse);
        chorus = new ArrayList(copy.chorus);
        bridge = new ArrayList(copy.bridge);
        lyricTypeQueue = new List<LyricType>(copy.lyricTypeQueue);
        LyricModels = new List<LyricModel>(copy.LyricModels);
        id = copy.id;
        number = copy.number;
        name = copy.name;
        isSelected = false;
    }
}