#nullable enable
using System;
using System.Windows.Input;
using Edimsha.Core.Language;
using Edimsha.Core.Logging.Implementation;
using Edimsha.WPF.Services.Dialogs;
using Edimsha.WPF.State.Navigators;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.Commands
{
    public class OpenOutputFolderCommand : ICommand
    {
        private readonly ViewModelBase _viewModel;
        private readonly ViewType _type;
        private readonly IDialogService _dialogService;

        public OpenOutputFolderCommand(ViewModelBase viewModel, ViewType type, IDialogService dialogService)
        {
            Logger.Log("Constructor");
            _viewModel = viewModel;
            _type = type;
            _dialogService = dialogService;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        /// <summary>
        /// Select the folder where to save the images
        /// </summary>
        /// <param name="parameter">Unused.</param>
        public void Execute(object? parameter)
        {
            var success =
                _dialogService.OpenFolderSelector(TranslationSource.GetTranslationFromString("select_folder"));

            if (success.Result == null) return;

            switch (_type)
            {
                case ViewType.Editor:
                {
                    if (_viewModel is EditorViewModel viewModel) viewModel.OutputFolder = success.Result;
                    break;
                }
                case ViewType.Conversor:
                {
                    if (_viewModel is ConversorViewModel viewModel) viewModel.OutputFolder = success.Result;
                    break;
                }
            }
        }

        public event EventHandler? CanExecuteChanged;
    }
}