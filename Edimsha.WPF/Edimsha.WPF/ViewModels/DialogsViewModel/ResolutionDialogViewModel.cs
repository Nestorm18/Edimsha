#nullable enable
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Edimsha.WPF.Commands;
using Edimsha.WPF.Commands.Dialogs;
using Edimsha.WPF.Models;
using Edimsha.WPF.Services.Data;

namespace Edimsha.WPF.ViewModels.DialogsViewModel
{
    public class ResolutionDialogViewModel : ViewModelBase
    {
        private readonly ILoadSettingsService _loadSettingsService;
        private readonly ISaveSettingsService _saveSettingsService;

        // Propertie returned
        public Resolution GetResolution()
        {
            return new()
            {
                Width = 1,
                Height = 1
            };
        }
        
        // Properties
        private ObservableCollection<Resolution> _resolutions;
        private bool _hasValidResolutions;
        private int _width;
        private int _heigth;
        private string _errorMessage;
        private int _cmbIndex;

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

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (value == _errorMessage) return;
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public int CmbIndex
        {
            get => _cmbIndex;
            set
            {
                if (value == _cmbIndex) return;
                _cmbIndex = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand CancelCommand { get; }
        public ICommand SaveResolutionCommand { get; }
        public ICommand RemoveResolutionCommand { get; }
        public ICommand LostFocusCommand { get; }
        public ICommand SelectionChangedCommand { get; }

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
            LostFocusCommand = new RelayCommand(UpdateWidthHeighTextboxes);
            SelectionChangedCommand = new ParameterizedRelayCommand(ComboboxSelectionChangedEvent);
            RemoveResolutionCommand = new RemoveResolutionCommand(this, _saveSettingsService);
            
            // TODO: cargar

            SetUserSettings();
        }

        #region Commands
        
        #endregion
        private void ComboboxSelectionChangedEvent(object parameter)
        {
            if (!(parameter is Resolution resolution)) return;
            
            Width = resolution.Width;
            Heigth = resolution.Height;
        }

        private void UpdateWidthHeighTextboxes()
        {
            OnPropertyChanged(nameof(Width));
            OnPropertyChanged(nameof(Heigth));
        }
        
        private void SetUserSettings()
        {
            LoadResolutions();
        }

        private void LoadResolutions()
        {
            var flag = false;
            Resolutions.Clear();

            var resolutions = _loadSettingsService.LoadResolutions();

            foreach (var resolution in resolutions)
            {
                Resolutions.Add(resolution);

                if (flag) continue;
                // Load first resolution and continue
                Width = resolution.Width;
                Heigth = resolution.Height;
                flag = true;
            }

            CmbIndex = 0;
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