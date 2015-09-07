using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpIniFileSerializer.IniAttributes
{
    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Struct | System.AttributeTargets.Class | System.AttributeTargets.Property)]
    public class IniSectionName : System.Attribute
    {
        public string section;

        public IniSectionName(string section)
        {
            this.section = section;
        }
    }
}
