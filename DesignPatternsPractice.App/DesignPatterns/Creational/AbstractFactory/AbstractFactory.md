# Abstract Factory Pattern

## Overview
The Abstract Factory pattern provides an interface for creating families of related objects without specifying their concrete classes. Think of it like a furniture store that has different style collections - Modern, Victorian, and Art Deco. Each collection has chairs, tables, and sofas that all match the same style.

## Problem It Solves
Imagine you're building a cross-platform GUI application that needs to look native on different operating systems:
- Windows: Windows-style buttons, checkboxes, windows
- Mac: Mac-style buttons, checkboxes, windows
- Linux: Linux-style buttons, checkboxes, windows

Without Abstract Factory, you might end up with:
```csharp
// BAD: Mixing different UI styles
var button = new WindowsButton();      // Windows style
var checkbox = new MacCheckbox();      // Mac style - Inconsistent!
var window = new LinuxWindow();        // Linux style - Even more inconsistent!
```

This creates an inconsistent user interface where components don't match.

## Real-World Analogy
Think of a **themed restaurant chain**. Each location has a different theme (Italian, Mexican, Asian), but within each restaurant, everything matches:

**Italian Restaurant:**
- Italian furniture
- Italian music
- Italian menu
- Italian decorations

**Mexican Restaurant:**
- Mexican furniture
- Mexican music
- Mexican menu
- Mexican decorations

You can't mix a Mexican menu with Italian furniture - everything in each restaurant must belong to the same theme family.

## Implementation Details

### Basic Structure
```csharp
// Abstract products
public interface IButton { void Paint(); }
public interface ICheckbox { void Paint(); }

// Abstract factory
public interface IGUIFactory
{
    IButton CreateButton();
    ICheckbox CreateCheckbox();
}

// Concrete products for Windows
public class WindowsButton : IButton
{
    public void Paint() => Console.WriteLine("Windows Button");
}

public class WindowsCheckbox : ICheckbox
{
    public void Paint() => Console.WriteLine("Windows Checkbox");
}

// Concrete factory for Windows
public class WindowsFactory : IGUIFactory
{
    public IButton CreateButton() => new WindowsButton();
    public ICheckbox CreateCheckbox() => new WindowsCheckbox();
}
```

### Key Components
1. **Abstract Factory**: Interface for creating families of products
2. **Concrete Factories**: Implement the abstract factory for specific product families
3. **Abstract Products**: Interfaces for different types of products
4. **Concrete Products**: Specific implementations of products
5. **Client**: Uses only abstract interfaces

## Example from Our Code
```csharp
// Abstract products
public interface IButton
{
    string Paint();
}

public interface ICheckbox
{
    string Paint();
}

// Abstract factory
public interface IGUIFactory
{
    IButton CreateButton();
    ICheckbox CreateCheckbox();
}

// Windows family
public class WindowsButton : IButton
{
    public string Paint() => "Render a button in Windows style.";
}

public class WindowsCheckbox : ICheckbox
{
    public string Paint() => "Render a checkbox in Windows style.";
}

public class WindowsFactory : IGUIFactory
{
    public IButton CreateButton() => new WindowsButton();
    public ICheckbox CreateCheckbox() => new WindowsCheckbox();
}

// Mac family
public class MacButton : IButton
{
    public string Paint() => "Render a button in Mac style.";
}

public class MacCheckbox : ICheckbox
{
    public string Paint() => "Render a checkbox in Mac style.";
}

public class MacFactory : IGUIFactory
{
    public IButton CreateButton() => new MacButton();
    public ICheckbox CreateCheckbox() => new MacCheckbox();
}
```

## Real-World Examples

### 1. **Game Theme System**
```csharp
// Abstract factory for game themes
public interface IGameThemeFactory
{
    IBackground CreateBackground();
    ICharacterSprite CreateCharacter();
    IMusic CreateMusic();
}

// Medieval theme
public class MedievalThemeFactory : IGameThemeFactory
{
    public IBackground CreateBackground() => new CastleBackground();
    public ICharacterSprite CreateCharacter() => new KnightSprite();
    public IMusic CreateMusic() => new MedievalMusic();
}

// Sci-Fi theme
public class SciFiThemeFactory : IGameThemeFactory
{
    public IBackground CreateBackground() => new SpaceStationBackground();
    public ICharacterSprite CreateCharacter() => new AstronautSprite();
    public IMusic CreateMusic() => new ElectronicMusic();
}
```

### 2. **Database Connection Factory**
```csharp
public interface IDatabaseFactory
{
    IConnection CreateConnection();
    ICommand CreateCommand();
    IDataAdapter CreateDataAdapter();
}

public class SqlServerFactory : IDatabaseFactory
{
    public IConnection CreateConnection() => new SqlConnection();
    public ICommand CreateCommand() => new SqlCommand();
    public IDataAdapter CreateDataAdapter() => new SqlDataAdapter();
}

public class OracleFactory : IDatabaseFactory
{
    public IConnection CreateConnection() => new OracleConnection();
    public ICommand CreateCommand() => new OracleCommand();
    public IDataAdapter CreateDataAdapter() => new OracleDataAdapter();
}
```

### 3. **Document Processing System**
```csharp
public interface IDocumentFactory
{
    IDocumentReader CreateReader();
    IDocumentWriter CreateWriter();
    IDocumentValidator CreateValidator();
}

public class PdfDocumentFactory : IDocumentFactory
{
    public IDocumentReader CreateReader() => new PdfReader();
    public IDocumentWriter CreateWriter() => new PdfWriter();
    public IDocumentValidator CreateValidator() => new PdfValidator();
}

public class WordDocumentFactory : IDocumentFactory
{
    public IDocumentReader CreateReader() => new WordReader();
    public IDocumentWriter CreateWriter() => new WordWriter();
    public IDocumentValidator CreateValidator() => new WordValidator();
}
```

## Usage Example
```csharp
public class Application
{
    private IGUIFactory _factory;
    private IButton _button;
    private ICheckbox _checkbox;

    public Application(IGUIFactory factory)
    {
        _factory = factory;
    }

    public void CreateUI()
    {
        _button = _factory.CreateButton();
        _checkbox = _factory.CreateCheckbox();
    }

    public void RenderUI()
    {
        _button.Paint();
        _checkbox.Paint();
    }
}

// Usage
IGUIFactory factory;
if (operatingSystem == "Windows")
    factory = new WindowsFactory();
else
    factory = new MacFactory();

var app = new Application(factory);
app.CreateUI();
app.RenderUI();
```

## Benefits
- **Consistency**: Ensures related objects are used together
- **Flexibility**: Easy to switch between product families
- **Isolation**: Separates concrete classes from client code
- **Easy Extension**: Adding new product families is straightforward

## Drawbacks
- **Complexity**: Requires many interfaces and classes
- **Rigid Structure**: Adding new products to existing families requires changing all factories
- **Over-engineering**: Can be overkill for simple scenarios

## When to Use
✅ **Use When:**
- You need to create families of related objects
- You want to ensure objects from the same family are used together
- You need to support multiple product lines
- You want to provide a library without exposing implementation details

❌ **Avoid When:**
- You only have one product family
- Product families rarely change
- You need to add new product types frequently

## Abstract Factory vs Factory Method
| Abstract Factory | Factory Method |
|---|---|
| Creates families of objects | Creates single objects |
| Uses composition | Uses inheritance |
| Emphasizes product families | Emphasizes product creation |
| More complex | Simpler |

## Best Practices
1. **Start Simple**: Don't use Abstract Factory until you actually need product families
2. **Consistent Naming**: Use consistent naming conventions across families
3. **Error Handling**: Handle cases where factory creation might fail
4. **Documentation**: Document which products belong to which families
5. **Testing**: Test each factory independently

## Common Mistakes
1. **Overuse**: Using Abstract Factory when Factory Method would suffice
2. **Mixing Families**: Accidentally using products from different families
3. **Tight Coupling**: Making client code depend on concrete factories
4. **Poor Organization**: Not clearly defining product family boundaries

## Summary
The Abstract Factory pattern is like a themed furniture store. When you choose a style (Modern, Traditional, or Contemporary), you get all matching pieces - chairs, tables, sofas, and lamps that work together harmoniously. You can't accidentally mix a modern chair with a traditional table because each factory only produces items from its designated style family.
