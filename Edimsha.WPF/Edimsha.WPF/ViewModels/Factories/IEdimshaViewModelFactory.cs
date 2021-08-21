using Edimsha.Core.Models;

namespace Edimsha.WPF.ViewModels.Factories
{
    public interface IEdimshaViewModelFactory
    {
        ViewModelBase CreateViewModel(ViewType viewType);
    }
}