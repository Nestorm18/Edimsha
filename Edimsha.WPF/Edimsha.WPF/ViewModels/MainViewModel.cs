using System.Windows;
using System.Windows.Input;
using Edimsha.WPF.Commands;
using Edimsha.WPF.State.Navigators;
using Edimsha.WPF.ViewModels.Factories;

namespace Edimsha.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // IOC
        private readonly IEdimshaViewModelFactory _viewModelFactory;

        // Properties
        public ViewModelBase CurrentModeViewModel { get; }

        // Commands
        public ICommand Salir { get; }

        // Constructor
        public MainViewModel(IEdimshaViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
            CurrentModeViewModel = _viewModelFactory.CreateViewModel(ViewType.Editor);

            // Commands
            Salir = new QuitCommand();
        }
    }
}