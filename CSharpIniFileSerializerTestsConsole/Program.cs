using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpIniFileSerializerTests;

namespace CSharpIniFileSerializerTestsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            IniWriteTests iniWriteTests = new IniWriteTests();

            iniWriteTests.WriteListOfObjectTest2();

        }
    }
}
