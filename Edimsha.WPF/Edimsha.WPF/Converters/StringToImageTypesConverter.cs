using System;
using System.Globalization;
using System.Windows.Data;
using Edimsha.Core.Models;

namespace Edimsha.WPF.Converters
{
    public class StringToImageTypesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            if (value is not ImageTypesConversor format) return null;

            return format;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            if (value is not string format) return null;

            format = format.Substring(2).ToUpper();

            var tryParse = Enum.TryParse(format, out ImageTypesConversor outFormat);

            return tryParse ? outFormat : null;
        }
    }
}