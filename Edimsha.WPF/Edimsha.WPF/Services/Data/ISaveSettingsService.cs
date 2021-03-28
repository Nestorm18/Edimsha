using System.Threading.Tasks;

namespace Edimsha.WPF.Services.Data
{
    public interface ISaveSettingsService
    {
        Task<bool> SaveConfigurationSettings<T>(string settingName, T value);
    }
}