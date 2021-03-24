#nullable enable
using System;
using System.Windows.Input;
using Edimsha.WPF.State.Navigators;
using Edimsha.WPF.ViewModels;
using Edimsha.WPF.ViewModels.Factories;

namespace Edimsha.WPF.Commands
{
    public class ChangeModeCommand : ICommand
    {
        private readonly MainViewModel _viewModel;
        private readonly IEdimshaViewModelFactory _viewModelFactory;

        public ChangeModeCommand(MainViewModel viewModel, IEdimshaViewModelFactory viewModelFactory)
        {
            _viewModel = viewModel;
            _viewModelFactory = viewModelFactory;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter != null)
            {
                _viewModel.CurrentModeViewModel = _viewModelFactory.CreateViewModel((ViewType) parameter);
            }
        }

        public event EventHandler? CanExecuteChanged;
    }
}