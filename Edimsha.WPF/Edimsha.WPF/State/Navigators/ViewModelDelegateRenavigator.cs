using Edimsha.Core.Logging.Implementation;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.State.Navigators
{
    public class ViewModelDelegateRenavigator<TViewModel> : IRenavigator where TViewModel : ViewModelBase
    {
        private readonly INavigator _navigator;
        private readonly CreateViewModel<TViewModel> _createViewModel;

        public ViewModelDelegateRenavigator(INavigator navigator, CreateViewModel<TViewModel> createViewModel)
        {
            Logger.Log("Constructor");
            _navigator = navigator;
            _createViewModel = createViewModel;
        }

        public void Renavigate()
        {
            Logger.Log("Renavigating");
            _navigator.CurrentViewModel = _createViewModel();
        }
    }
}