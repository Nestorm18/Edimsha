using System;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.State.Navigators
{
    public enum ViewType
    {
        Editor,
        Converter
    }

    public interface INavigator
    {
        ViewModelBase CurrentViewModel { get; set; }
        event Action StateChanged;
    }
}