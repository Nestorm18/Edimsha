#nullable enable
using System;
using System.Windows.Input;
using Edimsha.Core.Language;
using Edimsha.Core.Models;
using Edimsha.Core.Settings;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.Utils;
using Edimsha.WPF.ViewModels.DialogsViewModel;
using Microsoft.Extensions.Options;

namespace Edimsha.WPF.Commands.Dialogs
{
    public class RemoveResolutionCommand : ICommand
    {
        // Log
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        private readonly ResolutionDialogViewModel _resolutionDialogViewModel;
        private readonly ISaveSettingsService _saveSettingsService;
        private readonly IOptions<ConfigPaths> _options;

        public RemoveResolutionCommand(
            ResolutionDialogViewModel resolutionDialogViewModel,
            ISaveSettingsService saveSettingsService,
            IOptions<ConfigPaths> options)
        {
            _resolutionDialogViewModel = resolutionDialogViewModel;
            _saveSettingsService = saveSettingsService;
            _options = options;
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
            Logger.Info("Deleting resolution");

            var currentResolution = ResolutionValidator.IsParameterValid(parameter);

            if (!currentResolution.IsValid()) return;

            RemoveResolution(currentResolution);

            //TODO: Resolutions
            // _saveSettingsService.SaveListToFile(_resolutionDialogViewModel.Resolutions,_options.Value.EditorPaths);
           
            // _saveSettingsService.SaveConfigurationSettings<List<string>, EditorOptions>("Paths", _resolutionDialogViewModel.PathList.ToList(), _options.Value.EditorOptions);

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