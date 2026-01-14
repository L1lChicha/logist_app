using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace logist_app.ViewModels;

public partial class AppSettingsViewModel : ObservableObject
{
    public ObservableCollection<string> ThemeOptions { get; } = new()
    {
        "Системная",
        "Светлая",
        "Тёмная"
    };

    [ObservableProperty]
    private int selectedThemeIndex;

    [ObservableProperty]
    private bool isDarkTheme;

    public AppSettingsViewModel()
    {
        LoadSavedTheme();
    }

    private void LoadSavedTheme()
    {
        var savedTheme = Preferences.Default.Get("app_theme", "System");
        
        SelectedThemeIndex = savedTheme switch
        {
            "Light" => 1,
            "Dark" => 2,
            _ => 0 // System
        };

        IsDarkTheme = savedTheme == "Dark" || 
            (savedTheme == "System" && Application.Current?.RequestedTheme == AppTheme.Dark);
    }

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

        IsDarkTheme = themeKey == "Dark" || 
            (themeKey == "System" && Application.Current?.RequestedTheme == AppTheme.Dark);
    }

    public static void ApplyTheme(string themeKey)
    {
        if (Application.Current == null) return;

        Application.Current.UserAppTheme = themeKey switch
        {
            "Light" => AppTheme.Light,
            "Dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified // System default
        };
    }

    [RelayCommand]
    private async Task Logout()
    {
        SecureStorage.Default.Remove("auth_token");
        await Shell.Current.GoToAsync("//LoginPageView");
    }
}

