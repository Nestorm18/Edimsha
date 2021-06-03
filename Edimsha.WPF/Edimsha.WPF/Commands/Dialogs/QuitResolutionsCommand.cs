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

        /// <summary>
        /// Close the dialog box and leave the width and height values to -1 used in GetResolution() to be returned as null.
        /// </summary>
        /// <param name="parameter">The window to close as a parameter</param>
        public void Execute(object? parameter)
        {
            _resolutionDialogViewModel.BypassWidthOrHeightLimitations = true;
            _resolutionDialogViewModel.Width = -1;
            _resolutionDialogViewModel.Heigth = -1;
            _resolutionDialogViewModel.BypassWidthOrHeightLimitations = false;
            
            if (parameter == null) return;
            var window = (Window) parameter;
            window.Close();
        }

        public event EventHandler? CanExecuteChanged;
    }
}