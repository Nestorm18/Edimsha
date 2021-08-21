using System;
using Edimsha.Core.Models;

namespace Edimsha.WPF.ViewModels.Factories
{
    public class EdimshaViewModelFactory : IEdimshaViewModelFactory
    {
        private readonly CreateViewModel<EditorViewModel> _createEditorViewModel;
        private readonly CreateViewModel<ConversorViewModel> _createConversorViewModel;

        public EdimshaViewModelFactory(
            CreateViewModel<EditorViewModel> createEditorViewModel,
            CreateViewModel<ConversorViewModel> createConversorViewModel
        )
        {
            _createEditorViewModel = createEditorViewModel;
            _createConversorViewModel = createConversorViewModel;
        }

        public ViewModelBase CreateViewModel(ViewType viewType)
        {
            switch (viewType)
            {
                case ViewType.Editor:
                    return _createEditorViewModel();
                case ViewType.Converter:
                    return _createConversorViewModel();
                default:
                    throw new ArgumentException("El ViewType con contiene un ViewModel.", nameof(viewType));
            }
        }
    }
}