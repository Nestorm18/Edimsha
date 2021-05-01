#nullable enable
using System;
using System.Windows;
using System.Windows.Input;
using Edimsha.WPF.ViewModels.DialogsViewModel;

namespace Edimsha.WPF.Commands.Dialogs
{
    public class QuitResolutionsCommand : ICommand
    {
        private readonly ResolutionDialogViewModel _resolutionDialogViewModel;

        public QuitResolutionsCommand(ResolutionDialogViewModel resolutionDialogViewModel)
        {
            _resolutionDialogViewModel = resolutionDialogViewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _resolutionDialogViewModel.Width = -1;
            _resolutionDialogViewModel.Heigth = -1;
            
            if (parameter == null) return;
            var window = (Window) parameter;
            window.Close();
        }

        public event EventHandler? CanExecuteChanged;
    }
}