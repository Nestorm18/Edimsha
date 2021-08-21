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
        
        public static string GetDescription(this Languages val)
        {
            Logger.Info("Languages");

            var attributes = (DescriptionAttribute[]) val
                .GetType()
                .GetField(val.ToString())
                ?.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes != null && attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

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
        
        public static void SetLanguage(string locale)
        {
            Logger.Info("Locale");
            if (string.IsNullOrEmpty(locale)) locale = Languages.English.GetDescription();
            TranslationSource.Instance.CurrentCulture = new System.Globalization.CultureInfo(locale);
        }

        public static Languages ResolveLanguage(string locale)
        {
            Logger.Info("Locale");
            return AvaliableLanguages.GetValueFromDescription<Languages>(locale);
        }
    }
}