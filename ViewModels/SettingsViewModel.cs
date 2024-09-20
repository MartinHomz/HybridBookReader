using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HybridBookReader.Helpers;
using HybridBookReader.Managers;

namespace HybridBookReader.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    //Odkaz na lokalizaci pro binding
    public LocalizationManager LocalizationManager => LocalizationManager.Instance;

    /// <summary>
    /// Nastavená jazyková lokalizace
    /// </summary>
    private string _selectedLanguage;
    /// <summary>
    /// Byla provedena změna nastaveni?
    /// </summary>
    [ObservableProperty] bool settingsChanged = false;
    /// <summary>
    /// Název vybraného jazyka
    /// </summary>
    [ObservableProperty] string selectedLanguageFormatName;
    /// <summary>
    /// Čas notifikace pro připomenutí čtení
    /// </summary>
    [ObservableProperty] DateTime notificationTime;
    /// <summary>
    /// Povolení zobrazení notifikace pro připomenutí čtení
    /// </summary>
    [ObservableProperty] bool allowNotification;

    public SettingsViewModel()
    {
        NotificationTime = App.SettingsManager.NotificationTime;
        _selectedLanguage = LocalizationManager.SavedLanguage;
        UpdateLanguageName();
    }

    /// <summary>
    /// Nastavení české jazykové lokalizace
    /// </summary>
    [RelayCommand]
    private void SetCzechLanguage()
    {
        _selectedLanguage = LocalizationManager.languageCzech;
        UpdateLanguageName();
        SettingsChanged = true;
    }

    /// <summary>
    /// Nastavení anglické jazykové lokalizace
    /// </summary>
    [RelayCommand]
    private void SetEnglishLanguage()
    {
        _selectedLanguage = LocalizationManager.languageEnglish;
        UpdateLanguageName();
        SettingsChanged = true;
    }

    /// <summary>
    /// Uložení nastavení formuláře do uživatelského nastavení
    /// </summary>
    [RelayCommand]
    private async Task SaveSettings()
    {
        if (SettingsChanged)
        {
            SettingsChanged = false;

            //Nastavení jazyka
            if (_selectedLanguage != LocalizationManager.SavedLanguage)
                LocalizationManager.SetCulture(_selectedLanguage);

            //Nastavení notifikací (pouze pro Android a iOS)
#if ANDROID || IOS
        UpdateNotification();
#endif

            //Notifikace o provedení změn
            await UIHelper.ShowApplySettingsToast();
        }
    }

    /// <summary>
    /// Nastavení původních hodnot do uživatelského nastavení
    /// </summary>
    [RelayCommand]
    private async Task DefualtSettings()
    {
        //Nastavení notifikací (pouze pro Android a iOS)
#if ANDROID || IOS
        NotificationTime = App.SettingsManager.GetDefaultNotificationTime();
        AllowNotification = false;
        UpdateNotification();
#endif

        //Nastavení jazyka
        LocalizationManager.SetDefaultCulture();
        _selectedLanguage = LocalizationManager.SavedLanguage;
        UpdateLanguageName();

        //Notifikace o provedení změn
        await UIHelper.ShowDefaultSettingsToast();
    }

    /// <summary>
    /// Export dat z databáze do JSON souboru
    /// </summary>
    [RelayCommand]
    private async Task DataExport()
    {
        await App.StateManager.SaveExportFile();
    }

    /// <summary>
    /// Import dat z JSON souboru do databáze
    /// </summary>
    [RelayCommand]
    private async Task DataImport()
    {
        await App.StateManager.LoadImportFile();
    }

    /// <summary>
    /// Nastavení názvu jazyka dle právě zvoleného jazyka
    /// </summary>
    private void UpdateLanguageName()
    {
        SelectedLanguageFormatName = LocalizationManager.GetLanguageNameByFormatName(_selectedLanguage);
    }

    /// <summary>
    /// Nastavení notifikace
    /// </summary>
    private void UpdateNotification()
    {
        App.SettingsManager.DeleteNotification();
        App.SettingsManager.SetNotification(NotificationTime, AllowNotification);
    }
}
