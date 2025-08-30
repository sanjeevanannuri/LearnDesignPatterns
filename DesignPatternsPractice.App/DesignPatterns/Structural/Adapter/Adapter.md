# Adapter Pattern

## Overview
The Adapter pattern allows incompatible interfaces to work together. Think of it like a power adapter when you travel - your laptop has a specific plug type, but the wall outlets in different countries have different shapes. The adapter converts one interface to another without changing the original functionality.

## Problem It Solves
Imagine you have a music player app that works with MP3 files, but now you want to add support for different audio formats like MP4 and VLC:
- Your existing player interface expects MP3 methods
- The new audio libraries have completely different interfaces
- You don't want to rewrite your entire music player
- You need to make incompatible things work together

Without Adapter, you'd need to modify your core player:
```csharp
// BAD: Modifying existing code for every new format
public class MusicPlayer
{
    public void Play(string audioType, string fileName)
    {
        if (audioType == "mp3")
        {
            // MP3 playing logic
        }
        else if (audioType == "mp4")
        {
            // MP4 playing logic - added later
        }
        else if (audioType == "vlc")
        {
            // VLC playing logic - added later
        }
        // This violates Open/Closed Principle
    }
}
```

This violates the Open/Closed Principle and makes the code harder to maintain.

## Real-World Analogy
Think of a **universal power adapter**:
1. Your laptop has a specific plug (existing interface)
2. The wall outlet has a different shape (incompatible interface)
3. The adapter has both shapes - it fits the outlet and accepts your plug
4. Electricity flows through the adapter without being changed
5. Your laptop works in any country without modification

Or consider a **language translator**:
- You speak English (client)
- The shopkeeper speaks only French (service)
- A translator (adapter) speaks both languages
- You tell the translator what you want in English
- The translator tells the shopkeeper in French
- You can shop without learning French

## Implementation Details

### Basic Structure
```csharp
// Target interface - what the client expects
public interface ITarget
{
    void Request();
}

// Adaptee - the incompatible class we want to use
public class Adaptee
{
    public void SpecificRequest()
    {
        Console.WriteLine("Called SpecificRequest()");
    }
}

// Adapter - makes Adaptee compatible with ITarget
public class Adapter : ITarget
{
    private Adaptee _adaptee;

    public Adapter(Adaptee adaptee)
    {
        _adaptee = adaptee;
    }

    public void Request()
    {
        _adaptee.SpecificRequest();
    }
}
```

### Key Components
1. **Target Interface**: The interface the client expects
2. **Adaptee**: The existing class with incompatible interface
3. **Adapter**: Converts Adaptee's interface to Target interface
4. **Client**: Uses the Target interface

## Example from Our Code
```csharp
// Target interface - what our system expects
public interface IPaymentProcessor
{
    void ProcessPayment(decimal amount);
}

// Adaptee classes - external payment services with different interfaces
public class PayPalService
{
    public void MakePayment(double amount)
    {
        Console.WriteLine($"Paid ${amount} via PayPal");
    }
}

public class StripeService
{
    public void ChargeCard(float amount)
    {
        Console.WriteLine($"Charged ${amount} via Stripe");
    }
}

// Adapters - make external services compatible with our interface
public class PayPalAdapter : IPaymentProcessor
{
    private readonly PayPalService _payPalService;

    public PayPalAdapter(PayPalService payPalService)
    {
        _payPalService = payPalService;
    }

    public void ProcessPayment(decimal amount)
    {
        // Convert decimal to double and adapt the method call
        _payPalService.MakePayment((double)amount);
    }
}

public class StripeAdapter : IPaymentProcessor
{
    private readonly StripeService _stripeService;

    public StripeAdapter(StripeService stripeService)
    {
        _stripeService = stripeService;
    }

    public void ProcessPayment(decimal amount)
    {
        // Convert decimal to float and adapt the method call
        _stripeService.ChargeCard((float)amount);
    }
}

// Client code - uses consistent interface
public class ShoppingCart
{
    private readonly IPaymentProcessor _paymentProcessor;

    public ShoppingCart(IPaymentProcessor paymentProcessor)
    {
        _paymentProcessor = paymentProcessor;
    }

    public void Checkout(decimal totalAmount)
    {
        Console.WriteLine($"Processing payment of ${totalAmount}");
        _paymentProcessor.ProcessPayment(totalAmount);
    }
}

// Usage
var paypalService = new PayPalService();
var paypalAdapter = new PayPalAdapter(paypalService);
var cart1 = new ShoppingCart(paypalAdapter);
cart1.Checkout(99.99m);

var stripeService = new StripeService();
var stripeAdapter = new StripeAdapter(stripeService);
var cart2 = new ShoppingCart(stripeAdapter);
cart2.Checkout(149.50m);
```

## Types of Adapters

### 1. **Object Adapter (Composition)**
Uses composition to adapt:
```csharp
public class ObjectAdapter : ITarget
{
    private Adaptee _adaptee; // Has-a relationship

    public ObjectAdapter(Adaptee adaptee)
    {
        _adaptee = adaptee;
    }

    public void Request()
    {
        _adaptee.SpecificRequest();
    }
}
```

### 2. **Class Adapter (Inheritance)**
Uses inheritance to adapt (less common in C#):
```csharp
public class ClassAdapter : Adaptee, ITarget
{
    public void Request()
    {
        SpecificRequest(); // Inherited method
    }
}
```

## Real-World Examples

### 1. **Database Adapters**
```csharp
// Target interface
public interface IDataRepository
{
    void Save(object data);
    object Load(int id);
}

// Legacy file system
public class LegacyFileSystem
{
    public void WriteToFile(string data, string filename)
    {
        File.WriteAllText(filename, data);
    }

    public string ReadFromFile(string filename)
    {
        return File.ReadAllText(filename);
    }
}

// Modern database
public class ModernDatabase
{
    public void Insert(object entity)
    {
        Console.WriteLine($"Inserted {entity} into database");
    }

    public T Select<T>(int id)
    {
        Console.WriteLine($"Selected entity with ID {id}");
        return default(T);
    }
}

// Adapters
public class FileSystemAdapter : IDataRepository
{
    private readonly LegacyFileSystem _fileSystem;

    public FileSystemAdapter(LegacyFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public void Save(object data)
    {
        var filename = $"data_{DateTime.Now.Ticks}.txt";
        _fileSystem.WriteToFile(data.ToString(), filename);
    }

    public object Load(int id)
    {
        var filename = $"data_{id}.txt";
        return _fileSystem.ReadFromFile(filename);
    }
}

public class DatabaseAdapter : IDataRepository
{
    private readonly ModernDatabase _database;

    public DatabaseAdapter(ModernDatabase database)
    {
        _database = database;
    }

    public void Save(object data)
    {
        _database.Insert(data);
    }

    public object Load(int id)
    {
        return _database.Select<object>(id);
    }
}

// Client can use either storage method transparently
public class DataService
{
    private readonly IDataRepository _repository;

    public DataService(IDataRepository repository)
    {
        _repository = repository;
    }

    public void ProcessData(object data, int id)
    {
        _repository.Save(data);
        var loaded = _repository.Load(id);
        Console.WriteLine($"Processed: {loaded}");
    }
}
```

### 2. **Third-Party Library Integration**
```csharp
// Your application's interface
public interface ILogger
{
    void LogInfo(string message);
    void LogError(string message);
    void LogWarning(string message);
}

// Third-party logging library (NLog)
public class NLogLogger
{
    public void Info(string msg) => Console.WriteLine($"[NLog INFO] {msg}");
    public void Error(string msg) => Console.WriteLine($"[NLog ERROR] {msg}");
    public void Warn(string msg) => Console.WriteLine($"[NLog WARN] {msg}");
}

// Another third-party library (Serilog)
public class SerilogLogger
{
    public void Information(string message) => Console.WriteLine($"[Serilog INFO] {message}");
    public void Fatal(string message) => Console.WriteLine($"[Serilog FATAL] {message}");
    public void Warning(string message) => Console.WriteLine($"[Serilog WARN] {message}");
}

// Adapters
public class NLogAdapter : ILogger
{
    private readonly NLogLogger _nlog;

    public NLogAdapter(NLogLogger nlog)
    {
        _nlog = nlog;
    }

    public void LogInfo(string message) => _nlog.Info(message);
    public void LogError(string message) => _nlog.Error(message);
    public void LogWarning(string message) => _nlog.Warn(message);
}

public class SerilogAdapter : ILogger
{
    private readonly SerilogLogger _serilog;

    public SerilogAdapter(SerilogLogger serilog)
    {
        _serilog = serilog;
    }

    public void LogInfo(string message) => _serilog.Information(message);
    public void LogError(string message) => _serilog.Fatal(message);
    public void LogWarning(string message) => _serilog.Warning(message);
}

// Your application code remains unchanged
public class BusinessService
{
    private readonly ILogger _logger;

    public BusinessService(ILogger logger)
    {
        _logger = logger;
    }

    public void DoWork()
    {
        _logger.LogInfo("Starting work");
        try
        {
            // Business logic
            _logger.LogInfo("Work completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred: {ex.Message}");
        }
    }
}
```

### 3. **Legacy System Integration**
```csharp
// Modern interface
public interface IWeatherService
{
    Task<WeatherData> GetWeatherAsync(string city);
}

// Legacy weather system
public class LegacyWeatherSystem
{
    public string GetTemperature(string location)
    {
        return $"75°F in {location}";
    }

    public string GetHumidity(string location)
    {
        return $"60% humidity in {location}";
    }

    public string GetConditions(string location)
    {
        return $"Sunny in {location}";
    }
}

// Adapter for legacy system
public class LegacyWeatherAdapter : IWeatherService
{
    private readonly LegacyWeatherSystem _legacySystem;

    public LegacyWeatherAdapter(LegacyWeatherSystem legacySystem)
    {
        _legacySystem = legacySystem;
    }

    public async Task<WeatherData> GetWeatherAsync(string city)
    {
        // Simulate async operation
        await Task.Delay(100);

        // Combine legacy calls into modern format
        var temperature = _legacySystem.GetTemperature(city);
        var humidity = _legacySystem.GetHumidity(city);
        var conditions = _legacySystem.GetConditions(city);

        return new WeatherData
        {
            City = city,
            Temperature = temperature,
            Humidity = humidity,
            Conditions = conditions,
            LastUpdated = DateTime.Now
        };
    }
}

public class WeatherData
{
    public string City { get; set; }
    public string Temperature { get; set; }
    public string Humidity { get; set; }
    public string Conditions { get; set; }
    public DateTime LastUpdated { get; set; }
}
```

## Benefits
- **Integration**: Makes incompatible classes work together
- **Reusability**: Reuse existing code without modification
- **Separation of Concerns**: Keeps adaptation logic separate
- **Open/Closed Principle**: Add new adapters without changing existing code
- **Flexibility**: Switch between different implementations easily

## Drawbacks
- **Complexity**: Adds another layer of abstraction
- **Performance**: Small overhead from additional method calls
- **Maintenance**: More classes to maintain
- **Design Overhead**: May be overkill for simple adaptations

## When to Use
✅ **Use When:**
- You need to use an existing class with incompatible interface
- You want to integrate third-party libraries
- You're working with legacy systems
- You need to make incompatible interfaces work together
- You want to reuse existing functionality

❌ **Avoid When:**
- Interfaces are already compatible
- You can modify the existing classes
- The adaptation is trivial
- You're overengineering a simple problem

## Adapter vs Other Patterns

| Pattern | Purpose | When to Use |
|---------|---------|-------------|
| **Adapter** | Makes incompatible interfaces compatible | Existing code with different interfaces |
| **Facade** | Simplifies a complex subsystem | Complex system needs simple interface |
| **Decorator** | Adds new functionality | Extending behavior dynamically |
| **Proxy** | Controls access to an object | Access control, lazy loading, caching |

## Best Practices
1. **Keep It Simple**: Don't over-complicate the adaptation
2. **Document Transformations**: Clearly document what gets adapted
3. **Handle Edge Cases**: Consider null values and error conditions
4. **Performance Considerations**: Minimize conversion overhead
5. **Consistent Naming**: Use clear, descriptive names for adapters
6. **Single Responsibility**: Each adapter should handle one adaptation

## Common Mistakes
1. **Over-Adaptation**: Trying to adapt too much functionality
2. **Leaky Abstraction**: Exposing adapted interface details
3. **Performance Issues**: Unnecessary conversions or operations
4. **Tight Coupling**: Making the adapter depend on too many things

## Modern C# Features
```csharp
// Using extension methods for simple adaptations
public static class StringExtensions
{
    public static IEnumerable<char> AsCharEnumerable(this string str)
    {
        return str.ToCharArray();
    }
}

// Using implicit operators for type adaptation
public class Temperature
{
    private double _celsius;

    public Temperature(double celsius)
    {
        _celsius = celsius;
    }

    // Implicit adapter for double
    public static implicit operator double(Temperature temp)
    {
        return temp._celsius;
    }

    // Implicit adapter from double
    public static implicit operator Temperature(double celsius)
    {
        return new Temperature(celsius);
    }
}
```

## Testing Adapters
```csharp
[Test]
public void PayPalAdapter_ProcessPayment_CallsPayPalService()
{
    // Arrange
    var mockPayPalService = new Mock<PayPalService>();
    var adapter = new PayPalAdapter(mockPayPalService.Object);

    // Act
    adapter.ProcessPayment(100.00m);

    // Assert
    mockPayPalService.Verify(x => x.MakePayment(100.00), Times.Once);
}
```

## Summary
The Adapter pattern is like a universal translator that makes incompatible things work together. It's especially useful when you need to integrate third-party libraries, work with legacy systems, or make existing code work with new interfaces. Think of it as the "compatibility layer" that allows different systems to communicate without changing their core functionality.

The key insight is that sometimes it's better to adapt what you have rather than rebuild everything from scratch. Like using a power adapter when traveling - you don't need to buy a new laptop for each country, you just need the right adapter.
