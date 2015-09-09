using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nini.Config;
using CSharpIniFileSerializer.IniAttributes;
using System.Globalization;
using System.Drawing;
using System.Collections;
using CSharpIniFileSerializer.IniEnums;
using System.Reflection;

namespace CSharpIniFileSerializer
{
    class IniReader
    {
        private static object GetValue<T>(ref T obj, MemberInfo member, ref IConfigSource source, ref IniSettings settings, IniAttributesManager attributes, ref Stack<object> recurciveStackOverFlow, ref Stack<string> depth)
        {
            /* Generic types */
            if (MemberInfoHelper.GetType(member).IsGenericValue())
            {
                Console.WriteLine("Generic types");

                if (source.Configs[attributes.sectionName] == null)
                    return null;

                return source.Configs[attributes.sectionName].GetGenericValue(MemberInfoHelper.GetType(member), attributes.fieldName, SerializerParser.ParseGenericValue(MemberInfoHelper.GetType(member), obj)); //attributes.defaultValue);
            }
            /* Array and IList */
            else if (MemberInfoHelper.GetType(member).GetInterface(typeof(IList).Name) != null || MemberInfoHelper.GetType(member).IsArray)
            {
                Console.WriteLine("Array and IList");

                Type type = MemberInfoHelper.GetType(member).GetIListElementType();

                IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));

                IniArrayDelimiter arrayDelimiter = (IniArrayDelimiter)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniArrayDelimiter);
                IniArrayField arrayField = (IniArrayField)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniArrayField);
                IniArrayType arrayType = (IniArrayType)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniArrayType);

                ArrayType arrayMode = (arrayType == null) ? settings.DefaultArrayType : arrayType.type;
                char delimiter = (arrayDelimiter == null) ? (char)settings.DefaultArrayDelimiter : (char)arrayDelimiter.delimiter;

                for (int i = 0; ; i++)
                {
                    IConfig config = null;
                    string arraySectionName = attributes.sectionName;
                    string arrayFieldName = attributes.fieldName;

                    if (arrayMode == ArrayType.Section)
                    {
                        arraySectionName = String.Format("{0}{1}{2}", attributes.sectionName, delimiter, i);
                        arrayFieldName = attributes.fieldName;
                    }
                    if (arrayMode == ArrayType.Key)
                    {
                        arraySectionName = attributes.sectionName;
                        arrayFieldName = String.Format("{0}{1}{2}", attributes.fieldName, delimiter, i);
                    }

                    Console.WriteLine(arraySectionName);
                    Console.WriteLine(arrayFieldName);

                    config = source.Configs[arraySectionName];

                    object subObj = Activator.CreateInstance(type);

                    if (type.IsGenericValue())
                    {
                        if (config == null || !config.Contains(arrayFieldName))
                            break;

                        object value = config.GetGenericValue(type, arrayFieldName, attributes.defaultValue);
                        Console.WriteLine(value);

                        if (value != null)
                            list.Add(value);
                    }
                    else
                    {
                        depth.Push(arraySectionName.Split((char)settings.DefaultObjectDelimiter).Last());
                        if (!Deserialize(ref subObj, ref source, ref settings, ref recurciveStackOverFlow, ref depth))
                        {
                            depth.Pop();
                            break;
                        }
                        depth.Pop();
                        list.Add(subObj);
                    }
                }

                if (list.Count == 0)
                    return null;

                if (MemberInfoHelper.GetType(member).IsArray)
                {
                    Array array = Array.CreateInstance(type, list.Count);
                    list.CopyTo(array, 0);
                    return array;
                }
                return list;
            }
            else if (MemberInfoHelper.GetType(member).IsClass || MemberInfoHelper.GetType(member).IsStruct())
            {
                Console.WriteLine("Class");
                object value = member.GetValue(obj); //Activator.CreateInstance(type);
                if (value != null)
                {
                    Console.WriteLine(attributes.sectionName);
                    //depth.Push(infoHelper.sectionName);
                    depth.Push(attributes.sectionName.Split((char)settings.DefaultObjectDelimiter).Last());
                    if (!Deserialize(ref value, ref source, ref settings, ref recurciveStackOverFlow, ref depth))
                    {
                        //depth.Pop();
                        //return null;
                    }
                    depth.Pop();
                    return value;
                }
            }
            return null;
        }

        public static bool ContainsSection(IConfigSource source, string section)
        {
            foreach (var c in source.Configs)
            {
                IConfig conf = c as IConfig;

                if (conf.Name.StartsWith(section))
                    return true;
            }

            return false;
        }

        public static bool Deserialize<T>(ref T obj, ref IConfigSource source, ref IniSettings settings, ref Stack<object> recurciveStackOverFlow, ref Stack<string> depth)
        {
            foreach (var member in IniSerializer.GetMemberInfo<T>(obj, settings))
            {
                try
                {
                    IniAttributesManager attributes = new IniAttributesManager(member, obj, settings);

                    if (attributes.iniIgnore != null)
                        continue;

                    if (depth.Count != 0 && settings.EnableDepthSectionNaming)
                    {
                        char delimeter = (char)settings.DefaultObjectDelimiter;
                        attributes.sectionName = String.Format("{2}{1}{0}", attributes.sectionName, delimeter, depth.Reverse().Aggregate((i, j) => i + delimeter + j));
                    }

                    Console.WriteLine(attributes.sectionName);
                    if (ContainsSection(source, attributes.sectionName))
                    {
                        object value = GetValue<T>(ref obj, member, ref source, ref settings, attributes, ref recurciveStackOverFlow, ref depth);

                        if (value != null)
                            member.SetValue(obj, value);
                    }
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return true;
        }
    }
}
