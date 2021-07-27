using System.Collections.Generic;
using Edimsha.Core.Models;
using Edimsha.Core.Settings;
using Edimsha.WPF.State.Navigators;

namespace Edimsha.WPF.Services.Data
{
    public interface ILoadSettingsService
    {
        T LoadConfigurationSetting<T, C>(string settingName, string filePath);

        IEnumerable<string> GetSavedPaths(ViewType type);

        IEnumerable<Resolution> LoadResolutions();

        ConfigEditor GetConfigFormViewType(ViewType type);

        bool StillPathsSameFromLastSession(ViewType type);

        IEnumerable<string> GetPathChanges(ViewType type);
        
        //TODO: Cargar lista de opciones ben feito!
    }
}