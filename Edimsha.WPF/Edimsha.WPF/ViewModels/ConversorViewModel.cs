using System;
using System.Collections.ObjectModel;
using System.IO;
using Edimsha.Core.Logging.Implementation;

namespace Edimsha.WPF.ViewModels
{
    public class ConversorViewModel : ViewModelBase
    {
        private ObservableCollection<string> _pathList;
        private string _outputFolder;

        // Properties

        #region Properties

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
                // TODO: Cargar esto cuando funcione
                // _saveSettingsService.SaveConfigurationSettings(ViewType.Editor, "OutputFolder", _outputFolder);

                OnPropertyChanged();
            }
        }

        # endregion

        public ConversorViewModel()
        {
            Logger.Log("Constructor");
            Console.WriteLine("Test CONVERSOR-VM");
        }
    }
}