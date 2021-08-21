using System.Windows.Data;
using Edimsha.Core.Language;

namespace Edimsha.WPF.Language
{
    public class LocalizationExtension : Binding
    {
        // Log
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        public LocalizationExtension(string name) : base("[" + name + "]")
        {
            Logger.Info($"Name: {name}");
            Mode = BindingMode.OneWay;
            Source = TranslationSource.Instance;
        }
    }
}