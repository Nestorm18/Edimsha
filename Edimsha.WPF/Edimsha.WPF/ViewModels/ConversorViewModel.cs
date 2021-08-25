using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Edimsha.Core.Conversor;
using Edimsha.Core.Language;
using Edimsha.Core.Models;
using Edimsha.Core.Settings;
using Edimsha.Core.Utils;
using Edimsha.WPF.Commands;
using Edimsha.WPF.Commands.Basics;
using Edimsha.WPF.Commands.Conversor;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.Services.Dialogs;
using Edimsha.WPF.Utils;
using Edimsha.WPF.ViewModels.Contracts;
using Edimsha.WPF.ViewModels.ViewModelCommon;
using Microsoft.Extensions.Options;

namespace Edimsha.WPF.ViewModels
{
    public class ConversorViewModel : CommonViewModel, IFileDragDropTarget, IViewType, IExtraProperties
    {
        // IOC
        private IOptions<ConfigPaths> _options;

        // Fields
        private readonly bool _isLoadingSettings;
        private ObservableCollection<string> _imageFormats = null!;
        private CancellationTokenSource _token;

        // Properties

        #region Properties

        private string _outputFolder;
        private string _edimsha;
        private ImageTypesConversor _currentFormat;

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

        public bool IterateSubdirectories
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool, ConversorOptions>(nameof(IterateSubdirectories), _options.Value.ConversorOptions);
            set
            {
                UpdateSetting(nameof(IterateSubdirectories), value).ConfigureAwait(false);
                OnPropertyChanged();
            }
        }

        public bool CleanListOnExit
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool, ConversorOptions>(nameof(CleanListOnExit), _options.Value.ConversorOptions);
            set
            {
                UpdateSetting(nameof(CleanListOnExit), value).ConfigureAwait(false);
                OnPropertyChanged();
            }
        }

        public int CurrentIndex
        {
            get => LoadSettingsService.LoadConfigurationSetting<int, ConversorOptions>(nameof(CurrentIndex), _options.Value.ConversorOptions);
            set
            {
                UpdateSetting(nameof(CurrentIndex), value).ConfigureAwait(false);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> ImageFormats
        {
            get => _imageFormats;
            set
            {
                if (value == _imageFormats) return;
                _imageFormats = value;
                OnPropertyChanged();
            }
        }

        public ImageTypesConversor CurrentFormat
        {
            get => _currentFormat;
            set
            {
                UpdateSetting(nameof(CurrentFormat), value).ConfigureAwait(false);
                OnPropertyChanged();
            }
        }

        # endregion

        // Commands

        #region Commands

        # endregion

        public ConversorViewModel(
            ISaveSettingsService saveSettingsService,
            ILoadSettingsService loadSettingsService,
            IDialogService dialogService,
            IOptions<ConfigPaths> options)
            : base(loadSettingsService, saveSettingsService, dialogService)
        {
            _options = options;
            Logger.Info("Constructor");

            PathList = new ObservableCollection<string>();
            ImageFormats = new ObservableCollection<string>();

            // Commands
            // Mouse context
            DeleteItemCommand = new DeleteItemsCommand(this);
            DeleteAllItemsCommand = new DeleteItemsCommand(this, true);
            // Parameter buttons
            OpenImagesCommand = new OpenImagesCommand(this, DialogService, GetViewModelType());
            OpenOutputFolderCommand = new OpenOutputFolderCommand<ConversorViewModel>(this, DialogService);
            // Run buttons
            ResetCommand = new ResetConversorCommand(this);
            CancelCommand = new RelayCommand(Cancel);
            StartCommand = new RelayCommand(Start);

            // Loaded
            _isLoadingSettings = SetUserSettings();

            PathList.CollectionChanged += UrlsOnCollectionChanged;
        }

        private bool SetUserSettings()
        {
            Logger.Info("Loading saved settings");
            StatusBar = "application_started";

            var isPathsDifferent = LoadSettingsService.StillPathsSameFromLastSession<ConversorOptions>(_options.Value.ConversorOptions);
            if (!isPathsDifferent) LaunchPathChangedMessageDialog();

            LoadSettingsService.LoadConfigurationSetting<List<string>, ConversorOptions>("Paths", _options.Value.ConversorOptions)?.ForEach(PathList.Add);
            Edimsha = LoadSettingsService.LoadConfigurationSetting<string, ConversorOptions>(nameof(Edimsha), _options.Value.ConversorOptions);
            OutputFolder = LoadSettingsService.LoadConfigurationSetting<string, ConversorOptions>(nameof(OutputFolder), _options.Value.ConversorOptions);

            FillAvaliableConvertTo();

            IsRunningUi = true;

            UrlsOnCollectionChanged(null, null);

            // Configuration has finished so its false for _isLoadingSettings
            return false;
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

        private void FillAvaliableConvertTo()
        {
            var imageType = ImageFormatsFromViewType.GetImageType(ViewType.Conversor);

            if (imageType == null) return;

            foreach (var type in imageType)
            {
                var format = $"*.{type.ToString()?.ToLower()}";
                ImageFormats.Add(format);
            }
        }

        public async void SavePaths()
        {
            Logger.Info("Saving paths");

            var success = await SaveSettingsService.SaveConfigurationSettings<List<string>, ConversorOptions>("Paths", PathList.ToList(), _options.Value.ConversorOptions);
            if (!success) StatusBar = "error_saving_editor_paths";
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

            var success = await SaveSettingsService.SaveConfigurationSettings<T, ConversorOptions>(setting, value, _options.Value.ConversorOptions);

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

            var config = LoadSettingsService.GetFullConfig<ConversorOptions>(_options.Value.ConversorOptions);

            var editor = new Conversor(config);
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
            DialogService.PathsRemovedLastSession<ConversorOptions>(LoadSettingsService, SaveSettingsService, _options.Value.ConversorOptions);
        }

        public ViewType GetViewModelType()
        {
            return ViewType.Conversor;
        }
    }
}