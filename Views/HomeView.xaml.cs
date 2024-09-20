using HybridBookReader.Models;
using HybridBookReader.ViewModels;
using System.Collections.ObjectModel;

namespace HybridBookReader.Views;

public partial class HomeView : ContentPage
{
    public HomeView(HomeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        await App.StateManager.Init();
        ((HomeViewModel)BindingContext).SetLibrary();
    }

    private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection[0] != null)
        {
            Book selectedBook = e.CurrentSelection[0] as Book;
            ((HomeViewModel)BindingContext).SelectedBook = selectedBook;
        }
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Book selectedBook = ((VisualElement)sender).BindingContext as Book;
        await ((HomeViewModel)BindingContext).ReadBookTapped(selectedBook);
    }
}