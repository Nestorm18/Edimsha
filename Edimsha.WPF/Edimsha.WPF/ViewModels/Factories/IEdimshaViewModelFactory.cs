using Edimsha.WPF.State.Navigators;

namespace Edimsha.WPF.ViewModels.Factories
{
    public interface IEdimshaViewModelFactory
    {
        ViewModelBase CreateViewModel(ViewType viewType);
    }
}