using Edimsha.WPF.Settings;

namespace Edimsha.WPF.Services.Data
{
    public class SettingsService
    {
        protected readonly string SettingsPath;
        protected readonly string EditorPathsJson;
        protected readonly string ConversorPathsJson;

        protected SettingsService(ConfigPaths settingsPath)
        {
            SettingsPath = settingsPath.SettingsFile;
            EditorPathsJson = settingsPath.EditorPathsJson;
            ConversorPathsJson = settingsPath.ConversorPathsJson;
        }
    }
}