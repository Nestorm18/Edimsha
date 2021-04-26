using System.Collections.Generic;
using System.Threading.Tasks;
using Edimsha.WPF.Models;
using Edimsha.WPF.State.Navigators;

namespace Edimsha.WPF.Services.Data
{
    public interface ISaveSettingsService
    {
        Task<bool> SaveConfigurationSettings<T>(string settingName, T value);

        Task<bool> SavePathsListview(IEnumerable<string> values, ViewType viewType);

        Task<bool> SaveResolutions(IEnumerable<Resolution> resolutions);
    }
}