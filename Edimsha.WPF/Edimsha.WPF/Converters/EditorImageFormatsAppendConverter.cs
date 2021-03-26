using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Edimsha.WPF.Converters
{
    /// <summary>
    /// A converter to append image formats of editor view. Used in a internationalized label.
    /// NOTE: Only for editor, enum cannot be generic.
    /// </summary>
    public class EditorImageFormatsAppendConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "Not found";

            var currentText = (string) value;
            var editorImageFormats = GetAllImageFormatsInline();
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
        /// <returns>Formatted text</returns>
        private string GetAllImageFormatsInline()
        {
            var imageTypesEditors = Enum.GetValues(typeof(ImageTypesEditor)).Cast<ImageTypesEditor>();
            var builder = new StringBuilder();
            
            foreach (var imageTypesEditor in imageTypesEditors)
            {
                var format = $"*.{imageTypesEditor.ToString().ToLower()}, ";
                builder.Append(format);
            }

            var clearEnd = builder.Remove(builder.Length - 2, 2);
            
            return $"({clearEnd})";
        }
    }
}