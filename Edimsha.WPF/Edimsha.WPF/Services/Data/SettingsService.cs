using Edimsha.Core.Logging.Implementation;
using Edimsha.WPF.Settings;

namespace Edimsha.WPF.Services.Data
{
    public class SettingsService
    {
        protected readonly string SettingsEditor;
        protected readonly string SettingsConversor;
        protected readonly string EditorPathsJson;
        protected readonly string ConversorPathsJson;
        protected readonly string ResolutionsJson;

        protected SettingsService(ConfigPaths settingsPath)
        {
            Logger.Log($"SettingsPath : {settingsPath}");
            SettingsEditor = settingsPath.SettingsEditor;
            SettingsConversor = settingsPath.SettingsConversor;
            EditorPathsJson = settingsPath.EditorPathsJson;
            ConversorPathsJson = settingsPath.ConversorPathsJson;
            ResolutionsJson = settingsPath.ResolutionsJson;
        }
    }
}