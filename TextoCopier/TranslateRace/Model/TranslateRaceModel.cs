﻿namespace Lyt.TranslateRace.Model;

using static Lyt.Avalonia.Persistence.FileManagerModel;
using Mod = Lyt.TranslateRace.Model;

public sealed partial class TranslateRaceModel : ModelBase
{
    private static readonly char[] separator = ['\t', '\r', '\n'];

    private readonly FileManagerModel fileManager;
    private readonly HashSet<string> italian;
    private readonly Dictionary<string, string> italianToEnglish;
    private readonly IRandomizer randomizer;

    public TranslateRaceModel(
        FileManagerModel fileManager, IMessenger messenger, ILogger logger, IRandomizer randomizer) : base(messenger, logger)
    {
        this.fileManager = fileManager;
        this.ShouldAutoSave = true;
        this.italian = new(2048, StringComparer.InvariantCultureIgnoreCase);
        this.italianToEnglish = new(2048);
        this.randomizer = randomizer;
        this.GameHistory = new GameHistory();
    }

    public bool IsReady { get; private set; }

    [JsonIgnore]
    // Not serialized - No model changed event
    // This is EXPLICITLY saved and loaded 
    public GameHistory GameHistory { get; private set; }

    public List<Participant> Participants { get; private set; } = [];

    public override Task Initialize()
    {
        _ = Task.Factory.StartNew(this.LoadGameModel, TaskCreationOptions.LongRunning);
        return Task.CompletedTask;
    }

    public override Task Save()
    {
        if (this.GameHistory.IsDirty)
        {
            this.GameHistory.Save();
        }

        this.SaveParticipants();
        return Task.CompletedTask;
    }

    public override Task Shutdown()
    {
        this.Save();
        return Task.CompletedTask;
    }

    public void Add(GameResult gameResult) => this.GameHistory.Add(gameResult);

    public Statistics Statistics () => this.GameHistory.EvaluateStatistics();

    public List<string> RandomPicks(int needed)
    {
        HashSet<string> exclude = this.GameHistory.PlayedWords();
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

    private async void LoadGameModel()
    {
        try
        {
            // Wait a bit so that the UI has time to load 
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
            await Task.Delay(200);

            // Load game history, if any 
            this.GameHistory = new GameHistory(this.fileManager, this.Messenger, this.Logger);
            if (this.fileManager.Exists(Area.User, Kind.Json, GameHistory.GameHistoryFilename))
            {
                var gameHistory = this.fileManager.Load<GameHistory>(Area.User, Kind.Json, GameHistory.GameHistoryFilename);
                if (gameHistory is not null)
                {
                    this.GameHistory.GameResults = gameHistory.GameResults;
                }
            }

            // Load participants, if any 
            this.Participants = new List<Participant>(64);
            if (this.fileManager.Exists(Area.User, Kind.Json, Participant.ParticipantsFilename))
            {
                var participants = this.fileManager.Load<List<Participant>>(Area.User, Kind.Json, Participant.ParticipantsFilename);
                if (participants is not null)
                {
                    this.Participants = participants;
                }
            }

            // Load default participants, if nothing loaded  
            if (this.Participants.Count == 0 ) 
            {
                this.LoadDefaultParticipants();
                this.SaveParticipants();
            }

            // Load phrases, if any 
            // Load default phrases, if nothing loaded  

            //this.LoadPhrases();
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

    //private bool LoadLoadPhrases()
    //{
    //    try
    //    {
    //        WordTranslator idp = IdpParser.Load(this.fileManager, Mod.Language.Italian, "ITALIAN");
    //        WordTranslator udd = UddlParser.Load(this.fileManager, Mod.Language.Italian, "english-italian");
    //        WordTranslator dcc = DictCcParser.Load(this.fileManager, Mod.Language.Italian, "dictcc-en-it");

    //        bool DictionaryLookup(string italianWord, WordTranslator translator, out string translated)
    //        {
    //            translated = string.Empty;
    //            foreach (string englishWord in translator.Keys)
    //            {
    //                foreach (Word word in translator[englishWord])
    //                {
    //                    if (word is not null && !string.IsNullOrWhiteSpace(word.Text))
    //                    {
    //                        string translation = word.Text;
    //                        if (translation.Trim().ToLower() == italianWord.Trim().ToLower())
    //                        {
    //                            translated = englishWord;
    //                            if (IsPossibleItalianVerb(italianWord) &&
    //                                (word.Grammar == Grammar.Verb) &&
    //                                (!translated.StartsWith("to", StringComparison.InvariantCultureIgnoreCase)) &&
    //                                (!translated.EndsWith("ing", StringComparison.InvariantCultureIgnoreCase)))
    //                            {
    //                                // this.Logger.Warning("Verb: " + englishWord );
    //                                translated = string.Concat("to ", translated);
    //                            }

    //                            return true;
    //                        }
    //                    }
    //                }
    //            }

    //            return false;
    //        }

    //        foreach (string italianWord in this.italian)
    //        {
    //            bool found = false;
    //            string translated = string.Empty;
    //            if (DictionaryLookup(italianWord, idp, out translated))
    //            {
    //                found = true;
    //            }
    //            else if (DictionaryLookup(italianWord, udd, out translated))
    //            {
    //                found = true;
    //            }
    //            else if (DictionaryLookup(italianWord, dcc, out translated))
    //            {
    //                found = true;
    //            }

    //            if (found)
    //            {
    //                this.italianToEnglish.Add(italianWord.Trim().ToLower(), translated.Trim().ToLower());
    //                // Debug.WriteLine(italianWord + " : " + translated);
    //            }
    //            else
    //            {
    //                // Debug.WriteLine("*** Not found: " + italianWord);
    //            }
    //        }

    //        return true;
    //    }
    //    catch (Exception ex)
    //    {
    //        Debugger.Break();
    //        this.Logger.Warning(ex.ToString());
    //        return false;
    //    }
    //}

    private bool LoadDefaultParticipants()
    {
        try
        {
            HashSet<Participant> participants = new (64);
            string uriString = string.Format("avares://TranslateRace/Assets/Model/{0}.txt", "people");
            var streamReader = new StreamReader(AssetLoader.Open(new Uri(uriString)));
            string content = this.fileManager.LoadResourceFromStream<string>(FileManagerModel.Kind.Text, streamReader);
            string[] nameTokens = content.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            foreach (string token in nameTokens)
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    continue;
                }

                string name = token.Trim();
                if (name.Length > 1)
                {
                    _ = participants.Add(new Participant { Name = name.Capitalize() } );
                }
            }

            this.Participants = [.. participants];
            Debug.WriteLine("Participants count: " + participants.Count);
            return true;
        }
        catch (Exception ex)
        {
            Debugger.Break();
            this.Logger.Warning(ex.ToString());
            return false;
        }
    }

    private void SaveParticipants()
    {
        try
        {
            // Null check is needed !
            // If the File Manager is null we are currently loading the model and activating properties on a second instance 
            // causing dirtyness, and in such case we must avoid the null crash and anyway there is no need to save anything.
            if (this.fileManager is not null)
            {
                this.fileManager.Save(Area.User, Kind.Json, Participant.ParticipantsFilename, this.Participants);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            throw;
        }
    }

}