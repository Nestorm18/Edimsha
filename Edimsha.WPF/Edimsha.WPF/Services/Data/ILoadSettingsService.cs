using System;
using System.Collections.Generic;
using System.IO;
using Edimsha.Core.Models;

namespace Edimsha.WPF.Services.Data
{
    public interface ILoadSettingsService
    {
        /// <summary>
        /// Gets a setting from selected file and returns.
        /// </summary>
        /// <param name="settingName">A name of the setting inside a file.</param>
        /// <param name="filePath">A path to the saved setting file.</param>
        /// <typeparam name="T">A type of the requested setting. Ex.: int, double, bool.</typeparam>
        /// <typeparam name="TClass">A class that will be used to parse the file.</typeparam>
        /// <returns>The value that is stored in the file for the requested setting.</returns>
        /// <exception cref="ArgumentException">The setting passed as parameter not found.</exception>
        /// <exception cref="FileNotFoundException">The file passed as parameter not found.</exception>
        T LoadConfigurationSetting<T, TClass>(string settingName, string filePath);
        
        /// <summary>
        /// Loads the list of resolutions of the file passed as parameter.
        /// </summary>
        /// <param name="filePath">A path to the saved setting file.</param>
        /// <returns>A list of resolutions.</returns>
        ///<exception cref="FileNotFoundException">The file passed as parameter not found.</exception>
        IEnumerable<Resolution> LoadResolutions(string filePath);

        /// <summary>
        /// Gets the requested TClass configuration class filled with the values that are stored in the file that is passed as a parameter.
        /// </summary>
        /// <param name="filePath">A path to the saved setting file.</param>
        /// <typeparam name="TClass">A class that will be used to parse the file.</typeparam>
        /// <returns>A TClass filled from the values of the file.</returns>
        /// <exception cref="FileNotFoundException">The file passed as parameter not found.</exception>
        TClass GetFullConfig<TClass>(string filePath);
        
        /// <summary>
        /// Read the path file to check if they still exist locally.
        /// </summary>
        /// <param name="filePath">A path to the saved setting file.</param>
        /// <returns>A true if all routes persist from last session.</returns>
        /// <exception cref="FileNotFoundException">The file passed as parameter not found.</exception>
        bool StillPathsSameFromLastSession(string filePath);

        /// <summary>
        /// Obtain a list of the routes that have been deleted and were still stored in the file that was passed as a parameter.
        /// </summary>
        /// <param name="filePath">A path to the saved setting file.</param>
        /// <returns>Null if not found changes, otherwise return a list of strings containing the changes.</returns>
        /// <exception cref="FileNotFoundException">The file passed as parameter not found.</exception>
        IEnumerable<string> GetPathChanges(string filePath);
    }
}