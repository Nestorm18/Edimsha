using System;
using System.Windows;
using Edimsha.Core.Logging.Implementation;
using Edimsha.WPF.HostBuild;
using Edimsha.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Edimsha.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IHost _host;

        public App()
        {
            try
            {
                Logger.Log("App starts");
                _host = CreateHostBuilder().Build();
            }
            catch (Exception e)
            {
                Logger.Log(e.StackTrace);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args = null)
        {
            return Host.CreateDefaultBuilder(args)
                .AddConfiguration()
                .AddServices()
                .AddViewModels()
                .AddViews();
        }

        public static T GetRequiredServiceFromHost<T>()
        {
            return _host.Services.GetRequiredService<T>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                _host.Start();

                Window window = _host.Services.GetRequiredService<MainWindow>();
                window.Show();

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.StackTrace);
                throw;
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            try
            {
                await _host.StopAsync();
                _host.Dispose();

                base.OnExit(e);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.StackTrace);
                throw;
            }
        }
    }
}