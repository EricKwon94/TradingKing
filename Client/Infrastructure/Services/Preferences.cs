using Application.Services;

namespace Infrastructure.Services;

internal class Preferences : IPreferences
{
    public void Clear(string? sharedName = null)
    {
        Microsoft.Maui.Storage.Preferences.Default.Clear(sharedName);
    }

    public bool ContainsKey(string key, string? sharedName = null)
    {
        return Microsoft.Maui.Storage.Preferences.Default.ContainsKey(key, sharedName);
    }

    public T Get<T>(string key, T defaultValue, string? sharedName = null)
    {
        return Microsoft.Maui.Storage.Preferences.Default.Get(key, defaultValue, sharedName);
    }

    public void Remove(string key, string? sharedName = null)
    {
        Microsoft.Maui.Storage.Preferences.Default.Remove(key, sharedName);
    }

    public void Set<T>(string key, T value, string? sharedName = null)
    {
        Microsoft.Maui.Storage.Preferences.Default.Set(key, value, sharedName);
    }
}
