using HybridBookReader.Resources.Languages;
using System.ComponentModel;
using System.Globalization;

namespace HybridBookReader.Managers;

/// <summary>
/// Správce jazykové lokalizace
/// </summary>
public class LocalizationManager : INotifyPropertyChanged
{

    public const string languagePref = "language";
    public const string languageCzech = "cs-CZ";
    public const string languageEnglish = "en-US";


    public static LocalizationManager Instance { get; } = new LocalizationManager();

    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Aktuálně nastavený jazyk ve formátu (xx-XX)
    /// </summary>
    public string SavedLanguage { get; private set; }

    /// <summary>
    /// Inicializace jazykové lokalizace
    /// </summary>
    private LocalizationManager()
    {
        //Získání nastavení jazyka z lokálního uložiště
        string language = Preferences.Get(languagePref, null);

        if (!string.IsNullOrEmpty(language)) //Nastavení jazyka z uložiště
            SetCultureInternal(language);
        else //Nastavení jazyka zařízení
            Localization.Culture = CultureInfo.CurrentCulture;
    }

    /// <summary>
    /// Vrací text dle nastavené jazykové lokalizace
    /// </summary>
    /// <param name="resourceKey">Název klíče textu</param>
    /// <returns>Požadovaný textový řetězec</returns>
    public object this[string resourceKey]
        => Localization.ResourceManager.GetObject(resourceKey, Localization.Culture) ?? Array.Empty<byte>();

    /// <summary>
    /// Dle názvu formátu jazykové lokalizace (xx-XX) vrací název jazyka
    /// </summary>
    /// <param name="formatName">Jazyk ve formátu (xx-XX)</param>
    /// <returns>Název jazyka</returns>
    public string GetLanguageNameByFormatName(string formatName)
    {
        return formatName == languageCzech ? this["Czech"].ToString() : this["English"].ToString();
    }

    /// <summary>
    /// Nastavení jazykové lokalizace
    /// </summary>
    /// <param name="formatName">Jazyk ve formátu (xx-XX)</param>
    public void SetCulture(string formatName)
    {
        //Nastavení jazykové lokalizace
        SetCultureInternal(formatName);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        
        //Uložení jazykové lokalizace do lokálního uložiště
        Preferences.Set(languagePref, formatName);
    }

    /// <summary>
    /// Nastavení původní jazykovou lokalizaci (jazyk zařízení)
    /// </summary>
    public void SetDefaultCulture()
    {
        SetCulture(CultureInfo.CurrentCulture.Name);
    }

    /// <summary>
    /// Nastavení jazykové lokalizace
    /// </summary>
    /// <param name="formatName">Jazyk ve formátu (xx-XX)</param>
    private void SetCultureInternal(string formatName)
    {
        CultureInfo culture = new CultureInfo(formatName);

        //Nastavení jazykové lokalizace do proměnné
        SavedLanguage = formatName;

        //Nastavení jazykové lokalizace prostředí
        Localization.Culture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}
