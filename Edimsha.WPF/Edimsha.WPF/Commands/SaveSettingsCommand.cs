#nullable enable
using System;
using System.Windows.Input;

namespace Edimsha.WPF.Commands
{
    public class SaveSettingsCommand : ICommand
    {
        private readonly Action _action;

        public SaveSettingsCommand(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _action();
        }

        public event EventHandler? CanExecuteChanged;
    }
}