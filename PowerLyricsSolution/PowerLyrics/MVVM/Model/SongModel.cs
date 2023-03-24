using System.Collections;
using System.Collections.Generic;
using System.Windows.Documents;
using PowerLyrics.Core;

namespace PowerLyrics.MVVM.Model;

public class Song
{
    public int number { get; set; }
    
    public int id { get; set; }

    public string name { get; set; }
    public ArrayList verse { get; set; }

    public ArrayList chorus { get; set; }
    
    public ArrayList bridge { get; set; }

    public List<LyricType> lyricTypeQueue { get; set; }
    
    public bool isSelected { get; set; }

    public Song()
    {
        verse = new ArrayList();
        chorus = new ArrayList();
        bridge = new ArrayList();
        lyricTypeQueue = new List<LyricType>();
        isSelected = false;
    }

    public Song(Song copy)
    {
        verse = new ArrayList(copy.verse);
        chorus = new ArrayList(copy.chorus);
        bridge = new ArrayList(copy.bridge);
        lyricTypeQueue = new List<LyricType>(copy.lyricTypeQueue);
        id = copy.id;
        number = copy.number;
        name = copy.name;
        isSelected = false;
    }
}