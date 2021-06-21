using Edimsha.Core.Logging.Core;
using Edimsha.Core.Logging.Implementation;
using Edimsha.Core.Models;

namespace Edimsha.WPF.Utils
{
    public static class ResolutionValidator
    {
        /// <summary>
        /// Validates if the passed parameter is a <see cref="Resolution"/>.
        /// </summary>
        /// <param name="parameter">The parameter that must be an object of type resolution.</param>
        /// <returns>True if the parameter is of type <see cref="Resolution"/></returns>
        public static Resolution IsParameterValid(object parameter)
        {
            var values = (object[]) parameter!;
            var width = (string) values[0];
            var height = (string) values[1];

            Logger.Log($"width:{width}, height:{height}", LogLevel.Debug);

            // Not a valid value
            if (width == string.Empty || height == string.Empty) return new Resolution(0,0);

            var currentResolution = new Resolution(int.Parse(width), int.Parse(height));

            return currentResolution;
        }
    }
}