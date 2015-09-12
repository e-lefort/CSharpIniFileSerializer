using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nini.Config;
using System.Globalization;
using System.Drawing;

namespace CSharpIniFileSerializer.IniSerializer
{
    public static class IConfigHelper
    {
        public static object GetGenericValue(this IConfig config, Type type, string fieldName, string defaultValue)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (config == null)
                throw new ArgumentNullException("config");

            if (type == typeof(string))
            {
                return config.Get(fieldName, defaultValue);
            }
            else if (type == typeof(char))
            {
                return config.Get(fieldName, defaultValue);
            }
            else if (type == typeof(short))
            {
                return config.GetInt(fieldName, (String.IsNullOrEmpty(defaultValue)) ? 0 : Int16.Parse(defaultValue));
            }
            else if (type == typeof(int))
            {
                return config.GetInt(fieldName, (String.IsNullOrEmpty(defaultValue)) ? 0 : Int32.Parse(defaultValue));
            }
            else if (type == typeof(long))
            {
                return config.GetLong(fieldName, (String.IsNullOrEmpty(defaultValue)) ? 0 : Int64.Parse(defaultValue));
            }
            else if (type == typeof(float))
            {
                return float.Parse(config.Get(fieldName, (String.IsNullOrEmpty(defaultValue)) ? ".0" : defaultValue), CultureInfo.InvariantCulture);
            }
            else if (type == typeof(double))
            {
                return double.Parse(config.Get(fieldName, (String.IsNullOrEmpty(defaultValue)) ? ".0" : defaultValue), CultureInfo.InvariantCulture);
            }
            else if (type == typeof(bool))
            {
                return config.GetBoolean(fieldName, (String.IsNullOrEmpty(defaultValue)) ? false : Boolean.Parse(defaultValue));
            }
            else if (type == typeof(Color))
            {
                return ColorTranslator.FromHtml(config.Get(fieldName, (String.IsNullOrEmpty(defaultValue)) ? ColorTranslator.ToHtml(Color.Black) : defaultValue));
            }
            else if (type.IsEnum)
            {
                return Enum.Parse(type, config.Get(fieldName, defaultValue));
            }
            else if (type == typeof(DateTime))
            {
                return DateTime.Parse(config.Get(fieldName, (String.IsNullOrEmpty(defaultValue)) ? DateTime.Now.ToString() : defaultValue));
            }
            return null;
        }

        public static bool SetGenericValue(this IConfig config, Type type, string fieldName, object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (type == null)
                throw new ArgumentNullException("type");
            if (config == null)
                throw new ArgumentNullException("config");

            if (type.IsGenericValue())
            {
                config.Set(fieldName, Utils.ParseGenericValue(type, obj));
                return true;
            }
            return false;
        }
    }
}
