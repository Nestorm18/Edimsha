using System;
using System.Windows.Input;
using Edimsha.WPF.Utils;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.Commands
{
    public class OpenImagesCommand : ICommand
    {
        private readonly EditorViewModel _editorViewModel;

        public OpenImagesCommand(EditorViewModel editorViewModel)
        {
            _editorViewModel = editorViewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            ImageFormatsFronViewType.GetImageType(parameter);
            //TODO: Cargar un servicio de abrir dialogo de imagenes pasando parametro como filtro de imagen
        }

        public event EventHandler? CanExecuteChanged;
    }
}