using System.Collections.ObjectModel;
using logist_app.Core.Entities;
using logist_app.Models;

namespace logist_app.Services;

public class NotificationService
{
    // Коллекция, которая автоматически обновляет UI
    public ObservableCollection<AppNotification> Notifications { get; } = new();

    public void Add(string title, string message)
    {
        // Обязательно в MainThread, так как это привязано к UI
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Notifications.Insert(0, new AppNotification
            {
                Title = title,
                Message = message,
                Timestamp = DateTime.Now,
                IsRead = false
            });
        });
    }

    public void ClearAll()
    {
        MainThread.BeginInvokeOnMainThread(() => Notifications.Clear());
    }

    public void Remove(AppNotification item)
    {
        MainThread.BeginInvokeOnMainThread(() => Notifications.Remove(item));
    }
}