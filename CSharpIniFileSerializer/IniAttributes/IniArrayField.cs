using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpIniFileSerializer.IniEnums;

namespace CSharpIniFileSerializer.IniAttributes
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class IniArrayField : System.Attribute
    {
        private ArrayField value;
        private string custom;

        public string GetValue(IniSettings settings)
        {
            if (value == ArrayField.Custom)
                return custom;
            return ""; // settings.DefaultArrayField;
        }

        public IniArrayField(ArrayField value, string custom = "")
        {
            this.value = value;
            this.custom = custom;
        }
    }
}
