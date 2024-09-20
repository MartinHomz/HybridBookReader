using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using HybridBookReader.Managers;
using HybridBookReader.Helpers;
using HybridBookReader.Views;
using HybridBookReader.ViewModels;
using Plugin.LocalNotification;

namespace HybridBookReader;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            })
            .ConfigureEssentials(essentials =>
             {
                 essentials
                     .OnAppAction(App.HandleAppActions);
            });

        builder.Services.AddMauiBlazorWebView();

#if ANDROID || IOS
        builder.UseLocalNotification();
#endif

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        builder.Services.AddLocalization();

        //Helpers
        string dbPath = FileHelper.GetLocalFilePath("database.db3");

        //Managers
        builder.Services.AddSingleton<StateManager>(s => ActivatorUtilities.CreateInstance<StateManager>(s));
        builder.Services.AddSingleton<DataManager>(s => ActivatorUtilities.CreateInstance<DataManager>(s, dbPath));
        builder.Services.AddSingleton<SettingsManager>(s => ActivatorUtilities.CreateInstance<SettingsManager>(s));

        //Views & ViewModels
        builder.Services.AddTransient<HomeView>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<LibraryView>();
        builder.Services.AddTransient<LibraryViewModel>();
        builder.Services.AddTransient<ReaderView>();
        builder.Services.AddTransient<ReaderViewModel>();
        builder.Services.AddTransient<SettingsView>();
        builder.Services.AddTransient<SettingsViewModel>();

        return builder.Build();
    }
}
