using System.Collections.ObjectModel;
using System.Linq;
using Edimsha.WPF.Utils;

namespace Edimsha.WPF.ViewModels
{
    public class EditorViewModel : ViewModelBase, IFileDragDropTarget
    {
        // IOC

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

                IsCtxDelete = Urls.Count > 0;
                IsCtxDeleteAll = Urls.Count > 0;

                OnPropertyChanged();
            }
        }

        #endregion

        // Commands

        // Constructor
        public EditorViewModel()
        {
        }

        public void OnFileDrop(string[] filepaths)
        {
            if (Urls is null)
                Urls = new ObservableCollection<string>(filepaths.Distinct());
            else
            {
                var savedPaths = Urls.ToList();
                var newPaths = filepaths.ToList();

                var filePathsDistinct = savedPaths.Concat(newPaths).Distinct().ToList();

                Urls = new ObservableCollection<string>(filePathsDistinct);
            }
        }
    }
}