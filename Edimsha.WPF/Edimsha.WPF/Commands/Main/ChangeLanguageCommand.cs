#nullable enable
using System;
using System.Windows.Input;
using Edimsha.Core.Language;
using Edimsha.Core.Logging.Core;
using Edimsha.Core.Logging.Implementation;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.State.Navigators;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.Commands.Main
{
    public class ChangeLanguageCommand : ICommand
    {
        private readonly MainViewModel _viewModel;
        private readonly ISaveSettingsService _saveSettingsService;

        public ChangeLanguageCommand(MainViewModel viewModel, ISaveSettingsService saveSettingsService)
        {
            Logger.Log("Constructor");
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

            Logger.Log($"Changing language to {_viewModel.Language}", LogLevel.Debug);
            
            ChangeLanguage.SetLanguage(_viewModel.Language.GetDescription());
            _saveSettingsService.SaveConfigurationSettings(ViewType.Editor, "Language", _viewModel.Language.GetDescription());
            _saveSettingsService.SaveConfigurationSettings(ViewType.Conversor, "Language", _viewModel.Language.GetDescription());
        }

        public event EventHandler? CanExecuteChanged;
    }
}