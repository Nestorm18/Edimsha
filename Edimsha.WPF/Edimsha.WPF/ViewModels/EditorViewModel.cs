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
using Edimsha.WPF.Commands;
using Edimsha.WPF.Commands.Basics;
using Edimsha.WPF.Commands.Editor;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.Services.Dialogs;
using Edimsha.WPF.State.Navigators;
using Edimsha.WPF.Utils;
using Edimsha.WPF.ViewModels.Contracts;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace Edimsha.WPF.ViewModels
{
    public class EditorViewModel : CommonViewModel, IFileDragDropTarget, IViewType, IExtraFolder
    {
        // Fields
        private readonly bool _isLoadingSettings;
        private EditorBackgroundWorker _editorBackgroundWorker;

        // Properties

        #region Properties

        private string _edimsha;
        private double _compresionValue;
        private int _widthImage;
        private int _heightImage;
        private string _outputFolder;

        public bool CleanListOnExit
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool>(GetViewModelType(), nameof(CleanListOnExit));
            set
            {
                UpdateSetting(nameof(CleanListOnExit), value).ConfigureAwait(false);
                OnPropertyChanged();
            }
        }

        public bool AlwaysIncludeOnReplace
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool>(GetViewModelType(), nameof(AlwaysIncludeOnReplace));
            set
            {
                UpdateSetting(nameof(AlwaysIncludeOnReplace), value).ConfigureAwait(false);
                OnPropertyChanged();
            }
        }

        public bool KeepOriginalResolution
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool>(GetViewModelType(), nameof(KeepOriginalResolution));

            set
            {
                UpdateSetting(nameof(KeepOriginalResolution), value).ConfigureAwait(false);
                OnPropertyChanged();
            }
        }

        public bool OptimizeImage
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool>(GetViewModelType(), nameof(OptimizeImage));

            set
            {
                UpdateSetting(nameof(OptimizeImage), value).ConfigureAwait(false);
                OnPropertyChanged();
            }
        }

        public bool ReplaceForOriginal
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool>(GetViewModelType(), nameof(ReplaceForOriginal));

            set
            {
                UpdateSetting(nameof(ReplaceForOriginal), value).ConfigureAwait(false);
                OnPropertyChanged();
            }
        }

        public bool IterateSubdirectories
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool>(GetViewModelType(), nameof(IterateSubdirectories));
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

                SaveSettingsService.SaveConfigurationSettings(GetViewModelType(), "Edimsha", value);

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

                SaveSettingsService.SaveConfigurationSettings(GetViewModelType(), "CompresionValue", value);

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

                SaveSettingsService.SaveConfigurationSettings(GetViewModelType(), "Width", value);

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

                SaveSettingsService.SaveConfigurationSettings(GetViewModelType(), "Height", value);

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
                SaveSettingsService.SaveConfigurationSettings(GetViewModelType(), "OutputFolder", _outputFolder);

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
            IDialogService dialogService)
            : base(loadSettingsService, saveSettingsService, dialogService)
        {
            Logger.Info("Constructor");

            // Inicialize collection
            PathList = new ObservableCollection<string>();

            // Commands
            // Mouse context
            DeleteItemCommand = new DeleteItemsCommand(this);
            DeleteAllItemsCommand = new DeleteItemsCommand(this, true);
            // Parameter buttons
            OpenImagesCommand = new OpenImagesCommand(this, DialogService, GetViewModelType());
            OpenOutputFolderCommand = new OpenOutputFolderCommand<EditorViewModel>(this, DialogService);
            OpenResolutionsDialogCommand = new OpenResolutionsDialogCommand(this, DialogService, LoadSettingsService, SaveSettingsService);
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

            var success = SaveSettingsService.SavePaths(PathList, GetViewModelType());
            if (!success) StatusBar = "error_saving_editor_paths";
        }

        private bool SetUserSettings()
        {
            Logger.Info($"Loading saved settings");
            StatusBar = "application_started";

            var isPathsDifferent = LoadSettingsService.StillPathsSameFromLastSession(GetViewModelType());
            if (!isPathsDifferent) LaunchPathChangedMessageDialog();

            ((List<string>) LoadSettingsService.GetSavedPaths(GetViewModelType()))?.ForEach(PathList.Add);
            OutputFolder = LoadSettingsService.LoadConfigurationSetting<string>(GetViewModelType(), "OutputFolder");
            Edimsha = LoadSettingsService.LoadConfigurationSetting<string>(GetViewModelType(), "Edimsha");
            WidthImage = (int) LoadSettingsService.LoadConfigurationSetting<long>(GetViewModelType(), "Width");
            HeightImage = (int) LoadSettingsService.LoadConfigurationSetting<long>(GetViewModelType(), "Height");
            CompresionValue = LoadSettingsService.LoadConfigurationSetting<double>(GetViewModelType(), "CompresionValue");

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
            var success = await SaveSettingsService.SaveConfigurationSettings(GetViewModelType(), setting, value);

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

            var paths = PathList;
            var config = LoadSettingsService.GetConfigFormViewType(GetViewModelType());

            _editorBackgroundWorker = new EditorBackgroundWorker(paths, config, new Resolution(WidthImage, HeightImage));
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
            DialogService.PathsRemovedLastSession(LoadSettingsService, SaveSettingsService, GetViewModelType());
        }

        public ViewType GetViewModelType()
        {
            return ViewType.Editor;
        }
    }
}