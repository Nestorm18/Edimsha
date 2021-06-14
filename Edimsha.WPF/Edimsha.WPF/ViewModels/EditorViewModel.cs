using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Edimsha.Core.Logging.Implementation;
using Edimsha.WPF.Commands;
using Edimsha.WPF.Converters;
using Edimsha.WPF.Lang;
using Edimsha.WPF.Models;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.Services.Dialogs;
using Edimsha.WPF.Services.Editor;
using Edimsha.WPF.State.Navigators;
using Edimsha.WPF.Utils;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
#pragma warning disable 4014

namespace Edimsha.WPF.ViewModels
{
    public class EditorViewModel : ViewModelBase, IFileDragDropTarget
    {
        // IOC
        private readonly ISaveSettingsService _saveSettingsService;
        private readonly ILoadSettingsService _loadSettingsService;
        private readonly IDialogService _dialogService;

        // Fields
        private readonly bool _isLoadingSettings;
        private EditorBackgroundWorker _editorBackgroundWorker;

        // Properties

        #region Properties

        private bool _isRunningUi;
        private bool _isStartedUi;
        private bool _isCtxDelete;
        private bool _isCtxDeleteAll;
        private string _statusBar;
        private string _statusBar2;
        private ObservableCollection<string> _urls;
        private string _outputFolder;
        private string _edimsha;
        private double _compresionValue;
        private int _widthImage;
        private int _heightImage;
        private int _pbPosition;

        public bool CleanListOnExit
        {
            get => _loadSettingsService.LoadConfigurationSetting<bool>(ViewType.Editor, nameof(CleanListOnExit));
            set
            {
                UpdateSetting(nameof(CleanListOnExit), value);
                OnPropertyChanged();
            }
        }

        public bool AlwaysIncludeOnReplace
        {
            get => _loadSettingsService.LoadConfigurationSetting<bool>(ViewType.Editor, nameof(AlwaysIncludeOnReplace));
            set
            {
                UpdateSetting(nameof(AlwaysIncludeOnReplace), value);
                OnPropertyChanged();
            }
        }

        public bool KeepOriginalResolution
        {
            get => _loadSettingsService.LoadConfigurationSetting<bool>(ViewType.Editor, nameof(KeepOriginalResolution));

            set
            {
                UpdateSetting(nameof(KeepOriginalResolution), value);
                OnPropertyChanged();
            }
        }

        public bool OptimizeImage
        {
            get => _loadSettingsService.LoadConfigurationSetting<bool>(ViewType.Editor, nameof(OptimizeImage));

            set
            {
                UpdateSetting(nameof(OptimizeImage), value);
                OnPropertyChanged();
            }
        }

        public bool ReplaceForOriginal
        {
            get => _loadSettingsService.LoadConfigurationSetting<bool>(ViewType.Editor, nameof(ReplaceForOriginal));

            set
            {
                UpdateSetting(nameof(ReplaceForOriginal), value);
                OnPropertyChanged();
            }
        }

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

        public bool IterateSubdirectories
        {
            get => _loadSettingsService.LoadConfigurationSetting<bool>(ViewType.Editor, nameof(IterateSubdirectories));
            set
            {
                UpdateSetting(nameof(IterateSubdirectories), value);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Urls
        {
            get => _urls;
            set
            {
                if (value == _urls) return;
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
                _saveSettingsService.SaveConfigurationSettings(ViewType.Editor, "OutputFolder", _outputFolder);

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

                _saveSettingsService.SaveConfigurationSettings(ViewType.Editor, "Edimsha", value);

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

                _saveSettingsService.SaveConfigurationSettings(ViewType.Editor, "CompresionValue", value);

                OnPropertyChanged();
            }
        }

        public int WidthImage
        {
            get => _widthImage;
            set
            {
                if (value == _widthImage) return;
                _widthImage = value;

                _saveSettingsService.SaveConfigurationSettings(ViewType.Editor, "Width", value);

                OnPropertyChanged();
            }
        }

        public int HeightImage
        {
            get => _heightImage;
            set
            {
                if (value == _heightImage) return;
                _heightImage = value;

                _saveSettingsService.SaveConfigurationSettings(ViewType.Editor, "Height", value);

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

        #endregion

        // Commands

        #region Commands

        public ICommand DeleteItemCommand { get; }

        public ICommand DeleteAllItemsCommand { get; }

        public ICommand OpenImagesCommand { get; }

        public ICommand OpenOutputFolderCommand { get; }

        public ICommand OpenResolutionsDialogCommand { get; }

        public ICommand ResetCommand { get; }

        public ICommand CancelCommand { get; }

        public ICommand StartCommand { get; }

        #endregion

        // Constructor
        public EditorViewModel(
            ISaveSettingsService saveSettingsService,
            ILoadSettingsService loadSettingsService,
            IDialogService dialogService)
        {
            Logger.Log("Constructor");

            _saveSettingsService = saveSettingsService;
            _loadSettingsService = loadSettingsService;
            _dialogService = dialogService;

            var ts = TranslationSource.Instance;
            ts.PropertyChanged += LanguageOnPropertyChanged;

            Urls = new ObservableCollection<string>();

            // Commands
            // Mouse context
            DeleteItemCommand = new DeleteItemsCommand(this);
            DeleteAllItemsCommand = new DeleteItemsCommand(this, true);
            // Parameter buttons
            OpenImagesCommand = new OpenImagesCommand(this, _dialogService);
            OpenOutputFolderCommand = new OpenOutputFolderCommand(this, _dialogService);
            OpenResolutionsDialogCommand = new OpenResolutionsDialogCommand(this, _dialogService, _loadSettingsService, _saveSettingsService);
            // Run buttons
            ResetCommand = new ResetEditorCommand(this);
            CancelCommand = new RelayCommand(Cancel);
            StartCommand = new RelayCommand(Start);

            // Loaded
            _isLoadingSettings = SetUserSettings();

            // Load faster all paths if is after SetUserSettings() intead new ObservableCollection<string>();
            // Example from 3,157 seconds to 0,065
            Urls.CollectionChanged += UrlsOnCollectionChanged;
        }

        public void OnFileDrop(string[] filepaths)
        {
            Logger.Log($"Filepaths: {filepaths}");

            var pathsUpdated = FileDragDropHelper.IsDirectoryDropped(filepaths.ToList(), IterateSubdirectories);

            var listCleaned =
                ListCleaner.PathWithoutDuplicatesAndGoodFormats(Urls.ToList(), pathsUpdated, ModeImageTypes.Editor);

            Urls.Clear();
            foreach (var s in listCleaned) Urls.Add(s);

            SavePaths();
        }

        internal void SavePaths()
        {
            Logger.Log("Saving paths");

            var success = _saveSettingsService.SavePathsListview(Urls, ViewType.Editor);
            if (!success) StatusBar = "error_saving_editor_paths";
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
            Logger.Log("Language changed");
            OnPropertyChanged(nameof(StatusBar));
        }

        private bool SetUserSettings()
        {
            Logger.Log($"Loading saved settings");
            StatusBar = "application_started";

            var isPathsDifferent = _loadSettingsService.StillPathsSameFromLastSession(ViewType.Editor);
            if (!isPathsDifferent) LaunchPathChangedMessageDialog();

            _loadSettingsService.LoadPathsListview(ViewType.Editor)?.ForEach(Urls.Add);
            OutputFolder = _loadSettingsService.LoadConfigurationSetting<string>(ViewType.Editor, "OutputFolder");
            Edimsha = _loadSettingsService.LoadConfigurationSetting<string>(ViewType.Editor, "Edimsha");
            WidthImage = (int) _loadSettingsService.LoadConfigurationSetting<long>(ViewType.Editor, "Width");
            HeightImage = (int) _loadSettingsService.LoadConfigurationSetting<long>(ViewType.Editor, "Height");
            CompresionValue = _loadSettingsService.LoadConfigurationSetting<double>(ViewType.Editor, "CompresionValue");

            IsRunningUi = true;

            UrlsOnCollectionChanged(null, null);

            // Configuration has finished so its false for _isLoadingSettings
            return false;
        }

        private void UrlsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Logger.Log($"Paths updated");
            if (_isLoadingSettings) return;
            var isEnabled = Urls.Count > 0;

            IsCtxDelete = isEnabled;
            IsCtxDeleteAll = isEnabled;
            IsStartedUi = isEnabled;
        }

        private async Task UpdateSetting<T>(string setting, T value)
        {
            Logger.Log($"setting: {setting}, Value: {value}");
            var success = await _saveSettingsService.SaveConfigurationSettings(ViewType.Editor, setting, value);

            if (!success) StatusBar = "the_option_could_not_be_saved";
        }

        private void Cancel()
        {
            Logger.Log("Canceled edition");
            _editorBackgroundWorker.CancelAsync();

            IsRunningUi = true;
        }

        private void Start()
        {
            Logger.Log("Started edition");
            IsRunningUi = false;

            var paths = _urls;
            var config = _loadSettingsService.GetConfigFormViewType(ViewType.Editor);

            _editorBackgroundWorker = new EditorBackgroundWorker(paths, config, new Resolution {Width = WidthImage, Height = HeightImage});
            _editorBackgroundWorker.ProgressChanged += Worker_ProgressChanged;
            _editorBackgroundWorker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            _editorBackgroundWorker.RunWorkerAsync();
        }

        // BackgroundWorker
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!(e.UserState is MyUserState state)) return;

            // Percentage calculation
            PbPosition = (int) (e.ProgressPercentage * 100 / state.CountPaths);

            StatusBar = "procesing";
            StatusBar2 = $"{e.ProgressPercentage} -> {state.CountPaths}";
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Logger.Log("Cancelled by user...");
                StatusBar = "cancelled_by_user";
                StatusBar2 = "";
            }
            else
            {
                _editorBackgroundWorker.ProgressChanged -= Worker_ProgressChanged;
                _editorBackgroundWorker.RunWorkerCompleted -= Worker_RunWorkerCompleted;
                IsRunningUi = true;
            }
        }

        // Message paths deleted
        private void LaunchPathChangedMessageDialog()
        {
            _dialogService.PathsRemovedLastSession(_loadSettingsService, ViewType.Editor);
        }
    }
}