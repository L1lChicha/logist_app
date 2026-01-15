// NotificationsViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using logist_app.Core.Entities;
using logist_app.Models;
using logist_app.Services;
using System.Collections.ObjectModel;

namespace logist_app.ViewModels;

public partial class NotificationsViewModel : ObservableObject
{
    private readonly NotificationService _notificationService;

    // Прямая ссылка на коллекцию в сервисе
    public ObservableCollection<AppNotification> Notifications => _notificationService.Notifications;

    public NotificationsViewModel(NotificationService notificationService)
    {
        _notificationService = notificationService;
        // Здесь больше ничего не нужно!
    }

    [RelayCommand]
    void ClearAll() => _notificationService.ClearAll();

    [RelayCommand]
    void Delete(AppNotification item) => _notificationService.Remove(item);
}