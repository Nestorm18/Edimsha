
using Edimsha.WPF.ViewModels;
using Edimsha.WPF.ViewModels.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Edimsha.WPF.HostBuild
{
    public static class AddViewModelsHostBuilderExtensions
    {
        public static IHostBuilder AddViewModels(this IHostBuilder host)
        {
            host.ConfigureServices(services =>
            {
                services.AddTransient<MainViewModel>();
                services.AddTransient<EditorViewModel>();
                services.AddTransient<ConversorViewModel>();

                services.AddSingleton<CreateViewModel<EditorViewModel>>(s => s.GetRequiredService<EditorViewModel>);
                services.AddSingleton<CreateViewModel<ConversorViewModel>>(s => s.GetRequiredService<ConversorViewModel>);

                services.AddSingleton<IEdimshaViewModelFactory, EdimshaViewModelFactory>();
            });

            return host;
        }
    }
}