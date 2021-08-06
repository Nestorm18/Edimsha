using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Edimsha.Core.Conversor;
using Edimsha.Core.Settings;
using Edimsha.WPF.Commands;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.Services.Dialogs;
using Edimsha.WPF.State.Navigators;
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

        // Properties

        #region Properties

        private string _outputFolder;

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

        public bool IterateSubdirectories
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool, ConversorConfig>(nameof(IterateSubdirectories), _options.Value.ConversorConfig);
            set
            {
                UpdateSetting(nameof(IterateSubdirectories), value).ConfigureAwait(false);
                OnPropertyChanged();
            }
        }

        public bool CleanListOnExit
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool, ConversorConfig>(nameof(CleanListOnExit), _options.Value.ConversorConfig);
            set
            {
                UpdateSetting(nameof(CleanListOnExit), value).ConfigureAwait(false);
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
        
        # endregion

        // Commands

        #region Commands

        # endregion

        public ConversorViewModel(
            ISaveSettingsService saveSettingsService,
            ILoadSettingsService loadSettingsService,
            IDialogService dialogService,
            IOptions<ConfigPaths> options)
            : base(
                loadSettingsService,
                saveSettingsService,
                dialogService)
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

            // Loaded
            _isLoadingSettings = SetUserSettings();

            PathList.CollectionChanged += UrlsOnCollectionChanged;
        }

        private bool SetUserSettings()
        {
            Logger.Info("Loading saved settings");
            StatusBar = "application_started";

            var isPathsDifferent = LoadSettingsService.StillPathsSameFromLastSession(_options.Value.ConversorPaths);
            if (!isPathsDifferent) LaunchPathChangedMessageDialog();
            
            ((List<string>) LoadSettingsService.GetSavedPaths(_options.Value.ConversorPaths))?.ForEach(PathList.Add);

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
            var imageType = ImageFormatsFromViewType.GetImageType(ViewType.Converter);
            
            if (imageType == null) return;
           
            foreach (var type in imageType)
            {
                var format = $"*.{type.ToString()?.ToLower()}";
                ImageFormats.Add(format);
            }
        }

        private void SavePaths()
        {
            Logger.Info("Saving paths");

            var success = SaveSettingsService.SaveListToFile(PathList, _options.Value.ConversorPaths);
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

            var success = await SaveSettingsService.SaveConfigurationSettings<T, ConversorConfig>(setting, value, _options.Value.ConversorConfig);

            if (!success) StatusBar = "the_option_could_not_be_saved";
        }

        // Message paths deleted
        private void LaunchPathChangedMessageDialog()
        {
            DialogService.PathsRemovedLastSession(LoadSettingsService, SaveSettingsService, _options.Value.EditorPaths);
        }
        
        public ViewType GetViewModelType()
        {
            return ViewType.Converter;
        }
    }
}