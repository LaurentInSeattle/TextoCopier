namespace Lyt.TranslateRace.Model.History;

using static Lyt.Persistence.FileManagerModel;

public sealed class GameHistory : ModelBase
{
    public static readonly string GameHistoryFilename = "translaterace_history";

    private readonly FileManagerModel fileManager;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
    // Only for deserialization 
    public GameHistory() : base(null, null)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8618
    {
        /* ==>  Only for deserialization */
    }

    public GameHistory(FileManagerModel fileManager, IMessenger messenger, ILogger logger) : base(messenger, logger)
    {
        this.fileManager = fileManager;
        this.ShouldAutoSave = false;
        this.GameResults = [];
    }

    // Serialized -  No model changed event
    [JsonRequired]
    public List<GameResult> GameResults { get; set; }

    public override Task Initialize() => Task.CompletedTask;

    public void Load()
    {
        try
        {
            if (this.fileManager is not null)
            {
                Debugger.Break();
                var gameResults =
                    this.fileManager.Load<List<GameResult>>(Area.User, Kind.Json, GameHistory.GameHistoryFilename);
                this.GameResults = gameResults;
            }

            //string root = WpfExtensions.ApplicationDataFolder("LYT", "Parole"); 
            //string fileName = Word.Length == 5 ? FileName5 : FileName6;
            //string path = Path.Combine(root, fileName);
            //if (File.Exists(path))
            //{
            //    var serializer = new XmlSerializer(typeof(History));
            //    if (serializer != null)
            //    {
            //        using var reader = new FileStream(path, FileMode.Open);
            //        if (reader != null)
            //        {
            //            if (serializer.Deserialize(reader) is History history && 
            //                !history.GameEntries.IsNullOrEmpty())
            //            {
            //                this.GameEntries.AddRange(history.GameEntries);
            //            }
            //        }
            //    }
            //}
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    public override Task Save()
    {
        try
        {
            // Null check is needed !
            // If the File Manager is null we are currently loading the model and activating properties on a second instance 
            // causing dirtyness, and in such case we must avoid the null crash and anyway there is no need to save anything.
            if (this.fileManager is not null)
            {
                this.fileManager.Save(Area.User, Kind.Json, GameHistory.GameHistoryFilename, this);
                base.Save();
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            throw;
        }
    }

    public void Add(GameResult gameResult)
    {
        this.GameResults.Add(gameResult);
        this.IsDirty = true;
    }

    public HashSet<string> PlayedWords()
    {
        var hashSet = new HashSet<string>(this.GameResults.Count);
        //foreach (var gameEntry in this.GameResults)
        //{
        //    //foreach (string word in gameEntry.Words)
        //    //{
        //    //    _ = hashSet.Add(word);
        //    //}
        //}

        return hashSet;
    }

    public Statistics EvaluateStatistics()
    {
        int wins = (from entry in this.GameResults where entry.IsWon select entry).Count();
        int count = this.GameResults.Count;
        int winRate = count == 0 ? 0 : (int)((0.5f + (100 * wins)) / count);
        long durationLong = (from entry in this.GameResults select entry.GameDuration.Ticks).Sum();
        TimeSpan durationTs = new(durationLong);
        var streaks = this.CalculateStreaks();
        var statistics = new Statistics
        {
            Wins = wins,
            Losses = this.GameResults.Count - wins,
            WinRate = count == 0 ? 0 : (int)((0.5f + (100 * wins)) / count),
            Duration = durationTs,
            BestStreak = streaks.Item1,
            CurrentStreak = streaks.Item2,
        };
        return statistics;
    }

    private (int, int) CalculateStreaks()
    {
        int longestStreak = 0;
        int currentStreak = 0;
        foreach (var entry in this.GameResults)
        {
            if (entry.IsWon)
            {
                currentStreak++;
            }
            else
            {
                if (currentStreak > longestStreak)
                {
                    longestStreak = currentStreak;
                }

                currentStreak = 0;
            }
        }

        if (currentStreak > longestStreak)
        {
            longestStreak = currentStreak;
        }

        return (longestStreak, currentStreak);
    }
}
