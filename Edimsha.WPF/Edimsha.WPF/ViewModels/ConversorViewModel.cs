using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Edimsha.WPF.Commands;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.Services.Dialogs;
using Edimsha.WPF.State.Navigators;
using Edimsha.WPF.Utils;
using Edimsha.WPF.ViewModels.Contracts;

namespace Edimsha.WPF.ViewModels
{
    public class ConversorViewModel : CommonViewModel, IFileDragDropTarget, IViewType, IExtraFolder
    {
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
                SaveSettingsService.SaveConfigurationSettings(GetViewModelType(), "OutputFolder", _outputFolder);

                OnPropertyChanged();
            }
        }
        
        public bool IterateSubdirectories
        {
            get => LoadSettingsService.LoadConfigurationSetting<bool>(ViewType.Editor, nameof(IterateSubdirectories));
            set
            {
                UpdateSetting(nameof(IterateSubdirectories), value);
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
            IDialogService dialogService)
            : base(loadSettingsService, saveSettingsService, dialogService)
        {
            Logger.Info("Constructor");

            PathList = new ObservableCollection<string>();

            // Commands
            // Mouse context
            DeleteItemCommand = new DeleteItemsCommand(this);
            DeleteAllItemsCommand = new DeleteItemsCommand(this, true);

            // Loaded
            // _isLoadingSettings = SetUserSettings();
            
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

            var success = SaveSettingsService.SavePaths(PathList, ViewType.Converter);
            if (!success) StatusBar = "error_saving_editor_paths";
        }

        private void UrlsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Logger.Info($"Paths updated");
            if (_isLoadingSettings) return;
            var isEnabled = PathList.Count > 0;

            // IsCtxDelete = isEnabled;
            // IsCtxDeleteAll = isEnabled;
            // IsStartedUi = isEnabled;
        }

        private async Task UpdateSetting<T>(string setting, T value)
        {
            Logger.Info($"setting: {setting}, Value: {value}");
            var success = await SaveSettingsService.SaveConfigurationSettings(ViewType.Converter, setting, value);

            if (!success) StatusBar = "the_option_could_not_be_saved";
        }

        public ViewType GetViewModelType()
        {
            return ViewType.Converter;
        }
    }
}