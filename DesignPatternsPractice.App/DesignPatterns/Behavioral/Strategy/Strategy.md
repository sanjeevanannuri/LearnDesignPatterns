# Strategy Pattern

## Overview
The Strategy Pattern defines a family of algorithms, encapsulates each one, and makes them interchangeable. This pattern lets the algorithm vary independently from clients that use it.

## Real-World Analogy
Think of a GPS navigation app. When you want to go from point A to point B, the app can offer different strategies:
- **Fastest route** (highway-focused)
- **Shortest route** (distance-optimized)
- **Scenic route** (tourist-friendly)
- **Eco-friendly route** (fuel-efficient)

The GPS app (context) can switch between these strategies based on your preference, but the core navigation functionality remains the same.

## When to Use Strategy Pattern

### ✅ Use When:
- You have multiple ways to perform a task
- You want to choose the algorithm at runtime
- You have conditional statements that select different behaviors
- You want to avoid exposing complex, algorithm-specific data structures

### ❌ Avoid When:
- You only have one algorithm
- The algorithms rarely change
- The number of strategies is very small and stable

## Components

1. **Strategy Interface**: Defines the contract for all concrete strategies
2. **Concrete Strategies**: Different implementations of the algorithm
3. **Context**: Maintains a reference to a strategy object and delegates work to it

## Implementation Examples

### 1. Payment Processing System
```csharp
// Different payment methods with varying processing logic
var cart = new ShoppingCart();
cart.AddItem("Laptop");
cart.AddItem("Mouse");

// Credit Card Payment
cart.SetPaymentStrategy(new CreditCardPayment("1234-5678-9012-3456", "John Doe", "123", "12/25"));
cart.Checkout(1299.99m);

// PayPal Payment
cart.SetPaymentStrategy(new PayPalPayment("john@example.com", "password123"));
cart.Checkout(1299.99m);

// Bitcoin Payment
cart.SetPaymentStrategy(new BitcoinPayment("1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa"));
cart.Checkout(1299.99m);
```

### 2. Sorting Algorithms
```csharp
// Different sorting strategies for different scenarios
var numbers = new List<int> { 64, 34, 25, 12, 22, 11, 90 };
var sorter = new SortContext();

// Small dataset - Bubble Sort
sorter.SetSortStrategy(new BubbleSortStrategy());
sorter.SortNumbers(new List<int>(numbers));

// Large dataset - Quick Sort
sorter.SetSortStrategy(new QuickSortStrategy());
sorter.SortNumbers(new List<int>(numbers));

// Stable sorting needed - Merge Sort
sorter.SetSortStrategy(new MergeSortStrategy());
sorter.SortNumbers(new List<int>(numbers));
```

### 3. File Compression
```csharp
// Different compression algorithms
var files = new List<string> { "document.pdf", "image.jpg", "data.csv" };
var compressor = new CompressionContext();

// For maximum compression
compressor.SetCompressionStrategy(new RarCompression());
compressor.CompressFiles(files);

// For compatibility
compressor.SetCompressionStrategy(new ZipCompression());
compressor.CompressFiles(files);
```

## Benefits

1. **Runtime Algorithm Selection**: Choose the best algorithm based on current conditions
2. **Eliminates Conditionals**: Replace complex if-else or switch statements
3. **Easy Extension**: Add new algorithms without modifying existing code
4. **Testability**: Each strategy can be tested independently
5. **Single Responsibility**: Each strategy has one reason to change

## Drawbacks

1. **Increased Number of Classes**: Each strategy needs its own class
2. **Client Awareness**: Clients must understand different strategies
3. **Communication Overhead**: Additional interface layer between client and algorithm

## Real-World Use Cases

### 1. **E-commerce Platforms**
- Pricing strategies (regular, bulk discount, member discount)
- Shipping strategies (standard, express, overnight)
- Recommendation algorithms (collaborative filtering, content-based, hybrid)

### 2. **Game Development**
- AI behavior strategies (aggressive, defensive, neutral)
- Movement strategies (walking, running, flying)
- Combat strategies (melee, ranged, magic)

### 3. **Financial Applications**
- Trading strategies (day trading, swing trading, buy-and-hold)
- Risk assessment algorithms (conservative, moderate, aggressive)
- Portfolio rebalancing strategies

### 4. **Data Processing**
- File parsing strategies (CSV, JSON, XML)
- Data validation strategies (strict, lenient, custom)
- Export formats (PDF, Excel, CSV)

### 5. **Social Media Platforms**
- Content ranking algorithms (chronological, engagement-based, relevance)
- Notification strategies (immediate, batched, digest)
- Privacy strategies (public, friends-only, private)

## Common Variations

### 1. **Factory + Strategy**
```csharp
public class PaymentStrategyFactory
{
    public static IPaymentStrategy CreateStrategy(PaymentType type, string details)
    {
        return type switch
        {
            PaymentType.CreditCard => new CreditCardPayment(details),
            PaymentType.PayPal => new PayPalPayment(details),
            PaymentType.Bitcoin => new BitcoinPayment(details),
            _ => throw new ArgumentException("Unknown payment type")
        };
    }
}
```

### 2. **Strategy with State**
```csharp
public class AdaptiveCompressionContext
{
    private ICompressionStrategy _strategy;
    
    public void CompressFiles(List<string> files)
    {
        // Choose strategy based on file types and sizes
        if (files.All(f => f.EndsWith(".txt")) && files.Sum(GetFileSize) < 1000)
            _strategy = new ZipCompression();
        else
            _strategy = new RarCompression();
            
        _strategy.CompressFiles(files);
    }
}
```

## Best Practices

1. **Keep Strategies Stateless**: Make strategies immutable when possible
2. **Use Dependency Injection**: Inject strategies rather than creating them
3. **Consider Performance**: Some algorithms are better for certain data sizes
4. **Document Strategy Selection**: Make it clear when to use each strategy
5. **Provide Default Strategy**: Always have a sensible default option

## Related Patterns

- **State Pattern**: Both use composition and delegation, but State allows changing behavior based on internal state
- **Template Method**: Provides algorithm structure, Strategy provides different algorithms
- **Bridge Pattern**: Both use composition, but Bridge focuses on separating abstraction from implementation
- **Command Pattern**: Both encapsulate behavior, but Command focuses on requests/operations

The Strategy Pattern is fundamental for creating flexible, maintainable code that can adapt to changing requirements and different runtime conditions.
