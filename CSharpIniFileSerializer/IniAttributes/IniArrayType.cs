using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpIniFileSerializer.IniEnums;

namespace CSharpIniFileSerializer.IniAttributes
{
    [System.AttributeUsage(System.AttributeTargets.Field | AttributeTargets.Property)]
    public class IniArrayType : System.Attribute
    {
        public ArrayType type;

        public IniArrayType(ArrayType type)
        {
            this.type = type;
        }
    }
}
