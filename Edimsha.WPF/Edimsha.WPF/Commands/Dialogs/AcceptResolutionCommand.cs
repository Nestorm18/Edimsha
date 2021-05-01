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

        public void Execute(object? parameter)
        {
            if (parameter == null) return;
            var window = (Window) parameter;
            window.Close();
        }

        public event EventHandler? CanExecuteChanged;
    }
}