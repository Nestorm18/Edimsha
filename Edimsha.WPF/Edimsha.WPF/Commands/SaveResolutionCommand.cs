#nullable enable
using System;
using System.Windows.Input;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.ViewModels.DialogsViewModel;

namespace Edimsha.WPF.Commands
{
    public class SaveResolutionCommand : ICommand
    {
        private readonly ResolutionDialogViewModel _resolutionDialogViewModel;
        private readonly ISaveSettingsService _saveSettingsService;

        public SaveResolutionCommand(
            ResolutionDialogViewModel resolutionDialogViewModel,
            ISaveSettingsService saveSettingsService)
        {
            _resolutionDialogViewModel = resolutionDialogViewModel;
            _saveSettingsService = saveSettingsService;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            var values = (object[])parameter!;
            var width = (string)values[0];
            var height = (string)values[1];

            if (width == string.Empty || height == string.Empty)
            {
                return;
            }

            Console.WriteLine($"{width}, {height}");
        }

        public event EventHandler? CanExecuteChanged;
    }
}