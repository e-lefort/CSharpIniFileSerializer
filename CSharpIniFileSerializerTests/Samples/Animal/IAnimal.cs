using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpIniFileSerializerTests.Samples.Animal
{
    interface IAnimal
    {
        string Describe();

        string Name { get; set; }
    }
}
