﻿namespace Lyt.WordRush.Model.History; 

public sealed class Statistics
{
    public TimeSpan Duration { get; set; }

    public int Wins { get; set; }

    public int Losses { get; set; }

    public int WinRate { get; set; }

    public int CurrentStreak { get; set; }

    public int BestStreak { get; set; }

    public List<int> Histogram { get; set; } = [];
}