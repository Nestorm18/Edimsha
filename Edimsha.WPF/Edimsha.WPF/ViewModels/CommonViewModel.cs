using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Edimsha.Core.Language;
using Edimsha.WPF.Converters;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.Services.Dialogs;

namespace Edimsha.WPF.ViewModels
{
    public class CommonViewModel : ViewModelBase
    {
        // IOC
        protected readonly ILoadSettingsService LoadSettingsService;
        protected readonly ISaveSettingsService SaveSettingsService;
        protected readonly IDialogService DialogService;

        // Properties

        #region Properties

        private bool _isRunningUi;
        private bool _isStartedUi;
        private bool _isCtxDelete;
        private bool _isCtxDeleteAll;
        private string _statusBar;
        private string _statusBar2;
        private int _pbPosition;
        private ObservableCollection<string> _pathList = null!;

        public bool IsRunningUi
        {
            get => _isRunningUi;
            set
            {
                _isRunningUi = value;
                IsStartedUi = value;
                OnPropertyChanged();
            }
        }

        public bool IsStartedUi
        {
            get => _isStartedUi;
            set
            {
                if (value == _isStartedUi) return;
                _isStartedUi = value;
                OnPropertyChanged();
            }
        }

        public bool IsCtxDelete
        {
            get => _isCtxDelete;
            set
            {
                _isCtxDelete = value;
                OnPropertyChanged();
            }
        }

        public bool IsCtxDeleteAll
        {
            get => _isCtxDeleteAll;
            set
            {
                _isCtxDeleteAll = value;
                OnPropertyChanged();
            }
        }

        public string StatusBar
        {
            get => _statusBar;
            set
            {
                _statusBar = value;
                OnPropertyChanged();
            }
        }

        public string StatusBar2
        {
            get => _statusBar2;
            set
            {
                _statusBar2 = value;
                OnPropertyChanged();
            }
        }

        public int PbPosition
        {
            get => _pbPosition;
            set
            {
                if (value == _pbPosition) return;
                _pbPosition = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> PathList
        {
            get => _pathList;
            set
            {
                if (value == _pathList) return;
                _pathList = value;
                OnPropertyChanged();
            }
        }

        #endregion

        // Commands

        #region Commands

        public ICommand DeleteItemCommand { get; protected init; }

        public ICommand DeleteAllItemsCommand { get; protected init; }

        public ICommand OpenImagesCommand { get; protected init; }

        public ICommand OpenOutputFolderCommand { get; protected init; }

        public ICommand ResetCommand { get; protected init; }

        public ICommand CancelCommand { get; protected init; }

        public ICommand StartCommand { get; protected init; }

        #endregion

        protected CommonViewModel(
            ILoadSettingsService loadSettingsService,
            ISaveSettingsService saveSettingsService,
            IDialogService dialogService)
        {
            Logger.Info("Constructor");
            
            LoadSettingsService = loadSettingsService;
            SaveSettingsService = saveSettingsService;
            DialogService = dialogService;
            
            var ts = TranslationSource.Instance;
            ts.PropertyChanged += LanguageOnPropertyChanged;
            
        }
        
        /// <summary>
        /// Using code behind translation, the "OnPropertyChanged" property of the element
        /// that will be updated when changing languages must be called. 
        /// <para>Example:</para>
        /// Set text when UI starts; set new value you need if you update your text in any part on the viewmodel
        /// passing the translation key.
        /// <code>StatusBar = "application_started";</code>
        /// To update the current showing text to new language you must add you property like this.
        /// <code>OnPropertyChanged(nameof(StatusBar));</code>
        /// <para>NOTE: Use <see cref="LangKeyToTranslationConverter"/> in your text binding in XAML.</para>
        /// </summary>
        private void LanguageOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Logger.Info("Language changed");
            OnPropertyChanged(nameof(StatusBar));
        }

    }
}