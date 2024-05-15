namespace Lyt.TextoCopier.Models;

public sealed class LocalizerModel : ModelBase
{
    // TO DO : Autopopulate this 
    public HashSet<string> Languages = ["en-US" , "fr-FR", "it-IT"];

    // TODO: Make that parametrizable 
    public const string LanguagesFolder = "Languages";
    public const string AssetsFolder = "//TextoCopier/Assets";

    public override Task Initialize() => Task.CompletedTask; 

    private string? currentLanguage;  
    private ResourceInclude? currentLanguageResource; 

    public bool SelectLanguage(string targetLanguage)
    {
        var app = App.Current;
        if (app is null)
        {
            this.Logger.Warning("No application object");
            return false;
        }

        if (!this.Languages.Contains(targetLanguage))
        {
            this.Logger.Error(targetLanguage + "is not a supported language." );
            return false;
        }

        var mergedDictionaries = app.Resources.MergedDictionaries.ToList();
        if (mergedDictionaries is null)
        {
            this.Logger.Warning("Failed get the MergedDictionaries");
            return false;
        }

        var translations =
            mergedDictionaries.OfType<ResourceInclude>()
            .FirstOrDefault(x => x.Source?.OriginalString?.Contains(LanguagesFolder) ?? false);
        if (translations is not null)
        {
            this.Logger.Info("Removed current language");
            mergedDictionaries.Remove(translations);
        }
        else
        {
            this.Logger.Warning("Failed get any Resource Includes");
        }

        try
        {
            string uriString = string.Format("avares:{0}/{1}/{2}.axaml", AssetsFolder, LanguagesFolder, targetLanguage);
            var uri = new Uri(uriString);
            var newLanguage = new ResourceInclude(uri) { Source = uri };
            app.Resources.MergedDictionaries.Add(newLanguage);
            this.currentLanguageResource = newLanguage;
            this.currentLanguage = targetLanguage; 
            this.Logger.Info("Added new language: " + targetLanguage);
            return true;
        }
        catch (Exception ex)
        {
            this.Logger.Error("Exception thrown trying to switch language\n" + ex.ToString());
            return false;
        }
    }

    public string Lookup(string localizationKey)
    {
        var app = App.Current;
        if (app is null)
        {
            this.Logger.Warning("No application object");
            return string.Empty;
        }

        if (string.IsNullOrWhiteSpace(this.currentLanguage) || this.currentLanguageResource is null)
        {
            this.Logger.Warning("No language loaded");
            return string.Empty;
        }

        if ( this.currentLanguageResource.TryGetResource(localizationKey, app.ActualThemeVariant, out object? resource))
        {
            if ( resource is string localized)
            {
                return localized;
            }
        }

        this.Logger.Warning("Failed to translate: " + localizationKey + " for language: " + this.currentLanguage);
        return string.Empty;
    }
}
