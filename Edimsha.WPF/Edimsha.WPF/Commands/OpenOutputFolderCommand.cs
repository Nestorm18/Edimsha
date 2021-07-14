#nullable enable
using System;
using System.Windows.Input;
using Edimsha.Core.Language;
using Edimsha.WPF.Services.Dialogs;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.Commands
{
    public class OpenOutputFolderCommand : ICommand
    {
        // Log
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        
        private readonly ViewModelBase _viewModel;
        private readonly IDialogService _dialogService;

        public OpenOutputFolderCommand(ViewModelBase viewModel, IDialogService dialogService)
        {
            _logger.Info("Constructor");
            _viewModel = viewModel;
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
            var success = _dialogService.OpenFolderSelector(TranslationSource.GetTranslationFromString("select_folder"));

            if (success.Result == null) return;
            
            _viewModel.OutputFolder = success.Result;
        }

        public event EventHandler? CanExecuteChanged;
    }
}