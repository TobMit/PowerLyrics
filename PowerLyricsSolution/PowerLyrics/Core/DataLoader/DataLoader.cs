using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace PowerLyrics.Core.DataLoader;

public class DataLoader
{
    private static Regex _regex;
    private static string[] paths;
    public DataLoader()
    {
        _regex = new Regex(@"\s+");
        paths = Directory.GetFiles("Songs");
        
        foreach (string path in paths)
        {
            
            /*string[] totoJeTest = nacitanaPiesen.Split(" ");
            foreach (string hodnota in totoJeTest)
            {
            Debug.WriteLine(hodnota);
            }*/
            
            Debug.WriteLine(loadSong(path));
        }
    }

    private string loadSong(string path)
    {
        string loadedSong = File.ReadAllText(path);
        return _regex.Replace(loadedSong, " ");
    }
}