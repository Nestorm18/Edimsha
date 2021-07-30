#nullable enable
using System;
using System.Linq;
using System.Windows.Input;
using Edimsha.Core.Language;
using Edimsha.WPF.Services.Dialogs;
using Edimsha.WPF.State.Navigators;
using Edimsha.WPF.Utils;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.Commands
{
    public class OpenImagesCommand : ICommand
    {
        // Log
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        private readonly CommonViewModel _viewModel;
        private readonly IDialogService _dialogService;
        private readonly ViewType _mode;

        public OpenImagesCommand(CommonViewModel viewModel, IDialogService dialogService, ViewType mode)
        {
            Logger.Info("Constructor");
            _viewModel = viewModel;
            _dialogService = dialogService;
            _mode = mode;
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

            var urls = await _dialogService.OpenFileSelector(TranslationSource.GetTranslationFromString("select_images"), filter, true);

            if (urls == null) return;

            // Clear Urls before add new ones.
            var listCleaned = ListCleaner.PathWithoutDuplicatesAndGoodFormats(
                _viewModel.PathList.ToList(),
                urls.ToArray(),
                _mode);

            _viewModel.PathList.Clear();

            foreach (var s in listCleaned) _viewModel.PathList.Add(s);
        }

        public event EventHandler? CanExecuteChanged;
    }
}