#nullable enable
using System;
using System.Windows;
using System.Windows.Input;
using Edimsha.Core.Logging.Implementation;

namespace Edimsha.WPF.Commands.Main
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

            Logger.Log("Clossing");
            var window = (Window) parameter;
            window.Close();
        }

        public event EventHandler? CanExecuteChanged;
    }
}