namespace Lyt.TranslateRace.Model;

using static Lyt.Persistence.FileManagerModel;

public sealed partial class TranslateRaceModel : ModelBase
{
    // public const int WinScore = 9;
    public const int WinScore = 42;

    private static readonly char[] separator = ['\t', '\r', '\n'];

    private readonly FileManagerModel fileManager;
    private readonly HashSet<string> italian;
    private readonly Dictionary<string, string> italianToEnglish;
    private readonly IRandomizer randomizer;

    public TranslateRaceModel(
        FileManagerModel fileManager, 
        IMessenger messenger, ILogger logger, IRandomizer randomizer) : base(messenger, logger)
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
    public List<Participant> Participants { get; private set; } = [];

    [JsonIgnore]
    // Not serialized - No model changed event
    // This is EXPLICITLY saved and loaded 
    public List<Phrase> Phrases { get; private set; } = [];

    [JsonIgnore]
    // Not serialized - No model changed event, transient 
    public Team? WinningTeam { get; set; }

    [JsonIgnore]
    // Not serialized - No model changed event, transient 
    public Team? LosingTeam { get; set; }

    public override Task Initialize()
    {
        _ = Task.Factory.StartNew(this.LoadGameModel, TaskCreationOptions.LongRunning);
        return Task.CompletedTask;
    }

    public override Task Save()
    {
        this.SaveParticipants();
        this.SavePhrases();
        return Task.CompletedTask;
    }

    public override Task Shutdown()
    {
        this.Save();
        return Task.CompletedTask;
    }

    public Phrase PickPhrase(PhraseDifficulty phraseDifficulty)
    {
        // TODO: Randomize 
        var first = (from phrase in this.Phrases 
                 where phrase.Difficulty == phraseDifficulty 
                 select phrase)
                 .FirstOrDefault();
        if ( first is not null)
        {
            return first; 
        }

        return this.Phrases[0]! ; 
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
        //bool found = false;
        //int retries = 10 * (5 + exclude.Count);
        //while (!found)
        //{
        //    int choice = this.randomizer.Next(source.Length);
        //    string word = source[choice];
        //    if (!exclude.Contains(word) && !alreadyFound.Contains(word))
        //    {
        //        return word;
        //    }

        //    --retries;
        //    if (retries <= 0)
        //    {
        //        break;
        //    }
        //}

        return string.Empty;
    }

    private async void LoadGameModel()
    {
        try
        {
            // Wait a bit so that the UI has time to load 
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
            await Task.Delay(200);

            // Load participants, if any 
            this.LoadParticipants();

            // Load default participants, if nothing loaded  
            if (this.Participants.Count == 0)
            {
                this.LoadDefaultParticipants();
                this.SaveParticipants();
            }

            // Load phrases, if any 
            this.LoadPhrases (); 
            this.Participants = new List<Participant>(64);
            if (this.fileManager.Exists(Area.User, Kind.Json, Participant.ParticipantsFilename))
            {
                var participants = this.fileManager.Load<List<Participant>>(Area.User, Kind.Json, Participant.ParticipantsFilename);
                if (participants is not null)
                {
                    this.Participants = participants;
                }
            }

            // Load default phrases, if nothing loaded  
            if (this.Phrases.Count == 0)
            {
                this.LoadDefaultPhrases();
                this.SavePhrases();
            } 

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

    #region Phrases

    private void LoadPhrases()
    {
        this.Phrases = new List<Phrase>(4096);
        if (this.fileManager.Exists(Area.User, Kind.Json, Phrase.PhrasesFilename))
        {
            var phrases = this.fileManager.Load<List<Phrase>>(Area.User, Kind.Json, Phrase.PhrasesFilename);
            if (phrases is not null)
            {
                this.Phrases = phrases;
            }
        }
    }

    private bool LoadDefaultPhrases()
    {
        try
        {
            this.Phrases = Phrase.DefaultPhrases;
            Debug.WriteLine("Phrases count: " + this.Phrases.Count);
            return true;
        }
        catch (Exception ex)
        {
            Debugger.Break();
            this.Logger.Warning(ex.ToString());
            return false;
        }
    }

    private void SavePhrases()
    {
        try
        {
            // Null check is needed !
            // If the File Manager is null we are currently loading the model and activating properties on a second instance 
            // causing dirtyness, and in such case we must avoid the null crash and anyway there is no need to save anything.
            this.fileManager?.Save(Area.User, Kind.Json, Phrase.PhrasesFilename, this.Phrases);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            throw;
        }
    }

    #endregion Phrases

    #region Participants 

    private void LoadParticipants()
    {
        this.Participants = new List<Participant>(64);
        if (this.fileManager.Exists(Area.User, Kind.Json, Participant.ParticipantsFilename))
        {
            var participants = this.fileManager.Load<List<Participant>>(Area.User, Kind.Json, Participant.ParticipantsFilename);
            if (participants is not null)
            {
                this.Participants = participants;
            }
        }
    } 

    private bool LoadDefaultParticipants()
    {
        try
        {
            HashSet<Participant> participants = new(64);
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
                    _ = participants.Add(new Participant { Name = name.Capitalize() });
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
            this.fileManager?.Save(Area.User, Kind.Json, Participant.ParticipantsFilename, this.Participants);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            throw;
        }
    }

    public bool DeleteParticipant(Participant participant)
    {
        Participant? toDelete =
            (from p in this.Participants
             where p.Name.Trim().Equals(participant.Name, StringComparison.CurrentCultureIgnoreCase)
             select p).FirstOrDefault();
        if (toDelete is not null)
        {
            this.Participants.Remove(toDelete);
            return true;
        }

        return false;
    }

    public bool ValidateNewParticipantForAdd(string name, out string message)
    {
        message = string.Empty;
        name = name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            message = "Il nome del nuovo partecipante non può essere vuoto.";
            return false;
        }

        if (name.Length <= 2)
        {
            message = "Il nome del nuovo partecipante è troppo corto.";
            return false;
        }

        if (name.Length >= 32)
        {
            message = "Il nome del nuovo partecipante è troppo longo.";
            return false;
        }

        Participant? same =
            (from p in this.Participants
             where p.Name.Trim().Equals(name, StringComparison.CurrentCultureIgnoreCase)
             select p).FirstOrDefault();
        if (same is not null)
        {
            message = "Il nome del nuovo partecipante è già stato preso.";
            return false;
        }

        return true;
    }

    public bool AddParticipant(string name, out string message)
    {
        if (!this.ValidateNewParticipantForAdd(name, out message))
        {
            return false;
        }

        message = string.Empty;
        name = name.Trim();
        Participant participant = new() { Name = name.Capitalize() };
        this.Participants.Add(participant);
        return true;
    }

    #endregion Participants 
}
