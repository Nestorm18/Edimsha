using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Edimsha.WPF.Services.Data
{
    public interface ISaveSettingsService
    {
        /// <summary>
        /// Save a setting using a specified format of a class (used for json structure) in a file passed as parameter.
        /// </summary>
        /// <param name="settingName">A name of the setting inside a file.</param>
        /// <param name="value">A value to write.</param>
        /// <param name="filePath">A path to the saved setting file.</param>
        /// <typeparam name="T">A type of the value.</typeparam>
        /// <typeparam name="TClass">A class that represents a structure of json file.</typeparam>
        /// <returns>True if write has sucesfully completed.</returns>
        /// <exception cref="FileNotFoundException">The file passed as parameter not found.</exception>
        Task<bool> SaveConfigurationSettings<T, TClass>(string settingName, T value, string filePath);

        /// <summary>
        /// Receives a list and saves them to the specified file.
        /// </summary>
        /// <param name="list">A list to be stored.</param>
        /// <param name="filePath">A path to the saved setting file.</param>
        /// <exception cref="FileNotFoundException">The file passed as parameter not found.</exception>
        bool SaveListToFile<T>(IEnumerable<T> list, string filePath);
    }
}