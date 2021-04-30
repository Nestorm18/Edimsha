#nullable enable
using System;
using System.Windows.Input;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.Services.Dialogs;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.Commands
{
    public class OpenResolutionsDialogCommand : ICommand
    {
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
            _editorViewModel = editorViewModel;
            _dialogService = dialogService;
            _loadSettingsService = loadSettingsService;
            _saveSettingsService = saveSettingsService;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            var res = _dialogService.OpenResolutionDialog(_loadSettingsService, _saveSettingsService);

            if (res == null) return;

            _editorViewModel.HeightImage = res.Result.Height;
            _editorViewModel.WidthImage = res.Result.Width;
        }

        public event EventHandler? CanExecuteChanged;
    }
}