#nullable enable
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Edimsha.Core.Language;
using Edimsha.Core.Logging.Implementation;
using Edimsha.Core.Models;
using Edimsha.WPF.Services.Dialogs;
using Edimsha.WPF.State.Navigators;
using Edimsha.WPF.Utils;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.Commands
{
    public class OpenImagesCommand : ICommand
    {
        private readonly ViewModelBase _viewModel;
        private readonly IDialogService _dialogService;

        public OpenImagesCommand(ViewModelBase viewModel, IDialogService dialogService)
        {
            Logger.Log("Constructor");
            _viewModel = viewModel;
            _dialogService = dialogService;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        /// <summary>
        /// In case Drag and Drop does not work, you can select folders or images.
        /// </summary>
        /// <param name="parameter">A current <see cref="ViewType"/> in use.</param>
        public async void Execute(object? parameter)
        {
            var filter = FilterCreator.Create(ImageFormatsFromViewType.GetImageType(parameter));

            var urls = await _dialogService.OpenFileSelector(
                TranslationSource.GetTranslationFromString("select_images"), filter, true);

            if (urls == null) return;

            // Clear Urls before add new ones.
            var listCleaned = ListCleaner.PathWithoutDuplicatesAndGoodFormats(
                _viewModel.PathList.ToList(),
                urls.ToArray(),
                ModeImageTypes.Editor);

            _viewModel.PathList.Clear();

            foreach (var s in listCleaned) _viewModel.PathList.Add(s);
        }

        public event EventHandler? CanExecuteChanged;
    }
}