#nullable enable
using System;
using System.Windows;
using System.Windows.Input;

namespace Edimsha.WPF.Commands
{
    public class QuitCommand : ICommand
    {
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        /// <summary>
        /// Closes the window that is passed by parameter
        /// </summary>
        /// <param name="parameter">Window to close.</param>
        public void Execute(object? parameter)
        {
            if (parameter == null) return;
            var window = (Window) parameter;
            window.Close();
        }

        public event EventHandler? CanExecuteChanged;
    }
}