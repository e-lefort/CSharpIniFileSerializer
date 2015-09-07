using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using CSharpIniFileSerializer;
using CSharpIniFileSerializer.IniAttributes;
using CSharpIniFileSerializer.IniEnums;

namespace CSharpIniFileSerializerWinFormSample
{
    public enum PropertyGridEnum
    {
        [Description("Premier")]
        First = 0,
        [Description("Dernier")]
        Last
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    [IniSectionName("CLASS_SECTION")]
    class PropertyGridSimpleDemoClass
    {
        Person m_Person = new Person();

        List<Person> m_GroupOfPerson = new List<Person>();

        PropertyGridSimpleDemoClass m_DisplayPropertyGridSimpleDemoClass;
        public PropertyGridSimpleDemoClass DisplayPropertyGridSimpleDemoClass
        {
            get { return m_DisplayPropertyGridSimpleDemoClass; }
            set { m_DisplayPropertyGridSimpleDemoClass = value; }
        }

        [IniSectionName("ListOfDouble")]
        [IniFieldName("value")]
        [IniArrayDelimiter(ArrayDelimiter.Underscore)]
        List<double> m_ListOfDouble;
        public List<double> ListOfDouble
        {
            get { return m_ListOfDouble; }
            set { m_ListOfDouble = value; }
        }

        [IniSectionName("GLOBAL")]
        List<string> m_ListOfString;
        public List<string> ListOfString
        {
            get { return m_ListOfString; }
            set { m_ListOfString = value; }
        }

        [IniSectionName("ArrayOfInt")]
        [IniFieldName("value")]
        [IniArrayType(ArrayType.Key)]
        [IniArrayDelimiter(ArrayDelimiter.Underscore)]
        int[] m_ArrayOfInt = new int[0];
        public int[] ArrayOfInt
        {
            get { return m_ArrayOfInt; }
            set { m_ArrayOfInt = value; }
        }

        [IniSectionName("GLOBAL")]
        string m_DisplayString;
        public string DisplayString
        {
            get { return m_DisplayString; }
            set { m_DisplayString = value; }
        }

        [IniSectionName("GLOBAL")]
        PropertyGridEnum m_DisplayEnum;
        public PropertyGridEnum DisplayEnum
        {
            get { return m_DisplayEnum; }
            set { m_DisplayEnum = value; }
        }

        [IniSectionName("GLOBAL")]
        double m_DisplayDouble;
        public double DisplayDouble
        {
            get { return m_DisplayDouble; }
            set { m_DisplayDouble = value; }
        }

        [IniSectionName("GLOBAL")]
        [IniIgnore]
        int m_DisplayInt;
        public int DisplayInt
        {
            get { return m_DisplayInt; }
            set { m_DisplayInt = value; }
        }

        [IniFieldName("DisplayBool")]
        bool m_DisplayBool;
        //[ReadOnly(true)]
        public bool DisplayBool
        {
            get { return m_DisplayBool; }
            set { m_DisplayBool = value; }
        }

        [IniSectionName("COLOUR")]
        Color m_DisplayColors;
        [Description("Couleur description")]
        [Category("Couleur")]
        [DisplayName("Couleur")]
        public Color DisplayColors
        {
            get { return m_DisplayColors; }
            set { m_DisplayColors = value; }
        }

        DateTime m_DisplayDate = DateTime.Now;
        public DateTime DisplayDate
        {
            get { return m_DisplayDate; }
            set { m_DisplayDate = value; }
        }

        public PropertyGridSimpleDemoClass()
        {
            m_ListOfString = new List<string>();
            m_ListOfString.Add("value 1");
            m_ListOfString.Add("value 2");

            m_ListOfDouble = new List<double>();
            m_ListOfDouble.Add(0.1);
            m_ListOfDouble.Add(0.2);

            m_DisplayPropertyGridSimpleDemoClass = this;
            m_Person = new Person() { FirstName = "Alice", LastName = "Cooper", DateOfBirth = DateTime.Parse("4/02/1948") };

            m_GroupOfPerson.Add(new Person() { FirstName = "Alice", LastName = "Cooper", DateOfBirth = DateTime.Now });
            m_GroupOfPerson.Add(new Person() { FirstName = "Marilyin", LastName = "Manson", DateOfBirth = DateTime.Now });
        }

        public static PropertyGridSimpleDemoClass Load()
        {
            PropertyGridSimpleDemoClass obj = new PropertyGridSimpleDemoClass();
            IniSerializer.Deserialize<PropertyGridSimpleDemoClass>(ref obj, Path.Combine(Directory.GetCurrentDirectory(), "settings.ini"));
            return obj;
        }

        public static void Save(PropertyGridSimpleDemoClass obj)
        {
            IniSerializer.Serialize<PropertyGridSimpleDemoClass>(obj, Path.Combine(Directory.GetCurrentDirectory(), "settings.ini"));
        }
    }
}
