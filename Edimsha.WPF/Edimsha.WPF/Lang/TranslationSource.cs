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

        /// <summary>
        /// Search for the related translation that is passed as a string parameter.
        /// </summary>
        /// <param name="value">Key used for translated text.</param>
        /// <returns>Current language in use.</returns>
        public static string GetTranslationFromString(string value)
        {
            return Instance[value];
        }
    }
}