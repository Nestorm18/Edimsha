#nullable enable
using System;
using System.Windows.Input;
using Edimsha.Core.Language;
using Edimsha.Core.Models;
using Edimsha.Core.Settings;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.ViewModels;
using Microsoft.Extensions.Options;

namespace Edimsha.WPF.Commands.Main
{
    public class ChangeLanguageCommand : ICommand
    {
        private readonly MainViewModel _viewModel;
        private readonly ISaveSettingsService _saveSettingsService;
        private readonly IOptions<ConfigPaths> _options;

        // Log
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public ChangeLanguageCommand(MainViewModel viewModel, ISaveSettingsService saveSettingsService, IOptions<ConfigPaths> options)
        {
            Logger.Info("Constructor");
            _viewModel = viewModel;
            _saveSettingsService = saveSettingsService;
            _options = options;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        /// <summary>
        /// Switches to the language passed by parameter and saves the selection in a configuration file.
        /// </summary>
        /// <param name="parameter">Selected language.</param>
        /// <exception cref="Exception">Language not found.</exception>
        public void Execute(object? parameter)
        {
            if (parameter != null) _viewModel.Language = (Languages) parameter;

            Logger.Info($"Changing language to {_viewModel.Language}");

            ChangeLanguage.SetLanguage(_viewModel.Language.GetDescription());

            _saveSettingsService.SaveConfigurationSettings<string,
                EditorConfig>("Language",
                _viewModel.Language.GetDescription(),
                _options.Value.EditorConfig);

            _saveSettingsService.SaveConfigurationSettings<string,
                ConversorConfig>("Language",
                _viewModel.Language.GetDescription(),
                _options.Value.ConversorConfig);
        }

        public event EventHandler? CanExecuteChanged;
    }
}