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
        public void WriteObjectTest()
        {
            IniSettings settings = new IniSettings() { SetTypeInfo = TypeInfo.Properties };

            Person person = new Person() { FirstName = "Alice", LastName = "Cooper", DateOfBirth = DateTime.Parse("4/02/1948") };

            CSharpIniFileSerializer.IniSerializer.IniWriter writer = new CSharpIniFileSerializer.IniSerializer.IniWriter();
            writer.settings = settings;
            writer.Serialize<Person>(person, Path.Combine(Directory.GetCurrentDirectory(), "WriteDefaultTest.ini"));

            Person person2 = new Person();

            using (StreamReader sr = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "WriteDefaultTest.ini"), true))
            {
                CSharpIniFileSerializer.IniSerializer.IniReader reader = new CSharpIniFileSerializer.IniSerializer.IniReader();
                reader.settings = settings;
                reader.Deserialize<Person>(ref person2, sr);
            }

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

            CSharpIniFileSerializer.IniSerializer.IniWriter writer = new CSharpIniFileSerializer.IniSerializer.IniWriter();
            writer.settings = new IniSettings() { SetTypeInfo = TypeInfo.Properties, DefaultArrayType = ArrayType.Section };
            writer.Serialize<GroupOfPerson>(origin, Path.Combine(Directory.GetCurrentDirectory(), "WriteListOfObjectTest.ini"));

            GroupOfPerson serialized = new GroupOfPerson();

            using (StreamReader sr = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "WriteListOfObjectTest.ini"), true))
            {
                CSharpIniFileSerializer.IniSerializer.IniReader reader = new CSharpIniFileSerializer.IniSerializer.IniReader();
                reader.settings = new IniSettings() { SetTypeInfo = TypeInfo.Properties, DefaultArrayType = ArrayType.Section };
                reader.Deserialize<GroupOfPerson>(ref serialized, sr);
            }

            writer = new CSharpIniFileSerializer.IniSerializer.IniWriter();
            writer.settings = new IniSettings() { SetTypeInfo = TypeInfo.Properties, DefaultArrayType = ArrayType.Section };
            writer.Serialize<GroupOfPerson>(serialized, Path.Combine(Directory.GetCurrentDirectory(), "WriteListOfObjectTest_serialized.ini"));

            Assert.AreEqual(origin.Persons.Count, serialized.Persons.Count);

            for (int i = 0; i < origin.Persons.Count; i++)
            {
                Assert.AreEqual(origin.Persons[i].FirstName, serialized.Persons[i].FirstName);
                Assert.AreEqual(origin.Persons[i].LastName, serialized.Persons[i].LastName);
                Assert.AreEqual(origin.Persons[i].DateOfBirth, serialized.Persons[i].DateOfBirth);
            }
        }

        [Test]
        public void WriteObjectOfListOfObjectTest()
        {
            IniSettings settings = new IniSettings() { SetTypeInfo = TypeInfo.Properties, DefaultArrayType = ArrayType.Section };

            GroupOfGroupOfPerson origin = new GroupOfGroupOfPerson();

            GroupOfPerson groupOfPerson = new GroupOfPerson();
            groupOfPerson.Persons.Add(new Person() { FirstName = "Alice", LastName = "Cooper", DateOfBirth = DateTime.Parse("4/02/1948") });
            groupOfPerson.Persons.Add(new Person() { FirstName = "Marilyin", LastName = "Manson", DateOfBirth = DateTime.Parse("5/01/1969") });

            GroupOfPerson groupOfPerson2 = new GroupOfPerson();
            groupOfPerson2.Persons.Add(new Person() { FirstName = "Alice", LastName = "Cooper", DateOfBirth = DateTime.Parse("4/02/1948") });
            groupOfPerson2.Persons.Add(new Person() { FirstName = "Marilyin", LastName = "Manson", DateOfBirth = DateTime.Parse("5/01/1969") });

            origin.GroupOfPersons.Add(groupOfPerson);
            origin.GroupOfPersons.Add(groupOfPerson2);

            CSharpIniFileSerializer.IniSerializer.IniWriter writer = new CSharpIniFileSerializer.IniSerializer.IniWriter();
            writer.settings = settings;
            writer.Serialize<GroupOfGroupOfPerson>(origin, Path.Combine(Directory.GetCurrentDirectory(), "WriteListOfObjectTest2.ini"));

            GroupOfGroupOfPerson serialized = new GroupOfGroupOfPerson();

            using (StreamReader sr = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "WriteListOfObjectTest2.ini"), true))
            {
                CSharpIniFileSerializer.IniSerializer.IniReader reader = new CSharpIniFileSerializer.IniSerializer.IniReader();
                reader.settings = settings;
                reader.Deserialize<GroupOfGroupOfPerson>(ref serialized, sr);
            }

            writer = new CSharpIniFileSerializer.IniSerializer.IniWriter();
            writer.settings = settings;
            writer.Serialize<GroupOfGroupOfPerson>(serialized, Path.Combine(Directory.GetCurrentDirectory(), "WriteListOfObjectTest2_serialized.ini"));
        }

        [Test]
        public void WriteObjectOfListInterface()
        {
            Chenille chenille = new Chenille();
            chenille.dogs.Add(new Dog("Fido"));
            chenille.dogs.Add(new Dog("Bob"));
            chenille.dogs.Add(new Dog("Adam"));

            CSharpIniFileSerializer.IniSerializer.IniWriter writer = new CSharpIniFileSerializer.IniSerializer.IniWriter();
            writer.Serialize<Chenille>(chenille, Path.Combine(Directory.GetCurrentDirectory(), "WriteIAnimal.ini"));
        }

        [Test]
        public void WriteListOfObject()
        {
            IniSettings settings = new IniSettings() { SetTypeInfo = TypeInfo.Properties, DefaultArrayType = ArrayType.Section };

            List<Dog> dogs = new List<Dog>();
            dogs.Add(new Dog("Fido"));
            dogs.Add(new Dog("Bob"));
            dogs.Add(new Dog("Adam"));

            CSharpIniFileSerializer.IniSerializer.IniWriter writer = new CSharpIniFileSerializer.IniSerializer.IniWriter();
            writer.settings = settings;
            writer.Serialize<List<Dog>>(dogs, Path.Combine(Directory.GetCurrentDirectory(), "WriteListOfObject.ini"));

            List<Dog> obj = new List<Dog>();

            using (StreamReader sr = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "WriteListOfObject.ini"), true))
            {
                CSharpIniFileSerializer.IniSerializer.IniReader reader = new CSharpIniFileSerializer.IniSerializer.IniReader();
                reader.settings = settings;
                reader.Deserialize<List<Dog>>(ref obj, sr);
            }

            Assert.AreEqual(obj.Count, dogs.Count);

            for (int i = 0; i < dogs.Count; i++ )
            {
                Assert.AreEqual(obj[i].Name, dogs[i].Name);
            }
        }

        [Test]
        public void WriteListOfListOfObject()
        {
            IniSettings settings = new IniSettings() { SetTypeInfo = TypeInfo.Properties, DefaultArrayType = ArrayType.Section };

            List<Dog> dogs = new List<Dog>();
            dogs.Add(new Dog("Fido"));
            dogs.Add(new Dog("Bob"));
            dogs.Add(new Dog("Adam"));

            List<List<Dog>> all = new List<List<Dog>>();
            all.Add(dogs);
            all.Add(dogs);

            CSharpIniFileSerializer.IniSerializer.IniWriter writer = new CSharpIniFileSerializer.IniSerializer.IniWriter();
            writer.settings = settings;
            //writer.Serialize<List<List<Dog>>>(all, Path.Combine(Directory.GetCurrentDirectory(), "WriteListOfListOfObject.ini"));

            List<List<Dog>> serialized = new List<List<Dog>>();

            using (StreamReader sr = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "WriteListOfListOfObject.ini"), true))
            {
                CSharpIniFileSerializer.IniSerializer.IniReader reader = new CSharpIniFileSerializer.IniSerializer.IniReader();
                reader.settings = settings;
                reader.Deserialize<List<List<Dog>>>(ref serialized, sr);
            }

        }
    }
}
