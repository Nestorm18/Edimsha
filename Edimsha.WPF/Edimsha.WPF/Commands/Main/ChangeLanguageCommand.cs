#nullable enable
using System;
using System.Windows.Input;
using Edimsha.Core.Language;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.State.Navigators;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.Commands.Main
{
    public class ChangeLanguageCommand : ICommand
    {
        private readonly MainViewModel _viewModel;
        private readonly ISaveSettingsService _saveSettingsService;

        // Log
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        
        public ChangeLanguageCommand(MainViewModel viewModel, ISaveSettingsService saveSettingsService)
        {
            _logger.Info("Constructor");
            _viewModel = viewModel;
            _saveSettingsService = saveSettingsService;
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

            _logger.Info($"Changing language to {_viewModel.Language}");
            
            ChangeLanguage.SetLanguage(_viewModel.Language.GetDescription());
            _saveSettingsService.SaveConfigurationSettings(ViewType.Editor, "Language", _viewModel.Language.GetDescription());
            _saveSettingsService.SaveConfigurationSettings(ViewType.Conversor, "Language", _viewModel.Language.GetDescription());
        }

        public event EventHandler? CanExecuteChanged;
    }
}