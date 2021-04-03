using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Edimsha.WPF.Commands;
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
        private readonly TranslationSource _ts;
        private string _statusBarCurrentText;
        private bool _isLoading;

        // Properties

        #region Properties

        private bool _cleanListOnExit;
        private bool _addOnReplace;
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

        public string StatusBar
        {
            get => _statusBar;
            set
            {
                _statusBar = _ts[value];
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

        #endregion

        // Commands
        public ICommand DeleteItemCommand { get; }
        public ICommand DeleteAllItemsCommand { get; }
        public ICommand CleanListOnExitCommand { get; }
        public ICommand IterateSubdirectoriesCommand { get; }
        public ICommand OpenImagesCommand { get; }

        // Constructor
        public EditorViewModel(
            ISaveSettingsService saveSettingsService,
            ILoadSettingsService loadSettingsService,
            IDialogService dialogService)
        {
            _saveSettingsService = saveSettingsService;
            _loadSettingsService = loadSettingsService;
            _dialogService = dialogService;

            _ts = TranslationSource.Instance;
            _ts.PropertyChanged += LanguageOnPropertyChanged;

            Urls = new ObservableCollection<string>();
            Urls.CollectionChanged += UrlsOnCollectionChanged;

            // Commands
            DeleteItemCommand = new DeleteItemsCommand(this);
            DeleteAllItemsCommand = new DeleteItemsCommand(this, true);
            CleanListOnExitCommand = new SaveSettingsCommand(async () => await UpdateSetting("CleanListOnExit", CleanListOnExit));
            IterateSubdirectoriesCommand = new SaveSettingsCommand(async () => await UpdateSetting("IterateSubdirectories", IterateSubdirectories));
            OpenImagesCommand = new OpenImagesCommand(this, _dialogService);

            // Loaded
            SetUserSettings();
        }

        public void SetStatusBar(string translationKey)
        {
            StatusBar = _statusBarCurrentText = translationKey;
        }

        private void LanguageOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            StatusBar = _statusBarCurrentText;
        }

        private void SetUserSettings()
        {
            _isLoading = true;
            SetStatusBar("application_started");

            _loadSettingsService.LoadPathsListview(ViewType.Editor)?.ForEach(Urls.Add);
            CleanListOnExit = _loadSettingsService.LoadConfigurationSetting<bool>("CleanListOnExit");
            IterateSubdirectories = _loadSettingsService.LoadConfigurationSetting<bool>("IterateSubdirectories");

            IsRunningUi = true;
            _isLoading = false;

            UrlsOnCollectionChanged(null, null);
        }

        private void UrlsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isLoading) return;
            IsCtxDelete = Urls.Count > 0;
            IsCtxDeleteAll = Urls.Count > 0;

            //FIXME: Guardar solo al salir, si esto consume muchos recursos
            var success = _saveSettingsService.SavePathsListview(Urls, ViewType.Editor);

            if (!success.Result) SetStatusBar("error_saving_editor_paths");
        }

        public void OnFileDrop(string[] filepaths)
        {
            var pathsUpdated = IsDirectoryDropped(filepaths.ToList());

            UpdateUrlsWithoutDuplicates(pathsUpdated);
        }

        private IEnumerable<string> IsDirectoryDropped(IEnumerable<string> filepaths)
        {
            var temp = new List<string>();

            foreach (var path in filepaths)
            {
                if (Directory.Exists(path))
                    temp.AddRange(IterateSubdirectories
                        ? Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                        : Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly));
                else
                    temp.Add(path);
            }

            return temp;
        }

        public void UpdateUrlsWithoutDuplicates(IEnumerable<string> filepaths)
        {
            var savedPaths = Urls.ToList();
            var newPaths = filepaths;

            // Concat two list and remove duplicates to show in listview
            var distinctPaths = savedPaths.Concat(newPaths).Distinct().ToList();

            var removeWrongFormats = (List<string>) RemoveWrongFormats(distinctPaths);

            Urls.Clear();
            foreach (var s in removeWrongFormats) Urls.Add(s);
        }

        private static IEnumerable<string> RemoveWrongFormats(IEnumerable<string> filepaths)
        {
            var imageType = ImageFormatsFromViewType.GetImageType(ModeImageTypes.Editor);

            var extensions = new List<string>();

            if (imageType != null)
                extensions.AddRange(from object type in imageType select $".{type.ToString()?.ToLower()}");

            return filepaths.Where(path => extensions.Contains(Path.GetExtension(path).ToLower())).ToList();
        }

        private async Task UpdateSetting<T>(string setting, T value)
        {
            var success = await _saveSettingsService.SaveConfigurationSettings(setting, value);

            if (!success) SetStatusBar("the_option_could_not_be_saved");
        }
    }
}