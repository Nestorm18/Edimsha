#nullable enable
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Edimsha.WPF.Annotations;
using Edimsha.WPF.Services.Data;
using Edimsha.WPF.State.Navigators;

namespace Edimsha.WPF.ViewModels
{
    public delegate TViewModel CreateViewModel<TViewModel>() where TViewModel : ViewModelBase;

    public class ViewModelBase : INotifyPropertyChanged
    {
        // Log
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        
        // IOC
        protected ISaveSettingsService SaveSettingsService;

        #region Properties

        private ObservableCollection<string> _pathList = null!;
        private string _outputFolder;

        public ObservableCollection<string> PathList
        {
            get => _pathList;
            set
            {
                if (value == _pathList) return;
                _pathList = value;
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
                SaveSettingsService.SaveConfigurationSettings(ViewType.Editor, "OutputFolder", _outputFolder);

                OnPropertyChanged();
            }
        }

        # endregion

        #region Constructor

        protected ViewModelBase(ISaveSettingsService saveSettingsService)
        {
            SaveSettingsService = saveSettingsService;
        }

        #endregion

        public static void Dispose()
        {
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            // Faster image procesing
            if (!propertyName.Contains("StatusBar") || !propertyName.Contains("PbPosition")) return;

            _logger.Info($"PropertyName: {propertyName}");
        }
    }
}