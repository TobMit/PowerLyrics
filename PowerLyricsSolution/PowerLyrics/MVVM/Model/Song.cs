using System.Collections;
using System.Collections.Generic;
using System.Windows.Documents;
using PowerLyrics.Core;

namespace PowerLyrics.MVVM.Model;

public class Song
{
    public int number { get; set; }

    public string name { get; set; }
    public ArrayList verse { get; set; }

    public ArrayList chorus { get; set; }
    
    public ArrayList bridge { get; set; }

    public List<LyricType> lyricTypeQueue { get; set; }

    public Song()
    {
        verse = new ArrayList();
        chorus = new ArrayList();
        bridge = new ArrayList();
        lyricTypeQueue = new List<LyricType>();
    }
}