using System.Collections.Generic;
using System.Threading.Tasks;
using Edimsha.WPF.Models;
using Edimsha.WPF.State.Navigators;

namespace Edimsha.WPF.Services.Data
{
    public interface ISaveSettingsService
    {
        Task<bool> SaveConfigurationSettings<T>(ViewType type, string settingName, T value);

        bool SavePaths(IEnumerable<string> values, ViewType viewType);

        void SaveResolutions(IEnumerable<Resolution> resolutions);
    }
}