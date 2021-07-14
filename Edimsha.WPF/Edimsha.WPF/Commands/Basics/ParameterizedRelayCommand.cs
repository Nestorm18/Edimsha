using System;
using System.Windows.Input;

namespace Edimsha.WPF.Commands.Basics
{
    public class ParameterizedRelayCommand : ICommand
    {
        // Log
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        
        private readonly Action<object> _command;
        private readonly Func<bool> _canExecute;

        public ParameterizedRelayCommand(Action<object> commandAction, Func<bool> canExecute = null)
        {
            _logger.Info("Constructor");
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
            _logger.Info($"Command with parameters executed");
            _command?.Invoke(parameter);
        }
    }
}