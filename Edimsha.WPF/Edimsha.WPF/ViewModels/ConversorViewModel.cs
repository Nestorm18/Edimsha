using System.IO;
using Edimsha.WPF.State.Navigators;
using Edimsha.WPF.ViewModels.Interfaces;

namespace Edimsha.WPF.ViewModels
{
    public class ConversorViewModel : ViewModelBase, IViewType
    {
        // Log
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        
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
            _logger.Info("Constructor");
        }

        public ViewType GetType()
        {
            return ViewType.Conversor;
        }
    }
}