using System.Collections.ObjectModel;
using Edimsha.WPF.Utils;

namespace Edimsha.WPF.ViewModels
{
    public class EditorViewModel : ViewModelBase, IFileDragDropTarget
    {
        // Commands

        // Constructor
        public EditorViewModel()
        {
        }

        public void OnFileDrop(string[] filepaths)
        {
            if (Urls is null)
                Urls = new ObservableCollection<string>(filepaths);
            else
                foreach (var path in filepaths)
                    Urls.Add(path);
            
            UpdateContextMenu();

        }

        private void UpdateContextMenu()
        {
            IsCTXDelete = true;
            IsCTXDeleteAll = true;
        }
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
        private ObservableCollection<string> _urls;
        private bool _isCtxDelete;
        private bool _isCtxDeleteAll;

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

        public bool IsCTXDelete
        {
            get => _isCtxDelete;
            set
            {
                if (value == _isCtxDelete) return;
                _isCtxDelete = value;
                OnPropertyChanged();
            }
        }

        public bool IsCTXDeleteAll
        {
            get => _isCtxDeleteAll;
            set
            {
                if (value == _isCtxDeleteAll) return;
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
    }
}