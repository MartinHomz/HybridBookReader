using HybridBookReader.Helpers;
using HybridBookReader.Models;
using SQLite;
using System.Text.Json;
using VersOne.Epub;

namespace HybridBookReader.Managers;

/// <summary>
/// Správce dat (databáze, interní uložiště, import a export)
/// </summary>
public class DataManager
{
    string _dbPath;
    private SQLiteAsyncConnection _conn;
    private HttpClient _httpClient = new HttpClient();

    public DataManager(string dbPath)
    {
        _dbPath = dbPath;
    }

    #region Init
    /// <summary>
    /// Navázání spojení s databází a případné vytvoření tabulek
    /// </summary>
    public async Task InitDb()
    {
        try
        {
            if (_conn == null)
            {
                _conn = new SQLiteAsyncConnection(_dbPath); //navázání spojení
                await CreateTables();  //tvorba tabulek
            }
        }
        catch (Exception ex)
        {
            await UIHelper.ShowUnexpectedErrorDialog(ex.Message);
        }
    }

    /// <summary>
    /// Tvorba tabulek, pokud ještě neexistují
    /// </summary>
    private async Task CreateTables()
    {
        try
        {
            //Dochází k vypnutí a zapnutí cizích klíčů, protože SQLite má problém s hlídáním integrity
            await _conn.ExecuteAsync("PRAGMA foreign_keys = OFF;");

            await _conn.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS Book(
                BookId INTEGER PRIMARY KEY AUTOINCREMENT,
                Title VARCHAR(50),
                Author VARCHAR(30),
                Description VARCHAR(250),
                BookPath VARCHAR(100),
                CoverImagePath VARCHAR(100),
                CFIPosition VARCHAR(50),
                LastOpened DATETIME,
                CurrentPage INTEGER,
                PagesCount INTEGER
                );");

            await _conn.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS Bookmark(
                    BookmarkId INTEGER PRIMARY KEY AUTOINCREMENT,
                    BookId INTEGER,
                    CFIPosition VARCHAR(50),
                    FOREIGN KEY(BookId) REFERENCES Book(BookId)
                    );");

            await _conn.ExecuteAsync("PRAGMA foreign_keys = ON;");
        }
        catch (Exception ex)
        {
            await UIHelper.ShowUnexpectedErrorDialog(ex.Message);
        }
    }
    #endregion Init
    #region Helpers

    #endregion Helpers
    #region Book
    /// <summary>
    /// Získání všech knih z databáze
    /// </summary>
    /// <returns>Seznam knih</returns>
    public async Task<List<Book>> GetBooksFromDb()
    {
        try
        {
            return await _conn.QueryAsync<Book>(@"SELECT * FROM Book;");
        }
        catch (Exception ex)
        {
            await UIHelper.ShowUnexpectedErrorDialog(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Uložení obrázku obalu knihy do lokálního uložiště
    /// </summary>
    /// <param name="coverImage"></param>
    /// <returns>Cesta k souboru obrázku obalu knihy</returns>
    public async Task<string> SaveBookCoverImage(byte[] coverImage)
    {
        try
        {
            string coverImageFileName = $"ci{Guid.NewGuid()}.jpg";
            string coverImageFilePath = FileHelper.GetLocalFilePath(coverImageFileName);
            File.WriteAllBytes(coverImageFilePath, coverImage);
            return coverImageFilePath;
        }
        catch (Exception ex)
        {
            await UIHelper.ShowUnexpectedErrorDialog(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Vložení nové knihy do databáze (ukládá pouze údaje známe při přídání nové knihy)
    /// </summary>
    /// <param name="book">Nová kniha</param>
    /// <returns>False = neúšpěšná akce</returns>
    public async Task<bool> InsertNewBookToDb(Book book)
    {
        try
        {
            int result = await _conn.ExecuteAsync($@"INSERT INTO Book(Title, Author, Description, BookPath, CoverImagePath) VALUES
                    ('{book.Title}', '{book.Author}','{book.Description}', '{book.BookPath}','{book.CoverImagePath}');");
            return result == 1;
        }
        catch (Exception ex)
        {
            await UIHelper.ShowUnexpectedErrorDialog(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Vložení nové knihy do databáze (ukládá všechny parametery)
    /// </summary>
    /// <param name="book">Nová kniha</param>
    /// <returns>False = neúšpěšná akce</returns>
    public async Task<bool> InsertBookToDb(Book book)
    {
        try
        {
            int result = await _conn.ExecuteAsync($@"INSERT INTO Book(Title, Author, Description, BookPath, CoverImagePath, CFIPosition, PagesCount, CurrentPage, LastOpened) VALUES
                    ('{book.Title}', '{book.Author}','{book.Description}','{book.BookPath}','{book.CoverImagePath}','{book.CFIPosition}',{book.PagesCount},{book.CurrentPage},'{book.LastOpened}');");
            return result == 1;
        }
        catch (Exception ex)
        {
            await UIHelper.ShowUnexpectedErrorDialog(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Odstranění knihy z databáze
    /// </summary>
    /// <param name="bookId">ID knihy</param>
    /// <returns>False = neúšpěšná akce</returns>
    public async Task<bool> DeleteBookFromDb(int bookId)
    {
        try
        {
            int result = await _conn.ExecuteAsync($@"DELETE FROM Book WHERE BookId={bookId};");
            await _conn.ExecuteAsync($@"DELETE FROM Bookmark WHERE BookId={bookId};");
            return result == 1;
        }
        catch (Exception ex)
        {
            await UIHelper.ShowUnexpectedErrorDialog(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Upravení dat knihy o průběh čtení
    /// </summary>
    /// <returns></returns>
    public async Task<bool> UpdateBookProgress(Book book)
    {
        try
        {
            int result = await _conn.ExecuteAsync($@"UPDATE Book 
                SET CFIPosition='{book.CFIPosition}', PagesCount={book.PagesCount}, CurrentPage={book.CurrentPage}, LastOpened='{book.LastOpened}'
                WHERE BookId={book.BookId};");
            return result == 1;
        }
        catch (Exception ex)
        {
            await UIHelper.ShowUnexpectedErrorDialog(ex.Message);
            return false;
        }
    }
    #endregion Book
    #region Bookmark
    /// <summary>
    /// Získání všech záložek z databáze
    /// </summary>
    /// <returns>Seznam záložek</returns>
    public async Task<List<Bookmark>> GetBookmarksFromDb()
    {
        try
        {
            return await _conn.QueryAsync<Bookmark>(@$"SELECT * FROM Bookmark");
        }
        catch (Exception ex)
        {
            await UIHelper.ShowUnexpectedErrorDialog(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Získání všech záložek dané knihy z databáze
    /// </summary>
    /// <param name="bookId">ID knihy</param>
    /// <returns>Seznam záložek</returns>
    public async Task<List<Bookmark>> GetBookmarksByBookIdFromDb(int bookId)
    {
        try
        {
            return await _conn.QueryAsync<Bookmark>(@$"SELECT * FROM Bookmark WHERE BookId={bookId};");
        }
        catch (Exception ex)
        {
            await UIHelper.ShowUnexpectedErrorDialog(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Vložení nové záložky do databáze
    /// </summary>
    /// <param name="bookmark">Nová záložka</param>
    /// <returns>False = neúšpěšná akce</returns>
    public async Task<bool> InsertBookmarkToDb(Bookmark bookmark)
    {
        try
        {
            int result = await _conn.ExecuteAsync($@"INSERT INTO Bookmark(BookId, CFIPosition) VALUES
                    ('{bookmark.BookId}', '{bookmark.CFIPosition}');");
            return result == 1;
        }
        catch (Exception ex)
        {
            await UIHelper.ShowUnexpectedErrorDialog(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Odstranění záložky z databáze
    /// </summary>
    /// <param name="bookId">ID knihy</param>
    /// <returns>False = neúšpěšná akce</returns>
    public async Task<bool> DeleteBookmarkFromDb(int bookmarkId)
    {
        try
        {
            int result = await _conn.ExecuteAsync($@"DELETE FROM Bookmark WHERE BookmarkId={bookmarkId};");
            return result == 1;
        }
        catch (Exception ex)
        {
            await UIHelper.ShowUnexpectedErrorDialog(ex.Message);
            return false;
        }
    }

    #endregion Bookmark
    #region Export/Import
    /// <summary>
    /// Odstranění všech dat z databáze
    /// </summary>
    public async Task DeleteAll()
    {
        try
        {
            await _conn.ExecuteAsync($@"DELETE FROM Book;");
            await _conn.ExecuteAsync($@"DELETE FROM Bookmarks;");
        }
        catch (Exception ex)
        {
            await UIHelper.ShowUnexpectedErrorDialog(ex.Message);
        }
    }
    /// <summary>
    /// Vložení všech dat z databáze do přenosové třídy a následně do řetězce formátu JSON
    /// </summary>
    /// <returns>Object přenosové třídy jako textový řetězec formátu JSON</returns>
    public async Task<string> ExportDbToJSON()
    {
        //Načtení dat do objektu přenosové třídy
        TransferModel transferModel = new TransferModel();
        transferModel.Books = await GetBooksFromDb();
        transferModel.Bookmarks = await GetBookmarksFromDb();

        //Převod do textového řetězce formátu JSON
        return JsonSerializer.Serialize(transferModel);
    }

    /// <summary>
    /// Nahrání dat z textového řetězce formutá JSON do databáze
    /// </summary>
    /// <param name="json">Importní textový řetězec formátu JSON</param>
    public async Task<bool> ImportFromJSONToDb(string json)
    {
        try
        {
            //Smazání všech dat
            await DeleteAll();

            //Převod dat do objektu přenosové třídy
            TransferModel transferModel = new TransferModel();
            transferModel = JsonSerializer.Deserialize<TransferModel>(json);

            //Načtení knih
            foreach (Book book in transferModel.Books)
            {
                //Uložení obrázku obalu knihy
                EpubBook epubBook = EpubReader.ReadBook(book.BookPath);
                string coverImageFilePath = string.Empty;

                if (epubBook.CoverImage != null && epubBook.CoverImage.Length > 0)
                    coverImageFilePath = await SaveBookCoverImage(epubBook.CoverImage);

                book.CoverImagePath = coverImageFilePath;

                //Uložení knihy do databáze
                if (!await InsertBookToDb(book))
                    return false;
            }

            //Načtení záložek
            foreach (Bookmark bookmark in transferModel.Bookmarks)
            {
                if (!await InsertBookmarkToDb(bookmark))
                    return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            await UIHelper.ShowUnexpectedErrorDialog(ex.Message);
            return false;
        }
    }
    #endregion Export/Import

    public async Task<(WordDictionary, string)> GetWordDefinitionFromDictionary(string word)
    {
        string error = string.Empty;
        WordDictionary wordDictionary = null;

        try
        {

            NetworkAccess accessType = Connectivity.Current.NetworkAccess;
            if (accessType == NetworkAccess.Internet)
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"https://api.dictionaryapi.dev/api/v2/entries/en/{word}");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    List<WordDictionary> wordDictionaryList = JsonSerializer.Deserialize<List<WordDictionary>>(content);
                    if (wordDictionaryList.Count > 0) wordDictionary = wordDictionaryList.FirstOrDefault();
                    else wordDictionary = null;
                }
                else error = LocalizationManager.Instance["NoApiData"].ToString();
            }
            else error = LocalizationManager.Instance["NoInternet"].ToString();
        }
        catch (Exception ex)
        {
            await UIHelper.ShowUnexpectedErrorDialog(ex.Message);
            error = LocalizationManager.Instance["UnexError"].ToString();
        }

        return (wordDictionary, error);
    }
}
