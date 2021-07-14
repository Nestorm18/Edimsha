#nullable enable
using System;
using System.Windows.Input;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.Commands
{
    public class DeleteItemsCommand : ICommand
    {
        // Log
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        
        private readonly bool _removeAll;
        private readonly ViewModelBase _viewModel;

        public DeleteItemsCommand(ViewModelBase viewModel, bool removeAll = false)
        {
            _logger.Info("Constructor");
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