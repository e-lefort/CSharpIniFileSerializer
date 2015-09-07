using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CSharpIniFileSerializerTests.Samples;
using CSharpIniFileSerializer;
using System.IO;
using CSharpIniFileSerializer.IniEnums;
using CSharpIniFileSerializerTests.Samples.Animal;

namespace CSharpIniFileSerializerTests
{
    [TestFixture]
    public class IniWriteTests
    {
        [Test]
        public void WriteDefaultTest()
        {
            Person person = new Person() { FirstName = "Alice", LastName = "Cooper", DateOfBirth = DateTime.Parse("4/02/1948") };

            IniSerializer.Serialize<Person>(person,
                Path.Combine(Directory.GetCurrentDirectory(), "WriteDefaultTest.ini"), 
                new IniSettings() { DefaultTypeInfo = TypeInfo.Properties });

            Person person2 = new Person();

            IniSerializer.Deserialize<Person>(ref person2,
                Path.Combine(Directory.GetCurrentDirectory(), "WriteDefaultTest.ini"),
                new IniSettings() { DefaultTypeInfo = TypeInfo.Properties });

            Assert.AreEqual(person.FirstName, person2.FirstName);
            Assert.AreEqual(person.LastName, person2.LastName);
            Assert.AreEqual(person.DateOfBirth, person2.DateOfBirth);
        }

        [Test]
        public void WriteListOfObjectTest()
        {
            GroupOfPerson origin = new GroupOfPerson();
            origin.Persons.Add(new Person() { FirstName = "Alice", LastName = "Cooper", DateOfBirth = DateTime.Parse("4/02/1948") });
            origin.Persons.Add(new Person() { FirstName = "Marilyin", LastName = "Manson", DateOfBirth = DateTime.Parse("5/01/1969") });

            IniSerializer.Serialize<GroupOfPerson>(origin,
                Path.Combine(Directory.GetCurrentDirectory(), "WriteListOfObjectTest.ini"),
                new IniSettings() { DefaultTypeInfo = TypeInfo.Properties, DefaultArrayType = ArrayType.Section });

            GroupOfPerson serialized = new GroupOfPerson();

            IniSerializer.Deserialize<GroupOfPerson>(ref serialized,
                Path.Combine(Directory.GetCurrentDirectory(), "WriteListOfObjectTest.ini"),
                new IniSettings() { DefaultTypeInfo = TypeInfo.Properties, DefaultArrayType = ArrayType.Section });

            IniSerializer.Serialize<GroupOfPerson>(serialized,
                Path.Combine(Directory.GetCurrentDirectory(), "WriteListOfObjectTest_serialized.ini"),
                new IniSettings() { DefaultTypeInfo = TypeInfo.Properties, DefaultArrayType = ArrayType.Section });

            Assert.AreEqual(origin.Persons.Count, serialized.Persons.Count);

            for (int i = 0; i < origin.Persons.Count; i++)
            {
                Assert.AreEqual(origin.Persons[i].FirstName, serialized.Persons[i].FirstName);
                Assert.AreEqual(origin.Persons[i].LastName, serialized.Persons[i].LastName);
                Assert.AreEqual(origin.Persons[i].DateOfBirth, serialized.Persons[i].DateOfBirth);
            }
        }

        [Test]
        public void WriteListOfObjectTest2()
        {
            GroupOfGroupOfPerson origin = new GroupOfGroupOfPerson();

            GroupOfPerson groupOfPerson = new GroupOfPerson();
            groupOfPerson.Persons.Add(new Person() { FirstName = "Alice", LastName = "Cooper", DateOfBirth = DateTime.Parse("4/02/1948") });
            groupOfPerson.Persons.Add(new Person() { FirstName = "Marilyin", LastName = "Manson", DateOfBirth = DateTime.Parse("5/01/1969") });

            GroupOfPerson groupOfPerson2 = new GroupOfPerson();
            groupOfPerson2.Persons.Add(new Person() { FirstName = "Alice", LastName = "Cooper", DateOfBirth = DateTime.Parse("4/02/1948") });
            groupOfPerson2.Persons.Add(new Person() { FirstName = "Marilyin", LastName = "Manson", DateOfBirth = DateTime.Parse("5/01/1969") });

            origin.GroupOfPersons.Add(groupOfPerson);
            origin.GroupOfPersons.Add(groupOfPerson2);

            IniSerializer.Serialize<GroupOfGroupOfPerson>(origin,
                Path.Combine(Directory.GetCurrentDirectory(), "WriteListOfObjectTest2.ini"),
                new IniSettings() { DefaultTypeInfo = TypeInfo.Properties, DefaultArrayType = ArrayType.Section });

            GroupOfGroupOfPerson serialized = new GroupOfGroupOfPerson();

            IniSerializer.Deserialize<GroupOfGroupOfPerson>(ref serialized,
                Path.Combine(Directory.GetCurrentDirectory(), "WriteListOfObjectTest2.ini"),
                new IniSettings() { DefaultTypeInfo = TypeInfo.Properties, DefaultArrayType = ArrayType.Section });

            IniSerializer.Serialize<GroupOfGroupOfPerson>(serialized,
               Path.Combine(Directory.GetCurrentDirectory(), "WriteListOfObjectTest2_serialized.ini"),
               new IniSettings() { DefaultTypeInfo = TypeInfo.Properties, DefaultArrayType = ArrayType.Section });
        }

        [Test]
        public void WriteDogs()
        {
            List<Dog> dogs = new List<Dog>();
            dogs.Add(new Dog("Fido"));
            dogs.Add(new Dog("Bob"));
            dogs.Add(new Dog("Adam"));

            IniSerializer.Serialize<List<Dog>>(dogs,
                Path.Combine(Directory.GetCurrentDirectory(), "WriteDogs.ini"),
                new IniSettings() { DefaultTypeInfo = TypeInfo.Properties, DefaultArrayType = ArrayType.Section });
        }
    }
}
