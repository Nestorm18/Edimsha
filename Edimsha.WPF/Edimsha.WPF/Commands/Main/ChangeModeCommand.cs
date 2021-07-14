#nullable enable
using System;
using System.Windows.Input;
using Edimsha.WPF.State.Navigators;
using Edimsha.WPF.ViewModels;
using Edimsha.WPF.ViewModels.Factories;

namespace Edimsha.WPF.Commands.Main
{
    public class ChangeModeCommand : ICommand
    {
        // Log
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        
        private readonly MainViewModel _viewModel;
        private readonly IEdimshaViewModelFactory _viewModelFactory;

        public ChangeModeCommand(MainViewModel viewModel, IEdimshaViewModelFactory viewModelFactory)
        {
            _logger.Info("Constructor");
            _viewModel = viewModel;
            _viewModelFactory = viewModelFactory;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        /// <summary>
        /// Switches between editor/converter mode
        /// </summary>
        /// <param name="parameter"><see cref="ViewType"/> to use.</param>
        public void Execute(object? parameter)
        {
            if (parameter == null) return;

            _logger.Info($"Changing mode to {(ViewType) parameter}");
            _viewModel.CurrentModeViewModel = _viewModelFactory.CreateViewModel((ViewType) parameter);
        }

        public event EventHandler? CanExecuteChanged;
    }
}