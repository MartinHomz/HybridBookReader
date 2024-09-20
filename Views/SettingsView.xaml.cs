using HybridBookReader.ViewModels;

namespace HybridBookReader.Views;

public partial class SettingsView : ContentPage
{
    public SettingsView(SettingsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        ((SettingsViewModel)BindingContext).SettingsChanged = true;
    }

    private void Switch_Toggled(object sender, ToggledEventArgs e)
    {
        ((SettingsViewModel)BindingContext).SettingsChanged = true;
    }
}