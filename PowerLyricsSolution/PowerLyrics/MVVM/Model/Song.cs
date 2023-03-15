using System.Collections;

namespace PowerLyrics.MVVM.Model;

public class Song
{
    public int number { get; set; }

    public string name { get; set; }
    public ArrayList verse { get; set; }

    public ArrayList chorus { get; set; }
    
    public ArrayList bridge { get; set; }

    public Song()
    {
        verse = new ArrayList();
        chorus = new ArrayList();
        bridge = new ArrayList();
    }
}