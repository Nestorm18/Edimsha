using System.Windows.Data;
using Edimsha.Core.Language;

namespace Edimsha.WPF.Language
{
    public class LocalizationExtension : Binding
    {
        // Log
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        
        public LocalizationExtension(string name) : base("[" + name + "]")
        {
            _logger.Info($"Name: {name}");
            Mode = BindingMode.OneWay;
            Source = TranslationSource.Instance;
        }
    }
}