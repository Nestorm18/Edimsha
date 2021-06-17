using Edimsha.Core.Logging.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Edimsha.WPF.HostBuild
{
    public static class AddConfigurationHostBuilderExtensions
    {
        public static IHostBuilder AddConfiguration(this IHostBuilder host)
        {
            Logger.Log("AddConfiguration");
            host.ConfigureAppConfiguration(c =>
            {
                c.AddJsonFile("appsettings.json");
                c.AddEnvironmentVariables();
            });

            return host;
        }
    }
}