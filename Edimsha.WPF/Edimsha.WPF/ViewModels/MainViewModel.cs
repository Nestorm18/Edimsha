using System.Windows;
using System.Windows.Input;
using Edimsha.WPF.Commands;
using Edimsha.WPF.Lang;
using Edimsha.WPF.State.Navigators;
using Edimsha.WPF.ViewModels.Factories;

namespace Edimsha.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // IOC
        private readonly IEdimshaViewModelFactory _viewModelFactory;
        private Languages _language = Languages.Spanish;

        // Properties
        public Languages Language
        {
            get => _language;
            set
            {
                _language = value;
                OnPropertyChanged();
            } 
        }

        // Commands
        public ICommand SalirCommand  { get; }
        public ICommand ChangeLanguageCommand { get; }

        // Viewmodel
        public ViewModelBase CurrentModeViewModel { get; }
        
        // Constructor
        public MainViewModel(IEdimshaViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
            CurrentModeViewModel = _viewModelFactory.CreateViewModel(ViewType.Editor);

            // Commands
            SalirCommand = new QuitCommand();
            ChangeLanguageCommand = new ChangeLanguageCommand(this);
        }
    }
}