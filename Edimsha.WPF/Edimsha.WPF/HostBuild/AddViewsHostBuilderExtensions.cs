using Edimsha.Core.Logging.Implementation;
using Edimsha.WPF.ViewModels;
using Edimsha.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Edimsha.WPF.HostBuild
{
    public static class AddViewsHostBuilderExtensions
    {
        public static IHostBuilder AddViews(this IHostBuilder host)
        { 
            Logger.Log("AddViews");
            host.ConfigureServices(services =>
            {
                services.AddSingleton(s => new MainWindow(s.GetRequiredService<MainViewModel>()));
            });

            return host;
        }
    }
}