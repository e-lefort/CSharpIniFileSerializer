using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpIniFileSerializer.IniAttributes
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class IniDefaultValue : System.Attribute
    {
        public string value;

        public IniDefaultValue(string value)
        {
            this.value = value;
        }
    }
}
