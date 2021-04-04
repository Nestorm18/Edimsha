using System;
using System.Globalization;
using System.Windows.Data;
using Edimsha.WPF.Lang;

namespace Edimsha.WPF.Converters
{
    public class LangKeyToTranslationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string key) return string.Empty;

            var ts = TranslationSource.Instance;

            return ts[key];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}