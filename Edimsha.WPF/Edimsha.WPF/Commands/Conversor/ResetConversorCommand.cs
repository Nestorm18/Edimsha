#nullable enable
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.Commands.Conversor
{
    public class ResetConversorCommand :ICommand
    {
        private readonly ConversorViewModel _conversorViewModel;

        public ResetConversorCommand(ConversorViewModel conversorViewModel)
        {
            _conversorViewModel = conversorViewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            // Paths
            _conversorViewModel.PathList = new ObservableCollection<string>();
            _conversorViewModel.SavePaths();

            // Parameters
            _conversorViewModel.CleanListOnExit = false;
            _conversorViewModel.IterateSubdirectories = false;
            _conversorViewModel.OutputFolder = string.Empty;
            _conversorViewModel.Edimsha = string.Empty;
            _conversorViewModel.PbPosition = 0;
            _conversorViewModel.StatusBar = "reset_completed";

            // Reset UI 
            _conversorViewModel.IsRunningUi = true;
            _conversorViewModel.IsStartedUi = false;
        }

        public event EventHandler? CanExecuteChanged;
    }
}