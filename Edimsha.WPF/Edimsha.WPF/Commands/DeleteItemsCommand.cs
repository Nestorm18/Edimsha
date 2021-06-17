#nullable enable
using System;
using System.Windows.Input;
using Edimsha.Core.Logging.Implementation;
using Edimsha.WPF.State.Navigators;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.Commands
{
    public class DeleteItemsCommand : ICommand
    {
        private readonly bool _removeAll;
        private readonly ViewModelBase _viewModel;
        private readonly ViewType _type;

        public DeleteItemsCommand(ViewModelBase viewModel, ViewType type, bool removeAll = false)
        {
            Logger.Log("Constructor");
            _viewModel = viewModel;
            _type = type;
            _removeAll = removeAll;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        /// <summary>
        /// Remove all paths from list or selected path.
        /// </summary>
        /// <param name="parameter">Path to delete.</param>
        public void Execute(object? parameter)
        {
            switch (_type)
            {
                case ViewType.Editor:
                {
                    var viewModel = (EditorViewModel) _viewModel;
                    if (_removeAll)
                        viewModel.PathList.Clear();
                    else
                        viewModel.PathList.Remove((string) parameter!);
                    break;
                }
                case ViewType.Conversor:
                {
                    var viewModel = (ConversorViewModel) _viewModel;
                    if (_removeAll)
                        viewModel.PathList.Clear();
                    else
                        viewModel.PathList.Remove((string) parameter!);
                    break;
                }
            }
        }

        public event EventHandler? CanExecuteChanged;
    }
}