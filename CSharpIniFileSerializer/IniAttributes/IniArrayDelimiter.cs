using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpIniFileSerializer.IniEnums;

namespace CSharpIniFileSerializer.IniAttributes
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class IniArrayDelimiter : System.Attribute
    {
        public ArrayDelimiter delimiter;

        public IniArrayDelimiter(ArrayDelimiter delimiter)
        {
            this.delimiter = delimiter;
        }
    }
}
