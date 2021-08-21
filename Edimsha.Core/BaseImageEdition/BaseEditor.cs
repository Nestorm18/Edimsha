using System.IO;

namespace Edimsha.Core.BaseImageEdition
{
    public class BaseEditor
    {
        /// <summary>
        /// Generates a save path for the current image passed as a path using the values provided.
        /// </summary>
        /// <param name="outputFolder">A image saving folder.</param>
        /// <param name="path">A current image path.</param>
        /// <param name="edimsha">A value of edimsha.</param>
        /// <returns>The new route generated using the parameters.</returns>
        protected static string GeneratesavePath(string outputFolder, string path, string edimsha)
        {
            var name = GenerateName(outputFolder, path, edimsha);

            return outputFolder.Equals(string.Empty)
                ? Path.Combine(Directory.GetParent(path)?.FullName ?? string.Empty, name)
                : Path.Combine(outputFolder, name);
        }

        /// <summary>
        /// Generates the name of the image using the values passed as parameters.
        /// </summary>
        /// <param name="outputFolder">A image saving folder.</param>
        /// <param name="path">A current image path.</param>
        /// <param name="edimsha">A value of edimsha.</param>
        /// <returns>The generated name.</returns>
        private static string GenerateName(string outputFolder, string path, string edimsha)
        {
            var samePath = IsSamePath(outputFolder, path);
            var imageName = Path.GetFileNameWithoutExtension(path);

            if (edimsha.Equals("edimsha_") && samePath) return imageName;

            return $"{edimsha}{imageName}";
        }

        /// <summary>
        /// Check that the path to save the image is different from the current path where the image is located.
        /// </summary>
        /// <param name="outputFolder">A image saving folder.</param>
        /// <param name="path">A current image path.</param>
        /// <returns>True if is same path.</returns>
        private static bool IsSamePath(string outputFolder, string path)
        {
            var outputDir = outputFolder;
            var currentDir = Directory.GetParent(path)?.FullName;

            return string.IsNullOrEmpty(outputDir) || Equals(outputDir, currentDir);
        }
    }
}