#nullable enable
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Edimsha.WPF.Lang;
using Edimsha.WPF.Models;
using Edimsha.WPF.Services.Dialogs;
using Edimsha.WPF.Utils;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.Commands
{
    public class OpenImagesCommand : ICommand
    {
        private readonly EditorViewModel _editorViewModel;
        private readonly IDialogService _dialogService;
        private readonly TranslationSource _ts;

        public OpenImagesCommand(EditorViewModel editorViewModel, IDialogService dialogService)
        {
            _editorViewModel = editorViewModel;
            _dialogService = dialogService;

            _ts = TranslationSource.Instance;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            var filter = CreateFilter(ImageFormatsFromViewType.GetImageType(parameter));

            var urls = await _dialogService.OpenFileSelector(_ts["select_images"], filter, true);
            
            if (urls == null) return;
           
            // Clear Urls before add new ones.
            var listCleaned = ListCleaner.PathWithoutDuplicatesAndGoodFormats(
                _editorViewModel.Urls.ToList(),
                urls.ToArray(),
                ModeImageTypes.Editor);

            _editorViewModel.Urls.Clear();
            foreach (var s in listCleaned) _editorViewModel.Urls.Add(s);
        }

        private string CreateFilter(IEnumerable getImageType)
        {
            var builder = new StringBuilder();
            var builderSecond = new StringBuilder();
            builder.Append(_ts["image_files"]);
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