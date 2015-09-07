using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CSharpIniFileSerializer
{
    public static class MemberInfoHelper
    {
        public static void SetValue(this MemberInfo member, object property, object value)
        {
            if (member.MemberType == MemberTypes.Property)
                ((PropertyInfo)member).SetValue(property, value, null);
            else if (member.MemberType == MemberTypes.Field)
                ((FieldInfo)member).SetValue(property, value);
            else
                throw new ArgumentException("Property must be of type FieldInfo or PropertyInfo", "member");
        }

        public static object GetValue(this MemberInfo member, object property)
        {
            if (member.MemberType == MemberTypes.Property)
                return ((PropertyInfo)member).GetValue(property, null);
            else if (member.MemberType == MemberTypes.Field)
                return ((FieldInfo)member).GetValue(property);
            else
                throw new ArgumentException("Property must be of type FieldInfo or PropertyInfo", "member");
        }

        public static Type GetType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                default:
                    throw new ArgumentException("MemberInfo must be if type FieldInfo, PropertyInfo or EventInfo", "member");
            }
        }
    }
}
