using System;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.State.Navigators
{
    public enum ViewType
    {
        Editor,
        Conversor
    }

    public interface INavigator
    {
        ViewModelBase CurrentViewModel { get; set; }
        event Action StateChanged;
    }
}