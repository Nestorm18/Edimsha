using Edimsha.Core.Settings;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.Services.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Edimsha.WPF.HostBuild
{
    public static class AddServicesHostBuilderExtensions
    {
        public static IHostBuilder AddServices(this IHostBuilder host)
        {
            host.ConfigureServices((context, services) =>
            {
                services.Configure<ConfigPaths>(context.Configuration.GetSection(nameof(ConfigPaths)));
                services.AddSingleton<ISaveSettingsService, SaveSettingsService>();
                services.AddSingleton<ILoadSettingsService, LoadSettingsService>();
                services.AddSingleton<IDialogService, DialogService>();
            });
            return host;
        }
    }
}