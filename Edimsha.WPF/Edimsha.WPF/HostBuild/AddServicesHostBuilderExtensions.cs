using Edimsha.Core.Logging.Implementation;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.Services.Dialogs;
using Edimsha.WPF.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Edimsha.WPF.HostBuild
{
    public static class AddServicesHostBuilderExtensions
    {
        public static IHostBuilder AddServices(this IHostBuilder host)
        {
            Logger.Log("AddViews");
            host.ConfigureServices((context, services) =>
            {
                var config = new ConfigPaths
                {
                    SettingsFile = context.Configuration.GetValue<string>("SETTINGS_FILE"),
                    EditorPathsJson = context.Configuration.GetValue<string>("EDITOR_PATHS_JSON"),
                    ConversorPathsJson = context.Configuration.GetValue<string>("CONVERSOR_PATHS_JSON"),
                    ResolutionsJson = context.Configuration.GetValue<string>("RESOLUTIONS_JSON")
                };
                services.AddSingleton<ISaveSettingsService>(new SaveSettingsService(config));
                services.AddSingleton<ILoadSettingsService>(new LoadSettingsService(config));
                services.AddSingleton<IDialogService, DialogService>();
            });
            return host;
        }
    }
}