namespace Edimsha.WPF.Services.Data
{
    public interface ISaveSettingsService
    {
        void SaveConfigurationSettings<T>(string settingNam, T value);
    }
}