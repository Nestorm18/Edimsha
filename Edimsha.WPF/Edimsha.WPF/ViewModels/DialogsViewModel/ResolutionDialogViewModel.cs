#nullable enable
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Edimsha.WPF.Commands;
using Edimsha.WPF.Models;
using Edimsha.WPF.Services.Data;

namespace Edimsha.WPF.ViewModels.DialogsViewModel
{
    public class ResolutionDialogViewModel : ViewModelBase
    {
        private readonly ILoadSettingsService _loadSettingsService;
        private readonly ISaveSettingsService _saveSettingsService;

        // Properties
        private ObservableCollection<Resolution> _resolutions;
        private bool _hasValidResolutions;

        public bool HasValidResolutions
        {
            get => _hasValidResolutions;
            set
            {
                if (value == _hasValidResolutions) return;
                _hasValidResolutions = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Resolution> Resolutions
        {
            get => _resolutions;
            set
            {
                if (Equals(value, _resolutions)) return;
                _resolutions = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand CancelCommand { get; }

        public ResolutionDialogViewModel(
            ILoadSettingsService loadSettingsService,
            ISaveSettingsService saveSettingsService)
        {
            _loadSettingsService = loadSettingsService;
            _saveSettingsService = saveSettingsService;

            Resolutions = new ObservableCollection<Resolution>();
            Resolutions.CollectionChanged += ResolutionsOnCollectionChanged;

            // Commands
            CancelCommand = new QuitCommand();

            SetUserSettings();
        }

        public Resolution GetResolution()
        {
            return new()
            {
                Width = 1,
                Height = 1
            };
        }

        private void SetUserSettings()
        {
            _loadSettingsService.LoadResolutions()?.ForEach(Resolutions.Add);
        }

        private void ResolutionsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (Resolutions.Count > 0)
            {
                HasValidResolutions = true;
            }
        }
    }
}