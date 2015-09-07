using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nini.Config;
using CSharpIniFileSerializer.IniAttributes;
using System.Drawing;
using System.Globalization;
using System.Collections;
using CSharpIniFileSerializer.IniEnums;

namespace CSharpIniFileSerializer
{
    class IniWriter
    {
        private static bool SetGenericValue(Type type, IConfig config, string fieldName, object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (type == null)
                throw new ArgumentNullException("type");
            if (config == null)
                throw new ArgumentNullException("config");

            if (type == typeof(string) || type == typeof(char)
                || type == typeof(int) || type == typeof(short) || type == typeof(long))
            {
                config.Set(fieldName, obj.ToString());
                return true;
            }
            else if (type == typeof(float))
            {
                config.Set(fieldName, ((float)obj).ToString("G0", CultureInfo.InvariantCulture));
                return true;
            }
            else if (type == typeof(double))
            {
                config.Set(fieldName, ((double)obj).ToString("G0", CultureInfo.InvariantCulture));
                return true;
            }
            else if (type == typeof(bool))
            {
                config.Set(fieldName, ((bool)obj ? "true" : "false"));
                return true;
            }
            else if (type == typeof(Color))
            {
                config.Set(fieldName, ColorTranslator.ToHtml((Color)obj));
                return true;
            }
            else if (type.IsEnum)
            {
                //DescriptionAttribute value = EnumHelper.GetAttributeOfType<DescriptionAttribute>(obj as Enum);
                //config.Set(fieldName, (value == null) ? obj.ToString() : value.Description);
                config.Set(fieldName, obj.ToString());
                return true;
            }
            else if (type == typeof(DateTime))
            {
                config.Set(fieldName, obj.ToString());
                return true;
            }
            return false;
        }

        public static void Serialize<T>(T obj, ref IConfigSource source, ref IniSettings settings, ref Stack<object> recurciveStackOverFlow, ref Stack<string> depth)
        {
            if (recurciveStackOverFlow.Contains(obj))
                return;

            recurciveStackOverFlow.Push(obj);

            foreach (var member in IniSerializer.GetMemberInfo<T>(obj, settings))
            {
                Type fieldType = MemberInfoHelper.GetType(member);

                IniFieldName iniFieldName = (IniFieldName)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniFieldName);
                IniSectionName iniSectionName = (IniSectionName)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniSectionName);
                iniSectionName = iniSectionName ?? (IniSectionName)obj.GetType().GetCustomAttributes(true).FirstOrDefault(x => x is IniSectionName);
                IniIgnore iniIgnore = (IniIgnore)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniIgnore);

                if (iniIgnore != null)
                    continue;

                string sectionName = (iniSectionName == null) ? obj.GetType().Name : iniSectionName.section;
                string fieldName = (iniFieldName == null) ? member.Name : iniFieldName.field;

                object value = member.GetValue(obj);

                if (value == null)
                    continue;

                if (depth.Count != 0)
                {
                    char delimeter = (char)settings.DefaultObjectDelimiter;
                    sectionName = String.Format("{2}{1}{0}", sectionName, delimeter, depth.Reverse().Aggregate((i, j) => i + delimeter + j));
                }

                IConfig config = source.Configs.Add(sectionName);
                /* Generic types */
                if (!SetGenericValue(fieldType, config, fieldName, value))
                {
                    source.Configs.Remove(config);

                    /* Array and IList */
                    if (fieldType.IsArray || fieldType.GetInterface(typeof(IList).Name) != null)
                    {
                        Type type = fieldType.GetElementType() ?? fieldType.GetGenericArguments().First();

                        IList list = value as IList;
                        if (list == null)
                            continue;

                        IniArrayDelimiter arrayDelimiter = (IniArrayDelimiter)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniArrayDelimiter);
                        IniArrayField arrayField = (IniArrayField)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniArrayField);
                        IniArrayType arrayType = (IniArrayType)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniArrayType);

                        ArrayType arrayMode = (arrayType == null) ? settings.DefaultArrayType : arrayType.type;

                        char delimiter = (arrayDelimiter == null) ? (char)settings.DefaultArrayDelimiter : (char)arrayDelimiter.delimiter;
                        for (int i = 0; i < list.Count; i++)
                        {
                            string arraySectionName = sectionName;
                            string arrayFieldName = fieldName;
                            if (arrayMode == ArrayType.Section)
                            {
                                arraySectionName = String.Format("{0}{1}{2}", sectionName, delimiter, i);
                                arrayFieldName = fieldName;
                            }
                            if (arrayMode == ArrayType.Key)
                            {
                                arraySectionName = sectionName;
                                arrayFieldName = String.Format("{0}{1}{2}", fieldName, delimiter, i);
                            }

                            config = source.Configs.Add(arraySectionName);
                            if (config == null)
                                break;

                            if (!IniWriter.SetGenericValue(type, config, arrayFieldName, list[i]))
                            {
                                if (arrayMode == ArrayType.Key)
                                    throw new ArgumentException(String.Format("Unable to convert {0} to keys", fieldType));

                                source.Configs.Remove(config);
                                depth.Push(arraySectionName.Split((char)settings.DefaultObjectDelimiter).Last());
                                Serialize(list[i], ref source, ref settings, ref recurciveStackOverFlow, ref depth);
                                depth.Pop();
                            }
                        }
                    }
                    else if (fieldType.IsClass)
                    {
                        depth.Push(sectionName.Split((char)settings.DefaultObjectDelimiter).Last());
                        Serialize(value, ref source, ref settings, ref recurciveStackOverFlow, ref depth);
                        depth.Pop();
                    }
                }
            }
            recurciveStackOverFlow.Pop();
        }
    }
}
