# Prototype Pattern

## Overview
The Prototype pattern creates new objects by cloning existing instances instead of creating them from scratch. Think of it like using a photocopier - instead of writing a document from the beginning each time, you make copies of an existing document and modify them as needed.

## Problem It Solves
Imagine you're building a document management system where you need to create many similar documents:
- Templates for reports, letters, and invoices
- Each document has complex formatting and structure
- Creating from scratch is expensive and time-consuming
- You want variations of existing documents

Without Prototype, you'd have to:
```csharp
// BAD: Recreating complex objects from scratch
var report1 = new ComplexReport();
report1.SetupHeaders();
report1.ConfigureFormatting();
report1.AddStandardSections();
report1.SetDefaultStyles();
// ... lots of setup code

var report2 = new ComplexReport();
report2.SetupHeaders();         // Repeating the same setup
report2.ConfigureFormatting();  // over and over again
report2.AddStandardSections();
report2.SetDefaultStyles();
// ... same setup code again
```

This is inefficient and error-prone.

## Real-World Analogy
Think of a **cookie cutter and dough**:
1. You have a master cookie cutter (prototype)
2. You roll out dough (the cloning process)
3. You press the cutter to create identical cookie shapes (clones)
4. You can then decorate each cookie differently (customize clones)

Or consider **Microsoft Word templates**:
- You have a resume template with formatting, sections, and layout
- You "clone" it by creating a new document from the template
- You fill in your specific information
- Everyone gets the same professional structure but with their own content

## Implementation Details

### Basic Structure
```csharp
// Prototype interface
public interface IPrototype<T>
{
    T Clone();
}

// Concrete prototype
public class ConcretePrototype : IPrototype<ConcretePrototype>
{
    public string Property1 { get; set; }
    public int Property2 { get; set; }
    
    public ConcretePrototype Clone()
    {
        // Shallow copy
        return (ConcretePrototype)this.MemberwiseClone();
        
        // Or deep copy for complex objects
        return new ConcretePrototype
        {
            Property1 = this.Property1,
            Property2 = this.Property2
        };
    }
}
```

### Key Components
1. **Prototype Interface**: Declares the cloning method
2. **Concrete Prototype**: Implements the cloning method
3. **Client**: Uses prototypes to create new objects
4. **Prototype Registry** (optional): Manages prototype instances

## Example from Our Code
```csharp
public interface ICloneable<T>
{
    T Clone();
}

public class Document : ICloneable<Document>
{
    public string Title { get; set; }
    public string Content { get; set; }
    public List<string> Tags { get; set; }
    public DateTime CreatedDate { get; set; }

    public Document(string title, string content)
    {
        Title = title;
        Content = content;
        Tags = new List<string>();
        CreatedDate = DateTime.Now;
    }

    public Document Clone()
    {
        var cloned = new Document(Title, Content)
        {
            CreatedDate = CreatedDate,
            Tags = new List<string>(Tags) // Deep copy of tags
        };
        return cloned;
    }

    public override string ToString()
    {
        return $"Document: {Title}, Content: {Content}, Tags: [{string.Join(", ", Tags)}]";
    }
}

// Usage
var template = new Document("Report Template", "This is a report template");
template.Tags.Add("Report");
template.Tags.Add("Template");

// Create clones and customize them
var monthlyReport = template.Clone();
monthlyReport.Title = "Monthly Report";
monthlyReport.Content = "Monthly sales report content";

var quarterlyReport = template.Clone();
quarterlyReport.Title = "Quarterly Report";
// Keeps original content and tags
```

## Types of Cloning

### 1. **Shallow Copy**
Copies the object but not the objects it references:
```csharp
public Person ShallowClone()
{
    return (Person)this.MemberwiseClone();
    // Address reference is shared between original and clone
}
```

### 2. **Deep Copy**
Copies the object and all objects it references:
```csharp
public Person DeepClone()
{
    return new Person
    {
        Name = this.Name,
        Age = this.Age,
        Address = new Address
        {
            Street = this.Address.Street,
            City = this.Address.City,
            ZipCode = this.Address.ZipCode
        }
    };
}
```

## Real-World Examples

### 1. **Game Characters**
```csharp
public class GameCharacter : ICloneable<GameCharacter>
{
    public string Name { get; set; }
    public int Level { get; set; }
    public List<string> Equipment { get; set; }
    public Stats BaseStats { get; set; }

    public GameCharacter Clone()
    {
        return new GameCharacter
        {
            Name = Name,
            Level = Level,
            Equipment = new List<string>(Equipment),
            BaseStats = BaseStats.Clone() // Assuming Stats also implements cloning
        };
    }
}

// Creating an army of similar soldiers
var soldierTemplate = new GameCharacter
{
    Name = "Soldier",
    Level = 5,
    Equipment = new List<string> { "Sword", "Shield", "Armor" },
    BaseStats = new Stats { Health = 100, Attack = 20, Defense = 15 }
};

var army = new List<GameCharacter>();
for (int i = 0; i < 10; i++)
{
    var soldier = soldierTemplate.Clone();
    soldier.Name = $"Soldier {i + 1}";
    army.Add(soldier);
}
```

### 2. **UI Components**
```csharp
public class Button : ICloneable<Button>
{
    public string Text { get; set; }
    public Color BackgroundColor { get; set; }
    public Font Font { get; set; }
    public Size Size { get; set; }
    public EventHandler ClickHandler { get; set; }

    public Button Clone()
    {
        return new Button
        {
            Text = Text,
            BackgroundColor = BackgroundColor,
            Font = Font.Clone(),
            Size = Size,
            ClickHandler = ClickHandler
        };
    }
}

// Creating consistent buttons across an application
var primaryButtonTemplate = new Button
{
    BackgroundColor = Color.Blue,
    Font = new Font("Arial", 12, FontStyle.Bold),
    Size = new Size(100, 30)
};

var saveButton = primaryButtonTemplate.Clone();
saveButton.Text = "Save";
saveButton.ClickHandler = SaveClickHandler;

var cancelButton = primaryButtonTemplate.Clone();
cancelButton.Text = "Cancel";
cancelButton.ClickHandler = CancelClickHandler;
```

### 3. **Configuration Objects**
```csharp
public class DatabaseConfig : ICloneable<DatabaseConfig>
{
    public string ConnectionString { get; set; }
    public int Timeout { get; set; }
    public bool EnableLogging { get; set; }
    public Dictionary<string, string> Parameters { get; set; }

    public DatabaseConfig Clone()
    {
        return new DatabaseConfig
        {
            ConnectionString = ConnectionString,
            Timeout = Timeout,
            EnableLogging = EnableLogging,
            Parameters = new Dictionary<string, string>(Parameters)
        };
    }
}

// Different environments with similar configurations
var baseConfig = new DatabaseConfig
{
    Timeout = 30,
    EnableLogging = true,
    Parameters = new Dictionary<string, string>
    {
        ["Pool Size"] = "10",
        ["SSL Mode"] = "Required"
    }
};

var developmentConfig = baseConfig.Clone();
developmentConfig.ConnectionString = "Server=dev-db;Database=MyApp_Dev;";
developmentConfig.EnableLogging = true;

var productionConfig = baseConfig.Clone();
productionConfig.ConnectionString = "Server=prod-db;Database=MyApp_Prod;";
productionConfig.EnableLogging = false;
```

## Prototype Registry
A registry can manage multiple prototypes:

```csharp
public class PrototypeRegistry
{
    private Dictionary<string, ICloneable<object>> _prototypes = new();

    public void RegisterPrototype(string key, ICloneable<object> prototype)
    {
        _prototypes[key] = prototype;
    }

    public T CreateInstance<T>(string key) where T : class
    {
        if (_prototypes.TryGetValue(key, out var prototype))
        {
            return prototype.Clone() as T;
        }
        throw new ArgumentException($"Prototype '{key}' not found");
    }
}

// Usage
var registry = new PrototypeRegistry();
registry.RegisterPrototype("report-template", reportTemplate);
registry.RegisterPrototype("invoice-template", invoiceTemplate);

var newReport = registry.CreateInstance<Document>("report-template");
```

## Benefits
- **Performance**: Faster than creating objects from scratch
- **Flexibility**: Easy to create variations of existing objects
- **Reduced Complexity**: Avoids complex initialization logic
- **Dynamic Creation**: Can create objects at runtime
- **Encapsulation**: Hides complex object creation details

## Drawbacks
- **Cloning Complexity**: Deep copying can be complex for nested objects
- **Circular References**: Can cause issues with deep copying
- **Memory Usage**: Keeping prototypes in memory
- **Maintenance**: Need to maintain cloning methods

## When to Use
✅ **Use When:**
- Object creation is expensive or complex
- You need many similar objects with slight variations
- You want to avoid subclassing just for object creation
- Objects are configured at runtime
- You need to create objects independent of their class

❌ **Avoid When:**
- Object creation is simple and fast
- Objects don't have much common structure
- You need completely different objects
- Cloning logic would be more complex than creation

## Shallow vs Deep Copy Decision
| Use Shallow Copy When | Use Deep Copy When |
|---|---|
| Referenced objects are immutable | Referenced objects are mutable |
| You want to share references | You want independent copies |
| Performance is critical | Data integrity is critical |
| Memory usage is a concern | You need complete isolation |

## Best Practices
1. **Document Cloning Behavior**: Clearly specify shallow vs deep copying
2. **Handle Circular References**: Avoid infinite loops in deep copying
3. **Use Appropriate Copy Type**: Choose shallow or deep based on needs
4. **Implement ICloneable Carefully**: Be consistent with cloning approach
5. **Test Thoroughly**: Ensure clones work independently
6. **Consider Immutability**: Immutable objects simplify cloning

## Common Mistakes
1. **Shallow Copy Issues**: Modifying shared references affects all clones
2. **Incomplete Deep Copy**: Forgetting to clone nested objects
3. **Performance Assumptions**: Assuming cloning is always faster
4. **Reference Sharing**: Unintentionally sharing mutable references

## Modern Alternatives
In modern C#, consider:
```csharp
// Record types with copy semantics
public record Person(string Name, int Age);
var copy = original with { Age = 25 };

// Serialization-based cloning
public T DeepClone<T>(T obj)
{
    var json = JsonSerializer.Serialize(obj);
    return JsonSerializer.Deserialize<T>(json);
}
```

## Summary
The Prototype pattern is like having a master template that you can photocopy and then modify. It's especially useful when you have complex objects that are expensive to create from scratch, or when you need many variations of a similar object. Think of it as the "copy and paste" of object creation - you start with something that works and make the changes you need.
