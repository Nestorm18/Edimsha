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

        // Property to return as Resolution
        public Resolution? GetResolution()
        {
            if (Width == -1 || Heigth == -1) return null;

            return new Resolution
            {
                Width = Width,
                Height = Heigth
            };
        }

        // Properties
        private bool _hasValidResolutions;
        private ObservableCollection<Resolution> _resolutions = null!;
        private int _cmbIndex;
        private int _width;
        private int _heigth;
        private string _errorMessage = null!;

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

        public int Width
        {
            get => _width;
            set
            {
                if ((value == _width || value <= 0) && value != -1) return;

                _width = value;
                OnPropertyChanged();
            }
        }

        public int Heigth
        {
            get => _heigth;
            set
            {
                if ((value == _heigth || value <= 0) && value != -1) return;
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

        // Commands
        public ICommand SelectionChangedCommand { get; }
       
        public ICommand SaveResolutionCommand { get; }
        
        public ICommand RemoveResolutionCommand { get; }
        
        public ICommand CancelCommand { get; }
       
        public ICommand AcceptCommand { get; }
       

        public ResolutionDialogViewModel(
            ILoadSettingsService loadSettingsService,
            ISaveSettingsService saveSettingsService)
        {
            // TODO: aniadir documentacion
            _loadSettingsService = loadSettingsService;
            ISaveSettingsService saveSettingsService1 = saveSettingsService;

            Resolutions = new ObservableCollection<Resolution>();
            Resolutions.CollectionChanged += ResolutionsOnCollectionChanged;

            // Commands
            SelectionChangedCommand = new ParameterizedRelayCommand(ComboboxSelectionChangedEvent);
            SaveResolutionCommand = new SaveResolutionCommand(this, saveSettingsService1);
            RemoveResolutionCommand = new RemoveResolutionCommand(this, saveSettingsService1);
            CancelCommand = new QuitResolutionsCommand(this);
            AcceptCommand = new AcceptResolutionCommand();

            SetUserSettings();
        }

        #region Commands

        #endregion

        private void ResolutionsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (Resolutions.Count > 0) HasValidResolutions = true;
        }

        private void ComboboxSelectionChangedEvent(object parameter)
        {
            if (!(parameter is Resolution resolution)) return;

            Width = resolution.Width;
            Heigth = resolution.Height;
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
    }
}