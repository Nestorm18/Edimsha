using System;
using System.Windows.Input;

namespace Edimsha.WPF.Commands.Basics
{
    public class ParameterizedRelayCommand : ICommand
    {
        // Log
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        private readonly Action<object> _command;
        private readonly Func<bool> _canExecute;

        public ParameterizedRelayCommand(Action<object> commandAction, Func<bool> canExecute = null)
        {
            Logger.Info("Constructor");
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
            Logger.Info($"Command with parameters executed");
            _command?.Invoke(parameter);
        }
    }
}