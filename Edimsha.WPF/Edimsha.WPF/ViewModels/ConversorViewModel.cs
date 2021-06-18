using System;
using System.IO;
using Edimsha.Core.Logging.Implementation;

namespace Edimsha.WPF.ViewModels
{
    public class ConversorViewModel : ViewModelBase
    {
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
                // TODO: Cargar esto cuando funcione
                // _saveSettingsService.SaveConfigurationSettings(ViewType.Editor, "OutputFolder", _outputFolder);

                OnPropertyChanged();
            }
        }

        # endregion

        public ConversorViewModel() : base(null)
        {
            Logger.Log("Constructor");
            Console.WriteLine("Test CONVERSOR-VM");
        }
    }
}