
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Edimsha.WPF.HostBuild
{
    public static class AddConfigurationHostBuilderExtensions
    {
        public static IHostBuilder AddConfiguration(this IHostBuilder host)
        {
            host.ConfigureAppConfiguration(c =>
            {
                c.AddJsonFile("appsettings.json");
                c.AddJsonFile("ConfigPaths.json");
                c.AddJsonFile("Resources/EditorOptions.json");
                c.AddJsonFile("Resources/ConversorOptions.json");
                c.AddEnvironmentVariables();
            });

            return host;
        }
    }
}