using System.Windows.Data;
using Edimsha.Core.Language;
using Edimsha.Core.Logging.Implementation;

namespace Edimsha.WPF.Language
{
    public class LocalizationExtension : Binding
    {
        public LocalizationExtension(string name) : base("[" + name + "]")
        {
            Logger.Log($"Name: {name}");
            Mode = BindingMode.OneWay;
            Source = TranslationSource.Instance;
        }
    }
}