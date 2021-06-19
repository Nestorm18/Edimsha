#nullable enable
using System;
using System.Windows.Input;
using Edimsha.Core.Logging.Implementation;
using Edimsha.WPF.State.Navigators;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.Commands
{
    public class DeleteItemsCommand : ICommand
    {
        private readonly bool _removeAll;
        private readonly ViewModelBase _viewModel;

        public DeleteItemsCommand(ViewModelBase viewModel, bool removeAll = false)
        {
            Logger.Log("Constructor");
            _viewModel = viewModel;
            _removeAll = removeAll;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        /// <summary>
        /// Remove all paths from list or selected path.
        /// </summary>
        /// <param name="parameter">Path to delete.</param>
        public void Execute(object? parameter)
        {
            if (_removeAll)
                _viewModel.PathList.Clear();
            else
                _viewModel.PathList.Remove((string) parameter!);
        }

        public event EventHandler? CanExecuteChanged;
    }
}