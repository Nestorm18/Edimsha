using System;
using System.Windows.Input;
using Edimsha.Core.Logging.Implementation;

namespace Edimsha.WPF.Commands
{
    public class RelayCommand : ICommand
    {
        private Action _mAction;

        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public RelayCommand(Action action)
        {
            Logger.Log("Constructor");
            _mAction = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Logger.Log("Relay executing");
            _mAction();
        }
    }
}