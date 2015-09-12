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
        private IEnumerable<MemberInfo> GetMemberInfo<T>(T obj, IniSettings settings)
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

        public void Serialize<T>(T obj, string path)
        {
            File.Delete(path);

            if (!File.Exists(path))
                File.Create(path).Close();

            base.source = new IniConfigSource(path);
            base.recurciveStackOverFlow = new Stack<object>();
            base.depth = new Stack<string>();

            try
            {
                SerializeObject(obj);
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

            if (depth.Count != 0 && settings.EnableDepthSectionNaming)
            {
                char delimeter = (char)settings.DefaultObjectDelimiter;
                attributes.sectionName = String.Format("{2}{1}{0}", attributes.sectionName, delimeter, depth.Reverse().Aggregate((i, j) => i + delimeter + j));
            }

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

            foreach (var member in Utils.GetMemberInfo(obj, settings))
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

            IList list = value as IList;
            if (list == null)
                return;

            IniCollectionAttributesManager collectionAttributes = new IniCollectionAttributesManager(member, attributes);

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
                    SerializeCollection(list[i], attributes, member);
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
