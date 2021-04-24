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
        private int _width;
        private int _heigth;

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

        public int Width
        {
            get => _width;
            set
            {
                if (value == _width || value <= 0) return;
                _width = value;
                OnPropertyChanged();
            }
        }

        public int Heigth
        {
            get => _heigth;
            set
            {
                if (value == _heigth || value <= 0) return;
                _heigth = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand CancelCommand { get; }
        public ICommand SaveResolutionCommand { get; }
        public ICommand LostFocusCommand { get; }

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
            SaveResolutionCommand = new SaveResolutionCommand(this, _saveSettingsService);
            LostFocusCommand = new RelayCommand(() =>
            {
                OnPropertyChanged(nameof(Width));
                OnPropertyChanged(nameof(Heigth));
            });

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
            LoadResolutions();
        }

        private void LoadResolutions()
        {
            Resolutions.Clear();

            var resolutions = _loadSettingsService.LoadResolutions();

            foreach (var resolution in resolutions)
            {
                Resolutions.Add(resolution);
                Width = resolution.Width;
                Heigth = resolution.Height;
            }
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