using HybridBookReader.Managers;
using HybridBookReader.Models;
using System.Globalization;

namespace HybridBookReader;

public partial class App : Application
{
    public static StateManager StateManager { get; private set; }
    public static DataManager DataManager { get; private set; }
    public static SettingsManager SettingsManager { get; private set; }

    public App(StateManager stateManager, DataManager databaseManager, SettingsManager settingsManager)
    {
        InitializeComponent();

        StateManager = stateManager;
        DataManager = databaseManager;
        SettingsManager = settingsManager;

        MainPage = new AppShell();
    }

    public static void HandleAppActions(AppAction appAction)
    {
        Current.Dispatcher.Dispatch(async () =>
        {
            await StateManager.Init();
            await StateManager.OpenBook(int.Parse(appAction.Id));
        });
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        Window window = base.CreateWindow(activationState);

        window.Destroying += async (s, e) =>
        {
            if (StateManager.CurrentBook != null)
                await StateManager.CloseBook(false);
        };

        return window;
    }
}
