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
            get => LoadSettingsService.LoadConfigurationSetting<bool, ConversorConfig>(nameof(IterateSubdirectories), _options.Value.SettingsConversor);
            set
            {
                UpdateSetting(nameof(IterateSubdirectories), value).ConfigureAwait(false);
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

            // Commands
            // Mouse context
            DeleteItemCommand = new DeleteItemsCommand(this);
            DeleteAllItemsCommand = new DeleteItemsCommand(this, true);

            // Loaded
            _isLoadingSettings = SetUserSettings();

            PathList.CollectionChanged += UrlsOnCollectionChanged;
        }

        private bool SetUserSettings()
        {
            IsRunningUi = true;

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

        private void SavePaths()
        {
            Logger.Info("Saving paths");

            var success = SaveSettingsService.SaveListToFile(PathList, _options.Value.ConversorPaths).Result;
            if (!success) StatusBar = "error_saving_editor_paths";
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

            var success = await SaveSettingsService.SaveConfigurationSettings<T, ConversorConfig>(setting, value, _options.Value.SettingsConversor);

            if (!success) StatusBar = "the_option_could_not_be_saved";
        }

        public ViewType GetViewModelType()
        {
            return ViewType.Converter;
        }
    }
}