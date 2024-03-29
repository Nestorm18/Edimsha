using System.Collections.Generic;
using System.IO;
using System.Linq;
using Edimsha.Core.Models;

namespace Edimsha.Core.Utils
{
    public static class ListCleaner
    {
        // Log
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        /// <summary>
        /// Eliminate duplicate elements and validate the formats. Use internally <see cref="RemoveWrongFormats"/> to this purpose.
        /// </summary>
        /// <param name="oldPaths">The list of currents paths.</param>
        /// <param name="newPaths">The list of new paths.</param>
        /// <param name="mode"><see cref="ViewType"/> the type of image mode that is used.</param>
        /// <returns>List with validated formats and no duplicates</returns>
        public static IEnumerable<string> PathWithoutDuplicatesAndGoodFormats(IEnumerable<string> oldPaths, IEnumerable<string> newPaths, ViewType mode)
        {
            Logger.Info("Cleaning paths");
            // Concat two list and remove duplicates to show in listview
            var distinctPaths = oldPaths.Concat(newPaths).Distinct().ToList();

            // Remove wrong formats for the current mode
            return (List<string>) RemoveWrongFormats(distinctPaths, mode);
        }

        /// <summary>
        /// Delete all the images from the list that do not contain the format that it should have in the indicated mode.
        /// <para>Example:</para>
        /// If it only supports .jpg and .png and the list contains a .bmp delete this last element.
        /// </summary>
        /// <param name="filepaths">List of strings with paths to check.</param>
        /// <param name="mode"><see cref="ViewType"/> the type of image mode that is used.</param>
        /// <returns>List with validated formats.</returns>
        private static IEnumerable<string> RemoveWrongFormats(IEnumerable<string> filepaths, ViewType mode) {
            Logger.Info("Removing");
            // Get all supported images formats for the current mode
            var imageType = ImageFormatsFromViewType.GetImageType(mode);

            var extensions = new List<string>();

            // Create a list of image formats, Ex.: [.png, .jpg]
            if (imageType != null)
                extensions.AddRange(from object type in imageType select $".{type.ToString()?.ToLower()}");

            // Filter the images that match the proposed formats and retuns
            return filepaths.Where(path => extensions.Contains(Path.GetExtension(path).ToLower())).ToList();
        }
    }
}