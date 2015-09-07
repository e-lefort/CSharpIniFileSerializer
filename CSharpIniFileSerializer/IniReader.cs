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
        private static bool IsGenericValue(Type type)
        {
            if (type == typeof(string) || type == typeof(char)
             || type == typeof(short) || type == typeof(int) || type == typeof(long)
             || type == typeof(float) || type == typeof(double) || type == typeof(Double)
             || type == typeof(bool)
             || type == typeof(Color) || type == typeof(DateTime)
             || type.IsEnum )
            {
                return true;
            }
            return false;
        }

        private static object GetGenericValue(Type type, IConfig config, string fieldName, string defaultValue)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (config == null)
                throw new ArgumentNullException("config");

            if (type == typeof(string))
            {
                return config.Get(fieldName, String.Empty);
            }
            else if (type == typeof(char))
            {
                return config.Get(fieldName, String.Empty);
            }
            else if (type == typeof(short))
            {
                return config.GetInt(fieldName, 0);
            }
            else if (type == typeof(int))
            {
                return config.GetInt(fieldName, 0);
            }
            else if (type == typeof(long))
            {
                return config.GetLong(fieldName, 0);
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
                return config.GetBoolean(fieldName, false);
            }
            else if (type == typeof(Color))
            {
                return ColorTranslator.FromHtml(config.Get(fieldName, ColorTranslator.ToHtml(Color.Black)));
            }
            else if (type.IsEnum)
            {
                return Enum.Parse(type, config.Get(fieldName));
            }
            else if (type == typeof(DateTime))
            {
                return DateTime.Parse(config.Get(fieldName, DateTime.Now.ToString()));
            }
            return null;
        }

        private static void ReadArray<T>(ref T obj, ref IConfigSource source, ref IniSettings settings, ref Stack<object> recurciveStackOverFlow, ref Stack<string> depth)
        {

        }

        private static object GetValue<T>(ref T obj, MemberInfo member, ref IConfigSource source, ref IniSettings settings, MemberInfoDatas infoHelper, ref Stack<object> recurciveStackOverFlow, ref Stack<string> depth)
        {
            /* Generic types */
            if (IsGenericValue(MemberInfoHelper.GetType(member)))
            {
                Console.WriteLine("Generic types");

                if (source.Configs[infoHelper.sectionName] == null)
                    return null;

                return IniReader.GetGenericValue(MemberInfoHelper.GetType(member), source.Configs[infoHelper.sectionName], infoHelper.fieldName, infoHelper.defaultValue);
            }
            /* Array and IList */
            else if (MemberInfoHelper.GetType(member).GetInterface(typeof(IList).Name) != null || MemberInfoHelper.GetType(member).IsArray)
            {
                Console.WriteLine("Array and IList");

                Type type = MemberInfoHelper.GetType(member).GetElementType() ?? MemberInfoHelper.GetType(member).GetGenericArguments().First();

                IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));

                IniArrayDelimiter arrayDelimiter = (IniArrayDelimiter)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniArrayDelimiter);
                IniArrayField arrayField = (IniArrayField)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniArrayField);
                IniArrayType arrayType = (IniArrayType)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniArrayType);

                ArrayType arrayMode = (arrayType == null) ? settings.DefaultArrayType : arrayType.type;
                char delimiter = (arrayDelimiter == null) ? (char)settings.DefaultArrayDelimiter : (char)arrayDelimiter.delimiter;

                for (int i = 0; ; i++ )
                {
                    IConfig config = null;
                    string arraySectionName = infoHelper.sectionName;
                    string arrayFieldName = infoHelper.fieldName;

                    if (arrayMode == ArrayType.Section)
                    {
                        arraySectionName = String.Format("{0}{1}{2}", infoHelper.sectionName, delimiter, i);
                        arrayFieldName = infoHelper.fieldName;
                    }
                    if (arrayMode == ArrayType.Key)
                    {
                        arraySectionName = infoHelper.sectionName;
                        arrayFieldName = String.Format("{0}{1}{2}", infoHelper.fieldName, delimiter, i);
                    }

                    Console.WriteLine(arraySectionName);
                    Console.WriteLine(arrayFieldName);

                    config = source.Configs[arraySectionName];

                    object subObj = Activator.CreateInstance(type);

                    if (IsGenericValue(type))
                    {
                        if (config == null || !config.Contains(arrayFieldName))
                            break;

                        object value = IniReader.GetGenericValue(type, config, arrayFieldName, infoHelper.defaultValue);
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
            else if (MemberInfoHelper.GetType(member).IsClass)
            {
                Console.WriteLine("Class");
                object value = member.GetValue(obj); //Activator.CreateInstance(type);
                if (value != null)
                {
                    Console.WriteLine(infoHelper.sectionName);
                    //depth.Push(infoHelper.sectionName);
                    depth.Push(infoHelper.sectionName.Split((char)settings.DefaultObjectDelimiter).Last());
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
                    MemberInfoDatas infoHelper = new MemberInfoDatas(member, obj);

                    if (infoHelper.iniIgnore != null)
                        continue;

                    if (depth.Count != 0)
                    {
                        char delimeter = (char)settings.DefaultObjectDelimiter;
                        infoHelper.sectionName = String.Format("{2}{1}{0}", infoHelper.sectionName, delimeter, depth.Reverse().Aggregate((i, j) => i + delimeter + j));
                    }

                    Console.WriteLine(infoHelper.sectionName);
                    if (ContainsSection(source, infoHelper.sectionName))
                    {
                        object value = GetValue<T>(ref obj, member, ref source, ref settings, infoHelper, ref recurciveStackOverFlow, ref depth);

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

        private class MemberInfoDatas
        {
            public IniFieldName iniFieldName { get; set; }
            public IniSectionName iniSectionName { get; set; }
            public IniIgnore iniIgnore { get; set; }
            public IniDefaultValue iniDefault { get; set; }

            public string sectionName;
            public string fieldName;
            public string defaultValue;

            public MemberInfoDatas(MemberInfo member, object obj)
            {
                this.iniFieldName = (IniFieldName)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniFieldName);
                this.iniSectionName = (IniSectionName)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniSectionName);
                this.iniSectionName = iniSectionName ?? (IniSectionName)obj.GetType().GetCustomAttributes(true).FirstOrDefault(x => x is IniSectionName);
                this.iniIgnore = (IniIgnore)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniIgnore);
                this.iniDefault = (IniDefaultValue)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniDefaultValue);

                this.sectionName = (iniSectionName == null) ? obj.GetType().Name : iniSectionName.section;
                this.fieldName = (iniFieldName == null) ? member.Name : iniFieldName.field;
                this.defaultValue = (iniDefault == null) ? String.Empty : iniDefault.value;
            }
        }
    }
}
