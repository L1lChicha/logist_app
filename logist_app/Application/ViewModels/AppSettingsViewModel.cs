using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Globalization;

namespace logist_app.ViewModels;

public partial class AppSettingsViewModel : ObservableObject
{
    // === 1. ТЕМА ОФОРМЛЕНИЯ ===

    [ObservableProperty]
    private int selectedThemeIndex; // 0=Auto, 1=Light, 2=Dark

    public AppSettingsViewModel()
    {
        LoadSavedTheme();
     //   LoadSavedLanguage();
    }

    // Команда, которую вызывают плитки в новом GUI
    [RelayCommand]
    private void SetTheme(string themeName)
    {
        SelectedThemeIndex = themeName switch
        {
            "Light" => 1,
            "Dark" => 2,
            _ => 0 // System
        };
        // Изменение индекса автоматически вызовет OnSelectedThemeIndexChanged
    }

    private void LoadSavedTheme()
    {
        var savedTheme = Preferences.Default.Get("app_theme", "System");
        SelectedThemeIndex = savedTheme switch
        {
            "Light" => 1,
            "Dark" => 2,
            _ => 0
        };
    }

    // Триггерится автоматически при изменении SelectedThemeIndex
    partial void OnSelectedThemeIndexChanged(int value)
    {
        var themeKey = value switch
        {
            1 => "Light",
            2 => "Dark",
            _ => "System"
        };

        Preferences.Default.Set("app_theme", themeKey);
        ApplyTheme(themeKey);
    }

    public static void ApplyTheme(string themeKey)
    {
        if (Application.Current == null) return;

        Application.Current.UserAppTheme = themeKey switch
        {
            "Light" => AppTheme.Light,
            "Dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };

        // Обновляем цвет системной полоски (для Windows)
       // logist_app.App.UpdateTitleBarColor();
    }


    // === 2. ЯЗЫКОВЫЕ НАСТРОЙКИ ===

    public class LanguageModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public ObservableCollection<LanguageModel> SupportedLanguages { get; } = new()
    {
        new LanguageModel { Name = "🇬🇧 English", Code = "en" },
        new LanguageModel { Name = "🇷🇺 Русский", Code = "ru" }
    };

    [ObservableProperty]
    private LanguageModel _selectedLanguage;

    //private void LoadSavedLanguage()
    //{
    //    // Получаем сохраненный код или текущий системный
    //    var currentCode = Preferences.Get("app_lang", LocalizationResourceManager.Current.CurrentCulture.TwoLetterISOLanguageName);

    //    // Находим объект в списке
    //    SelectedLanguage = SupportedLanguages.FirstOrDefault(l => l.Code == currentCode)
    //                       ?? SupportedLanguages.FirstOrDefault(l => l.Code == "en");
    //}

    //partial void OnSelectedLanguageChanged(LanguageModel value)
    //{
    //    if (value == null) return;

    //    var culture = new CultureInfo(value.Code);

    //    // Обновляем менеджер ресурсов
    //    LocalizationResourceManager.Current.SetCulture(culture);

    //    // Обновляем потоки
    //    Thread.CurrentThread.CurrentCulture = culture;
    //    Thread.CurrentThread.CurrentUICulture = culture;

    //    // Сохраняем выбор
    //    Preferences.Set("app_lang", value.Code);
    //}


    // === 3. ПРОЧЕЕ ===

    [RelayCommand]
    private async Task Logout()
    {
        SecureStorage.Default.Remove("auth_token");
        await Shell.Current.GoToAsync("//LoginPageView");
    }
}