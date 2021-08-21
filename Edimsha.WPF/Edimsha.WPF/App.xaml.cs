﻿using System;
using System.Windows;
using Edimsha.WPF.HostBuild;
using Edimsha.WPF.Views;
using Microsoft.Extensions.Hosting;

namespace Edimsha.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        // Log
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static IHost _host;

        public T GetService<T>() where T : class => _host.Services.GetService(typeof(T)) as T;

        public App()
        {
            try
            {
                Logger.Info("Build starts");
                _host = CreateHostBuilder().Build();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Stopped program because of exception");
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

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                Logger.Info("App starts");
                _host.Start();

                Window window = GetService<MainWindow>();
                window.Show();

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace, "Stopped program because of exception");
                throw;
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            try
            {
                Logger.Info("Closing app...");
                await _host.StopAsync();
                _host.Dispose();

                base.OnExit(e);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace, "Stopped program because of exception");
                throw;
            }
        }
    }
}