using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HybridBookReader.Helpers;
using HybridBookReader.Managers;
using HybridBookReader.Models;
using System.Collections.ObjectModel;

namespace HybridBookReader.ViewModels;

public partial class HomeViewModel : ObservableObject
{
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

    public HomeViewModel()
    {
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
        SelectedBook = null;
        await App.StateManager.OpenBook(book.BookId);
    }

    /// <summary>
    /// Nastavení knihovny na maximálně 5 naposledy čtených knih
    /// </summary>
    public void SetLibrary()
    {
        Library = new ObservableCollection<Book>(App.StateManager.Library
            .OrderByDescending(x => x.LastOpenedAsDate())
            .ThenByDescending(x => x.PercentageProgress)
            .ThenBy(x => x.Title)
            .Take(5));
    }
}
