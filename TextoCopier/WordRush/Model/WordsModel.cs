namespace Lyt.WordRush.Model;

using Mod = Lyt.WordRush.Model;

public sealed partial class WordsModel : ModelBase
{
    private static readonly char[] separator = [' ', '\t', '\r', '\n'];

    private readonly FileManagerModel fileManager;
    private readonly HashSet<string> italian;
    private readonly Dictionary<string, string> italianToEnglish;

    public WordsModel(FileManagerModel fileManager, IMessenger messenger, ILogger logger) : base(messenger, logger)
    {
        this.fileManager = fileManager;
        this.ShouldAutoSave = true;
        this.italian = new(2048, StringComparer.InvariantCultureIgnoreCase);
        this.italianToEnglish = new(2048);
    }

    public override Task Initialize()
    {
        /* TODO */
        this.LoadCommonWords();
        this.LoadDictionaries();
        return Task.CompletedTask;
    }

    private bool LoadDictionaries()
    {
        try
        {
            WordTranslator idp = IdpParser.Load(this.fileManager, Mod.Language.Italian, "ITALIAN");
            WordTranslator udd = UddlParser.Load(this.fileManager, Mod.Language.Italian, "english-italian");
            WordTranslator dcc = DictCcParser.Load(this.fileManager, Mod.Language.Italian, "dictcc-en-it");
            
            void DictionaryParse(WordTranslator translator)
            {
                foreach (string key in translator.Keys)
                {
                    foreach (Word word in translator[key])
                    {
                        if (word is not null && !string.IsNullOrWhiteSpace(word.Text))
                        {
                            string translation = word.Text;
                            if (this.italian.Contains(translation) && !this.italianToEnglish.ContainsKey(translation))
                            {
                                this.italianToEnglish.Add(translation, key);
                            }
                        }
                    }
                }
            }

            DictionaryParse(idp);
            DictionaryParse(udd);
            DictionaryParse(dcc);
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

                _ = this.italian.Add(token);
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
