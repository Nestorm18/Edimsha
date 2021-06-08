using System.Collections.Generic;
using Edimsha.WPF.Models;
using Edimsha.WPF.Settings;
using Edimsha.WPF.State.Navigators;

namespace Edimsha.WPF.Services.Data
{
    public interface ILoadSettingsService
    {
        T LoadConfigurationSetting<T>(ViewType type,string settingName);

        List<string> LoadPathsListview(ViewType type);
        
        IEnumerable<Resolution> LoadResolutions();

        Config GetConfigFormViewType(ViewType type);
    }
}