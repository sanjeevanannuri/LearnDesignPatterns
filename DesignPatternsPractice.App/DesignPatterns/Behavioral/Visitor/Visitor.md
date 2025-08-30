# Visitor Pattern

## Overview
The Visitor Pattern represents an operation to be performed on elements of an object structure. It lets you define a new operation without changing the classes of the elements on which it operates.

## Real-World Analogy
Think of a hospital where different specialists visit patients:
- **Patients** (elements) remain the same but accept different **doctors** (visitors)
- A **cardiologist** examines the heart, a **neurologist** checks the brain, an **orthopedist** looks at bones
- Each specialist performs different operations on the same patients
- New specialists can be added without changing the patients
- The **hospital** (object structure) coordinates the visits

## When to Use Visitor Pattern

### ✅ Use When:
- You need to perform many unrelated operations on objects in a complex structure
- The object structure rarely changes but you frequently add new operations
- You want to keep related operations together and separate them from unrelated ones
- You need to gather information from a complex object structure

### ❌ Avoid When:
- The object structure changes frequently (adding new element types is difficult)
- The operations are simple and don't justify the complexity
- You have only a few operations to perform
- Type safety is a major concern (visitor pattern can break encapsulation)

## Components

1. **Visitor Interface**: Declares visit methods for each element type
2. **Concrete Visitors**: Implement specific operations for each element type
3. **Element Interface**: Declares accept method that takes a visitor
4. **Concrete Elements**: Implement accept method to call visitor's visit method
5. **Object Structure**: Collection of elements that can be visited

## Implementation Examples

### 1. Shape Processing System
```csharp
// Same shapes, different operations
var shapes = new List<IShape>
{
    new Circle(5),
    new Rectangle(4, 6),
    new Triangle(3, 4)
};

// Calculate total area
var areaCalculator = new AreaCalculator();
foreach (var shape in shapes)
    shape.Accept(areaCalculator);
Console.WriteLine($"Total area: {areaCalculator.TotalArea:F2}");

// Calculate total perimeter
var perimeterCalculator = new PerimeterCalculator();
foreach (var shape in shapes)
    shape.Accept(perimeterCalculator);
Console.WriteLine($"Total perimeter: {perimeterCalculator.TotalPerimeter:F2}");

// Render shapes
var renderer = new ShapeRenderer();
foreach (var shape in shapes)
    shape.Accept(renderer);
```

### 2. File System Analysis
```csharp
// Build file system structure
var root = new Directory("root");
root.Add(new File("document.pdf", 1024));
root.Add(new File("image.jpg", 2048));

var subDir = new Directory("projects");
subDir.Add(new File("code.cs", 512));
subDir.Add(new File("readme.md", 256));
root.Add(subDir);

// Calculate total size
var sizeCalculator = new SizeCalculator();
root.Accept(sizeCalculator);
Console.WriteLine($"Total size: {sizeCalculator.TotalSize} bytes");

// Count files and types
var fileCounter = new FileCounter();
root.Accept(fileCounter);
fileCounter.PrintStatistics();

// Search for files
var searcher = new SearchVisitor("code");
root.Accept(searcher);
Console.WriteLine($"Found {searcher.FoundItems.Count} matching items");
```

### 3. Expression Tree Evaluation
```csharp
// Build expression: (5 + 3) * (10 - 4)
var expression = new Multiplication(
    new Addition(new Number(5), new Number(3)),
    new Subtraction(new Number(10), new Number(4))
);

// Evaluate expression
var evaluator = new EvaluationVisitor();
double result = expression.Accept(evaluator);
Console.WriteLine($"Result: {result}"); // Output: 48

// Print expression
var printer = new PrintVisitor();
Console.Write("Expression: ");
expression.Accept(printer);
Console.WriteLine(); // Output: ((5 + 3) * (10 - 4))
```

## Benefits

1. **Easy to Add Operations**: New visitors can be added without modifying existing elements
2. **Related Operations Together**: All operations for a specific purpose are in one visitor
3. **Visiting Across Class Hierarchies**: Can operate on unrelated classes that implement the element interface
4. **Data Accumulation**: Visitors can accumulate state during traversal

## Drawbacks

1. **Difficult to Add Elements**: Adding new element types requires updating all visitors
2. **Breaks Encapsulation**: Visitors often need access to element internals
3. **Dependency Issues**: Elements depend on visitor interface
4. **Complexity**: Can make simple operations unnecessarily complex

## Real-World Use Cases

### 1. **Compiler Design**
```csharp
// Abstract Syntax Tree processing
public interface IASTVisitor
{
    void Visit(ClassDeclaration node);
    void Visit(MethodDeclaration node);
    void Visit(VariableDeclaration node);
}

public class CodeGeneratorVisitor : IASTVisitor
{
    // Generate different code for each node type
}

public class SemanticAnalyzerVisitor : IASTVisitor
{
    // Perform type checking and semantic analysis
}
```

### 2. **Document Processing**
```csharp
public interface IDocumentElementVisitor
{
    void Visit(Paragraph paragraph);
    void Visit(Image image);
    void Visit(Table table);
    void Visit(Header header);
}

public class WordExporter : IDocumentElementVisitor
{
    // Export to Word format
}

public class PdfExporter : IDocumentElementVisitor
{
    // Export to PDF format
}

public class HtmlExporter : IDocumentElementVisitor
{
    // Export to HTML format
}
```

### 3. **Game Development**
```csharp
public interface IGameEntityVisitor
{
    void Visit(Player player);
    void Visit(Enemy enemy);
    void Visit(PowerUp powerUp);
    void Visit(Obstacle obstacle);
}

public class RenderVisitor : IGameEntityVisitor
{
    // Render different entity types
}

public class CollisionDetectionVisitor : IGameEntityVisitor
{
    // Handle collisions for different entities
}

public class AIUpdateVisitor : IGameEntityVisitor
{
    // Update AI behavior for applicable entities
}
```

### 4. **E-commerce System**
```csharp
public interface IOrderItemVisitor
{
    void Visit(PhysicalProduct product);
    void Visit(DigitalProduct product);
    void Visit(Service service);
    void Visit(Subscription subscription);
}

public class ShippingCalculator : IOrderItemVisitor
{
    // Calculate shipping costs (only for physical products)
}

public class TaxCalculator : IOrderItemVisitor
{
    // Calculate taxes based on item type and location
}

public class InventoryUpdater : IOrderItemVisitor
{
    // Update inventory levels appropriately
}
```

### 5. **Data Analysis Tools**
```csharp
public interface IDataNodeVisitor
{
    void Visit(NumericData data);
    void Visit(TextData data);
    void Visit(DateData data);
    void Visit(BooleanData data);
}

public class StatisticsCalculator : IDataNodeVisitor
{
    // Calculate statistics for different data types
}

public class DataValidator : IDataNodeVisitor
{
    // Validate data based on type-specific rules
}

public class DataExporter : IDataNodeVisitor
{
    // Export data in appropriate formats
}
```

## Visitor Pattern Variations

### 1. **Reflective Visitor**
Uses reflection to eliminate the need for multiple visit methods:
```csharp
public abstract class ReflectiveVisitor
{
    public void Visit(object element)
    {
        var method = GetType().GetMethod("VisitSpecific", new[] { element.GetType() });
        method?.Invoke(this, new[] { element });
    }
    
    // Specific visit methods for each type
    public abstract void VisitSpecific(Circle circle);
    public abstract void VisitSpecific(Rectangle rectangle);
}
```

### 2. **Acyclic Visitor**
Eliminates dependency cycles by using multiple single-method interfaces:
```csharp
public interface IVisitor { }

public interface ICircleVisitor : IVisitor
{
    void Visit(Circle circle);
}

public interface IRectangleVisitor : IVisitor
{
    void Visit(Rectangle rectangle);
}

public class AreaCalculator : ICircleVisitor, IRectangleVisitor
{
    public void Visit(Circle circle) { /* ... */ }
    public void Visit(Rectangle rectangle) { /* ... */ }
}
```

### 3. **Generic Visitor**
Uses generics to provide type safety:
```csharp
public interface IVisitor<TResult>
{
    TResult Visit<T>(T element) where T : IElement;
}

public interface IElement
{
    TResult Accept<TResult>(IVisitor<TResult> visitor);
}
```

## Best Practices

1. **Keep Visitors Stateless When Possible**: Makes them thread-safe and reusable
2. **Use Meaningful Names**: Visitor names should clearly indicate their purpose
3. **Consider Performance**: Visitor pattern can have overhead due to method calls
4. **Document Element Structure**: Make it clear what elements exist and their relationships
5. **Handle Null Cases**: Always check for null elements in visitors
6. **Consider Alternatives**: Sometimes other patterns (Strategy, Command) are simpler

## Common Pitfalls

1. **Frequent Element Changes**: Adding new element types breaks all existing visitors
2. **Tight Coupling**: Visitors often need intimate knowledge of element internals
3. **Method Explosion**: Large hierarchies lead to many visit methods
4. **Return Type Issues**: Different operations may need different return types

## Related Patterns

- **Composite**: Often used together - Visitor traverses Composite structures
- **Interpreter**: Both work with tree structures, but Interpreter focuses on language processing
- **Iterator**: Visitor focuses on operations, Iterator focuses on traversal
- **Strategy**: Alternative when you need to vary algorithms on single objects
- **Command**: Both encapsulate operations, but Command focuses on requests

## Visitor vs Other Patterns

| Pattern | When to Use | Key Difference |
|---------|-------------|----------------|
| **Strategy** | Operations on single objects | Visitor works on object structures |
| **Command** | Request/response operations | Visitor focuses on data structure traversal |
| **Template Method** | Algorithmic skeletons | Visitor adds operations externally |
| **Observer** | Event notifications | Visitor performs operations during traversal |

The Visitor Pattern is powerful for scenarios where you have a stable object structure but need to perform varying operations on it. It's particularly useful in compilers, document processors, and any system that needs to perform multiple unrelated operations on complex data structures.
