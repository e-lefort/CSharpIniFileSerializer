using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpIniFileSerializer.IniEnums
{
    [Flags]
    public enum TypeInfo
    {
        Fields = 1,
        Properties = 2,
        All = Fields | Properties
    }
}
