#nullable enable
using System;
using System.Windows.Input;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.Services.Dialogs;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.Commands.Editor
{
    public class OpenResolutionsDialogCommand : ICommand
    {
        // Log
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        
        private readonly EditorViewModel _editorViewModel;
        private readonly IDialogService _dialogService;
        private readonly ILoadSettingsService _loadSettingsService;
        private readonly ISaveSettingsService _saveSettingsService;

        public OpenResolutionsDialogCommand(
            EditorViewModel editorViewModel,
            IDialogService dialogService,
            ILoadSettingsService loadSettingsService,
            ISaveSettingsService saveSettingsService)
        {
            _logger.Info("Constructor");
            _editorViewModel = editorViewModel;
            _dialogService = dialogService;
            _loadSettingsService = loadSettingsService;
            _saveSettingsService = saveSettingsService;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        /// <summary>
        /// Opens the resolution selection dialog and updates the GUI with the corresponding values.
        /// </summary>
        /// <param name="parameter">Unused.</param>
        public void Execute(object? parameter)
        {
            _logger.Info("Open resolution dialog selector");

            var res = _dialogService.OpenResolutionDialog(_loadSettingsService, _saveSettingsService).Result;

            _logger.Info($"Resolution: {res}");

            if (res == null) return;

            _editorViewModel.Height = res.Height;
            _editorViewModel.Width = res.Width;
        }

        public event EventHandler? CanExecuteChanged;
    }
}