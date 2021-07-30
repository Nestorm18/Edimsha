#nullable enable
using System;
using System.Windows.Input;
using Edimsha.Core.Language;
using Edimsha.WPF.Services.Dialogs;
using Edimsha.WPF.ViewModels.Contracts;

namespace Edimsha.WPF.Commands
{
    public class OpenOutputFolderCommand<T> : ICommand
    {
        // Log
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        private readonly T _viewModel;
        private readonly IDialogService _dialogService;
        
        public OpenOutputFolderCommand(T viewModel, IDialogService dialogService)
        {
            Logger.Info("Constructor");
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
            
            if (_viewModel is IExtraProperties viewModel) viewModel.OutputFolder = success.Result;
        }

        public event EventHandler? CanExecuteChanged;
    }
}