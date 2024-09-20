using HybridBookReader.ViewModels;

namespace HybridBookReader.Views;

public partial class ReaderView : ContentPage
{
    public ReaderView(ReaderViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnDisappearing()
    {
        await App.StateManager.CloseBook(true);
    }
}