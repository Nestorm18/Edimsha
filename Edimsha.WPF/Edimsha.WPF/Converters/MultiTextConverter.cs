using System;
using System.Globalization;
using System.Windows.Data;

namespace Edimsha.WPF.Converters
{
    public class MultiTextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new[] {Binding.DoNothing};
        }
    }
}