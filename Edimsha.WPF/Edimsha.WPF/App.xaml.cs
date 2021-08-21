using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Edimsha.Core.Language;
using Edimsha.Core.Models;
using Edimsha.WPF.HostBuild;
using Edimsha.WPF.Views;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

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

                CheckResourceFiles();

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

        private void CheckResourceFiles()
        {
            const string editorPath = "Resources/EditorOptions.json";
            const string conversorPath = "Resources/ConversorOptions.json";

            if (!File.Exists(editorPath)) CreateEditorOptionsFile(editorPath);
            if (!File.Exists(conversorPath)) CreateConversorOptionsFile(conversorPath);
        }
        
        private static void CreateEditorOptionsFile(string editorPath)
        {
            var options = new EditorOptions
            {
                Language = Languages.English.GetDescription(),
                CleanListOnExit = false,
                IterateSubdirectories = false,
                OutputFolder = "",
                Edimsha = "",
                AlwaysIncludeOnReplace = false,
                KeepOriginalResolution = false,
                CompresionValue = 0.0,
                OptimizeImage = false,
                ReplaceForOriginal = false,
                Resolution = new Resolution(1920, 1080),
                Resolutions = new List<Resolution>
                {
                    new(1920, 1080)
                },
                Paths = new List<string>()
            };
           
            File.WriteAllTextAsync(editorPath, JsonConvert.SerializeObject(options, Formatting.Indented));
            
        }
        
        private static void CreateConversorOptionsFile(string conversorPath)
        {
            var options = new ConversorOptions()
            {
                Language = Languages.English.GetDescription(),
                CleanListOnExit = false,
                IterateSubdirectories = false,
                CurrentIndex = 1,
                OutputFolder = "",
                Edimsha = "",
                Paths = new List<string>()
            };
            
            File.WriteAllTextAsync(conversorPath, JsonConvert.SerializeObject(options, Formatting.Indented));
        }
    }
}