using System;
using System.Collections;
using System.Linq;
using Edimsha.Core.Models;

namespace Edimsha.WPF.Utils
{
    public static class ImageFormatsFromViewType
    {
        // Log
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        
        /// <summary>
        /// Gets a list of the available formats for the mode that is requested by parameter.
        /// </summary>
        /// <param name="parameter">A type of <see cref="Mode"/></param>
        /// <returns>List of avaliable formats for requested <see cref="Mode"/>.</returns>
        public static IEnumerable GetImageType(object parameter)
        {
            _logger.Info($"Parameter: {parameter}");
           
            // Gets all the values of the Enum and returns.
            
            IEnumerable imageTypes = parameter switch
            {
                Mode.Editor => Enum.GetValues(typeof(ImageTypesEditor)).Cast<ImageTypesEditor>(),
                Mode.Converter => Enum.GetValues(typeof(ImageTypesConversor)).Cast<ImageTypesConversor>(),
                _ => null
            };
            
            return imageTypes;
        }
    }
}