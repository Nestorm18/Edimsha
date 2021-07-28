using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Edimsha.WPF.Services.Data
{
    public interface ISaveSettingsService
    {
        Task<bool> SaveConfigurationSettings<T, TClass>(string settingName, T value, string filePath);

        /// <summary>
        /// Receives a list and saves them to the specified file.
        /// </summary>
        /// <param name="list">A list to be stored.</param>
        /// <param name="filePath">A path to the saved setting file.</param>
        /// <exception cref="FileNotFoundException">The file passed as parameter not found.</exception>
        Task<bool> SaveListToFile<T>(IEnumerable<T> list, string filePath);
    }
}