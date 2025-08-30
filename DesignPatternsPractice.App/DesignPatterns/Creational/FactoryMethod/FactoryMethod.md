# Factory Method Pattern

## Overview
The Factory Method pattern provides an interface for creating objects, but lets subclasses decide which class to instantiate. Think of it like a restaurant menu - you order "pizza" but the kitchen decides whether to make a pepperoni, margherita, or veggie pizza based on your specific request.

## Problem It Solves
Imagine you're building a document editor that needs to create different types of documents:
- Word documents (.docx)
- PDF documents (.pdf)
- PowerPoint presentations (.pptx)

Without Factory Method, your code might look like:
```csharp
// BAD: Tightly coupled code
if (type == "word")
    document = new WordDocument();
else if (type == "pdf")
    document = new PdfDocument();
else if (type == "powerpoint")
    document = new PowerPointDocument();
```

This creates problems:
- Hard to add new document types
- Violates Open/Closed Principle
- Code becomes messy with many if-else statements

## Real-World Analogy
Think of a **car manufacturing plant**. The plant has a general process for making cars (the factory method), but different assembly lines create different models:
- SUV assembly line creates SUVs
- Sedan assembly line creates sedans
- Truck assembly line creates trucks

Each assembly line follows the same general car-making process but produces different types of vehicles.

## Implementation Details

### Basic Structure
```csharp
// Product interface
public interface IProduct
{
    string Operation();
}

// Creator (Factory)
public abstract class Creator
{
    // Factory Method
    public abstract IProduct FactoryMethod();
    
    // Business logic that uses the factory method
    public string SomeOperation()
    {
        var product = FactoryMethod();
        return product.Operation();
    }
}

// Concrete Creator
public class ConcreteCreator : Creator
{
    public override IProduct FactoryMethod()
    {
        return new ConcreteProduct();
    }
}
```

### Key Components
1. **Product Interface**: Defines what all products can do
2. **Concrete Products**: Specific implementations of products
3. **Creator Class**: Abstract class with the factory method
4. **Concrete Creators**: Implement the factory method to create specific products

## Example from Our Code
```csharp
// Product interface
public interface IProduct
{
    string Operation();
}

// Concrete products
public class ConcreteProductA : IProduct
{
    public string Operation() => "Result of ConcreteProductA";
}

public class ConcreteProductB : IProduct
{
    public string Operation() => "Result of ConcreteProductB";
}

// Creator
public abstract class Creator
{
    public abstract IProduct FactoryMethod();
    
    public string SomeOperation()
    {
        var product = FactoryMethod();
        return $"Creator: {product.Operation()}";
    }
}

// Concrete creators
public class ConcreteCreatorA : Creator
{
    public override IProduct FactoryMethod() => new ConcreteProductA();
}

public class ConcreteCreatorB : Creator
{
    public override IProduct FactoryMethod() => new ConcreteProductB();
}
```

## Real-World Examples

### 1. **Document Creation System**
```csharp
public abstract class DocumentCreator
{
    public abstract IDocument CreateDocument();
    
    public void ProcessDocument()
    {
        var doc = CreateDocument();
        doc.Open();
        doc.Save();
        doc.Close();
    }
}

public class WordDocumentCreator : DocumentCreator
{
    public override IDocument CreateDocument() => new WordDocument();
}

public class PdfDocumentCreator : DocumentCreator
{
    public override IDocument CreateDocument() => new PdfDocument();
}
```

### 2. **Payment Processing**
```csharp
public abstract class PaymentProcessorFactory
{
    public abstract IPaymentProcessor CreateProcessor();
    
    public void ProcessPayment(decimal amount)
    {
        var processor = CreateProcessor();
        processor.ProcessPayment(amount);
    }
}

public class CreditCardProcessorFactory : PaymentProcessorFactory
{
    public override IPaymentProcessor CreateProcessor() => new CreditCardProcessor();
}

public class PayPalProcessorFactory : PaymentProcessorFactory
{
    public override IPaymentProcessor CreateProcessor() => new PayPalProcessor();
}
```

### 3. **Game Character Creation**
```csharp
public abstract class CharacterFactory
{
    public abstract ICharacter CreateCharacter();
    
    public void SetupCharacter()
    {
        var character = CreateCharacter();
        character.SetDefaultStats();
        character.EquipStartingGear();
    }
}

public class WarriorFactory : CharacterFactory
{
    public override ICharacter CreateCharacter() => new Warrior();
}

public class MageFactory : CharacterFactory
{
    public override ICharacter CreateCharacter() => new Mage();
}
```

## Benefits
- **Flexibility**: Easy to add new product types without changing existing code
- **Loose Coupling**: Client code doesn't depend on concrete classes
- **Single Responsibility**: Each creator has one reason to change
- **Open/Closed Principle**: Open for extension, closed for modification

## Drawbacks
- **Complexity**: Requires more classes and interfaces
- **Learning Curve**: Can be confusing for simple scenarios
- **Over-engineering**: Might be overkill for simple object creation

## When to Use
✅ **Use When:**
- You don't know beforehand the exact types of objects you need to create
- You want to provide a library of products and reveal only their interfaces
- You want to extend the internal components of a framework
- You need to delegate object creation to subclasses

❌ **Avoid When:**
- You only have one type of product
- Object creation is simple and unlikely to change
- Performance is critical (factory methods add overhead)

## Factory Method vs Simple Factory
| Factory Method | Simple Factory |
|---|---|
| Uses inheritance | Uses composition |
| Abstract factory method | Concrete factory class |
| Can be extended via subclassing | Extended by modifying factory |
| More flexible | Simpler to implement |

## Best Practices
1. **Meaningful Names**: Use descriptive names for factory methods
2. **Error Handling**: Handle cases where creation might fail
3. **Documentation**: Document what types each factory creates
4. **Validation**: Validate parameters before creating objects
5. **Caching**: Consider caching created objects if appropriate

## Comparison with Other Patterns
- **Abstract Factory**: Creates families of related objects
- **Builder**: Constructs complex objects step by step
- **Prototype**: Creates objects by cloning existing instances
- **Singleton**: Ensures only one instance exists

## Summary
The Factory Method pattern is like a specialized assembly line in a factory. Each assembly line (concrete creator) knows how to make a specific type of product, but they all follow the same general manufacturing process (abstract creator). This allows you to easily add new product types by creating new assembly lines without changing the overall factory structure.
