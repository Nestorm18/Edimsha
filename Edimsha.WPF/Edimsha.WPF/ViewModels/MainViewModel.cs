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

        // Properties
        private ViewModelBase _currentModeViewModel;
        private Languages _language = Languages.Spanish;
        private ViewType _mode = ViewType.Editor;

        public Languages Language
        {
            get => _language;
            set
            {
                _language = value;
                OnPropertyChanged();
            }
        }

        public ViewType Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand QuitCommand { get; }
        public ICommand ChangeLanguageCommand { get; }
        public ICommand ChangeModeCommand { get; }

        // Viewmodel
        public ViewModelBase CurrentModeViewModel
        {
            get => _currentModeViewModel;
            set
            {
                _currentModeViewModel = value;
                OnPropertyChanged();
            }
        }

        // Constructor
        public MainViewModel(IEdimshaViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
            CurrentModeViewModel = _viewModelFactory.CreateViewModel(Mode);

            // Commands
            QuitCommand = new QuitCommand();
            ChangeLanguageCommand = new ChangeLanguageCommand(this);
            ChangeModeCommand = new ChangeModeCommand(this, _viewModelFactory);
        }
    }
}