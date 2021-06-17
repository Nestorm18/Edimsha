#nullable enable
using System;
using System.Windows.Input;
using Edimsha.Core.Language;
using Edimsha.Core.Models;
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

        /// <summary>
        /// Removes the selected resolution from the combobox and from the save file.
        /// </summary>
        /// <param name="parameter"><see cref="Resolution"/> to remove.</param>
        public void Execute(object? parameter)
        {
            var values = (object[]) parameter!;
            var width = (string) values[0];
            var height = (string) values[1];

            // Not valid values
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

            _resolutionDialogViewModel.ErrorMessage = TranslationSource.GetTranslationFromString("deleted_resolution");
            _resolutionDialogViewModel.CmbIndex = 0;

            _saveSettingsService.SaveResolutions(_resolutionDialogViewModel.Resolutions);

            AllResolutionsDeleted();
        }

        /// <summary>
        /// Resets fields to default and disabled values as needed
        /// </summary>
        private void AllResolutionsDeleted()
        {
            if (_resolutionDialogViewModel.Resolutions.Count != 0) return;

            // Resets fields to basic
            _resolutionDialogViewModel.BypassWidthOrHeightLimitations = true;
            _resolutionDialogViewModel.Width = 0;
            _resolutionDialogViewModel.Heigth = 0;
            _resolutionDialogViewModel.HasValidResolutions = false;
            _resolutionDialogViewModel.BypassWidthOrHeightLimitations = false;
        }

        public event EventHandler? CanExecuteChanged;
    }
}