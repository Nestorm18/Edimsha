#nullable enable
using System;
using System.Windows.Input;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.Commands
{
    public class DeleteItemsCommand : ICommand
    {
        private readonly bool _removeAll;
        private readonly EditorViewModel _viewModel;

        public DeleteItemsCommand(EditorViewModel viewModel, bool removeAll = false)
        {
            _viewModel = viewModel;
            _removeAll = removeAll;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (_removeAll)
                _viewModel.Urls.Clear();
            else
                _viewModel.Urls.Remove((string) parameter!);
        }

        public event EventHandler? CanExecuteChanged;
    }
}