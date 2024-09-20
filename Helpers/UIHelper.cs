using CommunityToolkit.Maui.Alerts;
using HybridBookReader.Managers;

namespace HybridBookReader.Helpers;

/// <summary>
/// Pomocná statická třída pro práci s uživatelským rozhrání
/// </summary>
public static class UIHelper
{
    #region Picker

    /// <summary>
    /// Zobrazení dialogu pro výběr souboru
    /// </summary>
    /// <param name="title">Popisek dialogu</param>
    /// <returns>Vybraný soubor</returns>
    public static async Task<FileResult> ShowFilePicker(string title)
    {
        return await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = title,
            //FileTypes = FileHelper.GetEpubFileType()

        });
    }

    #endregion Picker
    #region Dialog

    /// <summary>
    /// Dialog pro potvrzení odstranění knihy
    /// </summary>
    /// <returns>True = Odstranit, False = Neodstranit</returns>
    public static async Task<bool> ShowDeleteBookDialog()
    {
        return (await Application.Current.MainPage.DisplayAlert(LocalizationManager.Instance["Warning"].ToString(), 
            LocalizationManager.Instance["QuestionDeleteBook"].ToString(), 
            LocalizationManager.Instance["Yes"].ToString(), 
            LocalizationManager.Instance["No"].ToString()));
    }

    /// <summary>
    /// Dialog zobrazující neočekávanou chybu
    /// </summary>
    /// <param name="message">Text chyby</param>
    public static async Task ShowUnexpectedErrorDialog(string message)
    {
        await Application.Current.MainPage.DisplayAlert($"{LocalizationManager.Instance["UnexError"]}", message, LocalizationManager.Instance["Ok"].ToString());
    }

    #endregion Dialog
    #region Toast
    /// <summary>
    /// Toast zobrazující informaci o přidání knihy
    /// </summary>
    /// <param name="bookTitle">Název knihy</param>
    public static async Task ShowBookAddedToast(string bookTitle)
    {
        await Toast.Make($"{LocalizationManager.Instance["NotifBookAdded"]} {bookTitle}").Show();
    }

    /// <summary>
    /// Toast zobrazující informaci o chybě při přidání knihy
    /// </summary>
    /// <param name="bookTitle">Název knihy</param>
    public static async Task ShowBookAddedErrorToast(string bookTitle)
    {
        await Toast.Make($"{LocalizationManager.Instance["NotifBookAddedError"]} {bookTitle}").Show();
    }

    /// <summary>
    /// Toast zobrazující informaci o odstranění knihy
    /// </summary>
    /// <param name="bookTitle">Název knihy</param>
    public static async Task ShowBookDeletedToast(string bookTitle)
    {
        await Toast.Make($"{LocalizationManager.Instance["NotifBookDeleted"]} {bookTitle}").Show();
    }

    /// <summary>
    /// Toast zobrazující informaci o chybě při odstranění knihy
    /// </summary>
    /// <param name="bookTitle">Název knihy</param>
    public static async Task ShowBookDeletedErrorToast(string bookTitle)
    {
        await Toast.Make($"{LocalizationManager.Instance["NotifBookDeletingError"]} {bookTitle}").Show();
    }

    /// <summary>
    /// Toast zobrazující informaci o potřebě výběru knihy
    /// </summary>
    public static async Task ShowNeedChooseBookWarningToast()
    {
        await Toast.Make(LocalizationManager.Instance["NeedChooseBook"].ToString()).Show();
    }

    /// <summary>
    /// Toast zobrazující informaci o aplikování nastavení
    /// </summary>
    public static async Task ShowApplySettingsToast()
    {
        await Toast.Make(LocalizationManager.Instance["ApplySettings"].ToString()).Show();
    }

    /// <summary>
    /// Toast zobrazující informaci o aplikování původního nastavení
    /// </summary>
    public static async Task ShowDefaultSettingsToast()
    {
        await Toast.Make(LocalizationManager.Instance["DefaultSettings"].ToString()).Show();
    }

    /// <summary>
    /// Toast zobrazující informaci o exportu dat
    /// </summary>
    public static async Task ShowExportDataToast()
    {
        await Toast.Make(LocalizationManager.Instance["NotifExportData"].ToString()).Show();
    }

    /// <summary>
    /// Toast zobrazující informaci o importu dat
    /// </summary>
    public static async Task ShowImportDataToast()
    {
        await Toast.Make(LocalizationManager.Instance["NotifImportData"].ToString()).Show();
    }

    /// <summary>
    /// Toast zobrazující informaci o chybě při importu dat
    /// </summary>
    public static async Task ShowImportDataErrorToast()
    {
        await Toast.Make(LocalizationManager.Instance["NotifImportDataError"].ToString()).Show();
    }

    /// <summary>
    /// Toast zobrazující informaci o přidání záložky
    /// </summary>
    public static async Task ShowBookmarkAddedToast()
    {
        await Toast.Make(LocalizationManager.Instance["NotifBookmarkAdded"].ToString()).Show();
    }

    /// <summary>
    /// Toast zobrazující informaci o přidání záložky
    /// </summary>
    public static async Task ShowBookmarkDeletedToast()
    {
        await Toast.Make(LocalizationManager.Instance["NotifBookmarkDeleted"].ToString()).Show();
    }
    #endregion Toast
}
