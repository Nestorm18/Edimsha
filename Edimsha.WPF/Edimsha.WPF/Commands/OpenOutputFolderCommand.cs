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
        private readonly TranslationSource _ts;

        public OpenOutputFolderCommand(EditorViewModel editorViewModel, IDialogService dialogService)
        {
            _editorViewModel = editorViewModel;
            _dialogService = dialogService;

            _ts = TranslationSource.Instance;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            var success = _dialogService.OpenFolderSelector(_ts["select_folder"]);

            if (success.Result == null) return;
            
            _editorViewModel.OutputFolder = success.Result;
        }

        public event EventHandler? CanExecuteChanged;
    }
}