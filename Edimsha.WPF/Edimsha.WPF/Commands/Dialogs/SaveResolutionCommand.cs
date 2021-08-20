#nullable enable
using System;
using System.Linq;
using System.Windows.Input;
using Edimsha.Core.Language;
using Edimsha.Core.Models;
using Edimsha.Core.Settings;
using Edimsha.WPF.Converters;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.Utils;
using Edimsha.WPF.ViewModels.DialogsViewModel;
using Microsoft.Extensions.Options;

namespace Edimsha.WPF.Commands.Dialogs
{
    public class SaveResolutionCommand : ICommand
    {
        // Log
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        private readonly ResolutionDialogViewModel _resolutionDialogViewModel;
        private readonly ISaveSettingsService _saveSettingsService;
        private readonly IOptions<ConfigPaths> _options;

        public SaveResolutionCommand(
            ResolutionDialogViewModel resolutionDialogViewModel,
            ISaveSettingsService saveSettingsService,
            IOptions<ConfigPaths> options)
        {
            Logger.Info("Constructor");

            _resolutionDialogViewModel = resolutionDialogViewModel;
            _saveSettingsService = saveSettingsService;
            _options = options;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        /// <summary>
        /// Saves the selected resolution from the combobox and from the save file.
        /// </summary>
        /// <param name="parameter"><see cref="Resolution"/> to save. Used <see cref="MultiTextConverter"/> XAML to get values of width and heigth.</param>
        public void Execute(object? parameter)
        {
            var currentResolution = ResolutionValidator.IsParameterValid(parameter);

            if (currentResolution == null) return;

            if (ExistCurrentResolution(currentResolution))
            {
                Logger.Info("the_resolution_already_exists");
                _resolutionDialogViewModel.ErrorMessage = TranslationSource.GetTranslationFromString("the_resolution_already_exists");
            }
            else
            {
                _resolutionDialogViewModel.Resolutions.Add(currentResolution);

                //TODO: Resolutions
                // _saveSettingsService.SaveListToFile(_resolutionDialogViewModel.Resolutions,_options.Value.EditorPaths);

                _resolutionDialogViewModel.ErrorMessage = TranslationSource.GetTranslationFromString("resolution_saved");

                _resolutionDialogViewModel.CmbIndex = _resolutionDialogViewModel.Resolutions.Count - 1;

                Logger.Info("resolution_saved");
            }
        }

        /// <summary>
        /// Check if the resolution already exists.
        /// </summary>
        /// <param name="currentResolution">Resolution to be saved.</param>
        /// <returns></returns>
        private bool ExistCurrentResolution(Resolution currentResolution)
        {
            Logger.Info(currentResolution.ToString());
            return _resolutionDialogViewModel.Resolutions.Any(resolution => resolution.Equals(currentResolution));
        }

        public event EventHandler? CanExecuteChanged;
    }
}