using logist_app.Core.Entities;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;

namespace logist_app
{
    public partial class RecurrenceModalPage : ContentPage
    {
        private Action<RecurrenceSettings> _onSave;

        // Конструктор принимает текущие настройки и callback функцию
        public RecurrenceModalPage(RecurrenceSettings currentSettings, Action<RecurrenceSettings> onSave)
        {
            InitializeComponent();
            _onSave = onSave;

            // Защита от null: если настроек нет, создаем новые
            currentSettings ??= new RecurrenceSettings { Type = "Weekly", Interval = 1 };

            // 1. Установка Типа (Weekly/Monthly/Daily)
            // Используем цикл для надежного сравнения строк без учета регистра
            bool typeFound = false;
            foreach (var item in TypePicker.ItemsSource)
            {
                if (item.ToString().Equals(currentSettings.Type, StringComparison.OrdinalIgnoreCase))
                {
                    TypePicker.SelectedItem = item;
                    typeFound = true;
                    break;
                }
            }
            if (!typeFound) TypePicker.SelectedIndex = 1; // Default Weekly

            // 2. Установка Интервала
            IntervalEntry.Text = currentSettings.Interval.ToString();

            // 3. Безопасное получение списка дней (защита от null)
            var days = currentSettings.DaysOfWeek ?? new List<DayOfWeek>();

            // 4. Проставляем галочки
            // В C# DayOfWeek.Monday == 1, DayOfWeek.Wednesday == 3, DayOfWeek.Friday == 5
            // Ваш JSON [1, 3, 5] корректно включит эти галочки.
            cbMon.IsChecked = days.Contains(DayOfWeek.Monday);    // 1
            cbTue.IsChecked = days.Contains(DayOfWeek.Tuesday);   // 2
            cbWed.IsChecked = days.Contains(DayOfWeek.Wednesday); // 3
            cbThu.IsChecked = days.Contains(DayOfWeek.Thursday);  // 4
            cbFri.IsChecked = days.Contains(DayOfWeek.Friday);    // 5
            cbSat.IsChecked = days.Contains(DayOfWeek.Saturday);  // 6
            cbSun.IsChecked = days.Contains(DayOfWeek.Sunday);    // 0

            // 5. Недели месяца (если есть)
            var weeks = currentSettings.WeeksOfMonth ?? new List<int>();
            cbW1.IsChecked = weeks.Contains(1);
            cbW2.IsChecked = weeks.Contains(2);
            cbW3.IsChecked = weeks.Contains(3);
            cbW4.IsChecked = weeks.Contains(4);
            cbW5.IsChecked = weeks.Contains(5);

            // 6. Дни месяца (для Monthly)
            var daysOfMonth = currentSettings.DaysOfMonth ?? new List<int>();
            if (daysOfMonth.Any())
            {
                DaysOfMonthEntry.Text = string.Join(", ", daysOfMonth);
            }

            // Обновляем видимость (чтобы показать WeeklySection сразу)
            UpdateVisibility();
        }

        private void OnTypeChanged(object sender, EventArgs e)
        {
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            string type = (string)TypePicker.SelectedItem;
            WeeklySection.IsVisible = type == "Weekly";
            MonthlySection.IsVisible = type == "Monthly";
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            var settings = new RecurrenceSettings
            {
                Type = (string)TypePicker.SelectedItem,
                Interval = int.TryParse(IntervalEntry.Text, out int i) ? i : 1
            };

            if (settings.Type == "Weekly")
            {
                if (cbMon.IsChecked) settings.DaysOfWeek.Add(DayOfWeek.Monday);
                if (cbTue.IsChecked) settings.DaysOfWeek.Add(DayOfWeek.Tuesday);
                if (cbWed.IsChecked) settings.DaysOfWeek.Add(DayOfWeek.Wednesday);
                if (cbThu.IsChecked) settings.DaysOfWeek.Add(DayOfWeek.Thursday);
                if (cbFri.IsChecked) settings.DaysOfWeek.Add(DayOfWeek.Friday);
                if (cbSat.IsChecked) settings.DaysOfWeek.Add(DayOfWeek.Saturday);
                if (cbSun.IsChecked) settings.DaysOfWeek.Add(DayOfWeek.Sunday);

                if (cbW1.IsChecked) settings.WeeksOfMonth.Add(1);
                if (cbW2.IsChecked) settings.WeeksOfMonth.Add(2);
                if (cbW3.IsChecked) settings.WeeksOfMonth.Add(3);
                if (cbW4.IsChecked) settings.WeeksOfMonth.Add(4);
                if (cbW5.IsChecked) settings.WeeksOfMonth.Add(5);
            }
            else if (settings.Type == "Monthly")
            {
                var parts = DaysOfMonthEntry.Text?.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts != null)
                {
                    foreach (var p in parts)
                    {
                        if (int.TryParse(p, out int day)) settings.DaysOfMonth.Add(day);
                    }
                }
            }

            // Возвращаем данные в MainPage
            _onSave?.Invoke(settings);
            await Navigation.PopModalAsync();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}