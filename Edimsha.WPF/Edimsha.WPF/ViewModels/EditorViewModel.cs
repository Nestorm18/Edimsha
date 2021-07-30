using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Edimsha.Core.Editor;
using Edimsha.Core.Models;
using Edimsha.Core.Settings;
using Edimsha.WPF.Commands;
using Edimsha.WPF.Commands.Basics;
using Edimsha.WPF.Commands.Editor;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.Services.Dialogs;
using Edimsha.WPF.State.Navigators;
using Edimsha.WPF.Utils;
using Edimsha.WPF.ViewModels.Contracts;
using Edimsha.WPF.ViewModels.ViewModelCommon;
using Microsoft.Extensions.Options;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace Edimsha.WPF.ViewModels
{
    public class EditorViewModel : CommonViewModel, IFileDragDropTarget, IViewType, IExtraProperties
    {
        //IOC
        private readonly IOptions<ConfigPaths> _options;

        // Fields
        private readonly bool _isLoadingSettings;
        private EditorBackgroundWorker _editorBackgroundWorker;

        // Properties

        #region Properties

        private string _edimsha;
        private double _compresionValue;
        private int _width;
        private int _height;
        private string _outputFolder;

        public bool CleanListOnExit
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool, EditorConfig>(nameof(CleanListOnExit), _options.Value.EditorConfig);
            set
            {
                UpdateSetting(nameof(CleanListOnExit), value).ConfigureAwait(false);
                OnPropertyChanged();
            }
        }

        public bool AlwaysIncludeOnReplace
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool, EditorConfig>(nameof(AlwaysIncludeOnReplace), _options.Value.EditorConfig);
            set
            {
                UpdateSetting(nameof(AlwaysIncludeOnReplace), value).ConfigureAwait(false);
                OnPropertyChanged();
            }
        }

        public bool KeepOriginalResolution
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool, EditorConfig>(nameof(KeepOriginalResolution), _options.Value.EditorConfig);

            set
            {
                UpdateSetting(nameof(KeepOriginalResolution), value).ConfigureAwait(false);
                OnPropertyChanged();
            }
        }

        public bool OptimizeImage
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool, EditorConfig>(nameof(OptimizeImage), _options.Value.EditorConfig);

            set
            {
                UpdateSetting(nameof(OptimizeImage), value).ConfigureAwait(false);
                OnPropertyChanged();
            }
        }

        public bool ReplaceForOriginal
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool, EditorConfig>(nameof(ReplaceForOriginal), _options.Value.EditorConfig);

            set
            {
                UpdateSetting(nameof(ReplaceForOriginal), value).ConfigureAwait(false);
                OnPropertyChanged();
            }
        }

        public bool IterateSubdirectories
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool, EditorConfig>(nameof(IterateSubdirectories), _options.Value.EditorConfig);
            set
            {
                UpdateSetting(nameof(IterateSubdirectories), value).ConfigureAwait(false);
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

                UpdateSetting(nameof(Edimsha), value).ConfigureAwait(false);

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

                UpdateSetting(nameof(CompresionValue), value).ConfigureAwait(false);

                OnPropertyChanged();
            }
        }

        public int Width
        {
            get => _width;
            set
            {
                if (value == _width) return;
                _width = value;

                FixResolutionLoading(Height, value);

                OnPropertyChanged();
            }
        }
        
        public int Height
        {
            get => _height;
            set
            {
                if (value == _height) return;
                _height = value;

                FixResolutionLoading(Width, value);

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
                UpdateSetting(nameof(OutputFolder), value).ConfigureAwait(false);

                OnPropertyChanged();
            }
        }

        #endregion

        // Commands

        #region Commands

        public ICommand OpenResolutionsDialogCommand { get; }

        #endregion

        // Constructor
        public EditorViewModel(
            ISaveSettingsService saveSettingsService,
            ILoadSettingsService loadSettingsService,
            IDialogService dialogService,
            IOptions<ConfigPaths> options)
            : base(loadSettingsService, saveSettingsService, dialogService)
        {
            Logger.Info("Constructor");
           
            //IOC
            _options = options;

            // Inicialize collection
            PathList = new ObservableCollection<string>();

            // Commands
            // Mouse context
            DeleteItemCommand = new DeleteItemsCommand(this);
            DeleteAllItemsCommand = new DeleteItemsCommand(this, true);
            // Parameter buttons
            OpenImagesCommand = new OpenImagesCommand(this, DialogService, GetViewModelType());
            OpenOutputFolderCommand = new OpenOutputFolderCommand<EditorViewModel>(this, DialogService);
            OpenResolutionsDialogCommand = new OpenResolutionsDialogCommand(this, DialogService, LoadSettingsService, SaveSettingsService, options);
            // Run buttons
            ResetCommand = new ResetEditorCommand(this);
            CancelCommand = new RelayCommand(Cancel);
            StartCommand = new RelayCommand(Start);

            // Loaded
            _isLoadingSettings = SetUserSettings();

            // Load faster all paths if is after SetUserSettings() intead new ObservableCollection<string>();
            // Example from 3,157 seconds to 0,065
            PathList.CollectionChanged += UrlsOnCollectionChanged;
        }

        public void OnFileDrop(string[] filepaths)
        {
            Logger.Info($"Filepaths: {filepaths}");

            var pathsUpdated = FileDragDropHelper.IsDirectoryDropped(filepaths.ToList(), IterateSubdirectories);

            var listCleaned = ListCleaner.PathWithoutDuplicatesAndGoodFormats(
                PathList.ToList(),
                pathsUpdated,
                GetViewModelType());

            PathList.Clear();
            foreach (var s in listCleaned) PathList.Add(s);

            // Fix not loading start button on drop after reset
            UrlsOnCollectionChanged(null, null);

            SavePaths();
        }

        internal void SavePaths()
        {
            Logger.Info("Saving paths");

            var success = SaveSettingsService.SaveListToFile(PathList, _options.Value.EditorPaths).Result;
            if (!success) StatusBar = "error_saving_editor_paths";
        }

        private bool SetUserSettings()
        {
            Logger.Info($"Loading saved settings");
            StatusBar = "application_started";

            var isPathsDifferent = LoadSettingsService.StillPathsSameFromLastSession(_options.Value.EditorPaths);
            if (!isPathsDifferent) LaunchPathChangedMessageDialog();

            ((List<string>) LoadSettingsService.GetSavedPaths(_options.Value.EditorPaths))?.ForEach(PathList.Add);
            OutputFolder = LoadSettingsService.LoadConfigurationSetting<string, EditorConfig>(nameof(OutputFolder), _options.Value.EditorConfig);
            Edimsha = LoadSettingsService.LoadConfigurationSetting<string, EditorConfig>(nameof(Edimsha), _options.Value.EditorConfig);
            Width = LoadSettingsService.LoadConfigurationSetting<Resolution, EditorConfig>(nameof(Resolution), _options.Value.EditorConfig).Width;
            Height = LoadSettingsService.LoadConfigurationSetting<Resolution, EditorConfig>(nameof(Resolution), _options.Value.EditorConfig).Height;
            CompresionValue = LoadSettingsService.LoadConfigurationSetting<double, EditorConfig>(nameof(CompresionValue), _options.Value.EditorConfig);

            IsRunningUi = true;

            UrlsOnCollectionChanged(null, null);

            // Configuration has finished so its false for _isLoadingSettings
            return false;
        }

        private void UrlsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Logger.Info($"Paths updated");
            if (_isLoadingSettings) return;
            var isEnabled = PathList.Count > 0;

            IsCtxDelete = isEnabled;
            IsCtxDeleteAll = isEnabled;
            IsStartedUi = isEnabled;
        }

        private async Task UpdateSetting<T>(string setting, T value)
        {
            Logger.Info($"setting: {setting}, Value: {value}");

            var success = await SaveSettingsService.SaveConfigurationSettings<T, EditorConfig>(setting, value, _options.Value.EditorConfig);

            if (!success) StatusBar = "the_option_could_not_be_saved";
        }

        private void Cancel()
        {
            Logger.Info("Canceled edition");
            _editorBackgroundWorker.CancelAsync();

            IsRunningUi = true;
        }

        private void Start()
        {
            Logger.Info("Started edition");
            IsRunningUi = false;

            var config = LoadSettingsService.GetFullConfig<EditorConfig>(_options.Value.EditorConfig);

            _editorBackgroundWorker = new EditorBackgroundWorker(PathList, config);
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
                Logger.Info("Cancelled by user...");
                StatusBar = "cancelled_by_user";
                StatusBar2 = "";
            }
            else
            {
                _editorBackgroundWorker.ProgressChanged -= Worker_ProgressChanged;
                _editorBackgroundWorker.RunWorkerCompleted -= Worker_RunWorkerCompleted;
                PbPosition = 100;
                IsRunningUi = true;
            }
        }

        // Message paths deleted
        private void LaunchPathChangedMessageDialog()
        {
            DialogService.PathsRemovedLastSession(LoadSettingsService, SaveSettingsService, _options.Value.EditorPaths);
        }

        public ViewType GetViewModelType()
        {
            return ViewType.Editor;
        }
        
        private void FixResolutionLoading(int property, int value)
        {
            if (property <= 0) return;

            var newResolution = new Resolution(value, property);

            UpdateSetting(nameof(Resolution), newResolution).ConfigureAwait(false);
        }
    }
}