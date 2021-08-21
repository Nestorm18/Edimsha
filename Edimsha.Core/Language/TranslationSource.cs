using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace Edimsha.Core.Language
{
    public class TranslationSource : INotifyPropertyChanged
    {
        public static TranslationSource Instance { get; } = new();

        private readonly ResourceManager _resManager = Resources.ResourceManager;
        private CultureInfo _currentCulture;

        // ReSharper disable once MemberCanBePrivate.Global
        public string this[string key] => _resManager.GetString(key, _currentCulture);

        /// <summary>
        /// Set the current <see cref="CultureInfo"/> for the UI.
        /// </summary>
        public CultureInfo CurrentCulture
        {
            set
            {
                if (Equals(_currentCulture, value)) return;
                _currentCulture = value;
                var evnt = PropertyChanged;
                evnt?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
            }
            // ReSharper disable once UnusedMember.Global
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