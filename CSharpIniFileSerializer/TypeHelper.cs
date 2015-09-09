using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Drawing;

namespace CSharpIniFileSerializer
{
    public static class TypeHelper
    {
        public static bool IsGenericValue(this Type type)
        {
            if (type == typeof(string) || type == typeof(char)
             || type == typeof(short) || type == typeof(int) || type == typeof(long)
             || type == typeof(float) || type == typeof(double) || type == typeof(Double)
             || type == typeof(bool)
             || type == typeof(Color) || type == typeof(DateTime)
             || type.IsEnum)
            {
                return true;
            }
            return false;
        }

        public static bool IsStruct(this Type type)
        {
            return type.IsValueType && !type.IsEnum;
        }

        public static Type GetIListElementType(this Type type)
        {
            foreach (Type interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType 
                    && interfaceType.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    return type.GetGenericArguments().First();
                }
            }
            return null;
        }
    }
}
