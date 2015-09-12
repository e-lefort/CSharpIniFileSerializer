using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nini.Config;

namespace CSharpIniFileSerializer.IniSerializer
{
    public class IniAbstractSerializer
    {
        protected IConfigSource source;
        protected Stack<object> recurciveStackOverFlow = new Stack<object>();
        protected Stack<string> depth = new Stack<string>();
        protected IniSettings settings = new IniSettings();

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
