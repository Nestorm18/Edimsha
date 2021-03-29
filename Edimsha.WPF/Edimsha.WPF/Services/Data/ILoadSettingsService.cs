using System.Collections.Generic;

namespace Edimsha.WPF.Services.Data
{
    public interface ILoadSettingsService
    {
        T LoadConfigurationSetting<T>(string settingName);

        List<string> LoadPathsListview();
    }
}