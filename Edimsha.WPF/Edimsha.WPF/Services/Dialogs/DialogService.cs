using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Edimsha.Core.Language;
using Edimsha.Core.Logging.Implementation;
using Edimsha.Core.Models;
using Edimsha.WPF.Annotations;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.State.Navigators;
using Edimsha.WPF.ViewModels.DialogsViewModel;
using Edimsha.WPF.Views.Dialogs;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

#pragma warning disable 1998

namespace Edimsha.WPF.Services.Dialogs
{
    public class DialogService : IDialogService
    {
        public async Task<List<string>> OpenFileSelector(string title, string filter, bool multiselect)
        {
            Logger.Log($"Title: {title}, filter: {filter}, multiselect: {multiselect}");
            var dlg = new OpenFileDialog()
            {
                Title = title,
                Filter = filter,
                Multiselect = multiselect
            };

            Logger.Log($"Returning fileNames: {dlg.FileNames}");
            return dlg.ShowDialog() == true ? new List<string>(dlg.FileNames) : null;
        }

        public async Task<string> OpenFolderSelector(string title)
        {
            Logger.Log($"Title: {title}");
            var dlg = new CommonOpenFileDialog {IsFolderPicker = true, Title = title};

            return dlg.ShowDialog() == CommonFileDialogResult.Ok ? dlg.FileName : null;
        }

        [CanBeNull]
        public async Task<Resolution> OpenResolutionDialog(ILoadSettingsService loadSettingsService, ISaveSettingsService saveSettingsService)
        {
            Logger.Log("Viewmodel");
            var vm = new ResolutionDialogViewModel(loadSettingsService, saveSettingsService);

            Logger.Log("Opening dialog");
            var dlg = new ResolutionDialog {DataContext = vm};

            // Prevent load resolution if closes with the X in titlebar
            dlg.Closing += (_, _) =>
            {
                if (vm.Width > 0 && vm.Heigth > 0) return;

                vm.BypassWidthOrHeightLimitations = true;
                vm.Width = -1;
                vm.Heigth = -1;
            };

            dlg.ShowDialog();

            Logger.Log($"Resolution: {vm.GetResolution()}");
            return vm.GetResolution();
        }

        public async Task PathsRemovedLastSession(ILoadSettingsService loadSettingsService, ISaveSettingsService saveSettingsService, ViewType type)
        {
            Logger.Log($"Cheking for type {type}");

            var result = MessageBox.Show(
                TranslationSource.GetTranslationFromString("the_paths_that_were_there_before_have_been_modified_click_on_yes_to_see_the_changes"),
                TranslationSource.GetTranslationFromString("modified_paths"),
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            var allPaths = (List<string>) loadSettingsService.GetSavedPaths(type);
            var deletedPaths = (List<string>) loadSettingsService.GetPathChanges(type);

            if (deletedPaths == null) return;

            var append = deletedPaths.Aggregate("", (current, text) => current + (text + "\n\n"));

            var avaliablePaths = allPaths.Except(deletedPaths);

            saveSettingsService.SavePaths(avaliablePaths, type);

            MessageBox.Show(append, TranslationSource.GetTranslationFromString("deleted_paths"), MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}