using System;
using System.Windows.Input;
using Edimsha.Core.Logging.Implementation;

namespace Edimsha.WPF.Commands
{
    public class ParameterizedRelayCommand : ICommand
    {
        private readonly Action<object> _command;
        private readonly Func<bool> _canExecute;

        public ParameterizedRelayCommand(Action<object> commandAction, Func<bool> canExecute = null)
        {
            Logger.Log("Constructor");
            _command = commandAction;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            Logger.Log("ParameterizedRelayCommand executing");
            _command?.Invoke(parameter);
        }
    }
}