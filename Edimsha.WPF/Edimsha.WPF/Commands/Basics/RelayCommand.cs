using System;
using System.Windows.Input;

// ReSharper disable UnusedParameter.Local

namespace Edimsha.WPF.Commands.Basics
{
    public class RelayCommand : ICommand
    {
        // Log
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        
        private readonly Action _mAction;

        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public RelayCommand(Action action)
        {
            _logger.Info("Constructor");
            _mAction = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _logger.Info("Relay executing");
            _mAction();
        }
    }
}