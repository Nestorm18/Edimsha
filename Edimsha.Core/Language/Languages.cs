using System;
using System.ComponentModel;

namespace Edimsha.Core.Language
{
    
    public enum Languages
    {
        [Description("en-US")] English,
        [Description("es-ES")] Spanish
    }

    public static class AvaliableLanguages
    {
        // Log
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        /// <summary>
        /// Gets the description of the enumeration.
        /// </summary>
        /// <param name="language">The language to obtain the description.</param>
        /// <returns>The corresponding description.</returns>
        public static string GetDescription(this Languages language)
        {
            Logger.Info("Languages");

            var attributes = (DescriptionAttribute[]) language
                .GetType()
                .GetField(language.ToString())
                ?.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes != null && attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        /// <summary>
        /// Uses a description in an enumeration to obtain its value.
        /// </summary>
        /// <param name="description">The corresponding description. Example "en-US" for English.</param>
        /// <typeparam name="T">The enum where the description will be located.</typeparam>
        /// <returns>The value that corresponds to the provided description.</returns>
        /// <exception cref="Exception">When the enum provided as <see cref="T"/> has not that desciption.</exception>
        public static T GetValueFromDescription<T>(string description) where T : Enum
        {
            Logger.Info("Description");
            
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description) return (T) field.GetValue(null);
                }
                else
                {
                    if (field.Name == description) return (T) field.GetValue(null);
                }
            }

            throw new Exception("GetValueFromDescription no ha encontrado el valor del lenguaje solicitado");
        }
    }

    public static class ChangeLanguage
    {
        // Log
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        /// <summary>
        /// Sets language for the entire UI.
        /// </summary>
        /// <param name="locale">The corresponding description in <see cref="Languages"/> set language in UI. Example "en-US" for English.</param>
        public static void SetLanguage(string locale)
        {
            Logger.Info("Locale");
            if (string.IsNullOrEmpty(locale)) locale = Languages.English.GetDescription();
            TranslationSource.Instance.CurrentCulture = new System.Globalization.CultureInfo(locale);
        }

        /// <summary>
        /// Gets the <see cref="Language"/> from his description value. 
        /// </summary>
        /// <param name="locale">The corresponding description. Example "en-US" for English.</param>
        /// <returns></returns>
        public static Languages ResolveLanguage(string locale)
        {
            Logger.Info("Locale");
            return AvaliableLanguages.GetValueFromDescription<Languages>(locale);
        }
    }
}