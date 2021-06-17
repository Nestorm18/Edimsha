#nullable enable
using System;
using System.Linq;
using System.Windows.Input;
using Edimsha.Core.Language;
using Edimsha.Core.Logging.Core;
using Edimsha.Core.Logging.Implementation;
using Edimsha.WPF.Converters;
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
            Logger.Log("Constructor");

            _resolutionDialogViewModel = resolutionDialogViewModel;
            _saveSettingsService = saveSettingsService;
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
            Logger.Log("Saving resolutions");

            var values = (object[]) parameter!;
            var width = (string) values[0];
            var height = (string) values[1];

            Logger.Log($"width:{width}, height:{height}", LogLevel.Debug);

            // Not valid values
            if (width == string.Empty || height == string.Empty) return;

            var currentResolution = new Resolution()
            {
                Width = int.Parse(width),
                Height = int.Parse(height)
            };

            if (ExistCurrentResolution(currentResolution))
            {
                Logger.Log("the_resolution_already_exists");
                _resolutionDialogViewModel.ErrorMessage =
                    TranslationSource.GetTranslationFromString("the_resolution_already_exists");
            }
            else
            {
                _resolutionDialogViewModel.Resolutions.Add(currentResolution);

                _saveSettingsService.SaveResolutions(_resolutionDialogViewModel.Resolutions);

                _resolutionDialogViewModel.ErrorMessage =
                    TranslationSource.GetTranslationFromString("resolution_saved");

                _resolutionDialogViewModel.CmbIndex = _resolutionDialogViewModel.Resolutions.Count - 1;

                Logger.Log("resolution_saved");
            }
        }

        /// <summary>
        /// Check if the resolution already exists.
        /// </summary>
        /// <param name="currentResolution">Resolution to be saved.</param>
        /// <returns></returns>
        private bool ExistCurrentResolution(Resolution currentResolution)
        {
            Logger.Log(currentResolution.ToString());
            return _resolutionDialogViewModel.Resolutions.Any(resolution => resolution.Equals(currentResolution));
        }

        public event EventHandler? CanExecuteChanged;
    }
}