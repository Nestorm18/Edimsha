#nullable enable
using System;
using System.Windows.Input;
using Edimsha.WPF.Lang;
using Edimsha.WPF.Models;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.ViewModels.DialogsViewModel;

namespace Edimsha.WPF.Commands.Dialogs
{
    public class RemoveResolutionCommand : ICommand
    {
        private readonly ResolutionDialogViewModel _resolutionDialogViewModel;
        private readonly ISaveSettingsService _saveSettingsService;

        public RemoveResolutionCommand(
            ResolutionDialogViewModel resolutionDialogViewModel,
            ISaveSettingsService saveSettingsService)
        {
            _resolutionDialogViewModel = resolutionDialogViewModel;
            _saveSettingsService = saveSettingsService;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            var values = (object[]) parameter!;
            var width = (string) values[0];
            var height = (string) values[1];

            if (width == string.Empty || height == string.Empty) return;

            var currentResolution = new Resolution()
            {
                Width = int.Parse(width),
                Height = int.Parse(height)
            };
            
            for (var i = 0; i < _resolutionDialogViewModel.Resolutions.Count; i++)
            {
                if (!_resolutionDialogViewModel.Resolutions[i].Equals(currentResolution)) continue;
                _resolutionDialogViewModel.Resolutions.RemoveAt(i);
                break;
            }
            
            var ts = TranslationSource.Instance;

            _resolutionDialogViewModel.ErrorMessage = ts["deleted_resolution"];
            _resolutionDialogViewModel.CmbIndex = 0;
            
            _saveSettingsService.SaveResolutions(_resolutionDialogViewModel.Resolutions);
        }

        public event EventHandler? CanExecuteChanged;
    }
}