using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace Edimsha.WPF.Lang
{
    public class TranslationSource : INotifyPropertyChanged
    {
        public static TranslationSource Instance { get; } = new();

        private readonly ResourceManager _resManager = Resources.ResourceManager;
        private CultureInfo _currentCulture;

        public string this[string key] => _resManager.GetString(key, _currentCulture);

        public CultureInfo CurrentCulture
        {
            set
            {
                if (Equals(_currentCulture, value)) return;
                _currentCulture = value;
                var @event = this.PropertyChanged;
                @event?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
            }
            get => _currentCulture;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}