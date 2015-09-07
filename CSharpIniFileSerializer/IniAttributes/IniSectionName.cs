using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpIniFileSerializer.IniAttributes
{
    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property)]
    public class IniFieldName : System.Attribute
    {
        public string field;

        public IniFieldName(string field)
        {
            this.field = field;
        }
    }
}
