#nullable enable
using System;
using System.Windows.Input;
using Edimsha.WPF.Lang;
using Edimsha.WPF.Services.Dialogs;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.Commands
{
    public class OpenOutputFolderCommand : ICommand
    {
        private readonly EditorViewModel _editorViewModel;
        private readonly IDialogService _dialogService;

        public OpenOutputFolderCommand(EditorViewModel editorViewModel, IDialogService dialogService)
        {
            _editorViewModel = editorViewModel;
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

            _editorViewModel.OutputFolder = success.Result;
        }

        public event EventHandler? CanExecuteChanged;
    }
}