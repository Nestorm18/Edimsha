using System.Collections.Generic;
using System.IO;
using System.Linq;
using Edimsha.WPF.Models;

namespace Edimsha.WPF.Utils
{
    public static class ListCleaner
    {
        public static IEnumerable<string> PathWithoutDuplicatesAndGoodFormats(IEnumerable<string> savedPaths, IEnumerable<string> droppedPaths, ModeImageTypes mode)
        {
            // Concat two list and remove duplicates to show in listview
            var distinctPaths = savedPaths.Concat(droppedPaths).Distinct().ToList();

            return (List<string>) RemoveWrongFormats(distinctPaths, mode);
        }

        private static IEnumerable<string> RemoveWrongFormats(IEnumerable<string> filepaths, ModeImageTypes mode)
        {
            var imageType = ImageFormatsFromViewType.GetImageType(mode);

            var extensions = new List<string>();

            if (imageType != null)
                extensions.AddRange(from object type in imageType select $".{type.ToString()?.ToLower()}");

            return filepaths.Where(path => extensions.Contains(Path.GetExtension(path).ToLower())).ToList();
        }
    }
}