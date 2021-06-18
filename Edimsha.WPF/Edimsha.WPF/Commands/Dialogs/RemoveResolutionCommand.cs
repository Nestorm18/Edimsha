#nullable enable
using System;
using System.Windows.Input;
using Edimsha.Core.Language;
using Edimsha.Core.Logging.Implementation;
using Edimsha.Core.Models;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.Utils;
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
            Logger.Log("Deleting resolution");

            var currentResolution = ResolutionValidator.IsParameterValid(parameter);

            if (currentResolution == null) return;

            RemoveResolution(currentResolution);

            _saveSettingsService.SaveResolutions(_resolutionDialogViewModel.Resolutions);

            AllResolutionsDeleted();
        }

        /// <summary>
        /// Remove the selected resolution from list.
        /// </summary>
        /// <param name="currentResolution">The resolution to remove.</param>
        private void RemoveResolution(Resolution currentResolution)
        {
            for (var i = 0; i < _resolutionDialogViewModel.Resolutions.Count; i++)
            {
                if (!_resolutionDialogViewModel.Resolutions[i].Equals(currentResolution)) continue;
                _resolutionDialogViewModel.Resolutions.RemoveAt(i);
                break;
            }
        }

        /// <summary>
        /// Resets fields to default and disabled values as needed
        /// </summary>
        private void AllResolutionsDeleted()
        {
            _resolutionDialogViewModel.ErrorMessage = TranslationSource.GetTranslationFromString("deleted_resolution");
            _resolutionDialogViewModel.CmbIndex = 0;

            if (_resolutionDialogViewModel.Resolutions.Count != 0) return;

            // Resets fields to basic
            UpdateUiWithNoResolutionsAvaliable();
        }

        /// <summary>
        /// Resets fields to basic UI with no resolutions.
        /// </summary>
        private void UpdateUiWithNoResolutionsAvaliable()
        {
            _resolutionDialogViewModel.BypassWidthOrHeightLimitations = true;
            _resolutionDialogViewModel.Width = 0;
            _resolutionDialogViewModel.Heigth = 0;
            _resolutionDialogViewModel.HasValidResolutions = false;
            _resolutionDialogViewModel.BypassWidthOrHeightLimitations = false;
        }

        public event EventHandler? CanExecuteChanged;
    }
}