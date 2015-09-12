using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nini.Config;
using System.IO;
using System.Collections;
using System.Reflection;
using CSharpIniFileSerializer.IniEnums;
using CSharpIniFileSerializer.IniAttributes;

namespace CSharpIniFileSerializer.IniSerializer
{
    public class IniReader : IniAbstractSerializer
    {
        public void Deserialize<T>(ref T obj, StreamReader reader)
        {
            base.source = new IniConfigSource(reader);
            base.recurciveStackOverFlow = new Stack<object>();
            base.depth = new Stack<string>();

            Type currenType = obj.GetType();
            MemberInfo member = Utils.GetMemberInfo(obj, settings.SetTypeInfo, settings.SetBindingFlags).First();
            IniAttributesManager attributes = new IniAttributesManager(member, obj, settings);

            if (currenType.IsGenericValue())
            {
                member.SetValue(obj, DeserializePrimitive(currenType, attributes.sectionName, attributes.fieldName, attributes.defaultValue));
            }
            else if (currenType.GetInterface(typeof(IList).Name) != null || currenType.IsArray)
            {
                attributes.sectionName = "ArrayOf" + MemberInfoHelper.GetType(member).GetIListElementType().Name;
                DeserializeCollection(obj, attributes, member);
            }
            else if (currenType.IsClass || currenType.IsStruct())
            {
                DeserializeObject(obj, settings.SetTypeInfo, settings.SetBindingFlags);
            }
        }

        private void DeserializeObject<T>(T obj, MemberInfo member)
        {
            Console.WriteLine("> DeserializeObject");

            Type currenType = MemberInfoHelper.GetType(member);
            IniAttributesManager attributes = new IniAttributesManager(member, obj, settings);

            if (attributes.Ignore)
                return;

            IncrementDepth(attributes);

            if (!ContainsSection(attributes.sectionName))
                return;

            if (currenType.IsGenericValue())
            {
                object value = DeserializePrimitive(currenType, attributes.sectionName, attributes.fieldName, attributes.defaultValue); 
                member.SetValue(obj, value);
            }
            else if (currenType.GetInterface(typeof(IList).Name) != null || currenType.IsArray)
            {
                DeserializeCollection(obj, attributes, member);
            }
            else if (currenType.IsClass || currenType.IsStruct())
            {
                depth.Push(attributes.sectionName.Split((char)settings.DefaultObjectDelimiter).Last());
                DeserializeObject(member.GetValue(obj), settings.SetTypeInfo, settings.SetBindingFlags);
                depth.Pop();
            }
        }

        private void DeserializeObject<T>(T obj, TypeInfo typeInfo, BindingFlags bindingFlags)
        {
            foreach (var member in Utils.GetMemberInfo(obj, typeInfo, bindingFlags))
            {
                DeserializeObject(obj, member);
            }
        }

        private object DeserializePrimitive(Type type, string section, string field, string defaultValue)
        {
            Console.WriteLine("> DeserializePrimitive");
            Console.WriteLine(">> " + section);

            return source.Configs[section].GetGenericValue(type, field, defaultValue);
        }

        private void DeserializeCollection<T>(T obj, IniAttributesManager attributes, MemberInfo member)
        {
            Console.WriteLine("> DeserializeCollection");
            Console.WriteLine(">> " + attributes.sectionName);

            Type currenType = MemberInfoHelper.GetType(member).GetIListElementType();

            IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(currenType));

            IniCollectionAttributesManager collectionAttributes = new IniCollectionAttributesManager(member, attributes);

            for (int i = 0; ; i++)
            {
                string arraySectionName = collectionAttributes.GetArraySectionName(i);
                string arrayFieldName = collectionAttributes.GetArrayFieldName(i);

                if (!ContainsSection(arraySectionName))
                    return;

                IConfig config = source.Configs[arraySectionName];

                if (currenType.IsGenericValue())
                {
                    if (config == null || !config.Contains(arrayFieldName))
                        break;

                    object value = DeserializePrimitive(currenType, arraySectionName, arrayFieldName, attributes.defaultValue);                  
                    list.Add(value);
                }
                else if (currenType.GetInterface(typeof(IList).Name) != null || currenType.IsArray)
                {
                    object subObj = Activator.CreateInstance(currenType);
                    depth.Push(arraySectionName.Split((char)settings.DefaultObjectDelimiter).Last());
                    DeserializeCollection(subObj, attributes, member);
                    depth.Pop();
                    list.Add(subObj);
                }
                else if (currenType.IsClass || currenType.IsStruct())
                {
                    object subObj = Activator.CreateInstance(currenType);
                    depth.Push(arraySectionName.Split((char)settings.DefaultObjectDelimiter).Last());
                    DeserializeObject(subObj, settings.SetTypeInfo, settings.SetBindingFlags);
                    depth.Pop();
                    list.Add(subObj);
                }
            }

            if (list.Count == 0)
                return;

            if (MemberInfoHelper.GetType(member).IsArray)
            {
                Array array = Array.CreateInstance(currenType, list.Count);
                list.CopyTo(array, 0);
                member.SetValue(obj, array);
                return;
            }

            member.SetValue(obj, list);
        }
    }
}
