# C# INI File Serializer
CSharpIniFileSerializer is a .NET Serializer for INI File. It use reflection to serialize/deserialize fields or/and properties of the given class.
Section names and field names are fully customisable with Attributes.
# Getting Started
## Simple example
```csharp
class MyClass
{
	[IniSectionName("GLOBAL")]
	string m_DisplayString;
	public string DisplayString
	{
		get { return m_DisplayString; }
		set { m_DisplayString = value; }
	}
	[IniSectionName("GLOBAL")]
	[IniFieldName("MyCustomName")]
	double m_DisplayDouble;
	public double DisplayDouble
	{
		get { return m_DisplayDouble; }
		set { m_DisplayDouble = value; }
	}
}
```
Output result (.ini)
```
[GLOBAL]
m_DisplayString=Hello World
MyCustomName=0.42
```
## Attributes
| Attribute        						| Description   		|
| -------------------------------------------------------------	| -----------------------------	|
| ``` [IniSectionName("GLOBAL") ```				| change default section name	|
| ``` [IniFieldName("DisplayBool")] ```				| change default field name	|
| ``` [IniArrayDelimiter(ArrayDelimiter.Underscore)] ```	| set array delimite		|
| ``` [IniArrayType(ArrayType.Section)] ```			| set array format      	|
| ``` [IniIgnore] ```						| ignore field or property	|
| ``` [IniDefaultValue("0.25")] ```				| set default value      	|

## Read from INI File
```csharp
MyClass obj = new MyClass();
using (StreamReader sr = new StreamReader(
	Path.Combine(Directory.GetCurrentDirectory(), "settings.ini"), true))
{
	IniReader reader = new IniReader();
	reader.Deserialize<MyClass>(ref obj, sr);
}
```
## Write to INI File
```csharp
IniWriter writer = new IniWriter();
writer.Serialize<MyClass>(obj, 
	Path.Combine(Directory.GetCurrentDirectory(), "settings.ini"));
```
# Supported Type
- Generic types (string, char, short, int, long, float, double, bool, Enum, Color, Date)
- Array and IList

# Advanced serialization (depth)
```csharp
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

static void Main(string[] args)
{
	GroupOfPerson gop = new GroupOfPerson();
	gop.Persons.Add(new Person() 
	{ 
		FirstName = "Alice", 
		LastName = "Cooper", 
		DateOfBirth = DateTime.Parse("4/02/1948") 
	});
	gop.Persons.Add(new Person() 
	{ 
		FirstName = "Marilyin", 
		LastName = "Manson", 
		DateOfBirth = DateTime.Parse("5/01/1969") 
	});
	
	IniWriter writer = new IniWriter();
	writer.settings = new IniSettings() { DefaultTypeInfo = TypeInfo.Properties };
	writer.Serialize<GroupOfPerson>(gop, 
		Path.Combine(Directory.GetCurrentDirectory(), "settings.ini"));
}
```
```
[GroupOfPerson:0/Person]
FirstName = Alice
LastName = Cooper
DateOfBirth = 04/02/1948 00:00:00
[GroupOfPerson:1/Person]
FirstName = Marilyin
LastName = Manson
DateOfBirth = 05/01/1969 00:00:00
```
