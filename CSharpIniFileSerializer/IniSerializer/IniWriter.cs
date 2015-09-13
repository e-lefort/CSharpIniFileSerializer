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
    public class IniWriter : IniAbstractSerializer
    {
        public void Serialize<T>(T obj, string path)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            File.Delete(path);

            if (!File.Exists(path))
                File.Create(path).Close();

            base.source = new IniConfigSource(path);
            base.recurciveStackOverFlow = new Stack<object>();
            base.depth = new Stack<string>();

            Type currenType = obj.GetType();
            MemberInfo member = Utils.GetMemberInfo(obj, settings.SetTypeInfo, settings.SetBindingFlags).First();
            IniAttributesManager attributes = new IniAttributesManager(member, obj, settings);

            try
            {
                if (currenType.GetInterface(typeof(IList).Name) != null || currenType.IsArray)
                {
                    IniCollectionAttributesManager collectionAttributes = new IniCollectionAttributesManager(member, attributes);
                    Type type = obj.GetType().GetGenericArguments()[0];
                    attributes.sectionName = "ArrayOf" + type.Name;
                    SerializeCollection(obj, type, collectionAttributes);
                }
                else if (currenType.IsClass || currenType.IsStruct())
                {
                    SerializeObject(obj);
                }
                else
                    throw new NotImplementedException();
            }
            finally
            {
                source.Save();   
            }
        }

        private void SerializeObject<T>(T obj, MemberInfo member)
        {
            Type currenType = MemberInfoHelper.GetType(member);
            IniAttributesManager attributes = new IniAttributesManager(member, obj, settings);

            if (attributes.Ignore)
                return;

            object value = member.GetValue(obj);

            IncrementDepth(attributes);

            if (currenType.IsGenericValue())
            {
                SerializePrimitive(currenType, attributes.sectionName, attributes.fieldName, value);
            }
            else if (currenType.GetInterface(typeof(IList).Name) != null || currenType.IsArray)
            {
                SerializeCollection(obj, attributes, member);
            }
            else if (currenType.IsClass || currenType.IsStruct())
            {
                depth.Push(attributes.sectionName.Split((char)settings.DefaultObjectDelimiter).Last());
                SerializeObject(member.GetValue(obj));
                depth.Pop();
            }
        }

        private void SerializeObject<T>(T obj)
        {
            if (recurciveStackOverFlow.Contains(obj))
                return;
            recurciveStackOverFlow.Push(obj);

            foreach (var member in Utils.GetMemberInfo(obj, settings.SetTypeInfo, settings.SetBindingFlags))
            {
                SerializeObject(obj, member);
            }

            recurciveStackOverFlow.Pop();
        }

        private void SerializePrimitive(Type type, string section, string field, object value)
        {
            source.Configs.Add(section).SetGenericValue(type, field, value);
        }

        private void SerializeCollection<T>(T obj, IniAttributesManager attributes, MemberInfo member)
        {
            object value = member.GetValue(obj);
            Type fieldType = MemberInfoHelper.GetType(member);
            Type currenType = fieldType.GetElementType() ?? fieldType.GetGenericArguments().First();

            IniCollectionAttributesManager collectionAttributes = new IniCollectionAttributesManager(member, attributes);

            SerializeCollection(value, currenType, collectionAttributes);
        }

        private void SerializeCollection(object value, Type currenType, IniCollectionAttributesManager collectionAttributes)
        {
            IList list = value as IList;
            if (list == null)
                return;

            for (int i = 0; i < list.Count; i++)
            {
                string arraySectionName = collectionAttributes.GetArraySectionName(i);
                string arrayFieldName = collectionAttributes.GetArrayFieldName(i);

                if (currenType.IsGenericValue())
                {
                    source.Configs.Add(arraySectionName);
                    SerializePrimitive(currenType, arraySectionName, arrayFieldName, list[i]);
                }
                else if (currenType.GetInterface(typeof(IList).Name) != null || currenType.IsArray)
                {
                    depth.Push(arraySectionName.Split((char)settings.DefaultObjectDelimiter).Last());
                    SerializeCollection(list[i], currenType.GetGenericArguments()[0], new IniCollectionAttributesManager(collectionAttributes.attributes));
                    depth.Pop();
                }
                else
                {
                    depth.Push(arraySectionName.Split((char)settings.DefaultObjectDelimiter).Last());
                    SerializeObject(list[i]);
                    depth.Pop();
                }

            }
        }
    }
}
