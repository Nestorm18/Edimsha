using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Edimsha.Core.Editor;
using Edimsha.Core.Language;
using Edimsha.Core.Models;
using Edimsha.Core.Settings;
using Edimsha.Core.Utils;
using Edimsha.WPF.Commands;
using Edimsha.WPF.Commands.Basics;
using Edimsha.WPF.Commands.Editor;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.Services.Dialogs;
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

        private CancellationTokenSource _token;

        // Properties

        #region Properties

        private string _edimsha;
        private double _compresionValue;
        private int _width;
        private int _height;
        private string _outputFolder;

        public bool CleanListOnExit
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool, EditorOptions>(nameof(CleanListOnExit), _options.Value.EditorOptions);
            set
            {
                UpdateSetting(nameof(CleanListOnExit), value).ConfigureAwait(false);
                OnPropertyChanged();
            }
        }

        public bool AlwaysIncludeOnReplace
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool, EditorOptions>(nameof(AlwaysIncludeOnReplace), _options.Value.EditorOptions);
            set
            {
                UpdateSetting(nameof(AlwaysIncludeOnReplace), value).ConfigureAwait(false);
                OnPropertyChanged();
            }
        }

        public bool KeepOriginalResolution
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool, EditorOptions>(nameof(KeepOriginalResolution), _options.Value.EditorOptions);

            set
            {
                UpdateSetting(nameof(KeepOriginalResolution), value).ConfigureAwait(false);
                OnPropertyChanged();
            }
        }

        public bool OptimizeImage
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool, EditorOptions>(nameof(OptimizeImage), _options.Value.EditorOptions);

            set
            {
                UpdateSetting(nameof(OptimizeImage), value).ConfigureAwait(false);
                OnPropertyChanged();
            }
        }

        public bool ReplaceForOriginal
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool, EditorOptions>(nameof(ReplaceForOriginal), _options.Value.EditorOptions);

            set
            {
                UpdateSetting(nameof(ReplaceForOriginal), value).ConfigureAwait(false);
                OnPropertyChanged();
            }
        }

        public bool IterateSubdirectories
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool, EditorOptions>(nameof(IterateSubdirectories), _options.Value.EditorOptions);
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

        public async void SavePaths()
        {
            Logger.Info("Saving paths");

            var success = await SaveSettingsService.SaveConfigurationSettings<List<string>, EditorOptions>("Paths", PathList.ToList(), _options.Value.EditorOptions);
            if (!success) StatusBar = "error_saving_editor_paths";
        }

        private bool SetUserSettings()
        {
            Logger.Info($"Loading saved settings");
            StatusBar = "application_started";

            var isPathsDifferent = LoadSettingsService.StillPathsSameFromLastSession<EditorOptions>(_options.Value.EditorOptions);
            if (!isPathsDifferent) LaunchPathChangedMessageDialog();

            LoadSettingsService.LoadConfigurationSetting<List<string>, EditorOptions>("Paths", _options.Value.EditorOptions)?.ForEach(PathList.Add);
            Edimsha = LoadSettingsService.LoadConfigurationSetting<string, EditorOptions>(nameof(Edimsha), _options.Value.EditorOptions);
            CompresionValue = LoadSettingsService.LoadConfigurationSetting<double, EditorOptions>(nameof(CompresionValue), _options.Value.EditorOptions);
            Width = LoadSettingsService.LoadConfigurationSetting<Resolution, EditorOptions>("Resolution", _options.Value.EditorOptions).Width;
            Height = LoadSettingsService.LoadConfigurationSetting<Resolution, EditorOptions>("Resolution", _options.Value.EditorOptions).Height;
            OutputFolder = LoadSettingsService.LoadConfigurationSetting<string, EditorOptions>(nameof(OutputFolder), _options.Value.EditorOptions);
            
            IsRunningUi = true;

            UrlsOnCollectionChanged(null, null);

            // Configuration has finished so its false for _isLoadingSettings
            return false;
        }

        private void UrlsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isLoadingSettings) return;
            var isEnabled = PathList.Count > 0;

            IsCtxDelete = isEnabled;
            IsCtxDeleteAll = isEnabled;
            IsStartedUi = isEnabled;
        }

        private async Task UpdateSetting<T>(string setting, T value)
        {
            Logger.Info($"setting: {setting}, Value: {value}");

            var success = await SaveSettingsService.SaveConfigurationSettings<T, EditorOptions>(setting, value, _options.Value.EditorOptions);

            if (!success) StatusBar = "the_option_could_not_be_saved";
        }

        private void Cancel()
        {
            Logger.Info("Canceled edition");
            _token.Cancel();
            IsRunningUi = true;
        }

        private async void Start()
        {
            Logger.Info("Started edition");
            IsRunningUi = false;

            var config = LoadSettingsService.GetFullConfig<EditorOptions>(_options.Value.EditorOptions);
            config.Paths = PathList.ToList();

            var editor = new Editor(config);
            var progress = new Progress<ProgressReport>();
            progress.ProgressChanged += ProgressOnProgressChanged;
            _token = new CancellationTokenSource();

            await Task.Run(() => { editor.ExecuteProcessing(progress, _token); });
        }

        private void ProgressOnProgressChanged(object sender, ProgressReport e)
        {
            switch (e.ReportType)
            {
                case ReportType.Percent:
                    PbPosition = (int) e.Data;
                    break;
                case ReportType.Message:
                    StatusBar = $"{TranslationSource.Instance["procesing"]}: {(string) e.Data}";
                    break;
                case ReportType.Finalizated:
                    PbPosition = 100;
                    IsRunningUi = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Message paths deleted
        private void LaunchPathChangedMessageDialog()
        {
            DialogService.PathsRemovedLastSession<EditorOptions>(LoadSettingsService, SaveSettingsService, _options.Value.EditorOptions);
        }

        public ViewType GetViewModelType()
        {
            return ViewType.Editor;
        }
    }
}