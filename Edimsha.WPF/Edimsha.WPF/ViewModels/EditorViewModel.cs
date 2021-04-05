using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
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
        private bool _alwaysIncludeOnReplace;
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
        private string _outputFolder;
        private string _edimsha;
        private double _compresionValue;

        public bool CleanListOnExit
        {
            get => _cleanListOnExit;
            set
            {
                _cleanListOnExit = value;
                OnPropertyChanged();
            }
        }

        public bool AlwaysIncludeOnReplace
        {
            get => _alwaysIncludeOnReplace;
            set
            {
                _alwaysIncludeOnReplace = value;
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

        public string OutputFolder
        {
            get => _outputFolder;
            set
            {
                if (value == _outputFolder) return;

                _outputFolder = Directory.Exists(value) ? value : string.Empty;
                _saveSettingsService.SaveConfigurationSettings("OutputFolder", _outputFolder);
                
                OnPropertyChanged();
            }
        }

        public string Edimsha
        {
            get => _edimsha;
            set
            {
                if (value == _edimsha) return;
                _edimsha = value;
                
                _saveSettingsService.SaveConfigurationSettings("Edimsha", value);
                
                OnPropertyChanged();
            }
        }

        public double CompresionValue
        {
            get => _compresionValue;
            set
            {
                if (value.Equals(_compresionValue)) return;
                _compresionValue = value;
               
                _saveSettingsService.SaveConfigurationSettings("CompresionValue", value);
               
                OnPropertyChanged();
            }
        }

        #endregion

        // Commands

        #region Commands
        
        public ICommand DeleteItemCommand { get; }
        
        public ICommand DeleteAllItemsCommand { get; }
        
        public ICommand CleanListOnExitCommand { get; }
        
        public ICommand IterateSubdirectoriesCommand { get; }
        
        public ICommand OpenImagesCommand { get; }
        
        public ICommand OpenOutputFolderCommand { get; }
        
        public ICommand AlwaysIncludeOnReplaceCommand { get; }
       
        public ICommand KeepOriginalResolutionCommand { get; }
      
        public ICommand OptimizeImageCommand { get; }
        
        public ICommand ReplaceForOriginalCommand { get; }
        
        #endregion

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
            OpenOutputFolderCommand = new OpenOutputFolderCommand(this, _dialogService);
            AlwaysIncludeOnReplaceCommand = new SaveSettingsCommand(async () => await UpdateSetting("AlwaysIncludeOnReplace", AlwaysIncludeOnReplace));
            KeepOriginalResolutionCommand = new SaveSettingsCommand(async () => await UpdateSetting("KeepOriginalResolution", KeepOriginalResolution));
            OptimizeImageCommand = new SaveSettingsCommand(async () => await UpdateSetting("OptimizeImage", OptimizeImage));
            ReplaceForOriginalCommand = new SaveSettingsCommand(async () => await UpdateSetting("ReplaceForOriginal", ReplaceForOriginal));

            // Loaded
            _isLoadingSettings = SetUserSettings();
        }

        public void OnFileDrop(string[] filepaths)
        {
            var pathsUpdated = FileDragDropHelper.IsDirectoryDropped(filepaths.ToList(), IterateSubdirectories);

            var listCleaned = ListCleaner.PathWithoutDuplicatesAndGoodFormats(Urls.ToList(), pathsUpdated, ModeImageTypes.Editor);

            Urls.Clear();
            foreach (var s in listCleaned) Urls.Add(s);

            SavePaths();
        }

        private void SavePaths()
        {
            var success = _saveSettingsService.SavePathsListview(Urls, ViewType.Editor);
            if (!success.Result) StatusBar = "error_saving_editor_paths";
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
            OutputFolder = _loadSettingsService.LoadConfigurationSetting<string>("OutputFolder");
            Edimsha = _loadSettingsService.LoadConfigurationSetting<string>("Edimsha");
            AlwaysIncludeOnReplace = _loadSettingsService.LoadConfigurationSetting<bool>("AlwaysIncludeOnReplace");
            KeepOriginalResolution = _loadSettingsService.LoadConfigurationSetting<bool>("KeepOriginalResolution");
            CompresionValue = _loadSettingsService.LoadConfigurationSetting<double>("CompresionValue");
            OptimizeImage = _loadSettingsService.LoadConfigurationSetting<bool>("OptimizeImage");
            ReplaceForOriginal = _loadSettingsService.LoadConfigurationSetting<bool>("ReplaceForOriginal");

            IsRunningUi = true;

            UrlsOnCollectionChanged(null, null);

            // Configuration has finished so its false for _isLoadingSettings
            return false;
        }

        private void UrlsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isLoadingSettings) return;
            IsCtxDelete = Urls.Count > 0;
            IsCtxDeleteAll = Urls.Count > 0;
        }

        private async Task UpdateSetting<T>(string setting, T value)
        {
            var success = await _saveSettingsService.SaveConfigurationSettings(setting, value);

            if (!success) StatusBar = "the_option_could_not_be_saved";
        }
    }
}