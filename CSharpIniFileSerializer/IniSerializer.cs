using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Drawing;
using Nini.Config;
using System.IO;
using System.Collections;
using System.ComponentModel;
using CSharpIniFileSerializer.IniAttributes;
using CSharpIniFileSerializer.IniEnums;

namespace CSharpIniFileSerializer
{
    public class IniSerializer
    {        
        public static IEnumerable<MemberInfo> GetMemberInfo<T>(T obj, IniSettings settings)
        {
            List<MemberInfo> members = new List<MemberInfo>();
            if ((settings.DefaultTypeInfo & TypeInfo.Fields) == TypeInfo.Fields)
            {
                members.AddRange(obj.GetType().GetFields(settings.DefaultBindingFlags));
            }
            if ((settings.DefaultTypeInfo & TypeInfo.Properties) == TypeInfo.Properties)
            {
                members.AddRange(obj.GetType().GetProperties(settings.DefaultBindingFlags));
            }
            return members;
        }

        public static void Serialize<T>(T obj, string path, IniSettings settings = null, bool overwrite = true)
        {
            settings = settings ?? new IniSettings();

            if (overwrite && File.Exists(path))
                File.Delete(path);

            if (!File.Exists(path))
                File.Create(path).Close();

            IConfigSource source = new IniConfigSource(path);

            try
            {
                Stack<object> recurciveStackOverFlow = new Stack<object>();
                Stack<string> depth = new Stack<string>();
                IniWriter.Serialize<T>(obj, ref source, ref settings, ref recurciveStackOverFlow, ref depth);
            }
            finally
            {
                source.Save();   
            }
        }

        public static void Deserialize<T>(ref T obj, string path, IniSettings settings = null)
        {
            settings = settings ?? new IniSettings();

            if (!File.Exists(path))
                File.Create(path).Close();

            using (StreamReader reader = new StreamReader(path, true))
            {
                IConfigSource source = new IniConfigSource(reader);

                Stack<object> recurciveStackOverFlow = new Stack<object>();
                Stack<string> depth = new Stack<string>();
                IniReader.Deserialize<T>(ref obj, ref source, ref settings, ref recurciveStackOverFlow, ref depth);
            }
        }
    }
}
