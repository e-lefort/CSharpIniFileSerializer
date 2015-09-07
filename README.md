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
### Section name
```csharp
[IniSectionName("GLOBAL")]
```
### Custom field name
```csharp
[IniFieldName("DisplayBool")]
```
### Personalize array delimiter
```csharp
[IniArrayDelimiter(ArrayDelimiter.Underscore)]
```
```csharp
Equal = '='
Colon = ':'
Space = ' '
Underscore = '_'
```
### Select array format
```csharp
[IniArrayType(ArrayType.Section)]
```
```csharp
Section or Key
```
### Ignore field or property
```csharp
[IniIgnore]
```
### Set default value
```csharp
[IniDefaultValue("0.25")]
```
## Read from INI File
```csharp
MyClass obj = new MyClass();
IniSerializer.Deserialize<MyClass>(ref obj,
	Path.Combine(Directory.GetCurrentDirectory(), "settings.ini"));
```
## Write to INI File
```csharp
IniSerializer.Serialize<MyClass>(obj, 
	Path.Combine(Directory.GetCurrentDirectory(), "settings.ini"));
```
# Supported Type
- Generic types (string, char, short, int, long, float, double, bool, Enum, Color, Date)
- Array and IList

# Advanced serilization (depth)
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
	
	IniSerializer.Serialize<GroupOfPerson>(gop,
		Path.Combine(Directory.GetCurrentDirectory(), "artists.ini"),
		new IniSettings() { DefaultTypeInfo = TypeInfo.Properties });
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
