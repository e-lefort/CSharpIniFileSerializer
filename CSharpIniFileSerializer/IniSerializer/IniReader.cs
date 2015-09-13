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
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (reader == null)
                throw new ArgumentNullException("reader");

            base.source = new IniConfigSource(reader);
            base.recurciveStackOverFlow = new Stack<object>();
            base.depth = new Stack<string>();

            Type currenType = obj.GetType();
            MemberInfo member = Utils.GetMemberInfo(obj, settings.SetTypeInfo, settings.SetBindingFlags).First();
            IniAttributesManager attributes = new IniAttributesManager(member, obj, settings);

            if (currenType.GetInterface(typeof(IList).Name) != null || currenType.IsArray)
            {
                IniCollectionAttributesManager collectionAttributes = new IniCollectionAttributesManager(attributes);
                attributes.sectionName = "ArrayOf" + obj.GetType().GetGenericArguments()[0].Name;
                DeserializeCollection(obj as IList, collectionAttributes);
            }
            else if (currenType.IsClass || currenType.IsStruct())
            {
                DeserializeObject(ref obj, settings.SetTypeInfo, settings.SetBindingFlags);
            }
            else
                throw new NotImplementedException();
        }

        private void DeserializeObject<T>(ref T obj, MemberInfo member)
        {
            Console.WriteLine("> DeserializeObject");

            Type currenType = MemberInfoHelper.GetType(member);
            IniAttributesManager attributes = new IniAttributesManager(member, obj, settings);

            if (attributes.Ignore)
                return;

            IncrementDepth(attributes);

            if (!ContainsSection(attributes.sectionName))
            {
                obj = default(T);
                return;
            }

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
                object refObj = member.GetValue(obj);
                DeserializeObject(ref refObj, settings.SetTypeInfo, settings.SetBindingFlags);
                depth.Pop();
            }
        }

        private void DeserializeObject<T>(ref T obj, TypeInfo typeInfo, BindingFlags bindingFlags)
        {
            foreach (var member in Utils.GetMemberInfo(obj, typeInfo, bindingFlags))
            {
                if (obj == null)
                    break;

                DeserializeObject(ref obj, member);
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

            DeserializeCollection(list, collectionAttributes);

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

        private void DeserializeCollection(IList list, IniCollectionAttributesManager collectionAttributes)
        {
            Type currenType = list.GetType().GetGenericArguments()[0];

            for (int i = 0; ; i++)
            {
                string arraySectionName = collectionAttributes.GetArraySectionName(i);
                string arrayFieldName = collectionAttributes.GetArrayFieldName(i);

                string sectionNameCached = arraySectionName;

                IConfig config = source.Configs[arraySectionName];

                if (currenType.IsGenericValue())
                {
                    if (!ContainsSection(arraySectionName))
                        break;

                    if (config == null || !config.Contains(arrayFieldName))
                        break;

                    object value = DeserializePrimitive(currenType, arraySectionName, arrayFieldName, collectionAttributes.attributes.defaultValue);
                    list.Add(value);
                }
                else if (currenType.GetInterface(typeof(IList).Name) != null || currenType.IsArray)
                {
                    if (!ContainsSection(arraySectionName))
                        break;

                    object subObj = Activator.CreateInstance(currenType);
                    depth.Push(arraySectionName.Split((char)settings.DefaultObjectDelimiter).Last());
                    DeserializeCollection(subObj as IList, new IniCollectionAttributesManager(collectionAttributes.attributes));
                    if (subObj == null)
                    {
                        depth.Pop();
                        break;
                    }
                    depth.Pop();
                    list.Add(subObj);
                }
                else if (currenType.IsClass || currenType.IsStruct())
                {
                    object subObj = Activator.CreateInstance(currenType);
                    depth.Push(arraySectionName.Split((char)settings.DefaultObjectDelimiter).Last());
                    DeserializeObject(ref subObj, settings.SetTypeInfo, settings.SetBindingFlags);
                    if (subObj == null)
                    {
                        depth.Pop();
                        break;
                    }
                    depth.Pop();
                    list.Add(subObj);
                }
            }
        }
    }
}
