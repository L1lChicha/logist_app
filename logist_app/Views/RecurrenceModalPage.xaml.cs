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

            // Инициализация полей (если настройки уже были)
            if (currentSettings != null)
            {
                TypePicker.SelectedItem = currentSettings.Type;
                IntervalEntry.Text = currentSettings.Interval.ToString();

                // Восстановление чекбоксов дней недели
                cbMon.IsChecked = currentSettings.DaysOfWeek.Contains(DayOfWeek.Monday);
                cbTue.IsChecked = currentSettings.DaysOfWeek.Contains(DayOfWeek.Tuesday);
                cbWed.IsChecked = currentSettings.DaysOfWeek.Contains(DayOfWeek.Wednesday);
                cbThu.IsChecked = currentSettings.DaysOfWeek.Contains(DayOfWeek.Thursday);
                cbFri.IsChecked = currentSettings.DaysOfWeek.Contains(DayOfWeek.Friday);
                cbSat.IsChecked = currentSettings.DaysOfWeek.Contains(DayOfWeek.Saturday);
                cbSun.IsChecked = currentSettings.DaysOfWeek.Contains(DayOfWeek.Sunday);

                // Восстановление недель
                cbW1.IsChecked = currentSettings.WeeksOfMonth.Contains(1);
                cbW2.IsChecked = currentSettings.WeeksOfMonth.Contains(2);
                cbW3.IsChecked = currentSettings.WeeksOfMonth.Contains(3);
                cbW4.IsChecked = currentSettings.WeeksOfMonth.Contains(4);
                cbW5.IsChecked = currentSettings.WeeksOfMonth.Contains(5);

                // Восстановление дней месяца
                DaysOfMonthEntry.Text = string.Join(", ", currentSettings.DaysOfMonth);
            }
            else
            {
                TypePicker.SelectedIndex = 1; // Default Weekly
            }

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