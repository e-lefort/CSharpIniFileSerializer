using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpIniFileSerializer.IniEnums
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue",
        Justification = "I'm defining and using enum values with specific characters and null character, aka '\0', has no purpose here.")]
    public enum ObjectDelimiter
    {
        Slash = '/',
        BackSlash = '\\',
        Dot = '.'
    }
}
