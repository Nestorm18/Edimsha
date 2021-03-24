using Edimsha.WPF.State.Navigators;
using Edimsha.WPF.ViewModels.Factories;

namespace Edimsha.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEdimshaViewModelFactory _viewModelFactory;
        
        public ViewModelBase CurrentModeViewModel { get; }
        
        public MainViewModel(IEdimshaViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
            CurrentModeViewModel = _viewModelFactory.CreateViewModel(ViewType.Editor);
        }
    }
}