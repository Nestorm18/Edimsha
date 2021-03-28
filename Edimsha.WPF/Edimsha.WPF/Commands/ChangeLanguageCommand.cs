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

        public void Execute(object? parameter)
        {
            if (parameter != null) _viewModel.Language = (Languages)parameter;

            switch (_viewModel.Language)
            {
                case Languages.English:
                    ChangeLanguage.SetLanguage("");
                    _saveSettingsService.SaveConfigurationSettings("Language", "En_en");
                    break;
                case Languages.Spanish:
                    ChangeLanguage.SetLanguage("Es_es");
                    _saveSettingsService.SaveConfigurationSettings("Language", "Es_es");
                    break;
                default:
                    throw new Exception("El idioma indicado no existe");
            }
        }

        public event EventHandler? CanExecuteChanged;
    }
}
