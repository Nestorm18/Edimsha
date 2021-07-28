using System.Collections.Generic;
using Edimsha.Core.Models;
using Edimsha.Core.Settings;
using Edimsha.WPF.State.Navigators;

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
        T LoadConfigurationSetting<T, TClass>(string settingName, string filePath);

        IEnumerable<string> GetSavedPaths(ViewType type);

        IEnumerable<Resolution> LoadResolutions();

        ConfigEditor GetConfigFormViewType(ViewType type);

        bool StillPathsSameFromLastSession(ViewType type);

        IEnumerable<string> GetPathChanges(ViewType type);
        
        //TODO: Cargar lista de opciones ben feito!
    }
}