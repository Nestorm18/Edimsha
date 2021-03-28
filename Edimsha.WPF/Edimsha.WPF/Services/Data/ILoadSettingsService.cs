using System.Collections.ObjectModel;

namespace Edimsha.WPF.Services.Data
{
    public interface ILoadSettingsService
    {
        T LoadConfigurationSetting<T>(string settingName);

        ObservableCollection<string> LoadPathsListview(ObservableCollection<string> paths);
    }
}