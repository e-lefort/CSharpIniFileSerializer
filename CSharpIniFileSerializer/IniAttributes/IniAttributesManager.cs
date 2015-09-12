using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpIniFileSerializer.IniAttributes;
using System.Reflection;
using CSharpIniFileSerializer.IniEnums;

namespace CSharpIniFileSerializer.IniAttributes
{
    public class IniAttributesManager
    {
        public IniFieldName iniFieldName { get; set; }
        public IniSectionName iniSectionName { get; set; }
        public IniIgnore iniIgnore { get; set; }
        public IniDefaultValue iniDefault { get; set; }

        public IniArrayDelimiter arrayDelimiter { get; set; }
        public IniArrayField arrayField { get; set; }
        public IniArrayType arrayType { get; set; }

        public IniSettings settings;

        public string sectionName;
        public string fieldName;
        public string defaultValue;

        public bool Ignore { get { return iniIgnore != null; } }
        public char ArrayDelimiter { get { return (arrayDelimiter == null) ? (char)settings.DefaultArrayDelimiter : (char)arrayDelimiter.delimiter; } }
        public ArrayType ArrayMode { get { return (arrayType == null) ? settings.DefaultArrayType : arrayType.type; } }

        public IniAttributesManager(MemberInfo member, object obj, IniSettings settings)
        {
            this.iniFieldName = (IniFieldName)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniFieldName);
            this.iniSectionName = (IniSectionName)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniSectionName);
            this.iniSectionName = iniSectionName ?? (IniSectionName)obj.GetType().GetCustomAttributes(true).FirstOrDefault(x => x is IniSectionName);
            this.iniIgnore = (IniIgnore)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniIgnore);
            this.iniDefault = (IniDefaultValue)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniDefaultValue);

            this.arrayDelimiter = (IniArrayDelimiter)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniArrayDelimiter);
            this.arrayField = (IniArrayField)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniArrayField);
            this.arrayType = (IniArrayType)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniArrayType);

            this.sectionName = (iniSectionName == null) ? obj.GetType().Name : iniSectionName.section;
            this.fieldName = (iniFieldName == null) ? member.Name : iniFieldName.field;
            this.defaultValue = (iniDefault == null) ? String.Empty : iniDefault.value;

            this.settings = settings;
        }
    }

    public class IniCollectionAttributesManager
    {
        IniAttributesManager attributes { get; set; }

        IniArrayDelimiter arrayDelimiter { get; set; }
        IniArrayField arrayField { get; set; }
        IniArrayType arrayType { get; set; }
        ArrayType arrayMode { get; set; }
        char delimiter { get; set; }

        public IniCollectionAttributesManager(MemberInfo member, IniAttributesManager attributes)
        {
            this.attributes = attributes;

            this.arrayDelimiter = (IniArrayDelimiter)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniArrayDelimiter);
            this.arrayField = (IniArrayField)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniArrayField);
            this.arrayType = (IniArrayType)member.GetCustomAttributes(true).FirstOrDefault(x => x is IniArrayType);
            this.arrayMode = (arrayType == null) ? attributes.settings.DefaultArrayType : arrayType.type;

            this.delimiter = (arrayDelimiter == null) ? (char)attributes.settings.DefaultArrayDelimiter : (char)arrayDelimiter.delimiter;
        }

        public string GetArraySectionName(int index)
        {
            if (arrayMode == ArrayType.Section)
            {
                return String.Format("{0}{1}{2}", attributes.sectionName, delimiter, index);
            }
            if (arrayMode == ArrayType.Key)
            {
                return attributes.sectionName;
            }

            throw new ArgumentException();
        }

        public string GetArrayFieldName(int index)
        {
            if (arrayMode == ArrayType.Section)
            {
                return attributes.fieldName;
            }
            if (arrayMode == ArrayType.Key)
            {
                return String.Format("{0}{1}{2}", attributes.fieldName, delimiter, index);
            }

            throw new ArgumentException();
        }
    }
}
