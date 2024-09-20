using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HybridBookReader.Helpers;
using HybridBookReader.Managers;
using HybridBookReader.Models;
using System.Text.Json;

namespace HybridBookReader.ViewModels;

public partial class ReaderViewModel : ObservableObject
{
    //Odkaz na lokalizaci pro binding
    public LocalizationManager LocalizationManager => LocalizationManager.Instance;

    /// <summary>
    /// Vybraná kniha
    /// </summary>
    [ObservableProperty] Book selectedBook;

    public ReaderViewModel()
    {
        selectedBook = App.StateManager.CurrentBook;
    }
}
