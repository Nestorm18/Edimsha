using System.Collections;
using System.Text;
using Edimsha.Core.Language;

namespace Edimsha.WPF.Utils
{
    public static class FilterCreator
    {
        /// <summary>
        /// Creates the filter used in file browsers based on the formats supported by the application at the current time.
        /// </summary>
        /// <param name="imageType">All avaliable format for current <see cref="ViewType"/>.</param>
        /// <returns>Formmated string to use as filter.</returns>
        public static string Create(IEnumerable imageType)
        {
            var builder = new StringBuilder();
            var builderSecond = new StringBuilder();
            builder.Append(TranslationSource.GetTranslationFromString("image_files"));
            builder.Append(' ');
            builder.Append('(');

            foreach (var type in imageType)
            {
                var format = $"*.{type.ToString()?.ToLower()};";
                builder.Append(format);
                builderSecond.Append(format);
            }

            builder.Remove(builder.Length - 1, 1);
            builder.Append(')');
            builder.Append('|');
            builder.Append(builderSecond);

            return builder.ToString();
        }
    }
}