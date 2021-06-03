#nullable enable
using System;
using System.Windows;
using System.Windows.Input;

namespace Edimsha.WPF.Commands.Dialogs
{
    public class AcceptResolutionCommand : ICommand
    {
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        /// <summary>
        /// Close the dialog box and leave the width and height values that are in the viewmodel to be returned in GetResolution().
        /// </summary>
        /// <param name="parameter">The window to close as a parameter.</param>
        public void Execute(object? parameter)
        {
            if (parameter == null) return;
            var window = (Window) parameter;
            window.Close();
        }

        public event EventHandler? CanExecuteChanged;
    }
}