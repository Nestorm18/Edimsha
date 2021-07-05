using System;
using System.Windows;
using Edimsha.Core.Logging.Core;
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
    public partial class App
    {
        private static IHost _host;

        public App()
        {
            try
            {
                Logger.Setup(LoggerMode.Normal);
                _host = CreateHostBuilder().Build();
            }
            catch (Exception e)
            {
                Logger.Log(e.StackTrace, LogLevel.Error);
                Logger.Close();
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
                Logger.Log("App OnStartup");
                _host.Start();

                Window window = GetRequiredServiceFromHost<MainWindow>();
                window.Show();

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.StackTrace, LogLevel.Error);
                Logger.Close();
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
                Logger.Log("Closing app...");
            }
            catch (Exception ex)
            {
                Logger.Log(ex.StackTrace, LogLevel.Error);
                throw;
            }
            finally
            {
                Logger.Close();
            }
        }
    }
}