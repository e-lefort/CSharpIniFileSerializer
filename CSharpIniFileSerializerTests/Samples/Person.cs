using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpIniFileSerializer.IniAttributes;
using CSharpIniFileSerializer.IniEnums;

namespace CSharpIniFileSerializerTests.Samples
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class GroupOfPerson
    {
        [IniArrayType(ArrayType.Section)]
        public List<Person> Persons { get; set; }

        public GroupOfPerson()
        {
            this.Persons = new List<Person>();
        }
    }

    public class GroupOfGroupOfPerson
    {
        [IniSectionName("LIST_OF_GROUP")]
        [IniArrayType(ArrayType.Section)]
        public List<GroupOfPerson> GroupOfPersons { get; set; }

        public GroupOfGroupOfPerson()
        {
            this.GroupOfPersons = new List<GroupOfPerson>();
        }
    }
}
