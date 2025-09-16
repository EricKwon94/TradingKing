using Application.Services;

namespace Infrastructure.Services;

internal class Preferences : IPreferences
{
    private readonly Microsoft.Maui.Storage.IPreferences _preferences = Microsoft.Maui.Storage.Preferences.Default;

    public void Clear(string? sharedName = null)
    {
        _preferences.Clear(sharedName);
    }

    public bool ContainsKey(string key, string? sharedName = null)
    {
        return _preferences.ContainsKey(key, sharedName);
    }

    public T Get<T>(string key, T defaultValue, string? sharedName = null)
    {
        return _preferences.Get(key, defaultValue, sharedName);
    }

    public void Remove(string key, string? sharedName = null)
    {
        _preferences.Remove(key, sharedName);
    }

    public void Set<T>(string key, T value, string? sharedName = null)
    {
        _preferences.Set(key, value, sharedName);
    }
}
