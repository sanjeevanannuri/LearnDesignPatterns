# Template Method Pattern

## Overview
The Template Method Pattern defines the skeleton of an algorithm in a base class, letting subclasses override specific steps of the algorithm without changing its structure.

## Real-World Analogy
Think of a cooking recipe. Every recipe follows the same general steps:
1. **Gather ingredients** (common step)
2. **Prepare ingredients** (varies by recipe)
3. **Cook/combine ingredients** (varies by recipe)
4. **Plate and serve** (common step)

The overall cooking process (template) remains the same, but the specific preparation and cooking methods vary depending on what you're making - pasta, stir-fry, or soup.

## When to Use Template Method Pattern

### ✅ Use When:
- You have multiple classes with similar algorithms that differ only in some steps
- You want to control the extension points of an algorithm
- You have common behavior that should be factored out to avoid code duplication
- You want to enforce a specific sequence of operations

### ❌ Avoid When:
- The algorithm steps are completely different across implementations
- You need complete flexibility in algorithm structure
- The template is too simple (only 1-2 steps)

## Components

1. **Abstract Class**: Defines the template method and abstract/concrete operations
2. **Template Method**: Defines the algorithm skeleton using primitive operations
3. **Primitive Operations**: Abstract methods that subclasses must implement
4. **Hook Methods**: Optional methods that provide default behavior but can be overridden

## Implementation Examples

### 1. Data Processing Pipeline
```csharp
// Different data formats but same processing workflow
var csvProcessor = new CsvDataProcessor();
csvProcessor.ProcessData(); // Read → Process → Validate → Save → Notify

var xmlProcessor = new XmlDataProcessor();
xmlProcessor.ProcessData(); // Same workflow, different implementation

var jsonProcessor = new JsonDataProcessor();
jsonProcessor.ProcessData(); // Can override specific steps like notifications
```

### 2. Beverage Making Process
```csharp
// Same preparation process, different specifics
var coffee = new Coffee();
coffee.MakeCoffee(); // Boil → Brew → Pour → Add condiments

var tea = new Tea();
tea.MakeCoffee(); // Same steps but steeping instead of brewing

var espresso = new Espresso();
espresso.MakeCoffee(); // High-pressure brewing variation
```

### 3. Game Framework
```csharp
// Common game loop structure
var chess = new Chess();
chess.Play(); // Initialize → Start → [Move → Check]* → End → Results

var ticTacToe = new TicTacToe();
ticTacToe.Play(); // Same game loop, different game rules
```

### 4. Report Generation
```csharp
// Consistent reporting workflow
var pdfReport = new PdfReport();
pdfReport.GenerateReport(); // Gather → Process → Format → Save → Email

var excelReport = new ExcelReport();
excelReport.GenerateReport(); // Same workflow, different formatting

var htmlReport = new HtmlReport();
htmlReport.GenerateReport(); // Additional web-specific data gathering
```

## Benefits

1. **Code Reuse**: Common algorithm structure is defined once
2. **Controlled Extension**: Subclasses can only override specific steps
3. **Inversion of Control**: Base class controls the algorithm flow
4. **Consistency**: Ensures all implementations follow the same process
5. **Maintenance**: Changes to algorithm structure only need to be made in one place

## Drawbacks

1. **Inheritance Dependency**: Requires inheritance, which can be limiting
2. **Complexity**: Can make simple algorithms unnecessarily complex
3. **Liskov Substitution**: Subclasses must maintain the contract of the base class
4. **Debug Difficulty**: Control flow jumps between base and derived classes

## Real-World Use Cases

### 1. **Web Framework Request Processing**
```csharp
public abstract class HttpHandler
{
    public void HandleRequest()
    {
        AuthenticateUser();      // Common
        ValidateInput();         // Common
        ProcessRequest();        // Varies
        GenerateResponse();      // Varies
        LogRequest();           // Common
    }
    
    protected abstract void ProcessRequest();
    protected abstract void GenerateResponse();
}
```

### 2. **Data Migration Tools**
```csharp
public abstract class DataMigrator
{
    public void MigrateData()
    {
        ConnectToSource();       // Common
        ExtractData();          // Varies by source
        TransformData();        // Varies by target
        LoadData();             // Varies by target
        ValidateMigration();    // Common
        Cleanup();              // Common
    }
}
```

### 3. **Testing Frameworks**
```csharp
public abstract class TestCase
{
    public void RunTest()
    {
        SetUp();                // Common setup
        ExecuteTest();          // Test-specific
        TearDown();             // Common cleanup
        ReportResults();        // Common
    }
    
    protected abstract void ExecuteTest();
}
```

### 4. **File Processing Systems**
```csharp
public abstract class FileProcessor
{
    public void ProcessFile(string filePath)
    {
        ValidateFile(filePath);  // Common
        OpenFile(filePath);      // Common
        ReadContent();           // Varies by format
        ProcessContent();        // Varies by purpose
        WriteOutput();           // Varies by target
        CloseFile();            // Common
    }
}
```

### 5. **Machine Learning Pipelines**
```csharp
public abstract class MLModel
{
    public void TrainModel()
    {
        LoadData();             // Common
        PreprocessData();       // Varies by model
        Train();                // Varies by algorithm
        Validate();             // Common
        SaveModel();            // Common
    }
}
```

## Types of Methods in Template Method

### 1. **Concrete Methods**
- Implemented in base class
- Common behavior shared by all subclasses
- Should not be overridden

### 2. **Abstract Methods**
- Must be implemented by subclasses
- Represent varying parts of the algorithm
- Force subclasses to provide specific behavior

### 3. **Hook Methods**
- Provide default behavior but can be overridden
- Optional extension points
- Allow customization without forcing it

```csharp
public abstract class PaymentProcessor
{
    // Template method
    public void ProcessPayment(decimal amount)
    {
        ValidateAmount(amount);     // Concrete method
        if (RequiresAuthentication()) // Hook method
        {
            AuthenticateUser();     // Concrete method
        }
        ChargePayment(amount);      // Abstract method
        SendConfirmation();         // Concrete method
    }
    
    // Concrete method
    protected void ValidateAmount(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Invalid amount");
    }
    
    // Hook method - default implementation
    protected virtual bool RequiresAuthentication()
    {
        return true;
    }
    
    // Abstract method - must be implemented
    protected abstract void ChargePayment(decimal amount);
    
    // Concrete method
    protected void SendConfirmation()
    {
        Console.WriteLine("Payment confirmation sent");
    }
}
```

## Best Practices

1. **Minimize Abstract Methods**: Only make methods abstract when necessary
2. **Use Hook Methods**: Provide optional extension points with sensible defaults
3. **Clear Documentation**: Document the purpose and expectations of each method
4. **Consistent Naming**: Use clear, descriptive names for template steps
5. **Avoid Deep Inheritance**: Keep inheritance hierarchies shallow
6. **Consider Composition**: Sometimes composition is better than inheritance

## Template Method vs Other Patterns

| Pattern | Purpose | Key Difference |
|---------|---------|----------------|
| **Strategy** | Choose algorithm at runtime | Template Method defines structure, Strategy chooses entire algorithm |
| **Factory Method** | Create objects | Template Method defines process, Factory Method creates products |
| **Observer** | Notify multiple objects | Template Method is about algorithm structure, not communication |
| **Command** | Encapsulate requests | Template Method defines process flow, Command encapsulates actions |

## Common Variations

### 1. **Multiple Template Methods**
```csharp
public abstract class DocumentProcessor
{
    // Template method for reading
    public void ReadDocument() { /* ... */ }
    
    // Template method for writing
    public void WriteDocument() { /* ... */ }
    
    // Template method for converting
    public void ConvertDocument() { /* ... */ }
}
```

### 2. **Parameterized Template Methods**
```csharp
public abstract class DataProcessor<T>
{
    public void ProcessData(List<T> data, ProcessingOptions options)
    {
        if (options.ShouldValidate) ValidateData(data);
        ProcessCore(data);
        if (options.ShouldCache) CacheResults(data);
    }
}
```

## Related Patterns

- **Factory Method**: Often used within Template Method to create objects
- **Strategy**: Alternative approach when you need more flexibility
- **Observer**: Can be used to notify about template method progress
- **Decorator**: Can be used to add behavior to template method steps

The Template Method Pattern is excellent for creating reusable algorithm frameworks while maintaining control over the overall process flow.
