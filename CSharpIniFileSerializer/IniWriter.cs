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

                if (depth.Count != 0 && settings.EnableDepthSectionNaming)
                {
                    char delimeter = (char)settings.DefaultObjectDelimiter;
                    sectionName = String.Format("{2}{1}{0}", sectionName, delimeter, depth.Reverse().Aggregate((i, j) => i + delimeter + j));
                }

                
                /* Generic types */
                if (fieldType.IsGenericValue())
                {
                    source.Configs.Add(sectionName).SetGenericValue(fieldType, fieldName, value);
                }
                /* Array and IList */
                else
                {
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

                            IConfig config = source.Configs.Add(arraySectionName);
                            if (config == null)
                                break;

                            if (!config.SetGenericValue(type, arrayFieldName, list[i]))
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
                    else if (fieldType.IsClass || fieldType.IsStruct())
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
