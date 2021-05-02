#nullable enable
using System;
using System.Linq;
using System.Windows.Input;
using Edimsha.WPF.Lang;
using Edimsha.WPF.Models;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.ViewModels.DialogsViewModel;

namespace Edimsha.WPF.Commands.Dialogs
{
    public class SaveResolutionCommand : ICommand
    {
        private readonly ResolutionDialogViewModel _resolutionDialogViewModel;
        private readonly ISaveSettingsService _saveSettingsService;

        public SaveResolutionCommand(
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
            var ts = TranslationSource.Instance;
            
            var values = (object[]) parameter!;
            var width = (string) values[0];
            var height = (string) values[1];

            if (width == string.Empty || height == string.Empty) return;

            var currentResolution = new Resolution()
            {
                Width = int.Parse(width),
                Height = int.Parse(height)
            };
            
            if (ExistCurrentResolution(currentResolution))
            {
                _resolutionDialogViewModel.ErrorMessage = ts["the_resolution_already_exists"];
            }
            else
            {
                _resolutionDialogViewModel.Resolutions.Add(currentResolution);
                
                _saveSettingsService.SaveResolutions(_resolutionDialogViewModel.Resolutions);
               
                _resolutionDialogViewModel.ErrorMessage = ts["resolution_saved"];
                _resolutionDialogViewModel.CmbIndex = _resolutionDialogViewModel.Resolutions.Count - 1;
            }
        }

        private bool ExistCurrentResolution(Resolution currentResolution)
        {
            return _resolutionDialogViewModel.Resolutions.Any(resolution => resolution.Equals(currentResolution));
        }

        public event EventHandler? CanExecuteChanged;
    }
}