namespace Lyt.WordRush.Model;

using static Lyt.Avalonia.Persistence.FileManagerModel;
using Mod = Lyt.WordRush.Model;

public sealed partial class WordsModel : ModelBase
{
    private static readonly char[] separator = [' ', '\t', '\r', '\n'];

    private readonly FileManagerModel fileManager;
    private readonly HashSet<string> italian;
    private readonly Dictionary<string, string> italianToEnglish;
    private readonly IRandomizer randomizer;

    public WordsModel(
        FileManagerModel fileManager, IMessenger messenger, ILogger logger, IRandomizer randomizer) : base(messenger, logger)
    {
        this.fileManager = fileManager;
        this.ShouldAutoSave = true;
        this.italian = new(2048, StringComparer.InvariantCultureIgnoreCase);
        this.italianToEnglish = new(2048);
        this.randomizer = randomizer;
    }

    public bool IsReady { get; private set; }

    [JsonIgnore]
    // Not serialized - No model changed event
    // This is EXPLICITLY saved and loaded 
    public GameHistory GameHistory { get; private set; }

    public override Task Initialize()
    {
        _ = Task.Factory.StartNew(this.LoadWordsDatabase, TaskCreationOptions.LongRunning);
        return Task.CompletedTask;
    }

    public override Task Save()
    {

        if (this.GameHistory.IsDirty)
        {
            this.GameHistory.Save();
        }

        return Task.CompletedTask;
    }

    public override Task Shutdown()
    {
        this.Save();
        return Task.CompletedTask;
    }

    public void Add(GameResult gameResult) => this.GameHistory.Add(gameResult);

    public Statistics Statistics () => this.GameHistory.EvaluateStatistics();

    public List<string> RandomPicks(int needed, HashSet<string> exclude)
    {
        string[] source = this.italian.ToArray();
        List<string> picks = new(needed);
        HashSet<string> alreadyFound = new(needed);
        while (needed > 0)
        {
            string word = this.RandomPick(source, exclude, alreadyFound);
            if (string.IsNullOrWhiteSpace(word))
            {
                break;
            }

            --needed;
            alreadyFound.Add(word);
            picks.Add(word);
        }

        while (needed > 0)
        {
            string word = this.RandomPick(source, [], alreadyFound);
            if (string.IsNullOrWhiteSpace(word))
            {
                break;
            }

            --needed;
            alreadyFound.Add(word);
            picks.Add(word);
        }

        return picks;
    }

    public string TranslateToEnglish(string italianWord)
    {
        if (this.italianToEnglish.TryGetValue(italianWord, out string? translated))
        {
            if (!string.IsNullOrWhiteSpace(translated))
            {
                return translated;
            }
        }

        throw new Exception("Failed to translate");
    }

    private string RandomPick(string[] source, HashSet<string> exclude, HashSet<string> alreadyFound)
    {
        bool found = false;
        int retries = 10 * (5 + exclude.Count);
        while (!found)
        {
            int choice = this.randomizer.Next(source.Length);
            string word = source[choice];
            if (!exclude.Contains(word) && !alreadyFound.Contains(word))
            {
                return word;
            }

            --retries;
            if (retries <= 0)
            {
                break;
            }
        }

        return string.Empty;
    }

    private async void LoadWordsDatabase()
    {
        try
        {
            // Wait a bit so that the UI has time to load 
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
            await Task.Delay(200);

            // Load game history 
            Debugger.Break();
            var gameHistory =
                this.fileManager.Load<GameHistory>(Area.User, Kind.Json, GameHistory.GameHistoryFilename);
            if ( gameHistory is not null)
            {
                this.GameHistory = gameHistory;
            }
            else
            {
                this.GameHistory = new GameHistory(this.fileManager, this.Messenger, this.Logger);
            }

            this.LoadCommonWords();
            this.LoadDictionaries();
            this.CleanDictionary();
            this.IsReady = true;
        }
        catch (Exception ex)
        {
            this.Logger.Warning(ex.Message);
            Debug.WriteLine(ex);
        }
        finally
        {
            Thread.CurrentThread.Priority = ThreadPriority.Normal;
        }
    }

    private bool LoadDictionaries()
    {
        try
        {
            WordTranslator idp = IdpParser.Load(this.fileManager, Mod.Language.Italian, "ITALIAN");
            WordTranslator udd = UddlParser.Load(this.fileManager, Mod.Language.Italian, "english-italian");
            WordTranslator dcc = DictCcParser.Load(this.fileManager, Mod.Language.Italian, "dictcc-en-it");

            bool DictionaryLookup(string italianWord, WordTranslator translator, out string translated)
            {
                translated = string.Empty;
                foreach (string englishWord in translator.Keys)
                {
                    foreach (Word word in translator[englishWord])
                    {
                        if (word is not null && !string.IsNullOrWhiteSpace(word.Text))
                        {
                            string translation = word.Text;
                            if (translation.Trim().ToLower() == italianWord.Trim().ToLower())
                            {
                                translated = englishWord;
                                if (IsPossibleItalianVerb(italianWord) &&
                                    (word.Grammar == Grammar.Verb) &&
                                    (!translated.StartsWith("to", StringComparison.InvariantCultureIgnoreCase)) &&
                                    (!translated.EndsWith("ing", StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    // this.Logger.Warning("Verb: " + englishWord );
                                    translated = string.Concat("to ", translated);
                                }

                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            foreach (string italianWord in this.italian)
            {
                bool found = false;
                string translated = string.Empty;
                if (DictionaryLookup(italianWord, idp, out translated))
                {
                    found = true;
                }
                else if (DictionaryLookup(italianWord, udd, out translated))
                {
                    found = true;
                }
                else if (DictionaryLookup(italianWord, dcc, out translated))
                {
                    found = true;
                }

                if (found)
                {
                    this.italianToEnglish.Add(italianWord.Trim().ToLower(), translated.Trim().ToLower());
                    // Debug.WriteLine(italianWord + " : " + translated);
                }
                else
                {
                    // Debug.WriteLine("*** Not found: " + italianWord);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Debugger.Break();
            this.Logger.Warning(ex.ToString());
            return false;
        }
    }

    private bool LoadCommonWords()
    {
        try
        {
            string uriString = string.Format("avares://WordRush/Assets/Words/{0}.txt", "comuni");
            var streamReader = new StreamReader(AssetLoader.Open(new Uri(uriString)));
            string content = this.fileManager.LoadResourceFromStream<string>(FileManagerModel.Kind.Text, streamReader);
            string[] commonTokens = content.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            foreach (string token in commonTokens)
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    continue;
                }

                if (WordsModel.HasNonItalianOrSpecialCharacters(token))
                {
                    continue;
                }

                string word = token.Trim().ToLower();
                if (word.Length > 1)
                {
                    _ = this.italian.Add(word);
                }
            }

            Debug.WriteLine("Common Word count: " + this.italian.Count);
            return true;
        }
        catch (Exception ex)
        {
            Debugger.Break();
            this.Logger.Warning(ex.ToString());
            return false;
        }
    }

    private void CleanDictionary()
    {
        List<string> toRemove = [];
        foreach (string word in this.italian)
        {
            if (!this.italianToEnglish.ContainsKey(word))
            {
                toRemove.Add(word);
            }
        }

        foreach (string removed in toRemove)
        {
            this.italian.Remove(removed);
        }

        Debug.WriteLine("To Remove: " + toRemove.Count);
        Debug.WriteLine("Left: " + this.italian.Count);
    }

    private static bool IsPossibleItalianVerb(string italianWord)
    {
        string word = italianWord.Trim(); 
        if (word.EndsWith("are", StringComparison.InvariantCultureIgnoreCase) ||
            word.EndsWith("ire", StringComparison.InvariantCultureIgnoreCase) ||
            word.EndsWith("ere", StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        return false;
    }

    private static bool HasNonItalianOrSpecialCharacters(string word)
    {
        foreach (char c in new char[] { 'j', 'k', 'w', 'x', 'y', '.', '\'', ',', ' ' })
        {
            if (word.Contains(c, StringComparison.InvariantCultureIgnoreCase))
            {
                Debug.WriteLine("Excluded: " + word);
                return true;
            }
        }

        return false;
    }
}
