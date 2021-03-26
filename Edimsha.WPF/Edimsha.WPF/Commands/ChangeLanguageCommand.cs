#nullable enable
using System;
using System.Windows.Input;
using Edimsha.WPF.Lang;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.Commands
{
    public class ChangeLanguageCommand : ICommand
    {
        private readonly MainViewModel _viewModel;

        public ChangeLanguageCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter != null) _viewModel.Language = (Languages) parameter;

            switch (_viewModel.Language)
            {
                case Languages.English:
                    ChangeLanguage.SetLanguage("");
                    break;
                case Languages.Spanish:
                    ChangeLanguage.SetLanguage("es_ES");
                    break;
                default:
                    throw new Exception("El idioma indicado no existe");
            }
        }

        public event EventHandler? CanExecuteChanged;
    }
}