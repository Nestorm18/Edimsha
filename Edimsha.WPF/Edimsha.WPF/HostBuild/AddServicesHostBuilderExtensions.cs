using Edimsha.WPF.Services.Data;
using Microsoft.Extensions.Configuration;
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
                var connectionString = context.Configuration.GetValue<string>("SETTINGS_FILE");
                services.AddSingleton<ISaveSettingsService>( new SaveSettingsService(connectionString));
            });
            return host;
        }
    }
}