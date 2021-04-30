#nullable enable
using System;
using System.Windows;
using System.Windows.Input;
using Edimsha.WPF.ViewModels.DialogsViewModel;

namespace Edimsha.WPF.Commands.Dialogs
{
    public class QuitCommandResolutions : ICommand
    {
        public QuitCommandResolutions(ResolutionDialogViewModel resolutionDialogViewModel)
        {
            resolutionDialogViewModel.Width = -1;
            resolutionDialogViewModel.Heigth = -1;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter == null) return;
            var window = (Window) parameter;
            window.Close();
        }

        public event EventHandler? CanExecuteChanged;
    }
}