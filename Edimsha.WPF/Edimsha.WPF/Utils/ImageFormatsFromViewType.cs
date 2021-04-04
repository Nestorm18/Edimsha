using System;
using System.Collections;
using System.Linq;
using Edimsha.WPF.Models;

namespace Edimsha.WPF.Utils
{
    public static class ImageFormatsFromViewType
    {
        /// <summary>
        /// Gets a list of the available formats for the mode that is requested by parameter.
        /// </summary>
        /// <param name="parameter">A type of <see cref="ModeImageTypes"/></param>
        /// <returns>List of avaliable formats for requested <see cref="ModeImageTypes"/>.</returns>
        public static IEnumerable GetImageType(object parameter)
        {
            // Gets all the values of the Enum and returns.
            IEnumerable imageTypes = parameter switch
            {
                ModeImageTypes.Editor => Enum.GetValues(typeof(ImageTypesEditor)).Cast<ImageTypesEditor>(),
                ModeImageTypes.Converter => Enum.GetValues(typeof(ImageTypesConversor)).Cast<ImageTypesConversor>(),
                _ => null
            };

            return imageTypes;
        }
    }
}