using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using logist_app.Core.Entities;
using logist_app.Infrastructure.Service;
using logist_app.Models;
using System.Collections.ObjectModel;

namespace logist_app.ViewModels
{
    public partial class NotificationsViewModel : ObservableObject
    {
        private readonly SignalRService _signalRService;

        // Коллекция, которая автоматически обновляет UI при добавлении элементов
        public ObservableCollection<AppNotification> Notifications { get; } = new();

        public NotificationsViewModel(SignalRService signalRService)
        {
            _signalRService = signalRService;

            // Подписываемся на событие получения сообщения
            _signalRService.OnNoteReceived += OnNotificationReceived;
        }

        private void OnNotificationReceived(ClientNoteNotification data)
        {
            // SignalR вызывает событие в фоновом потоке, а UI можно обновлять только в главном
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var newNote = new AppNotification
                {
                    Title = $"Проблема: {data.ClientId},\n{data.ClientName}",
                    Message = data.NotesAboutProblems ?? "Нет описания",
                    Timestamp = DateTime.Now,
                    IsRead = false
                };

                // Добавляем в начало списка (сверху самые новые)
                Notifications.Insert(0, newNote);
            });
        }

        [RelayCommand]
        void ClearAll()
        {
            Notifications.Clear();
        }

        [RelayCommand]
        void Delete(AppNotification item)
        {
            if (Notifications.Contains(item))
            {
                Notifications.Remove(item);
            }
        }
    }
}