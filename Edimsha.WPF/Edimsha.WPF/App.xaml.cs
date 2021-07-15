using System;
using System.Windows;
using Edimsha.WPF.HostBuild;
using Edimsha.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Edimsha.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        // Log
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
       
        private static IHost _host;

        public App()
        {
            try
            {
                _logger.Info("Build starts");
                _host = CreateHostBuilder().Build();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Stopped program because of exception");
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args = null)
        {
            return Host.CreateDefaultBuilder(args)
                .AddConfiguration()
                .AddServices()
                .AddViewModels()
                .AddViews();
        }

        private static T GetRequiredServiceFromHost<T>()
        {
            return _host.Services.GetRequiredService<T>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                _logger.Info("App starts");
                _host.Start();

                Window window = GetRequiredServiceFromHost<MainWindow>();
                window.Show();

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace, "Stopped program because of exception");
                throw;
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            try
            {
                _logger.Info("Closing app...");
                await _host.StopAsync();
                _host.Dispose();

                base.OnExit(e);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace, "Stopped program because of exception");
                throw;
            }
        }
    }
}