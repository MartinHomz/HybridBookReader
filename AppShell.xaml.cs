using HybridBookReader.Managers;
using HybridBookReader.ViewModels;
using HybridBookReader.Views;

namespace HybridBookReader;

public partial class AppShell : Shell
{
    //Odkaz na lokalizaci pro binding
    public LocalizationManager LocalizationManager => LocalizationManager.Instance;

    public AppShell()
    {
        InitializeComponent();

        BindingContext = this;

        Routing.RegisterRoute(nameof(ReaderView), typeof(ReaderView));
    }
}