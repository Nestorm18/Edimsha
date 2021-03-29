using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Edimsha.WPF.Commands;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.State.Navigators;
using Edimsha.WPF.Utils;

namespace Edimsha.WPF.ViewModels
{
    public class EditorViewModel : ViewModelBase, IFileDragDropTarget
    {
        // IOC
        private readonly ISaveSettingsService _saveSettingsService;
        private readonly ILoadSettingsService _loadSettingsService;

        // Properties

        #region Properties

        private bool _cleanListOnExit;
        private bool _addOnReplace;
        private bool _keepOriginalResolution;
        private bool _optimizeImage;
        private bool _replaceForOriginal;
        private bool _isRunningUi = true;
        private bool _isStartedUi;
        private bool _isCtxDelete;
        private bool _isCtxDeleteAll;
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

        // Constructor
        public EditorViewModel(ISaveSettingsService saveSettingsService, ILoadSettingsService loadSettingsService)
        {
            _saveSettingsService = saveSettingsService;
            _loadSettingsService = loadSettingsService;

            Urls = new ObservableCollection<string>();
            Urls.CollectionChanged += UrlsOnCollectionChanged;

            // Commands
            DeleteItemCommand = new DeleteItemsCommand(this);
            DeleteAllItemsCommand = new DeleteItemsCommand(this, true);
            CleanListOnExitCommand = new SaveSettingsCommand(async () => await UpdateSetting("CleanListOnExit", CleanListOnExit));

            // Loaded
            SetUserSettings();
        }

        private void SetUserSettings()
        {
            _loadSettingsService.LoadPathsListview()?.ForEach(Urls.Add);
            CleanListOnExit = _loadSettingsService.LoadConfigurationSetting<bool>("CleanListOnExit");
        }

        private void UrlsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsCtxDelete = Urls.Count > 0;
            IsCtxDeleteAll = Urls.Count > 0;

            var success = _saveSettingsService.SavePathsListview(Urls, ViewType.Editor);
            // TODO: Mostrar mensaje si falla al guardar configuracion
        }

        public void OnFileDrop(string[] filepaths)
        {
            var savedPaths = Urls.ToList();
            var newPaths = filepaths.ToList();

            // Concat two list and remove duplicates to show in listview
            var filePathsDistinct = savedPaths.Concat(newPaths).Distinct().ToList();

            Urls.Clear();
            foreach (var s in filePathsDistinct) Urls.Add(s);
        }

        private async Task UpdateSetting<T>(string setting, T value)
        {
            var success = await _saveSettingsService.SaveConfigurationSettings(setting, value);
            // TODO: Mostrar mensaje si falla al guardar configuracion
        }
    }
}