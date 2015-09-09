using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpIniFileSerializer.IniEnums;
using System.Reflection;
using CSharpIniFileSerializer.IniAttributes;
using System.IO;

namespace CSharpIniFileSerializer
{
    public class IniSettings
    {
        public ArrayDelimiter DefaultArrayDelimiter { get; set; }
        public BindingFlags SetBindingFlags { get; set; }
        public TypeInfo SetTypeInfo { get; set; }
        public ArrayType DefaultArrayType { get; set; }
        public ObjectDelimiter DefaultObjectDelimiter { get; set; }
        public bool EnableDepthSectionNaming { get; set; }

        public IniSettings()
        {
            DefaultArrayDelimiter = ArrayDelimiter.Colon;
            SetBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            SetTypeInfo = TypeInfo.Fields;
            DefaultArrayType = ArrayType.Section;
            DefaultObjectDelimiter = ObjectDelimiter.Slash;
            EnableDepthSectionNaming = true;
        }

        public static void Save(IniSettings obj)
        {
            IniSerializer.Serialize<IniSettings>(obj, Path.Combine(Directory.GetCurrentDirectory(), "inisettings.ini"), new IniSettings() { SetTypeInfo = TypeInfo.All });
        }

        public static IniSettings Load()
        {
            IniSettings settings = new IniSettings();
            IniSerializer.Deserialize<IniSettings>(ref settings, Path.Combine(Directory.GetCurrentDirectory(), "inisettings.ini"), new IniSettings() { SetTypeInfo = TypeInfo.Properties });
            return settings;
        }
    }
}
