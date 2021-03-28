using Edimsha.WPF.Services.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Edimsha.WPF.HostBuild
{
    public static class AddServicesHostBuilderExtensions
    {
        public static IHostBuilder AddServices(this IHostBuilder host)
        {
            host.ConfigureServices(services => { services.AddSingleton<ISaveSettingsService, SaveSettingsService>(); });
            return host;
        }
    }
}