using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nini.Config;
using CSharpIniFileSerializer.IniAttributes;

namespace CSharpIniFileSerializer.IniSerializer
{
    public class IniAbstractSerializer
    {
        protected IConfigSource source;
        protected Stack<object> recurciveStackOverFlow = new Stack<object>();
        protected Stack<string> depth = new Stack<string>();
        public IniSettings settings = new IniSettings();

        public void IncrementDepth(IniAttributesManager attributes)
        {
            if (depth.Count != 0 && settings.EnableDepthSectionNaming)
            {
                char delimeter = (char)settings.DefaultObjectDelimiter;
                attributes.sectionName = String.Format("{2}{1}{0}", attributes.sectionName, delimeter, depth.Reverse().Aggregate((i, j) => i + delimeter + j));
            }
        }

        public bool ContainsSection(string section)
        {
            foreach (var c in source.Configs)
            {
                IConfig conf = c as IConfig;

                if (conf.Name.StartsWith(section))
                    return true;
            }
            return false;
        }
    }
}
