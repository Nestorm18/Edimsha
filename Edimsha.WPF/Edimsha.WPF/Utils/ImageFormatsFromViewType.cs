using System;
using System.Collections;
using System.Linq;
using Edimsha.WPF.Models;

namespace Edimsha.WPF.Utils
{
    public static class ImageFormatsFromViewType
    {
        public static IEnumerable GetImageType(object parameter)
        {
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