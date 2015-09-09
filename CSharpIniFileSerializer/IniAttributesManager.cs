using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpIniFileSerializer.IniAttributes;
using System.Reflection;
using CSharpIniFileSerializer.IniEnums;

namespace CSharpIniFileSerializer
{
    class IniAttributesManager
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
}
