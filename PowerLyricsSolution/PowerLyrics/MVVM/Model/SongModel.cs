﻿using System.Collections;
using System.Collections.Generic;
using PowerLyrics.Core;

namespace PowerLyrics.MVVM.Model;

/**
 * Model načítanej piesne
 */
public class SongModel : ObservableObjects
{
    private int _id;

    private bool _isSelected;

    public SongModel()
    {
        verse = new ArrayList();
        chorus = new ArrayList();
        bridge = new ArrayList();
        lyricTypeQueue = new List<LyricType>();
        LyricModels = new List<LyricModel>();
        isSelected = false;
        number = 0;
        name = "New song";
    }

    public SongModel(SongModel copy)
    {
        if (copy != null)
        {
            verse = copy.verse != null ? new ArrayList(copy.verse) : new ArrayList();
            chorus = copy.chorus != null ? new ArrayList(copy.chorus) : new ArrayList();
            bridge = copy.bridge != null ? new ArrayList(copy.bridge) : new ArrayList();
            lyricTypeQueue = copy.lyricTypeQueue != null
                ? new List<LyricType>(copy.lyricTypeQueue)
                : new List<LyricType>();
            if (copy.LyricModels != null)
            {
                LyricModels = new List<LyricModel>();
                foreach (var lyricModel in copy.LyricModels) LyricModels.Add(new LyricModel(lyricModel));
            }

            id = copy.id;
            number = copy.number;
            name = copy.name;
            isSelected = false;
        }
    }

    public int number { get; set; }

    public int id
    {
        get => _id;
        set
        {
            _id = value;
            OnPropertyChanged();
        }
    }

    public string name { get; set; }
    public ArrayList verse { get; set; }

    public ArrayList chorus { get; set; }

    public ArrayList bridge { get; set; }

    public List<LyricType> lyricTypeQueue { get; set; }

    public bool isSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public List<LyricModel> LyricModels { get; set; }
}