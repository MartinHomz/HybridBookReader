using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HybridBookReader.Helpers;
using HybridBookReader.Managers;
using HybridBookReader.Models;
using System.Collections.ObjectModel;

namespace HybridBookReader.ViewModels;

public partial class LibraryViewModel : ObservableObject
{
    private enum eSortBy
    {
        Progress = 1,
        Name = 2,
        LastRead = 3
    }

    private const string librarySortBy = "libSortBy";

    private eSortBy _sortBy;

    //Odkaz na lokalizaci pro binding
    public LocalizationManager LocalizationManager => LocalizationManager.Instance;

    /// <summary>
    /// Seznam knih
    /// </summary>
    [ObservableProperty] ObservableCollection<Book> library;
    /// <summary>
    /// Vybraná kniha
    /// </summary>
    public Book SelectedBook { get; set; }

    public LibraryViewModel()
    {
        _sortBy = (eSortBy)Preferences.Get(librarySortBy, (int)eSortBy.Progress);
    }

    /// <summary>
    /// Přidání knihy do knihovny
    /// </summary>
    [RelayCommand]
    private async Task AddBook()
    {
        await App.StateManager.AddBook();
        SetLibrary();
        SelectedBook = null;
    }

    /// <summary>
    /// Odstranění knihy z knihovny
    /// </summary>
    [RelayCommand]
    private async Task DeleteBook()
    {
        if (SelectedBook != null)
        {
            if (await UIHelper.ShowDeleteBookDialog())
            {
                await App.StateManager.DeleteBook(SelectedBook);
                SetLibrary();
                SelectedBook = null;
            }
        }
        else
            await UIHelper.ShowNeedChooseBookWarningToast();
    }

    /// <summary>
    /// Otevření knihy pro čtení
    /// </summary>
    [RelayCommand]
    public async Task ReadBook()
    {
        if (SelectedBook != null)
        {
            await App.StateManager.OpenBook(SelectedBook.BookId);
            SelectedBook = null;
        }
        else
            await UIHelper.ShowNeedChooseBookWarningToast();
    }

    /// <summary>
    /// Otevření knihy pro čtení
    /// </summary>
    /// <param name="book">Vybraná kniha</param>
    public async Task ReadBookTapped(Book book)
    {
        await App.StateManager.OpenBook(book.BookId);
    }

    [RelayCommand]
    public void SortByName()
    {
        _sortBy = eSortBy.Name;
        Preferences.Set(librarySortBy, (int)_sortBy);
        SetLibrary();
    }

    [RelayCommand]
    public void SortByProgress()
    {
        _sortBy = eSortBy.Progress;
        Preferences.Set(librarySortBy, (int)_sortBy);
        SetLibrary();
    }

    [RelayCommand]
    public void SortByLastRead()
    {
        _sortBy = eSortBy.LastRead;
        Preferences.Set(librarySortBy, (int)_sortBy);
        SetLibrary();
    }

    /// <summary>
    /// Nastavení knihovny (všechny knihy)
    /// </summary>
    public void SetLibrary()
    {
        if (_sortBy == eSortBy.Name)
            Library = new ObservableCollection<Book>(App.StateManager.Library.OrderBy(x => x.Title));
        else if (_sortBy == eSortBy.Progress)
            Library = new ObservableCollection<Book>(App.StateManager.Library.OrderByDescending(x => x.PercentageProgress));
        else
            Library = new ObservableCollection<Book>(App.StateManager.Library.OrderByDescending(x => x.LastOpenedAsDate()));
    }
}
