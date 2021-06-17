#nullable enable
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Edimsha.WPF.ViewModels;

namespace Edimsha.WPF.Commands.Editor
{
    public class ResetEditorCommand : ICommand
    {
        private readonly EditorViewModel _editorViewModel;

        public ResetEditorCommand(EditorViewModel editorViewModel)
        {
            _editorViewModel = editorViewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            // Paths
            _editorViewModel.Urls = new ObservableCollection<string>();
            _editorViewModel.SavePaths();

            // Parameters
            _editorViewModel.CleanListOnExit = false;
            _editorViewModel.IterateSubdirectories = false;
            _editorViewModel.OutputFolder = string.Empty;
            _editorViewModel.Edimsha = string.Empty;
            _editorViewModel.AlwaysIncludeOnReplace = false;
            _editorViewModel.WidthImage = 0;
            _editorViewModel.HeightImage = 0;
            _editorViewModel.KeepOriginalResolution = false;
            _editorViewModel.CompresionValue = 0.0;
            _editorViewModel.OptimizeImage = false;
            _editorViewModel.ReplaceForOriginal = false;
            _editorViewModel.PbPosition = 0;
            _editorViewModel.StatusBar = "reset_completed";

            // Reset UI 
            _editorViewModel.IsRunningUi = true;
            _editorViewModel.IsStartedUi = false;
        }

        public event EventHandler? CanExecuteChanged;
    }
}