# Builder Pattern

## Overview
The Builder pattern constructs complex objects step by step. It allows you to create different types and representations of an object using the same construction process. Think of it like building a custom computer - you choose the motherboard, processor, RAM, storage, and graphics card one piece at a time.

## Problem It Solves
Imagine you're creating a `House` class that needs many optional parameters:
```csharp
// BAD: Constructor with too many parameters
public House(string walls, string roof, string door, bool hasGarage, 
           bool hasGarden, bool hasPool, bool hasBasement, int rooms, 
           string heating, string flooring, bool hasFireplace)
{
    // Constructor becomes unmanageable
}

// Usage becomes confusing
var house = new House("brick", "tile", "wood", true, false, 
                     true, false, 4, "gas", "hardwood", true);
```

Problems with this approach:
- Too many constructor parameters
- Hard to remember parameter order
- Difficult to create variations
- Some parameters might be optional

## Real-World Analogy
Think of **ordering a custom pizza**:
1. Start with a base (thin crust, thick crust, stuffed crust)
2. Add sauce (tomato, white sauce, BBQ)
3. Add cheese (mozzarella, cheddar, parmesan)
4. Add toppings (pepperoni, mushrooms, olives)
5. Choose size (small, medium, large)

The pizza builder helps you create exactly the pizza you want, step by step, and you can skip steps you don't need.

## Implementation Details

### Basic Structure
```csharp
// Product
public class House
{
    public string Walls { get; set; }
    public string Roof { get; set; }
    public string Door { get; set; }
    public bool HasGarage { get; set; }
    public bool HasGarden { get; set; }
}

// Builder interface
public interface IHouseBuilder
{
    IHouseBuilder SetWalls(string walls);
    IHouseBuilder SetRoof(string roof);
    IHouseBuilder SetDoor(string door);
    IHouseBuilder AddGarage();
    IHouseBuilder AddGarden();
    House Build();
}

// Concrete builder
public class HouseBuilder : IHouseBuilder
{
    private House _house = new House();
    
    public IHouseBuilder SetWalls(string walls)
    {
        _house.Walls = walls;
        return this;
    }
    
    public IHouseBuilder SetRoof(string roof)
    {
        _house.Roof = roof;
        return this;
    }
    
    // ... other methods
    
    public House Build() => _house;
}
```

### Key Components
1. **Product**: The complex object being built
2. **Builder Interface**: Defines building steps
3. **Concrete Builder**: Implements the building steps
4. **Director** (optional): Orchestrates the building process
5. **Fluent Interface**: Method chaining for easier usage

## Example from Our Code
```csharp
public class House
{
    public string Walls { get; set; }
    public string Roof { get; set; }
    public string Door { get; set; }
    public bool HasGarage { get; set; }
    public bool HasGarden { get; set; }
    public bool HasPool { get; set; }

    public override string ToString()
    {
        var features = new List<string>();
        if (HasGarage) features.Add("garage");
        if (HasGarden) features.Add("garden");
        if (HasPool) features.Add("pool");
        
        var featuresText = features.Any() ? $" with {string.Join(", ", features)}" : "";
        return $"House with {Walls} walls, {Roof} roof, {Door} door{featuresText}";
    }
}

public class HouseBuilder
{
    private readonly House _house = new House();

    public HouseBuilder SetWalls(string walls)
    {
        _house.Walls = walls;
        return this;
    }

    public HouseBuilder SetRoof(string roof)
    {
        _house.Roof = roof;
        return this;
    }

    public HouseBuilder SetDoor(string door)
    {
        _house.Door = door;
        return this;
    }

    public HouseBuilder AddGarage()
    {
        _house.HasGarage = true;
        return this;
    }

    public HouseBuilder AddGarden()
    {
        _house.HasGarden = true;
        return this;
    }

    public HouseBuilder AddPool()
    {
        _house.HasPool = true;
        return this;
    }

    public House Build()
    {
        return _house;
    }
}

// Usage
var house = new HouseBuilder()
    .SetWalls("wooden")
    .SetRoof("wooden")
    .SetDoor("wooden")
    .Build();
```

## Real-World Examples

### 1. **SQL Query Builder**
```csharp
public class SqlQueryBuilder
{
    private StringBuilder _query = new StringBuilder();
    
    public SqlQueryBuilder Select(params string[] columns)
    {
        _query.Append($"SELECT {string.Join(", ", columns)} ");
        return this;
    }
    
    public SqlQueryBuilder From(string table)
    {
        _query.Append($"FROM {table} ");
        return this;
    }
    
    public SqlQueryBuilder Where(string condition)
    {
        _query.Append($"WHERE {condition} ");
        return this;
    }
    
    public SqlQueryBuilder OrderBy(string column)
    {
        _query.Append($"ORDER BY {column} ");
        return this;
    }
    
    public string Build() => _query.ToString().Trim();
}

// Usage
var query = new SqlQueryBuilder()
    .Select("name", "email")
    .From("users")
    .Where("age > 18")
    .OrderBy("name")
    .Build();
// Result: "SELECT name, email FROM users WHERE age > 18 ORDER BY name"
```

### 2. **Email Builder**
```csharp
public class EmailBuilder
{
    private readonly Email _email = new Email();
    
    public EmailBuilder To(string recipient)
    {
        _email.Recipients.Add(recipient);
        return this;
    }
    
    public EmailBuilder Subject(string subject)
    {
        _email.Subject = subject;
        return this;
    }
    
    public EmailBuilder Body(string body)
    {
        _email.Body = body;
        return this;
    }
    
    public EmailBuilder Attach(string filePath)
    {
        _email.Attachments.Add(filePath);
        return this;
    }
    
    public EmailBuilder Priority(EmailPriority priority)
    {
        _email.Priority = priority;
        return this;
    }
    
    public Email Build() => _email;
}

// Usage
var email = new EmailBuilder()
    .To("john@example.com")
    .To("jane@example.com")
    .Subject("Meeting Tomorrow")
    .Body("Don't forget about our meeting at 2 PM")
    .Attach("agenda.pdf")
    .Priority(EmailPriority.High)
    .Build();
```

### 3. **HTTP Request Builder**
```csharp
public class HttpRequestBuilder
{
    private readonly HttpRequestMessage _request = new HttpRequestMessage();
    
    public HttpRequestBuilder Method(HttpMethod method)
    {
        _request.Method = method;
        return this;
    }
    
    public HttpRequestBuilder Url(string url)
    {
        _request.RequestUri = new Uri(url);
        return this;
    }
    
    public HttpRequestBuilder Header(string name, string value)
    {
        _request.Headers.Add(name, value);
        return this;
    }
    
    public HttpRequestBuilder Content(HttpContent content)
    {
        _request.Content = content;
        return this;
    }
    
    public HttpRequestMessage Build() => _request;
}

// Usage
var request = new HttpRequestBuilder()
    .Method(HttpMethod.Post)
    .Url("https://api.example.com/users")
    .Header("Authorization", "Bearer token123")
    .Header("Content-Type", "application/json")
    .Content(new StringContent(jsonData))
    .Build();
```

## Builder with Director
Sometimes you want to encapsulate common building sequences:

```csharp
public class HouseDirector
{
    private readonly IHouseBuilder _builder;
    
    public HouseDirector(IHouseBuilder builder)
    {
        _builder = builder;
    }
    
    public House BuildBasicHouse()
    {
        return _builder
            .SetWalls("brick")
            .SetRoof("tile")
            .SetDoor("wood")
            .Build();
    }
    
    public House BuildLuxuryHouse()
    {
        return _builder
            .SetWalls("marble")
            .SetRoof("slate")
            .SetDoor("mahogany")
            .AddGarage()
            .AddGarden()
            .AddPool()
            .Build();
    }
}

// Usage
var builder = new HouseBuilder();
var director = new HouseDirector(builder);
var luxuryHouse = director.BuildLuxuryHouse();
```

## Benefits
- **Readable Code**: Method chaining makes code easy to read
- **Flexible Construction**: Can create different variations easily
- **Step-by-Step Building**: Complex objects built incrementally
- **Immutable Results**: Can create immutable objects
- **Reusable**: Same builder can create multiple objects

## Drawbacks
- **More Code**: Requires additional builder classes
- **Memory Overhead**: Builder objects take memory
- **Complexity**: Can be overkill for simple objects

## When to Use
✅ **Use When:**
- Creating complex objects with many optional parameters
- You want to create different representations of the same object
- Construction process must allow different representations
- You want to isolate complex construction code

❌ **Avoid When:**
- Objects are simple with few parameters
- Object structure rarely changes
- Performance is critical and object creation is frequent

## Builder vs Other Patterns
| Pattern | Purpose | When to Use |
|---|---|---|
| Builder | Construct complex objects step-by-step | Many optional parameters |
| Factory Method | Create objects without specifying exact class | Different types of objects |
| Abstract Factory | Create families of related objects | Related object families |
| Prototype | Create objects by cloning | Expensive object creation |

## Best Practices
1. **Fluent Interface**: Return `this` from builder methods for chaining
2. **Validation**: Validate required fields in the `Build()` method
3. **Immutability**: Consider making the final product immutable
4. **Reset Method**: Provide a way to reset the builder for reuse
5. **Clear Documentation**: Document required vs optional steps

## Common Mistakes
1. **Mutable Products**: Allowing modification after building
2. **Missing Validation**: Not validating required fields
3. **Complex Builders**: Making builders too complex
4. **Overuse**: Using builders for simple objects

## Modern C# Features
With modern C# features, you can also use:
```csharp
// Object initializer syntax
var house = new House
{
    Walls = "brick",
    Roof = "tile",
    Door = "wood",
    HasGarage = true
};

// Record types with with expressions
public record House(string Walls, string Roof, string Door, bool HasGarage = false);
var luxuryHouse = basicHouse with { HasGarage = true };
```

## Summary
The Builder pattern is like assembling a custom computer or ordering a personalized sandwich at Subway. You go through each step, choosing exactly what you want, and end up with a product that's perfectly tailored to your needs. The builder guides you through the process, ensuring you don't miss important steps and making it easy to create variations.
