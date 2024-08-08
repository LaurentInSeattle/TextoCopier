
namespace Lyt.WordRush.Model.History;

public sealed class History : ModelBase
{
    private static readonly string FileName5 = "wordrush_history.xml";

    public History(FileManagerModel fileManager, IMessenger messenger, ILogger logger) : base(messenger, logger)
        => this.GameEntries = [];

    public List<GameEntry> GameEntries { get; set; }

    public override Task Initialize()
    {
        return Task.CompletedTask;
    }

    public void Load()
    {
        try
        {
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

    public void Save()
    {
        try
        {
            //string root = WpfExtensions.ApplicationDataFolder("LYT", "Parole");
            //string fileName = Word.Length == 5 ? FileName5 : FileName6;
            //string path = Path.Combine(root, fileName);
            //var serializer = new XmlSerializer(this.GetType());
            //using var writer = new FileStream(path, FileMode.Create);
            //if ((writer != null) && (serializer != null))
            //{
            //    serializer.Serialize(writer, this);
            //}
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    public void Add(GameEntry gameEntry) => this.GameEntries.Add(gameEntry);

    public HashSet<string> PlayedWords()
    {
        var hashSet = new HashSet<string>(this.GameEntries.Count);
        foreach (var gameEntry in this.GameEntries)
        {
            _ = hashSet.Add(gameEntry.Word);
        }

        return hashSet;
    }

    public Statistics EvaluateStatistics()
    {
        var statistics = new Statistics();
        int wins = (from entry in this.GameEntries where entry.IsWon select entry).Count();
        int count = this.GameEntries.Count;
        statistics.Wins = wins;
        statistics.Losses = this.GameEntries.Count - wins;
        statistics.WinRate = count == 0 ? 0 : (int)((0.5f + (100 * wins)) / count);
        long durationLong = (from entry in this.GameEntries select entry.Duration.Ticks).Sum();
        TimeSpan durationTs = new(durationLong);
        statistics.Duration = durationTs;
        var streaks = this.CalculateStreaks();
        statistics.BestStreak = streaks.Item1;
        statistics.CurrentStreak = streaks.Item2;

        //var list = new List<int> ();
        //for (int i = 0; i < Table.Rows; i++)
        //{
        //    int hist = 
        //        (from entry in this.GameEntries where entry.IsWon && entry.Steps == i select entry).Count();
        //    list.Add (hist);   
        //} 

        //statistics.Histogram = list;

        return statistics;
    }

    private (int,int) CalculateStreaks()
    {
        int longestStreak = 0;
        int currentStreak = 0;
        foreach (var entry in this.GameEntries)
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

        return (longestStreak,currentStreak);
    }
}
