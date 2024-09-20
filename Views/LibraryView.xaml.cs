using HybridBookReader.Models;
using HybridBookReader.ViewModels;
using System.Diagnostics;

namespace HybridBookReader.Views;

public partial class LibraryView : ContentPage
{
    public LibraryView(LibraryViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        ((LibraryViewModel)BindingContext).SetLibrary();
    }

    private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection[0] != null)
        {
            Book selectedBook = e.CurrentSelection[0] as Book;
            ((LibraryViewModel)BindingContext).SelectedBook = selectedBook;
        }
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Book selectedBook = ((VisualElement)sender).BindingContext as Book;
        await ((LibraryViewModel)BindingContext).ReadBookTapped(selectedBook);
    }
}