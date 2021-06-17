using System;
using System.Globalization;
using System.Windows.Data;
using Edimsha.Core.Language;

namespace Edimsha.WPF.Converters
{
    public class LangKeyToTranslationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is not string key ? string.Empty : TranslationSource.GetTranslationFromString(key);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new();
        }
    }
}