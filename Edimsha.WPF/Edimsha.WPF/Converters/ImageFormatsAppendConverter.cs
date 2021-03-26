using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Edimsha.WPF.Models;

namespace Edimsha.WPF.Converters
{
    /// <summary>
    /// A converter to append image formats. Used in a internationalized label.
    /// </summary>
    public class ImageFormatsAppendConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) throw new Exception("Value not found");
            if (parameter == null) throw new Exception("Parameter not found");

            var currentText = (string) value;
            var editorImageFormats = GetAllImageFormatsInline(parameter);
            var fullText = currentText + " " + editorImageFormats;

            return fullText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Exception("EditorImageFormatsAppendConverter ConvertBack no existe");
        }

        /// <summary>
        /// Gets all available image formats XXX YYY from Enum and returns them formatted in text (* .XXX, * .YYY)
        /// </summary>
        /// <param name="parameter">Can be <see cref="ModeImageTypes.Editor"/> or <see cref="ModeImageTypes.Converter"/>,
        /// used to make more generic.</param>
        /// <returns>Formatted text</returns>
        private static string GetAllImageFormatsInline(object parameter)
        {
            var imageType = GetImageType(parameter);

            var builder = new StringBuilder();

            if (imageType != null)
                foreach (var type in imageType)
                {
                    var format = $"*.{type.ToString()?.ToLower()}, ";
                    builder.Append(format);
                }

            var clearEnd = builder.Remove(builder.Length - 2, 2);

            return $"({clearEnd})";
        }

        private static IEnumerable GetImageType(object parameter)
        {
            IEnumerable imageTypes = null;

            switch (parameter)
            {
                case ModeImageTypes.Editor:
                    imageTypes = Enum.GetValues(typeof(ImageTypesEditor)).Cast<ImageTypesEditor>();
                    break;
                case ModeImageTypes.Converter:
                    imageTypes = Enum.GetValues(typeof(ImageTypesConversor)).Cast<ImageTypesConversor>();
                    break;
            }

            return imageTypes;
        }
    }
}