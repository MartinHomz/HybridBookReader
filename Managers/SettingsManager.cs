using Plugin.LocalNotification;

namespace HybridBookReader.Managers;

/// <summary>
/// Správce uživatelského nastavení
/// </summary>
public class SettingsManager
{
    private const string preferencesNotif = "notifTime";
    private const string preferencesAllowNotif = "allowNotif";

    /// <summary>
    /// Nastavený čas pro notifikaci připomenutí čtení
    /// </summary>
    public DateTime NotificationTime { get; private set; }
    /// <summary>
    /// Povolení zobrazení notifikace čtení
    /// </summary>
    public bool AllowNotification { get; private set; }

    public SettingsManager()
    {
        string time = Preferences.Get(preferencesNotif, string.Empty);
        NotificationTime = string.IsNullOrEmpty(time) ? GetDefaultNotificationTime() : DateTime.Parse(time);
        AllowNotification = Preferences.Get(preferencesAllowNotif, false);
    }

    /// <summary>
    /// Vrací původní nastavení pro čas notifikace (19:00)
    /// </summary>
    public DateTime GetDefaultNotificationTime()
    {
        return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 0, 0);
    }

    /// <summary>
    /// Nastavení notifikace
    /// </summary>
    /// <param name="notif">Čas notifikace</param>
    /// <param name="allow">Povolení notifikace</param>
    public void SetNotification(DateTime notif, bool allow)
    {
        NotificationTime = notif;
        AllowNotification = allow;

        //Uložení dat do lokálního uložiště
        Preferences.Set(preferencesNotif, NotificationTime.ToString());
        Preferences.Set(preferencesAllowNotif, AllowNotification);

        if (AllowNotification)
        {
            //Nastavení notifikace
            NotificationRequest request = new NotificationRequest
            {
                NotificationId = 1154,
                Title = LocalizationManager.Instance["ReadingTime"].ToString(),
                BadgeNumber = 24,
                CategoryType = NotificationCategoryType.Reminder,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = NotificationTime,
                    RepeatType = NotificationRepeat.Daily
                },
            };

            LocalNotificationCenter.Current.Show(request);
        }
    }

    /// <summary>
    /// Odstranění všech notifikací
    /// </summary>
    public void DeleteNotification()
    {
        LocalNotificationCenter.Current.CancelAll();
    }
}
