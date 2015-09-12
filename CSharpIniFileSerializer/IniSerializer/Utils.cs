using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using CSharpIniFileSerializer.IniEnums;
using System.Globalization;
using System.Drawing;

namespace CSharpIniFileSerializer.IniSerializer
{
    public class Utils
    {
        public static IEnumerable<MemberInfo> GetMemberInfo<T>(T obj, IniSettings settings)
        {
            List<MemberInfo> members = new List<MemberInfo>();
            if ((settings.SetTypeInfo & TypeInfo.Fields) == TypeInfo.Fields)
            {
                members.AddRange(obj.GetType().GetFields(settings.SetBindingFlags));
            }
            if ((settings.SetTypeInfo & TypeInfo.Properties) == TypeInfo.Properties)
            {
                members.AddRange(obj.GetType().GetProperties(settings.SetBindingFlags));
            }
            return members;
        }

        public static string ParseGenericValue(Type type, object obj)
        {
            if (type == typeof(string) || type == typeof(char)
                || type == typeof(int) || type == typeof(short) || type == typeof(long))
            {
                return obj.ToString();
            }
            else if (type == typeof(float))
            {
                return ((float)obj).ToString("G0", CultureInfo.InvariantCulture);
            }
            else if (type == typeof(double))
            {
                return ((double)obj).ToString("G0", CultureInfo.InvariantCulture);
            }
            else if (type == typeof(bool))
            {
                return ((bool)obj) ? "true" : "false";
            }
            else if (type == typeof(Color))
            {
                return ColorTranslator.ToHtml((Color)obj);
            }
            else if (type.IsEnum)
            {
                return obj.ToString();
            }
            else if (type == typeof(DateTime))
            {
                return obj.ToString();
            }
            return String.Empty;
        }
    }
}
