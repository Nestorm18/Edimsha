using System;

namespace Edimsha.WPF.ViewModels
{
    public class EditorViewModel : ViewModelBase
    {
        // IOC

        // Properties

        #region Checkboxes

        private bool _cleanListOnExit;
        private bool _addOnReplace;
        private bool _keepOriginalResolution;
        private bool _optimizeImage;
        private bool _replaceForOriginal;

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

        #endregion

        // Commands

        // Constructor
        public EditorViewModel()
        {
            Console.WriteLine("Test EDITOR-VM");
        }
    }
}