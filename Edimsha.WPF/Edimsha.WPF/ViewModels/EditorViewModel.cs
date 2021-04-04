using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Edimsha.WPF.Commands;
using Edimsha.WPF.Converters;
using Edimsha.WPF.Lang;
using Edimsha.WPF.Models;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.Services.Dialogs;
using Edimsha.WPF.State.Navigators;
using Edimsha.WPF.Utils;

namespace Edimsha.WPF.ViewModels
{
    public class EditorViewModel : ViewModelBase, IFileDragDropTarget
    {
        // IOC
        private readonly ISaveSettingsService _saveSettingsService;
        private readonly ILoadSettingsService _loadSettingsService;
        private readonly IDialogService _dialogService;

        // Fields
        
        private bool _isLoadingSettings;

        // Properties

        #region Properties

        private bool _cleanListOnExit;
        private bool _addOnReplace;
        private bool _keepOriginalResolution;
        private bool _optimizeImage;
        private bool _replaceForOriginal;
        private bool _isRunningUi;
        private bool _isStartedUi;
        private bool _isCtxDelete;
        private bool _isCtxDeleteAll;
        private string _statusBar;
        private bool _iterateSubdirectories;
        private ObservableCollection<string> _urls;

        public bool CleanListOnExit
        {
            get => _cleanListOnExit;
            set
            {
                _cleanListOnExit = value;
                OnPropertyChanged();
            }
        }

        public bool AddOnReplace
        {
            get => _addOnReplace;
            set
            {
                _addOnReplace = value;
                OnPropertyChanged();
            }
        }

        public bool KeepOriginalResolution
        {
            get => _keepOriginalResolution;

            set
            {
                _keepOriginalResolution = value;
                OnPropertyChanged();
            }
        }

        public bool OptimizeImage
        {
            get => _optimizeImage;

            set
            {
                _optimizeImage = value;
                OnPropertyChanged();
            }
        }

        public bool ReplaceForOriginal
        {
            get => _replaceForOriginal;

            set
            {
                _replaceForOriginal = value;
                OnPropertyChanged();
            }
        }

        public bool IsRunningUi
        {
            get => _isRunningUi;
            set
            {
                _isRunningUi = value;
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

        public bool IterateSubdirectories
        {
            get => _iterateSubdirectories;
            set
            {
                _iterateSubdirectories = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Urls
        {
            get => _urls;
            set
            {
                if (Equals(value, _urls)) return;
                _urls = value;
                OnPropertyChanged();
            }
        }

        #endregion

        // Commands
        public ICommand DeleteItemCommand { get; }
        public ICommand DeleteAllItemsCommand { get; }
        public ICommand CleanListOnExitCommand { get; }
        public ICommand IterateSubdirectoriesCommand { get; }
        public ICommand OpenImagesCommand { get; }

        // Constructor
        public EditorViewModel(
            ISaveSettingsService saveSettingsService,
            ILoadSettingsService loadSettingsService,
            IDialogService dialogService)
        {
            _saveSettingsService = saveSettingsService;
            _loadSettingsService = loadSettingsService;
            _dialogService = dialogService;

            var ts = TranslationSource.Instance;
            ts.PropertyChanged += LanguageOnPropertyChanged;

            Urls = new ObservableCollection<string>();
            Urls.CollectionChanged += UrlsOnCollectionChanged;

            // Commands
            DeleteItemCommand = new DeleteItemsCommand(this);
            DeleteAllItemsCommand = new DeleteItemsCommand(this, true);
            CleanListOnExitCommand = new SaveSettingsCommand(async () => await UpdateSetting("CleanListOnExit", CleanListOnExit));
            IterateSubdirectoriesCommand = new SaveSettingsCommand(async () => await UpdateSetting("IterateSubdirectories", IterateSubdirectories));
            OpenImagesCommand = new OpenImagesCommand(this, _dialogService);

            // Loaded
            _isLoadingSettings = SetUserSettings();
        }
        
        public void OnFileDrop(string[] filepaths)
        {
            var pathsUpdated = FileDragDropHelper.IsDirectoryDropped(filepaths.ToList(), IterateSubdirectories);

            var listCleaned = ListCleaned.PathWithoutDuplicatesAndGoodFormats(Urls.ToList(), pathsUpdated, ModeImageTypes.Editor);

            Urls.Clear();
            foreach (var s in listCleaned) Urls.Add(s);
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
            OnPropertyChanged(nameof(StatusBar));
        }

        private bool SetUserSettings()
        {
            StatusBar = "application_started";

            _loadSettingsService.LoadPathsListview(ViewType.Editor)?.ForEach(Urls.Add);
            CleanListOnExit = _loadSettingsService.LoadConfigurationSetting<bool>("CleanListOnExit");
            IterateSubdirectories = _loadSettingsService.LoadConfigurationSetting<bool>("IterateSubdirectories");

            IsRunningUi = true;
            
            UrlsOnCollectionChanged(null, null);

            // Configuration has finished
            return true;
        }

        private void UrlsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isLoadingSettings) return;
            IsCtxDelete = Urls.Count > 0;
            IsCtxDeleteAll = Urls.Count > 0;

            //FIXME: Guardar solo al salir, si esto consume muchos recursos
            var success = _saveSettingsService.SavePathsListview(Urls, ViewType.Editor);

            if (!success.Result) StatusBar = "error_saving_editor_paths";
        }

        private async Task UpdateSetting<T>(string setting, T value)
        {
            var success = await _saveSettingsService.SaveConfigurationSettings(setting, value);
            
            if (!success) StatusBar = "the_option_could_not_be_saved";
        }
    }
}