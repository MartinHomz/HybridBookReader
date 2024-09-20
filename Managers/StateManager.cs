using CommunityToolkit.Maui.Alerts;
using HybridBookReader.Helpers;
using HybridBookReader.Models;
using HybridBookReader.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using VersOne.Epub;

namespace HybridBookReader.Managers;

public class StateManager
{
    /// <summary>
    /// Aktuální seznam knih
    /// </summary>
    public List<Book> Library { get; set; } = new List<Book>();
    /// <summary>
    /// Právě vybraná kniha (pro přenos s Blazorem)
    /// </summary>
    public Book CurrentBook { get; private set; }
    private bool _initDone = false;

    public async Task Init()
    {
        if (!_initDone)
        {
            _initDone = true;
            await App.DataManager.InitDb();
            await UpdateLibrary(false);
        }
    }

    #region Book
    private async Task UpdateLibrary(bool listModified)
    {
        Library = await App.DataManager.GetBooksFromDb();

        if (listModified && AppActions.Current.IsSupported)
        {
            List<AppAction> actions = new List<AppAction>();

            if (Library.Count > 0)
            {

                List<Book> actionsLibrary = Library
                    .OrderByDescending(x => x.LastOpenedAsDate())
                    .ThenByDescending(x => x.PercentageProgress)
                    .ThenBy(x => x.Title)
                    .Take(5).ToList();

                foreach (Book book in actionsLibrary)
                {
                    actions.Add(new AppAction(book.BookId.ToString(), book.Title));
                }
            }

            await AppActions.Current.SetAsync(actions);
        }
    }

    /// <summary>
    /// Přidání nové knihy do knihovny (výběr souboru, uložení obrázku obalu, vložení do DB)
    /// </summary>
    public async Task AddBook()
    {
        try
        {
            //Výběr souboru
            FileResult result = await UIHelper.ShowFilePicker("Vyberte knihu");

            if (result != null)
            {
                EpubBook epubBook = EpubReader.ReadBook(result.FullPath);

                //Uložení obrázku obalu knihy
                string coverImageFilePath = string.Empty;
                if (epubBook.CoverImage != null && epubBook.CoverImage.Length > 0)
                    coverImageFilePath = await App.DataManager.SaveBookCoverImage(epubBook.CoverImage);

                //Uložení knihy do databáze
                bool saveOk = await App.DataManager.InsertNewBookToDb(
                new Book
                {
                    Title = epubBook.Title,
                    Author = epubBook.Author,
                    Description = epubBook.Description,
                    BookPath = result.FullPath,
                    CoverImagePath = coverImageFilePath
                });

                //Aktualizace seznamu knih a notifikace
                if (saveOk)
                {
                    await UpdateLibrary(true);
                    await UIHelper.ShowBookAddedToast(epubBook.Title);
                }
                else
                    await UIHelper.ShowBookAddedErrorToast(epubBook.Title);
            }
        }
        catch (Exception ex)
        {
            await UIHelper.ShowUnexpectedErrorDialog(ex.Message);
        }
    }

    /// <summary>
    /// Odstranění knihy z knihovny (odstranění obrázku obalu knihy, odstranění z DB)
    /// </summary>
    /// <param name="book"></param>
    public async Task DeleteBook(Book book)
    {
        //Smazání obrázku obalu knihy z lokálního uložiště
        if (!string.IsNullOrEmpty(book.CoverImagePath))
            File.Delete(FileHelper.GetLocalFilePath(book.CoverImagePath));

        //Smazání knihy z databáze
        bool deleteOk = await App.DataManager.DeleteBookFromDb(book.BookId);

        //Aktualizace seznamu knih a notifikace
        if (deleteOk)
        {
            await UpdateLibrary(true);
            await UIHelper.ShowBookDeletedToast(book.Title);
        }
        else
            await UIHelper.ShowBookDeletedErrorToast(book.Title);
    }

    /// <summary>
    /// Otevření knihy pro čtení (Nastavení aktuální knihy a přesměrování na čtečku)
    /// </summary>
    /// <param name="bookId">ID knihy pro čtení</param>
    public async Task OpenBook(int bookId)
    {
        Book book = Library.FirstOrDefault(x => x.BookId == bookId);
        if (book != null)
        {
            CurrentBook = book;
            await Shell.Current.GoToAsync(nameof(ReaderView));
        }
    }

    /// <summary>
    /// Zavření knihy (uložení průběhu čtení knihy)
    /// </summary>
    public async Task CloseBook(bool refresh)
    {
        CurrentBook.LastOpened = DateTime.Now.ToString();

        if (await App.DataManager.UpdateBookProgress(CurrentBook))
        {
            if (refresh)
                await UpdateLibrary(true);
        }
        else
            await UIHelper.ShowUnexpectedErrorDialog("UPDATE error");
        CurrentBook = null;
    }

    /// <summary>
    /// Přečte právě vybranou knihu a uloží do pole bytů
    /// </summary>
    /// <returns>Kniha jako pole bytů</returns>
    public byte[] CurrentBookToByteArray()
    {
        return FileHelper.FileToByteArray(CurrentBook.BookPath);
    }
    #endregion Book
    #region Export/Import
    /// <summary>
    /// Ukládání exportního souboru
    /// </summary>
    public async Task SaveExportFile()
    {
        try
        {
            string json = await App.DataManager.ExportDbToJSON();
            FileHelper.SaveTextFileToDocumentsDirectory("export.json", json);
            await UIHelper.ShowExportDataToast();
        }
        catch (Exception ex)
        {
            await UIHelper.ShowUnexpectedErrorDialog(ex.Message);
        }
    }

    /// <summary>
    /// Načtení importního souboru do databáze
    /// </summary>
    /// <returns></returns>
    public async Task LoadImportFile()
    {
        try
        {
            //Výběr souboru
            FileResult result = await UIHelper.ShowFilePicker("Vyberte importní soubor");

            if (result != null)
            {
                //Přečtení souboru a nahrání dat do databáze
                string json = FileHelper.ReadFile(result.FullPath);
                bool importOk = await App.DataManager.ImportFromJSONToDb(json);

                if (importOk)
                {
                    //Aktualizace seznamu knih a notifikace
                    await UpdateLibrary(true);
                    await UIHelper.ShowImportDataToast();
                }
                else
                    await UIHelper.ShowImportDataErrorToast();
            }
        }
        catch (Exception ex)
        {
            await UIHelper.ShowUnexpectedErrorDialog(ex.Message);
        }
    }
    #endregion Export/Import
}
