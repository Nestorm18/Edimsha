#nullable enable
using System;
using System.Windows.Input;
using Edimsha.WPF.Lang;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.Commands
{
    public class ChangeLanguageCommand : ICommand
    {
        private readonly MainViewModel _viewModel;
        private readonly ISaveSettingsService _saveSettingsService;

        public ChangeLanguageCommand(MainViewModel viewModel, ISaveSettingsService saveSettingsService)
        {
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

            switch (_viewModel.Language)
            {
                case Languages.English:
                    ChangeLanguage.SetLanguage("");
                    _saveSettingsService.SaveConfigurationSettings("Language", Languages.English.GetDescription());
                    break;
                case Languages.Spanish:
                    ChangeLanguage.SetLanguage(Languages.Spanish.GetDescription());
                    _saveSettingsService.SaveConfigurationSettings("Language", Languages.Spanish.GetDescription());
                    break;
                default:
                    throw new Exception("El idioma indicado no existe");
            }
        }

        public event EventHandler? CanExecuteChanged;
    }
}