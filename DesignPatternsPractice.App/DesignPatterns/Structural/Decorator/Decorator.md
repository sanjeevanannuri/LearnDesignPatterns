# Decorator Pattern

## Overview
The Decorator pattern allows you to add new functionality to objects dynamically without altering their structure. Think of it like dressing up - you can put on different layers of clothing (decorators) over your base outfit (object) to achieve different looks and protection, and you can mix and match these layers in any combination.

## Problem It Solves
Imagine you're building a coffee ordering system where customers can add various extras to their coffee:
- Base coffee: Espresso, Dark Roast, House Blend
- Add-ons: Milk, Sugar, Whip, Mocha, Soy, etc.
- Each combination should calculate the correct price and description

Without Decorator, you'd need a class for every combination:
```csharp
// BAD: Class explosion for every combination
public class EspressoWithMilk : Coffee { }
public class EspressoWithMilkAndSugar : Coffee { }
public class EspressoWithMilkAndSugarAndWhip : Coffee { }
public class DarkRoastWithSoy : Coffee { }
public class DarkRoastWithSoyAndMocha : Coffee { }
// ... hundreds of combinations
```

This approach doesn't scale and violates the Open/Closed Principle.

## Real-World Analogy
Think of **getting dressed**:
1. **Base Layer** (Core Object): T-shirt - provides basic coverage
2. **Decorators**: Sweater, jacket, coat - each adds warmth and changes appearance
3. **Stacking**: You can wear multiple layers in any combination
4. **Same Interface**: Whether you're wearing just a t-shirt or full winter gear, you're still "dressed"
5. **Dynamic**: You can add or remove layers throughout the day

Or consider **pizza toppings**:
- Base pizza (Margherita) has basic functionality
- Each topping (pepperoni, mushrooms, extra cheese) adds to cost and description
- You can combine toppings in any order
- Final result is still a pizza, regardless of how many toppings

## Implementation Details

### Basic Structure
```csharp
// Component interface - common operations
public abstract class Component
{
    public abstract string Operation();
}

// Concrete component - base functionality
public class ConcreteComponent : Component
{
    public override string Operation()
    {
        return "ConcreteComponent";
    }
}

// Base decorator
public abstract class Decorator : Component
{
    protected Component _component;

    public Decorator(Component component)
    {
        _component = component;
    }

    public override string Operation()
    {
        return _component?.Operation();
    }
}

// Concrete decorators - add specific functionality
public class ConcreteDecoratorA : Decorator
{
    public ConcreteDecoratorA(Component component) : base(component) { }

    public override string Operation()
    {
        return $"ConcreteDecoratorA({base.Operation()})";
    }
}

public class ConcreteDecoratorB : Decorator
{
    public ConcreteDecoratorB(Component component) : base(component) { }

    public override string Operation()
    {
        return $"ConcreteDecoratorB({base.Operation()})";
    }
}
```

### Key Components
1. **Component**: Interface for objects that can have responsibilities added dynamically
2. **ConcreteComponent**: Base object to which additional responsibilities can be attached
3. **Decorator**: Base class for all decorators, maintains reference to Component
4. **ConcreteDecorator**: Adds specific responsibilities to the component

## Example from Our Code
```csharp
// Component interface - base coffee functionality
public abstract class Coffee
{
    public abstract string GetDescription();
    public abstract double GetCost();
}

// Concrete components - base coffee types
public class Espresso : Coffee
{
    public override string GetDescription()
    {
        return "Espresso";
    }

    public override double GetCost()
    {
        return 1.99;
    }
}

public class DarkRoast : Coffee
{
    public override string GetDescription()
    {
        return "Dark Roast Coffee";
    }

    public override double GetCost()
    {
        return 0.99;
    }
}

public class HouseBlend : Coffee
{
    public override string GetDescription()
    {
        return "House Blend Coffee";
    }

    public override double GetCost()
    {
        return 0.89;
    }
}

// Base decorator
public abstract class CondimentDecorator : Coffee
{
    protected Coffee _coffee;

    public CondimentDecorator(Coffee coffee)
    {
        _coffee = coffee;
    }

    public override abstract string GetDescription();
}

// Concrete decorators - coffee add-ons
public class Milk : CondimentDecorator
{
    public Milk(Coffee coffee) : base(coffee) { }

    public override string GetDescription()
    {
        return _coffee.GetDescription() + ", Milk";
    }

    public override double GetCost()
    {
        return _coffee.GetCost() + 0.10;
    }
}

public class Mocha : CondimentDecorator
{
    public Mocha(Coffee coffee) : base(coffee) { }

    public override string GetDescription()
    {
        return _coffee.GetDescription() + ", Mocha";
    }

    public override double GetCost()
    {
        return _coffee.GetCost() + 0.20;
    }
}

public class Whip : CondimentDecorator
{
    public Whip(Coffee coffee) : base(coffee) { }

    public override string GetDescription()
    {
        return _coffee.GetDescription() + ", Whip";
    }

    public override double GetCost()
    {
        return _coffee.GetCost() + 0.15;
    }
}

public class Soy : CondimentDecorator
{
    public Soy(Coffee coffee) : base(coffee) { }

    public override string GetDescription()
    {
        return _coffee.GetDescription() + ", Soy";
    }

    public override double GetCost()
    {
        return _coffee.GetCost() + 0.15;
    }
}

// Usage - building decorated coffee
var beverage = new Espresso();
Console.WriteLine($"{beverage.GetDescription()} ${beverage.GetCost()}");

// Add decorators
beverage = new DarkRoast();
beverage = new Mocha(beverage);
beverage = new Mocha(beverage); // Double mocha!
beverage = new Whip(beverage);

Console.WriteLine($"{beverage.GetDescription()} ${beverage.GetCost()}");
// Output: "Dark Roast Coffee, Mocha, Mocha, Whip $1.54"

// Another combination
var anotherBeverage = new HouseBlend();
anotherBeverage = new Soy(anotherBeverage);
anotherBeverage = new Mocha(anotherBeverage);
anotherBeverage = new Whip(anotherBeverage);

Console.WriteLine($"{anotherBeverage.GetDescription()} ${anotherBeverage.GetCost()}");
// Output: "House Blend Coffee, Soy, Mocha, Whip $1.39"
```

## Real-World Examples

### 1. **Text Formatting System**
```csharp
// Component - text interface
public abstract class Text
{
    public abstract string GetContent();
}

// Concrete component - plain text
public class PlainText : Text
{
    private string _content;

    public PlainText(string content)
    {
        _content = content;
    }

    public override string GetContent()
    {
        return _content;
    }
}

// Base decorator for text formatting
public abstract class TextDecorator : Text
{
    protected Text _text;

    public TextDecorator(Text text)
    {
        _text = text;
    }

    public override string GetContent()
    {
        return _text.GetContent();
    }
}

// Concrete decorators - formatting options
public class BoldDecorator : TextDecorator
{
    public BoldDecorator(Text text) : base(text) { }

    public override string GetContent()
    {
        return $"<b>{_text.GetContent()}</b>";
    }
}

public class ItalicDecorator : TextDecorator
{
    public ItalicDecorator(Text text) : base(text) { }

    public override string GetContent()
    {
        return $"<i>{_text.GetContent()}</i>";
    }
}

public class UnderlineDecorator : TextDecorator
{
    public UnderlineDecorator(Text text) : base(text) { }

    public override string GetContent()
    {
        return $"<u>{_text.GetContent()}</u>";
    }
}

public class ColorDecorator : TextDecorator
{
    private string _color;

    public ColorDecorator(Text text, string color) : base(text)
    {
        _color = color;
    }

    public override string GetContent()
    {
        return $"<span style=\"color:{_color}\">{_text.GetContent()}</span>";
    }
}

// Usage - building formatted text
var text = new PlainText("Hello World");

// Apply multiple formatting decorators
text = new BoldDecorator(text);
text = new ItalicDecorator(text);
text = new ColorDecorator(text, "red");

Console.WriteLine(text.GetContent());
// Output: <span style="color:red"><i><b>Hello World</b></i></span>

// Different combination
var anotherText = new PlainText("Important Notice");
anotherText = new UnderlineDecorator(anotherText);
anotherText = new BoldDecorator(anotherText);
anotherText = new ColorDecorator(anotherText, "blue");

Console.WriteLine(anotherText.GetContent());
// Output: <span style="color:blue"><b><u>Important Notice</u></b></span>
```

### 2. **Data Processing Pipeline**
```csharp
// Component - data processor interface
public abstract class DataProcessor
{
    public abstract string ProcessData(string data);
}

// Concrete component - base processor
public class BaseDataProcessor : DataProcessor
{
    public override string ProcessData(string data)
    {
        return data;
    }
}

// Base decorator
public abstract class DataProcessorDecorator : DataProcessor
{
    protected DataProcessor _processor;

    public DataProcessorDecorator(DataProcessor processor)
    {
        _processor = processor;
    }

    public override string ProcessData(string data)
    {
        return _processor.ProcessData(data);
    }
}

// Concrete decorators - processing steps
public class EncryptionDecorator : DataProcessorDecorator
{
    public EncryptionDecorator(DataProcessor processor) : base(processor) { }

    public override string ProcessData(string data)
    {
        var processed = _processor.ProcessData(data);
        return $"ENCRYPTED({processed})";
    }
}

public class CompressionDecorator : DataProcessorDecorator
{
    public CompressionDecorator(DataProcessor processor) : base(processor) { }

    public override string ProcessData(string data)
    {
        var processed = _processor.ProcessData(data);
        return $"COMPRESSED({processed})";
    }
}

public class LoggingDecorator : DataProcessorDecorator
{
    public LoggingDecorator(DataProcessor processor) : base(processor) { }

    public override string ProcessData(string data)
    {
        Console.WriteLine($"Processing data: {data}");
        var result = _processor.ProcessData(data);
        Console.WriteLine($"Processed result: {result}");
        return result;
    }
}

public class ValidationDecorator : DataProcessorDecorator
{
    public ValidationDecorator(DataProcessor processor) : base(processor) { }

    public override string ProcessData(string data)
    {
        if (string.IsNullOrEmpty(data))
            throw new ArgumentException("Data cannot be null or empty");
        
        return _processor.ProcessData(data);
    }
}

// Usage - building processing pipeline
var processor = new BaseDataProcessor();

// Build pipeline: validate -> log -> compress -> encrypt
processor = new ValidationDecorator(processor);
processor = new LoggingDecorator(processor);
processor = new CompressionDecorator(processor);
processor = new EncryptionDecorator(processor);

var result = processor.ProcessData("sensitive data");
Console.WriteLine($"Final result: {result}");
// Output: ENCRYPTED(COMPRESSED(sensitive data))
```

### 3. **Vehicle Features System**
```csharp
// Component - vehicle interface
public abstract class Vehicle
{
    public abstract string GetDescription();
    public abstract decimal GetPrice();
    public abstract void Start();
}

// Concrete component - base vehicle
public class BasicCar : Vehicle
{
    public override string GetDescription()
    {
        return "Basic Car";
    }

    public override decimal GetPrice()
    {
        return 20000m;
    }

    public override void Start()
    {
        Console.WriteLine("Car engine started");
    }
}

// Base decorator
public abstract class VehicleDecorator : Vehicle
{
    protected Vehicle _vehicle;

    public VehicleDecorator(Vehicle vehicle)
    {
        _vehicle = vehicle;
    }

    public override string GetDescription()
    {
        return _vehicle.GetDescription();
    }

    public override decimal GetPrice()
    {
        return _vehicle.GetPrice();
    }

    public override void Start()
    {
        _vehicle.Start();
    }
}

// Concrete decorators - vehicle features
public class GPSDecorator : VehicleDecorator
{
    public GPSDecorator(Vehicle vehicle) : base(vehicle) { }

    public override string GetDescription()
    {
        return _vehicle.GetDescription() + " + GPS Navigation";
    }

    public override decimal GetPrice()
    {
        return _vehicle.GetPrice() + 2000m;
    }

    public override void Start()
    {
        _vehicle.Start();
        Console.WriteLine("GPS system activated");
    }
}

public class SunroofDecorator : VehicleDecorator
{
    public SunroofDecorator(Vehicle vehicle) : base(vehicle) { }

    public override string GetDescription()
    {
        return _vehicle.GetDescription() + " + Sunroof";
    }

    public override decimal GetPrice()
    {
        return _vehicle.GetPrice() + 1500m;
    }

    public override void Start()
    {
        _vehicle.Start();
        Console.WriteLine("Sunroof control system ready");
    }
}

public class LeatherSeatsDecorator : VehicleDecorator
{
    public LeatherSeatsDecorator(Vehicle vehicle) : base(vehicle) { }

    public override string GetDescription()
    {
        return _vehicle.GetDescription() + " + Leather Seats";
    }

    public override decimal GetPrice()
    {
        return _vehicle.GetPrice() + 3000m;
    }
}

public class TurboEngineDecorator : VehicleDecorator
{
    public TurboEngineDecorator(Vehicle vehicle) : base(vehicle) { }

    public override string GetDescription()
    {
        return _vehicle.GetDescription() + " + Turbo Engine";
    }

    public override decimal GetPrice()
    {
        return _vehicle.GetPrice() + 5000m;
    }

    public override void Start()
    {
        _vehicle.Start();
        Console.WriteLine("Turbo engine spooled up!");
    }
}

// Usage - building custom vehicle
var myCar = new BasicCar();

// Add features
myCar = new TurboEngineDecorator(myCar);
myCar = new GPSDecorator(myCar);
myCar = new LeatherSeatsDecorator(myCar);
myCar = new SunroofDecorator(myCar);

Console.WriteLine($"Vehicle: {myCar.GetDescription()}");
Console.WriteLine($"Price: ${myCar.GetPrice():N0}");
myCar.Start();

// Output:
// Vehicle: Basic Car + Turbo Engine + GPS Navigation + Leather Seats + Sunroof
// Price: $31,500
// Car engine started
// Turbo engine spooled up!
// GPS system activated
// Sunroof control system ready
```

## Types of Decorators

### 1. **Transparent Decorators**
Don't add new methods, only enhance existing ones:
```csharp
public class LoggingDecorator : ComponentDecorator
{
    public override string Operation()
    {
        Console.WriteLine("Logging operation");
        return base.Operation();
    }
}
```

### 2. **Behavioral Decorators**
Add new methods to the interface:
```csharp
public class ExtendedDecorator : ComponentDecorator
{
    public override string Operation()
    {
        return base.Operation();
    }

    public void NewBehavior()
    {
        Console.WriteLine("New behavior added");
    }
}
```

## Benefits
- **Flexible Extension**: Add functionality without modifying existing code
- **Runtime Composition**: Combine behaviors dynamically
- **Single Responsibility**: Each decorator has one reason to change
- **Alternative to Inheritance**: More flexible than static inheritance
- **Mix and Match**: Combine decorators in any order

## Drawbacks
- **Complexity**: Many small objects can be hard to understand
- **Debugging Difficulty**: Stack traces can be complex with many decorators
- **Performance**: Additional method calls and object creation overhead
- **Identity Problems**: Decorated object loses original identity

## When to Use
✅ **Use When:**
- You want to add responsibilities to objects dynamically
- You want to add functionality that can be withdrawn
- Extension by subclassing is impractical or impossible
- You need to combine multiple optional features
- You want to pay per feature rather than inherit everything

❌ **Avoid When:**
- The component interface is unstable
- You need to add core functionality (not optional features)
- Performance is critical and overhead matters
- The decoration combinations are few and fixed

## Decorator vs Other Patterns

| Pattern | Purpose | Key Difference |
|---------|---------|----------------|
| **Decorator** | Adds behavior dynamically | Preserves interface, stackable |
| **Adapter** | Makes incompatible interfaces work | Changes interface |
| **Composite** | Treats individual and composite objects uniformly | Part-whole hierarchies |
| **Strategy** | Encapsulates algorithms | Changes entire algorithm |
| **Proxy** | Controls access to an object | Controls access, not behavior |

## Best Practices
1. **Keep Decorators Light**: Each decorator should have a single responsibility
2. **Preserve Interface**: Decorated objects should work anywhere the original works
3. **Order Matters**: Consider the order of decorator application
4. **Use Factories**: Create common decorator combinations with factory methods
5. **Avoid Deep Nesting**: Too many decorators can hurt performance and readability
6. **Document Interactions**: Some decorators may not work well together

## Common Mistakes
1. **Heavy Decorators**: Adding too much functionality to a single decorator
2. **Interface Changes**: Changing the base interface breaks all decorators
3. **Order Dependencies**: Making decorators depend on specific ordering
4. **Reference Confusion**: Losing track of the original object reference

## Modern C# Features
```csharp
// Using extension methods for simple decorations
public static class StringDecorations
{
    public static string Bold(this string text) => $"<b>{text}</b>";
    public static string Italic(this string text) => $"<i>{text}</i>";
    public static string Color(this string text, string color) => $"<span style=\"color:{color}\">{text}</span>";
}

// Usage: Method chaining style
var formatted = "Hello".Bold().Italic().Color("red");

// Using delegates for behavioral decoration
public class DelegateDecorator
{
    private readonly Func<string, string> _operation;

    public DelegateDecorator(Func<string, string> operation)
    {
        _operation = operation;
    }

    public string Execute(string input)
    {
        return _operation(input);
    }
}

// Combining decorators with LINQ
var pipeline = new List<Func<string, string>>
{
    x => x.ToUpper(),
    x => $"[{x}]",
    x => $"PROCESSED: {x}"
};

var result = pipeline.Aggregate("hello", (current, func) => func(current));
// Output: "PROCESSED: [HELLO]"
```

## Testing Decorators
```csharp
[Test]
public void MochaDecorator_GetCost_AddsMochaCostToBaseCoffee()
{
    // Arrange
    var baseCoffee = new Mock<Coffee>();
    baseCoffee.Setup(c => c.GetCost()).Returns(1.00);
    var mocha = new Mocha(baseCoffee.Object);

    // Act
    var totalCost = mocha.GetCost();

    // Assert
    Assert.AreEqual(1.20, totalCost);
    baseCoffee.Verify(c => c.GetCost(), Times.Once);
}

[Test]
public void MultipleDecorators_GetDescription_CombinesAllDescriptions()
{
    // Arrange
    var coffee = new Espresso();
    coffee = new Milk(coffee);
    coffee = new Mocha(coffee);

    // Act
    var description = coffee.GetDescription();

    // Assert
    Assert.AreEqual("Espresso, Milk, Mocha", description);
}
```

## Advanced Patterns

### Decorator with Caching
```csharp
public class CachingDecorator : ComponentDecorator
{
    private readonly Dictionary<string, string> _cache = new();

    public override string Operation()
    {
        var key = "operation_result";
        if (_cache.TryGetValue(key, out var cached))
        {
            return cached;
        }

        var result = base.Operation();
        _cache[key] = result;
        return result;
    }
}
```

### Conditional Decorator
```csharp
public class ConditionalDecorator : ComponentDecorator
{
    private readonly Func<bool> _condition;

    public ConditionalDecorator(Component component, Func<bool> condition)
        : base(component)
    {
        _condition = condition;
    }

    public override string Operation()
    {
        if (_condition())
        {
            return $"Enhanced: {base.Operation()}";
        }
        return base.Operation();
    }
}
```

## Summary
The Decorator pattern is like customizing your coffee order - you start with a base (espresso) and add layers of enhancements (milk, sugar, whip) to create exactly what you want. Each addition enhances the original without changing its fundamental nature.

It's perfect when you need flexible, mix-and-match functionality that can be combined in various ways. Think of it as the "options package" pattern - customers can pick and choose which features they want, and you can add new options without rebuilding the entire system.

The key insight is that sometimes the best way to extend functionality is to wrap it in layers rather than trying to build everything into one complex object.
