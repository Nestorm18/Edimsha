using System.IO;

namespace Edimsha.Core.BaseImageEdition
{
    public class BaseEditor
    {
        protected static string GeneratesavePath(string outputFolder, string path, string edimsha)
        {
            var name = GenerateName(outputFolder, path, edimsha);

            return outputFolder.Equals(string.Empty)
                ? Path.Combine(Directory.GetParent(path)?.FullName ?? string.Empty, name)
                : Path.Combine(outputFolder, name);
        }

        private static string GenerateName(string outputFolder, string path, string edimsha)
        {
            var samePath = IsSamePath(outputFolder, path);
            var imageName = Path.GetFileNameWithoutExtension(path);

            if (edimsha.Equals("edimsha_") && samePath) return imageName;

            return $"{edimsha}{imageName}";
        }

        private static bool IsSamePath(string outputFolder, string path)
        {
            var outputDir = outputFolder;
            var currentDir = Directory.GetParent(path)?.FullName;

            return string.IsNullOrEmpty(outputDir) || Equals(outputDir, currentDir);
        }
    }
}