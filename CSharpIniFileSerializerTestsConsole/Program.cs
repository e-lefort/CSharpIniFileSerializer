using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpIniFileSerializerTests;
using System.IO;
using CSharpIniFileSerializer.IniSerializer;

namespace CSharpIniFileSerializerTestsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            IniWriteTests iniWriteTests = new IniWriteTests();

            iniWriteTests.WriteListOfObjectTest();

        }
    }
}
