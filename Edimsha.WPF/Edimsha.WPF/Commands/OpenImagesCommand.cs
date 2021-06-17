#nullable enable
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Edimsha.Core.Language;
using Edimsha.Core.Logging.Implementation;
using Edimsha.WPF.Models;
using Edimsha.WPF.Services.Dialogs;
using Edimsha.WPF.State.Navigators;
using Edimsha.WPF.Utils;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.Commands
{
    public class OpenImagesCommand : ICommand
    {
        private readonly ViewModelBase _viewModel;
        private readonly ViewType _type;
        private readonly IDialogService _dialogService;

        public OpenImagesCommand(ViewModelBase viewModel, ViewType type, IDialogService dialogService)
        {
            Logger.Log("Constructor");
            _viewModel = viewModel;
            _type = type;
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
            var filter = CreateFilter(ImageFormatsFromViewType.GetImageType(parameter));

            var urls = await _dialogService.OpenFileSelector(
                TranslationSource.GetTranslationFromString("select_images"), filter, true);

            if (urls == null) return;

            switch (_type)
            {
                case ViewType.Editor:
                {
                    var viewModel = (EditorViewModel) _viewModel;

                    // Clear Urls before add new ones.
                    var listCleaned = ListCleaner.PathWithoutDuplicatesAndGoodFormats(
                        viewModel.PathList.ToList(),
                        urls.ToArray(),
                        ModeImageTypes.Editor);

                    viewModel.PathList.Clear();

                    foreach (var s in listCleaned) viewModel.PathList.Add(s);

                    break;
                }
                case ViewType.Conversor:
                {
                    var viewModel = (ConversorViewModel) _viewModel;

                    // Clear Urls before add new ones.
                    var listCleaned = ListCleaner.PathWithoutDuplicatesAndGoodFormats(
                        viewModel.PathList.ToList(),
                        urls.ToArray(),
                        ModeImageTypes.Editor);

                    viewModel.PathList.Clear();

                    foreach (var s in listCleaned) viewModel.PathList.Add(s);

                    break;
                }
            }
        }

        /// <summary>
        /// Creates the filter used in file browsers based on the formats supported by the application at the current time.
        /// </summary>
        /// <param name="getImageType">All avaliable format for current <see cref="ViewType"/>.</param>
        /// <returns>Formmated string to use as filter.</returns>
        private static string CreateFilter(IEnumerable getImageType)
        {
            var builder = new StringBuilder();
            var builderSecond = new StringBuilder();
            builder.Append(TranslationSource.GetTranslationFromString("image_files"));
            builder.Append(' ');
            builder.Append('(');

            foreach (var type in getImageType)
            {
                var format = $"*.{type.ToString()?.ToLower()};";
                builder.Append(format);
                builderSecond.Append(format);
            }

            builder.Remove(builder.Length - 1, 1);
            builder.Append(')');
            builder.Append('|');
            builder.Append(builderSecond);

            return builder.ToString();
        }

        public event EventHandler? CanExecuteChanged;
    }
}