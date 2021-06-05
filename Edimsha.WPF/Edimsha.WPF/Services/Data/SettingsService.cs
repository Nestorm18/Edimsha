using Edimsha.Core.Logging.Implementation;
using Edimsha.WPF.Settings;

namespace Edimsha.WPF.Services.Data
{
    public class SettingsService
    {
        protected readonly string SettingsPath;
        protected readonly string EditorPathsJson;
        protected readonly string ConversorPathsJson;
        protected readonly string ResolutionsJson;

        protected SettingsService(ConfigPaths settingsPath)
        {
            Logger.Log($"SettingsPath : {settingsPath}");
            SettingsPath = settingsPath.SettingsFile;
            EditorPathsJson = settingsPath.EditorPathsJson;
            ConversorPathsJson = settingsPath.ConversorPathsJson;
            ResolutionsJson = settingsPath.ResolutionsJson;
        }
    }
}