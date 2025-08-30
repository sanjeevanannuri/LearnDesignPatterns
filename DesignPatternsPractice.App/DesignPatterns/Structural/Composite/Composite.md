# Composite Pattern

## Overview
The Composite pattern lets you compose objects into tree structures and treat individual objects and compositions uniformly. Think of it like a file system - whether you're dealing with a single file or a folder containing many files and subfolders, you can perform the same operations (copy, delete, move) on both.

## Problem It Solves
Imagine you're building a graphics application where you can draw shapes, and you also want to group shapes together:
- Individual shapes (Circle, Rectangle) have operations like draw(), move(), resize()
- Groups of shapes should support the same operations
- Groups can contain other groups (nested structures)
- Client code shouldn't need to distinguish between single shapes and groups

Without Composite, you'd need different handling:
```csharp
// BAD: Different handling for individuals and groups
public void ProcessGraphics(object graphic)
{
    if (graphic is Shape shape)
    {
        shape.Draw();
    }
    else if (graphic is List<object> group)
    {
        foreach (var item in group)
        {
            ProcessGraphics(item); // Recursive calls, complex logic
        }
    }
    // Gets messy with nested groups
}
```

This becomes complex and error-prone with deeply nested structures.

## Real-World Analogy
Think of a **company organizational chart**:
1. **Individual Employee** (Leaf): Has a name, salary, can work
2. **Department** (Composite): Contains employees and other departments, can calculate total salary
3. **Company** (Composite): Contains departments, can calculate company-wide metrics
4. **Same Interface**: Whether you ask an employee or department for their budget, they both respond appropriately

Or consider a **file system**:
- **File** (Leaf): Has size, can be copied, deleted, moved
- **Folder** (Composite): Contains files and other folders, operations affect all contents
- **Same Operations**: Copy works the same whether you select a file or folder

## Implementation Details

### Basic Structure
```csharp
// Component interface - common operations for both leaf and composite
public abstract class Component
{
    protected string _name;

    public Component(string name)
    {
        _name = name;
    }

    public abstract void Operation();
    
    // Default implementations for composite operations
    public virtual void Add(Component component)
    {
        throw new NotSupportedException();
    }

    public virtual void Remove(Component component)
    {
        throw new NotSupportedException();
    }

    public virtual Component GetChild(int index)
    {
        throw new NotSupportedException();
    }
}

// Leaf - represents end objects with no children
public class Leaf : Component
{
    public Leaf(string name) : base(name) { }

    public override void Operation()
    {
        Console.WriteLine($"Leaf {_name} operation");
    }
}

// Composite - can contain other components
public class Composite : Component
{
    private List<Component> _children = new List<Component>();

    public Composite(string name) : base(name) { }

    public override void Operation()
    {
        Console.WriteLine($"Composite {_name} operation");
        foreach (var child in _children)
        {
            child.Operation();
        }
    }

    public override void Add(Component component)
    {
        _children.Add(component);
    }

    public override void Remove(Component component)
    {
        _children.Remove(component);
    }

    public override Component GetChild(int index)
    {
        return _children[index];
    }
}
```

### Key Components
1. **Component**: Common interface for both leaf and composite objects
2. **Leaf**: Represents end objects in the composition (no children)
3. **Composite**: Stores child components and implements child-related operations
4. **Client**: Manipulates objects through the Component interface

## Example from Our Code
```csharp
// Component - common interface for files and directories
public abstract class FileSystemItem
{
    protected string _name;

    public FileSystemItem(string name)
    {
        _name = name;
    }

    public abstract void Display(int depth = 0);
    public abstract long GetSize();

    // Virtual methods for composite operations
    public virtual void Add(FileSystemItem item)
    {
        throw new InvalidOperationException("Cannot add to a file");
    }

    public virtual void Remove(FileSystemItem item)
    {
        throw new InvalidOperationException("Cannot remove from a file");
    }
}

// Leaf - represents individual files
public class File : FileSystemItem
{
    private long _size;

    public File(string name, long size) : base(name)
    {
        _size = size;
    }

    public override void Display(int depth = 0)
    {
        Console.WriteLine($"{new string(' ', depth * 2)}📄 {_name} ({_size} bytes)");
    }

    public override long GetSize()
    {
        return _size;
    }
}

// Composite - represents directories that can contain files and other directories
public class Directory : FileSystemItem
{
    private List<FileSystemItem> _items = new List<FileSystemItem>();

    public Directory(string name) : base(name) { }

    public override void Display(int depth = 0)
    {
        Console.WriteLine($"{new string(' ', depth * 2)}📁 {_name}/");
        foreach (var item in _items)
        {
            item.Display(depth + 1);
        }
    }

    public override long GetSize()
    {
        long totalSize = 0;
        foreach (var item in _items)
        {
            totalSize += item.GetSize();
        }
        return totalSize;
    }

    public override void Add(FileSystemItem item)
    {
        _items.Add(item);
    }

    public override void Remove(FileSystemItem item)
    {
        _items.Remove(item);
    }
}

// Usage - same operations work for files and directories
var root = new Directory("root");
var documents = new Directory("Documents");
var pictures = new Directory("Pictures");

var resume = new File("resume.pdf", 1024);
var photo1 = new File("vacation.jpg", 2048);
var photo2 = new File("family.jpg", 1536);

documents.Add(resume);
pictures.Add(photo1);
pictures.Add(photo2);

root.Add(documents);
root.Add(pictures);

// Same interface for individual files and complex directory structures
root.Display();
Console.WriteLine($"Total size: {root.GetSize()} bytes");

// Works the same for a single file
photo1.Display();
Console.WriteLine($"File size: {photo1.GetSize()} bytes");
```

## Real-World Examples

### 1. **UI Component Hierarchy**
```csharp
// Component - base UI element
public abstract class UIComponent
{
    protected string _name;

    public UIComponent(string name)
    {
        _name = name;
    }

    public abstract void Render();
    public abstract void HandleEvent(string eventType);

    public virtual void Add(UIComponent component)
    {
        throw new NotSupportedException($"Cannot add children to {GetType().Name}");
    }

    public virtual void Remove(UIComponent component)
    {
        throw new NotSupportedException($"Cannot remove children from {GetType().Name}");
    }
}

// Leaf components
public class Button : UIComponent
{
    private string _text;

    public Button(string name, string text) : base(name)
    {
        _text = text;
    }

    public override void Render()
    {
        Console.WriteLine($"Rendering Button: {_text}");
    }

    public override void HandleEvent(string eventType)
    {
        if (eventType == "click")
        {
            Console.WriteLine($"Button {_name} clicked!");
        }
    }
}

public class TextBox : UIComponent
{
    private string _placeholder;

    public TextBox(string name, string placeholder) : base(name)
    {
        _placeholder = placeholder;
    }

    public override void Render()
    {
        Console.WriteLine($"Rendering TextBox: {_placeholder}");
    }

    public override void HandleEvent(string eventType)
    {
        if (eventType == "focus")
        {
            Console.WriteLine($"TextBox {_name} focused!");
        }
    }
}

// Composite components
public class Panel : UIComponent
{
    private List<UIComponent> _components = new List<UIComponent>();

    public Panel(string name) : base(name) { }

    public override void Render()
    {
        Console.WriteLine($"Rendering Panel: {_name}");
        foreach (var component in _components)
        {
            component.Render();
        }
    }

    public override void HandleEvent(string eventType)
    {
        Console.WriteLine($"Panel {_name} handling event: {eventType}");
        foreach (var component in _components)
        {
            component.HandleEvent(eventType);
        }
    }

    public override void Add(UIComponent component)
    {
        _components.Add(component);
    }

    public override void Remove(UIComponent component)
    {
        _components.Remove(component);
    }
}

public class Window : UIComponent
{
    private List<UIComponent> _components = new List<UIComponent>();

    public Window(string name) : base(name) { }

    public override void Render()
    {
        Console.WriteLine($"╔══ Window: {_name} ══╗");
        foreach (var component in _components)
        {
            component.Render();
        }
        Console.WriteLine("╚════════════════════╝");
    }

    public override void HandleEvent(string eventType)
    {
        Console.WriteLine($"Window {_name} handling event: {eventType}");
        foreach (var component in _components)
        {
            component.HandleEvent(eventType);
        }
    }

    public override void Add(UIComponent component)
    {
        _components.Add(component);
    }

    public override void Remove(UIComponent component)
    {
        _components.Remove(component);
    }
}

// Usage - building complex UI from simple components
var loginWindow = new Window("Login");

var loginPanel = new Panel("LoginPanel");
var usernameField = new TextBox("username", "Enter username");
var passwordField = new TextBox("password", "Enter password");
var loginButton = new Button("loginBtn", "Login");
var cancelButton = new Button("cancelBtn", "Cancel");

var buttonPanel = new Panel("ButtonPanel");
buttonPanel.Add(loginButton);
buttonPanel.Add(cancelButton);

loginPanel.Add(usernameField);
loginPanel.Add(passwordField);
loginPanel.Add(buttonPanel);

loginWindow.Add(loginPanel);

// Same interface works for complex nested structure
loginWindow.Render();
loginWindow.HandleEvent("click");
```

### 2. **Organization Structure**
```csharp
// Component - organization unit
public abstract class OrganizationUnit
{
    protected string _name;

    public OrganizationUnit(string name)
    {
        _name = name;
    }

    public abstract decimal GetBudget();
    public abstract int GetEmployeeCount();
    public abstract void ShowStructure(int depth = 0);

    public virtual void Add(OrganizationUnit unit)
    {
        throw new NotSupportedException();
    }

    public virtual void Remove(OrganizationUnit unit)
    {
        throw new NotSupportedException();
    }
}

// Leaf - individual employee
public class Employee : OrganizationUnit
{
    private decimal _salary;
    private string _position;

    public Employee(string name, string position, decimal salary) : base(name)
    {
        _position = position;
        _salary = salary;
    }

    public override decimal GetBudget()
    {
        return _salary;
    }

    public override int GetEmployeeCount()
    {
        return 1;
    }

    public override void ShowStructure(int depth = 0)
    {
        Console.WriteLine($"{new string(' ', depth * 2)}👤 {_name} - {_position} (${_salary:N0})");
    }
}

// Composite - department
public class Department : OrganizationUnit
{
    private List<OrganizationUnit> _units = new List<OrganizationUnit>();
    private decimal _operatingBudget;

    public Department(string name, decimal operatingBudget) : base(name)
    {
        _operatingBudget = operatingBudget;
    }

    public override decimal GetBudget()
    {
        decimal totalBudget = _operatingBudget;
        foreach (var unit in _units)
        {
            totalBudget += unit.GetBudget();
        }
        return totalBudget;
    }

    public override int GetEmployeeCount()
    {
        int count = 0;
        foreach (var unit in _units)
        {
            count += unit.GetEmployeeCount();
        }
        return count;
    }

    public override void ShowStructure(int depth = 0)
    {
        Console.WriteLine($"{new string(' ', depth * 2)}🏢 {_name} (Operating: ${_operatingBudget:N0})");
        foreach (var unit in _units)
        {
            unit.ShowStructure(depth + 1);
        }
    }

    public override void Add(OrganizationUnit unit)
    {
        _units.Add(unit);
    }

    public override void Remove(OrganizationUnit unit)
    {
        _units.Remove(unit);
    }
}

// Usage - building company structure
var company = new Department("TechCorp Inc.", 100000);

var engineering = new Department("Engineering", 50000);
var marketing = new Department("Marketing", 30000);

// Add employees to engineering
engineering.Add(new Employee("Alice Johnson", "Senior Developer", 95000));
engineering.Add(new Employee("Bob Smith", "DevOps Engineer", 85000));

// Add employees to marketing
marketing.Add(new Employee("Carol Davis", "Marketing Manager", 75000));
marketing.Add(new Employee("David Wilson", "Content Creator", 55000));

// Add departments to company
company.Add(engineering);
company.Add(marketing);

// Same operations work for individuals, departments, and entire company
company.ShowStructure();
Console.WriteLine($"Total Company Budget: ${company.GetBudget():N0}");
Console.WriteLine($"Total Employees: {company.GetEmployeeCount()}");

// Works the same for individual departments
Console.WriteLine($"\nEngineering Budget: ${engineering.GetBudget():N0}");
Console.WriteLine($"Engineering Employees: {engineering.GetEmployeeCount()}");
```

### 3. **Menu System**
```csharp
// Component - menu item
public abstract class MenuItem
{
    protected string _name;

    public MenuItem(string name)
    {
        _name = name;
    }

    public abstract void Execute();
    public abstract void Display(int depth = 0);

    public virtual void Add(MenuItem item)
    {
        throw new NotSupportedException();
    }

    public virtual void Remove(MenuItem item)
    {
        throw new NotSupportedException();
    }
}

// Leaf - action menu item
public class ActionMenuItem : MenuItem
{
    private Action _action;

    public ActionMenuItem(string name, Action action) : base(name)
    {
        _action = action;
    }

    public override void Execute()
    {
        Console.WriteLine($"Executing: {_name}");
        _action?.Invoke();
    }

    public override void Display(int depth = 0)
    {
        Console.WriteLine($"{new string(' ', depth * 2)}• {_name}");
    }
}

// Composite - submenu
public class SubMenu : MenuItem
{
    private List<MenuItem> _items = new List<MenuItem>();

    public SubMenu(string name) : base(name) { }

    public override void Execute()
    {
        Console.WriteLine($"Opening submenu: {_name}");
        Display();
    }

    public override void Display(int depth = 0)
    {
        Console.WriteLine($"{new string(' ', depth * 2)}📁 {_name}");
        foreach (var item in _items)
        {
            item.Display(depth + 1);
        }
    }

    public override void Add(MenuItem item)
    {
        _items.Add(item);
    }

    public override void Remove(MenuItem item)
    {
        _items.Remove(item);
    }
}

// Usage - building menu system
var mainMenu = new SubMenu("Main Menu");

var fileMenu = new SubMenu("File");
fileMenu.Add(new ActionMenuItem("New", () => Console.WriteLine("Creating new file...")));
fileMenu.Add(new ActionMenuItem("Open", () => Console.WriteLine("Opening file...")));
fileMenu.Add(new ActionMenuItem("Save", () => Console.WriteLine("Saving file...")));

var editMenu = new SubMenu("Edit");
editMenu.Add(new ActionMenuItem("Cut", () => Console.WriteLine("Cutting text...")));
editMenu.Add(new ActionMenuItem("Copy", () => Console.WriteLine("Copying text...")));
editMenu.Add(new ActionMenuItem("Paste", () => Console.WriteLine("Pasting text...")));

mainMenu.Add(fileMenu);
mainMenu.Add(editMenu);
mainMenu.Add(new ActionMenuItem("Exit", () => Console.WriteLine("Exiting application...")));

// Same interface for simple actions and complex submenus
mainMenu.Display();
```

## Benefits
- **Uniform Treatment**: Handle individual objects and compositions the same way
- **Recursive Structure**: Natural support for tree-like hierarchies
- **Simplifies Client Code**: No need to distinguish between leaf and composite objects
- **Easy Extension**: Add new component types without changing existing code
- **Flexible Structure**: Build complex structures from simple components

## Drawbacks
- **Overly General**: Interface might be too general for some components
- **Type Safety**: Hard to restrict components to specific types
- **Runtime Checking**: May need runtime checks for component types
- **Performance**: Recursive operations can be expensive for deep trees

## When to Use
✅ **Use When:**
- You want to represent part-whole hierarchies
- You want clients to treat individual objects and compositions uniformly
- You have tree-like object structures
- You want to ignore the difference between compositions and individual objects

❌ **Avoid When:**
- Object hierarchy is simple and flat
- You need strong type safety for components
- Performance is critical and tree traversal is expensive
- The objects don't naturally form a tree structure

## Composite vs Other Patterns

| Pattern | Purpose | Key Difference |
|---------|---------|----------------|
| **Composite** | Treats individual and composite objects uniformly | Focus on part-whole hierarchies |
| **Decorator** | Adds behavior to objects dynamically | Focus on adding responsibilities |
| **Chain of Responsibility** | Passes requests along a chain | Focus on request handling chain |
| **Strategy** | Encapsulates algorithms | Focus on interchangeable algorithms |

## Best Practices
1. **Common Interface**: Keep the component interface as simple as possible
2. **Transparent Composition**: Make composite operations transparent to clients
3. **Child Management**: Decide whether to put child management in Component or Composite
4. **Ordering**: Consider if child ordering matters in composite operations
5. **Caching**: Cache expensive operations like size calculations
6. **Memory Management**: Be careful with circular references

## Common Mistakes
1. **Interface Pollution**: Adding too many operations to the base component
2. **Type Confusion**: Not handling leaf vs composite operations properly
3. **Deep Recursion**: Not protecting against stack overflow in deep trees
4. **Shared References**: Accidentally sharing child objects between composites

## Design Variations

### 1. **Transparent Composite** (Used in our examples)
Child management operations are in the base Component class:
```csharp
public abstract class Component
{
    public virtual void Add(Component child) { throw new NotSupportedException(); }
    public virtual void Remove(Component child) { throw new NotSupportedException(); }
}
```

### 2. **Safe Composite**
Child management operations only in Composite class:
```csharp
public abstract class Component
{
    public abstract void Operation();
}

public class Composite : Component
{
    public void Add(Component child) { /* implementation */ }
    public void Remove(Component child) { /* implementation */ }
}
```

## Modern C# Features
```csharp
// Using IEnumerable for iteration
public class ModernComposite : Component, IEnumerable<Component>
{
    private List<Component> _children = new();

    public void Add(Component child) => _children.Add(child);
    
    public IEnumerator<Component> GetEnumerator() => _children.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

// Using LINQ operations
var totalSize = composite.Sum(child => child.GetSize());
var allFiles = composite.OfType<File>().ToList();

// Using async operations
public abstract class AsyncComponent
{
    public abstract Task<long> GetSizeAsync();
}

public class AsyncComposite : AsyncComponent
{
    public override async Task<long> GetSizeAsync()
    {
        var tasks = _children.Select(child => child.GetSizeAsync());
        var sizes = await Task.WhenAll(tasks);
        return sizes.Sum();
    }
}
```

## Testing Composite Pattern
```csharp
[Test]
public void Composite_GetSize_ReturnsSumOfChildren()
{
    // Arrange
    var composite = new Directory("test");
    composite.Add(new File("file1.txt", 100));
    composite.Add(new File("file2.txt", 200));

    // Act
    var totalSize = composite.GetSize();

    // Assert
    Assert.AreEqual(300, totalSize);
}

[Test]
public void Composite_Display_CallsDisplayOnAllChildren()
{
    // Arrange
    var mockChild1 = new Mock<FileSystemItem>("child1");
    var mockChild2 = new Mock<FileSystemItem>("child2");
    var composite = new Directory("parent");
    
    // Act
    composite.Add(mockChild1.Object);
    composite.Add(mockChild2.Object);
    composite.Display();

    // Assert
    mockChild1.Verify(c => c.Display(1), Times.Once);
    mockChild2.Verify(c => c.Display(1), Times.Once);
}
```

## Summary
The Composite pattern is like building with Lego blocks - individual pieces and complex constructions can be manipulated the same way. Whether you're moving a single block or an entire castle, the operations are the same.

It's perfect for hierarchical structures where you want to treat individual items and groups of items uniformly. Think file systems, UI components, organization charts, or any tree-like structure where the same operations apply at every level.

The key insight is that sometimes the most powerful approach is to make complex structures look and behave just like simple objects from the client's perspective.
