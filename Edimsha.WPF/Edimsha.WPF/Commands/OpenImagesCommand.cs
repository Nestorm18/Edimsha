#nullable enable
using System;
using System.Collections;
using System.Resources;
using System.Text;
using System.Windows.Input;
using Edimsha.WPF.Lang;
using Edimsha.WPF.Services.Dialogs;
using Edimsha.WPF.Utils;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.Commands
{
    public class OpenImagesCommand : ICommand
    {
        private readonly EditorViewModel _editorViewModel;
        private readonly IDialogService _dialogService;
        private readonly ResourceManager _rs;

        public OpenImagesCommand(EditorViewModel editorViewModel, IDialogService dialogService)
        {
            _editorViewModel = editorViewModel;
            _dialogService = dialogService;

            // TODO: No carga el idioma correcto
            _rs = new ResourceManager(typeof(Resources));
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            var filter = CreateFilter(ImageFormatsFronViewType.GetImageType(parameter));

            var urls = await _dialogService.OpenFileSelector(_rs.GetString("select_images"), filter, true);
            
            // Clear Urls before add new ones.
            if (urls != null) _editorViewModel.UpdateUrlsWithoutDuplicates(urls.ToArray());
        }

        private string CreateFilter(IEnumerable getImageType)
        {
            var builder = new StringBuilder();
            var builderSecond = new StringBuilder();
            builder.Append(_rs.GetString("image_files"));
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