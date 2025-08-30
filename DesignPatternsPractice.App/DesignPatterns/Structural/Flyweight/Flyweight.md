# Flyweight Pattern

## Overview
The Flyweight pattern minimizes memory usage by sharing common parts of object state among multiple objects. Think of it like a text editor - instead of storing font information (Arial, 12pt, Bold) with every single character, you store the font once and reference it from all characters that use the same formatting.

## Problem It Solves
Imagine you're building a word processor that displays thousands of characters:
- Each character has: letter, font, size, color, position
- Without optimization: 10,000 characters × (letter + font + size + color + position) = huge memory usage
- Many characters share the same formatting (intrinsic state)
- Only position changes between characters (extrinsic state)

Without Flyweight, memory usage explodes:
```csharp
// BAD: Each character object stores all properties
public class Character
{
    public char Letter { get; set; }        // Intrinsic (sharable)
    public string Font { get; set; }        // Intrinsic (sharable)
    public int Size { get; set; }           // Intrinsic (sharable)
    public string Color { get; set; }       // Intrinsic (sharable)
    public int X { get; set; }              // Extrinsic (unique per instance)
    public int Y { get; set; }              // Extrinsic (unique per instance)
}

// Creates 10,000 objects with duplicated font information
var characters = new List<Character>();
for (int i = 0; i < 10000; i++)
{
    characters.Add(new Character 
    { 
        Letter = 'A', 
        Font = "Arial", 
        Size = 12, 
        Color = "Black",
        X = i * 10, 
        Y = 100 
    });
}
```

## Real-World Analogy
Think of a **library with books**:
1. **Book Content** (Intrinsic State): The actual text, author, title - shared among all copies
2. **Location** (Extrinsic State): Which shelf, which library, who borrowed it - unique per copy
3. **Optimization**: One master copy of content, multiple location records
4. **Memory Savings**: Instead of 1000 full books, you have 1 content + 1000 location records

Or consider **emoji in messaging**:
- **Emoji Image** (Intrinsic): The actual 😀 graphic data - shared
- **Position** (Extrinsic): Where it appears in each message - unique
- **Efficiency**: One 😀 image stored, used millions of times across all messages

## Implementation Details

### Basic Structure
```csharp
// Flyweight interface - operations that can act on intrinsic and extrinsic state
public interface IFlyweight
{
    void Operation(ExtrinsicState extrinsicState);
}

// Concrete flyweight - stores intrinsic state, operates on extrinsic state
public class ConcreteFlyweight : IFlyweight
{
    private readonly string _intrinsicState;

    public ConcreteFlyweight(string intrinsicState)
    {
        _intrinsicState = intrinsicState;
    }

    public void Operation(ExtrinsicState extrinsicState)
    {
        Console.WriteLine($"Intrinsic: {_intrinsicState}, Extrinsic: {extrinsicState}");
    }
}

// Extrinsic state - stored by client
public class ExtrinsicState
{
    public int X { get; set; }
    public int Y { get; set; }
    
    public override string ToString() => $"({X}, {Y})";
}

// Flyweight factory - ensures flyweights are shared
public class FlyweightFactory
{
    private readonly Dictionary<string, IFlyweight> _flyweights = new();

    public IFlyweight GetFlyweight(string key)
    {
        if (!_flyweights.ContainsKey(key))
        {
            _flyweights[key] = new ConcreteFlyweight(key);
        }
        return _flyweights[key];
    }

    public int GetFlyweightCount() => _flyweights.Count;
}
```

### Key Components
1. **Flyweight**: Interface for flyweights to receive and act on extrinsic state
2. **ConcreteFlyweight**: Implements Flyweight and stores intrinsic state
3. **FlyweightFactory**: Creates and manages flyweight instances
4. **Context**: Maintains extrinsic state and references to flyweight
5. **Client**: Maintains extrinsic state and passes it to flyweight operations

## Example from Our Code
```csharp
// Flyweight interface
public interface ICharacterFlyweight
{
    void Display(int x, int y, string context);
}

// Concrete flyweight - stores intrinsic state (font info)
public class CharacterFlyweight : ICharacterFlyweight
{
    private readonly char _character;
    private readonly string _font;
    private readonly int _size;
    private readonly string _color;

    public CharacterFlyweight(char character, string font, int size, string color)
    {
        _character = character;
        _font = font;
        _size = size;
        _color = color;
        
        // Simulate resource-intensive operation
        Console.WriteLine($"Creating flyweight for '{character}' ({font}, {size}pt, {color})");
    }

    public void Display(int x, int y, string context)
    {
        Console.WriteLine($"Displaying '{_character}' at ({x}, {y}) " +
                         $"[{_font}, {_size}pt, {_color}] in context: {context}");
    }
}

// Flyweight factory - manages shared instances
public class CharacterFlyweightFactory
{
    private static readonly Dictionary<string, ICharacterFlyweight> _flyweights = new();

    public static ICharacterFlyweight GetCharacterFlyweight(char character, string font, int size, string color)
    {
        string key = $"{character}_{font}_{size}_{color}";
        
        if (!_flyweights.ContainsKey(key))
        {
            _flyweights[key] = new CharacterFlyweight(character, font, size, color);
        }

        return _flyweights[key];
    }

    public static int GetFlyweightCount()
    {
        return _flyweights.Count;
    }

    public static void ListFlyweights()
    {
        Console.WriteLine($"Total flyweights created: {_flyweights.Count}");
        foreach (var key in _flyweights.Keys)
        {
            Console.WriteLine($"  - {key}");
        }
    }
}

// Context - stores extrinsic state and reference to flyweight
public class CharacterContext
{
    private readonly ICharacterFlyweight _flyweight;
    private readonly int _x;
    private readonly int _y;
    private readonly string _context;

    public CharacterContext(char character, string font, int size, string color, 
                           int x, int y, string context)
    {
        _flyweight = CharacterFlyweightFactory.GetCharacterFlyweight(character, font, size, color);
        _x = x;
        _y = y;
        _context = context;
    }

    public void Display()
    {
        _flyweight.Display(_x, _y, _context);
    }
}

// Document - client that uses flyweights
public class Document
{
    private readonly List<CharacterContext> _characters = new();

    public void AddCharacter(char character, string font, int size, string color, 
                           int x, int y, string context)
    {
        var characterContext = new CharacterContext(character, font, size, color, x, y, context);
        _characters.Add(characterContext);
    }

    public void Display()
    {
        Console.WriteLine("\n=== Document Display ===");
        foreach (var character in _characters)
        {
            character.Display();
        }
        Console.WriteLine($"\nDocument contains {_characters.Count} characters");
        Console.WriteLine($"But only {CharacterFlyweightFactory.GetFlyweightCount()} flyweight objects were created");
    }
}

// Usage - demonstrating memory savings
var document = new Document();

// Add many characters with same formatting (shares flyweights)
string text = "Hello World! This is a test document with repeated characters.";
for (int i = 0; i < text.Length; i++)
{
    document.AddCharacter(text[i], "Arial", 12, "Black", i * 10, 100, "main-text");
}

// Add some characters with different formatting
document.AddCharacter('!', "Arial", 16, "Red", 600, 100, "emphasis");
document.AddCharacter('!', "Arial", 16, "Red", 610, 100, "emphasis");
document.AddCharacter('A', "Times", 14, "Blue", 620, 100, "header");

document.Display();
CharacterFlyweightFactory.ListFlyweights();
```

## Real-World Examples

### 1. **Tree Rendering in Games**
```csharp
// Flyweight for tree types
public interface ITreeType
{
    void Render(int x, int y, int size, string season);
}

public class TreeTypeFlyweight : ITreeType
{
    private readonly string _name;
    private readonly string _textureFile;
    private readonly string _meshFile;

    public TreeTypeFlyweight(string name, string textureFile, string meshFile)
    {
        _name = name;
        _textureFile = textureFile;
        _meshFile = meshFile;
        
        // Simulate loading expensive resources
        Console.WriteLine($"Loading tree type: {name}");
        Console.WriteLine($"  - Loading texture: {textureFile}");
        Console.WriteLine($"  - Loading 3D mesh: {meshFile}");
    }

    public void Render(int x, int y, int size, string season)
    {
        Console.WriteLine($"Rendering {_name} tree at ({x}, {y}) " +
                         $"size: {size}%, season: {season}");
        // Use _textureFile and _meshFile for actual rendering
    }
}

// Factory for tree types
public class TreeTypeFactory
{
    private static readonly Dictionary<string, ITreeType> _treeTypes = new();

    public static ITreeType GetTreeType(string name, string textureFile, string meshFile)
    {
        string key = $"{name}_{textureFile}_{meshFile}";
        
        if (!_treeTypes.ContainsKey(key))
        {
            _treeTypes[key] = new TreeTypeFlyweight(name, textureFile, meshFile);
        }

        return _treeTypes[key];
    }

    public static int GetLoadedTreeTypes() => _treeTypes.Count;
}

// Context for individual trees
public class Tree
{
    private readonly ITreeType _treeType;
    private readonly int _x, _y;
    private readonly int _size;
    private readonly string _season;

    public Tree(string name, string textureFile, string meshFile, 
               int x, int y, int size, string season)
    {
        _treeType = TreeTypeFactory.GetTreeType(name, textureFile, meshFile);
        _x = x;
        _y = y;
        _size = size;
        _season = season;
    }

    public void Render()
    {
        _treeType.Render(_x, _y, _size, _season);
    }
}

// Forest - manages many trees
public class Forest
{
    private readonly List<Tree> _trees = new();

    public void PlantTree(string type, string texture, string mesh, 
                         int x, int y, int size, string season)
    {
        var tree = new Tree(type, texture, mesh, x, y, size, season);
        _trees.Add(tree);
    }

    public void RenderForest()
    {
        Console.WriteLine("\n=== Rendering Forest ===");
        foreach (var tree in _trees)
        {
            tree.Render();
        }
        Console.WriteLine($"\nForest contains {_trees.Count} trees");
        Console.WriteLine($"Only {TreeTypeFactory.GetLoadedTreeTypes()} tree type flyweights loaded");
    }
}

// Usage - creating a forest with many trees
var forest = new Forest();

// Plant many oak trees (shares same flyweight)
for (int i = 0; i < 100; i++)
{
    forest.PlantTree("Oak", "oak_texture.jpg", "oak_mesh.obj", 
                    i * 50, 100, 100, "Spring");
}

// Plant some pine trees (different flyweight)
for (int i = 0; i < 50; i++)
{
    forest.PlantTree("Pine", "pine_texture.jpg", "pine_mesh.obj", 
                    i * 50, 200, 120, "Winter");
}

// Plant more oaks in different positions (reuses flyweight)
for (int i = 0; i < 25; i++)
{
    forest.PlantTree("Oak", "oak_texture.jpg", "oak_mesh.obj", 
                    i * 75, 300, 90, "Fall");
}

forest.RenderForest();
```

### 2. **Web Page Elements**
```csharp
// Flyweight for CSS styles
public interface IStyleFlyweight
{
    void ApplyStyle(string elementId, string content);
}

public class CSSStyleFlyweight : IStyleFlyweight
{
    private readonly string _className;
    private readonly Dictionary<string, string> _cssProperties;

    public CSSStyleFlyweight(string className, Dictionary<string, string> cssProperties)
    {
        _className = className;
        _cssProperties = new Dictionary<string, string>(cssProperties);
        
        Console.WriteLine($"Creating CSS style flyweight: {className}");
    }

    public void ApplyStyle(string elementId, string content)
    {
        Console.WriteLine($"Applying style '{_className}' to element '{elementId}': {content}");
        foreach (var property in _cssProperties)
        {
            Console.WriteLine($"  {property.Key}: {property.Value}");
        }
    }
}

// Factory for CSS styles
public class StyleFactory
{
    private static readonly Dictionary<string, IStyleFlyweight> _styles = new();

    public static IStyleFlyweight GetStyle(string className, Dictionary<string, string> cssProperties)
    {
        if (!_styles.ContainsKey(className))
        {
            _styles[className] = new CSSStyleFlyweight(className, cssProperties);
        }

        return _styles[className];
    }

    public static int GetStyleCount() => _styles.Count;
}

// Context for individual web elements
public class WebElement
{
    private readonly IStyleFlyweight _style;
    private readonly string _elementId;
    private readonly string _content;

    public WebElement(string elementId, string content, string styleClass, 
                     Dictionary<string, string> cssProperties)
    {
        _elementId = elementId;
        _content = content;
        _style = StyleFactory.GetStyle(styleClass, cssProperties);
    }

    public void Render()
    {
        _style.ApplyStyle(_elementId, _content);
    }
}

// Web page - manages many elements
public class WebPage
{
    private readonly List<WebElement> _elements = new();

    public void AddElement(string elementId, string content, string styleClass, 
                          Dictionary<string, string> cssProperties)
    {
        var element = new WebElement(elementId, content, styleClass, cssProperties);
        _elements.Add(element);
    }

    public void RenderPage()
    {
        Console.WriteLine("\n=== Rendering Web Page ===");
        foreach (var element in _elements)
        {
            element.Render();
        }
        Console.WriteLine($"\nPage contains {_elements.Count} elements");
        Console.WriteLine($"Only {StyleFactory.GetStyleCount()} style flyweights created");
    }
}

// Usage - creating a web page with many styled elements
var page = new WebPage();

// Define common styles
var headerStyle = new Dictionary<string, string>
{
    ["font-size"] = "24px",
    ["font-weight"] = "bold",
    ["color"] = "#333"
};

var paragraphStyle = new Dictionary<string, string>
{
    ["font-size"] = "14px",
    ["line-height"] = "1.6",
    ["color"] = "#666"
};

var buttonStyle = new Dictionary<string, string>
{
    ["background-color"] = "#007bff",
    ["color"] = "white",
    ["padding"] = "10px 20px",
    ["border-radius"] = "5px"
};

// Add many elements with shared styles
page.AddElement("h1", "Main Title", "header", headerStyle);
page.AddElement("h2", "Section Title", "header", headerStyle);
page.AddElement("h3", "Subsection Title", "header", headerStyle);

for (int i = 1; i <= 10; i++)
{
    page.AddElement($"p{i}", $"This is paragraph {i} content.", "paragraph", paragraphStyle);
}

for (int i = 1; i <= 5; i++)
{
    page.AddElement($"btn{i}", $"Button {i}", "button", buttonStyle);
}

page.RenderPage();
```

### 3. **Particle System**
```csharp
// Flyweight for particle types
public interface IParticleType
{
    void Update(float x, float y, float velocityX, float velocityY, float life);
    void Render(float x, float y, float alpha);
}

public class ParticleTypeFlyweight : IParticleType
{
    private readonly string _textureName;
    private readonly float _mass;
    private readonly string _color;

    public ParticleTypeFlyweight(string textureName, float mass, string color)
    {
        _textureName = textureName;
        _mass = mass;
        _color = color;
        
        Console.WriteLine($"Loading particle type: {textureName} ({color}, mass: {mass})");
    }

    public void Update(float x, float y, float velocityX, float velocityY, float life)
    {
        // Physics calculations using intrinsic properties
        Console.WriteLine($"Updating particle at ({x:F1}, {y:F1}) " +
                         $"velocity: ({velocityX:F1}, {velocityY:F1}) life: {life:F2}");
    }

    public void Render(float x, float y, float alpha)
    {
        Console.WriteLine($"Rendering {_color} {_textureName} at ({x:F1}, {y:F1}) alpha: {alpha:F2}");
    }
}

// Factory for particle types
public class ParticleTypeFactory
{
    private static readonly Dictionary<string, IParticleType> _particleTypes = new();

    public static IParticleType GetParticleType(string texture, float mass, string color)
    {
        string key = $"{texture}_{mass}_{color}";
        
        if (!_particleTypes.ContainsKey(key))
        {
            _particleTypes[key] = new ParticleTypeFlyweight(texture, mass, color);
        }

        return _particleTypes[key];
    }

    public static int GetParticleTypeCount() => _particleTypes.Count;
}

// Context for individual particles
public class Particle
{
    private readonly IParticleType _particleType;
    private float _x, _y;
    private float _velocityX, _velocityY;
    private float _life;

    public Particle(string texture, float mass, string color, 
                   float x, float y, float velocityX, float velocityY)
    {
        _particleType = ParticleTypeFactory.GetParticleType(texture, mass, color);
        _x = x;
        _y = y;
        _velocityX = velocityX;
        _velocityY = velocityY;
        _life = 1.0f;
    }

    public void Update(float deltaTime)
    {
        _x += _velocityX * deltaTime;
        _y += _velocityY * deltaTime;
        _life -= deltaTime;
        
        _particleType.Update(_x, _y, _velocityX, _velocityY, _life);
    }

    public void Render()
    {
        if (_life > 0)
        {
            _particleType.Render(_x, _y, _life);
        }
    }

    public bool IsAlive() => _life > 0;
}

// Particle system - manages many particles
public class ParticleSystem
{
    private readonly List<Particle> _particles = new();

    public void EmitParticle(string type, float mass, string color, 
                           float x, float y, float velocityX, float velocityY)
    {
        var particle = new Particle(type, mass, color, x, y, velocityX, velocityY);
        _particles.Add(particle);
    }

    public void Update(float deltaTime)
    {
        foreach (var particle in _particles)
        {
            particle.Update(deltaTime);
        }

        // Remove dead particles
        _particles.RemoveAll(p => !p.IsAlive());
    }

    public void Render()
    {
        Console.WriteLine("\n=== Particle System Render ===");
        foreach (var particle in _particles)
        {
            particle.Render();
        }
        Console.WriteLine($"Active particles: {_particles.Count}");
        Console.WriteLine($"Particle type flyweights: {ParticleTypeFactory.GetParticleTypeCount()}");
    }
}

// Usage - creating particle effects
var particleSystem = new ParticleSystem();

// Emit fire particles (many particles, few types)
for (int i = 0; i < 20; i++)
{
    particleSystem.EmitParticle("flame", 0.1f, "red", 100, 100, 
                               (float)(Random.Shared.NextDouble() - 0.5) * 10,
                               (float)(Random.Shared.NextDouble() - 0.5) * 10);
}

// Emit smoke particles
for (int i = 0; i < 15; i++)
{
    particleSystem.EmitParticle("smoke", 0.05f, "gray", 105, 95,
                               (float)(Random.Shared.NextDouble() - 0.5) * 5,
                               (float)(Random.Shared.NextDouble() - 0.5) * 5);
}

// Emit spark particles
for (int i = 0; i < 10; i++)
{
    particleSystem.EmitParticle("spark", 0.2f, "yellow", 98, 102,
                               (float)(Random.Shared.NextDouble() - 0.5) * 15,
                               (float)(Random.Shared.NextDouble() - 0.5) * 15);
}

particleSystem.Update(0.016f); // 60 FPS
particleSystem.Render();
```

## Benefits
- **Memory Efficiency**: Dramatically reduces memory usage for large numbers of similar objects
- **Performance**: Fewer objects means less garbage collection pressure
- **Centralized Management**: Shared state is managed in one place
- **Consistency**: Changes to intrinsic state affect all instances uniformly

## Drawbacks
- **Complexity**: Separating intrinsic and extrinsic state can be complex
- **Context Management**: Client must manage extrinsic state
- **Method Calls**: May introduce overhead if extrinsic state is large
- **Design Constraints**: Not all objects are suitable for flyweight treatment

## When to Use
✅ **Use When:**
- You need to create a large number of similar objects
- Storage costs are high due to object quantity
- Objects contain intrinsic state that can be shared
- Extrinsic state can be computed or stored separately
- Object groups can be replaced by few shared objects

❌ **Avoid When:**
- Objects don't share common state
- The cost of determining shared state is higher than storage savings
- You have few objects
- All object state is extrinsic

## Flyweight vs Other Patterns

| Pattern | Purpose | Key Difference |
|---------|---------|----------------|
| **Flyweight** | Shares intrinsic state to save memory | Focus on memory efficiency |
| **Singleton** | Ensures single instance | Focus on single instance |
| **Prototype** | Creates objects by cloning | Focus on object creation |
| **Factory** | Creates objects | Focus on creation logic |

## Best Practices
1. **Clear State Separation**: Clearly identify intrinsic vs extrinsic state
2. **Immutable Flyweights**: Make flyweights immutable when possible
3. **Factory Management**: Use factory to ensure sharing
4. **Context Efficiency**: Keep extrinsic state lightweight
5. **Memory Profiling**: Measure actual memory savings
6. **Documentation**: Document what state is intrinsic vs extrinsic

## Common Mistakes
1. **Mutable Flyweights**: Making flyweight state changeable breaks sharing
2. **Wrong State Classification**: Putting extrinsic state in flyweight
3. **Premature Optimization**: Using flyweight when memory isn't an issue
4. **Complex Context**: Making extrinsic state too complex

## Modern C# Features
```csharp
// Using concurrent collections for thread safety
public class ThreadSafeFlyweightFactory
{
    private static readonly ConcurrentDictionary<string, IFlyweight> _flyweights = new();

    public static IFlyweight GetFlyweight(string key)
    {
        return _flyweights.GetOrAdd(key, k => new ConcreteFlyweight(k));
    }
}

// Using records for immutable flyweights
public record FontFlyweight(string Name, int Size, string Style) : IFontFlyweight
{
    public void RenderText(string text, int x, int y)
    {
        Console.WriteLine($"Rendering '{text}' at ({x}, {y}) with {Name} {Size}pt {Style}");
    }
}

// Using weak references for optional caching
public class WeakFlyweightFactory
{
    private static readonly Dictionary<string, WeakReference> _flyweights = new();

    public static IFlyweight GetFlyweight(string key)
    {
        if (_flyweights.TryGetValue(key, out var weakRef) && 
            weakRef.Target is IFlyweight existing)
        {
            return existing;
        }

        var flyweight = new ConcreteFlyweight(key);
        _flyweights[key] = new WeakReference(flyweight);
        return flyweight;
    }
}
```

## Testing Flyweights
```csharp
[Test]
public void FlyweightFactory_GetSameFlyweight_ReturnsSameInstance()
{
    // Arrange & Act
    var flyweight1 = CharacterFlyweightFactory.GetCharacterFlyweight('A', "Arial", 12, "Black");
    var flyweight2 = CharacterFlyweightFactory.GetCharacterFlyweight('A', "Arial", 12, "Black");

    // Assert
    Assert.AreSame(flyweight1, flyweight2);
}

[Test]
public void FlyweightFactory_DifferentParameters_ReturnsDifferentInstances()
{
    // Arrange & Act
    var flyweight1 = CharacterFlyweightFactory.GetCharacterFlyweight('A', "Arial", 12, "Black");
    var flyweight2 = CharacterFlyweightFactory.GetCharacterFlyweight('B', "Arial", 12, "Black");

    // Assert
    Assert.AreNotSame(flyweight1, flyweight2);
}
```

## Memory Analysis Example
```csharp
public class MemoryAnalysis
{
    public static void CompareMemoryUsage()
    {
        // Without Flyweight: 10,000 character objects
        var withoutFlyweight = new List<FullCharacter>();
        for (int i = 0; i < 10000; i++)
        {
            withoutFlyweight.Add(new FullCharacter('A', "Arial", 12, "Black", i, 100));
        }

        // With Flyweight: 1 flyweight + 10,000 contexts
        var withFlyweight = new List<CharacterContext>();
        for (int i = 0; i < 10000; i++)
        {
            withFlyweight.Add(new CharacterContext('A', "Arial", 12, "Black", i, 100, "text"));
        }

        Console.WriteLine($"Without Flyweight: {withoutFlyweight.Count} full objects");
        Console.WriteLine($"With Flyweight: {withFlyweight.Count} contexts + {CharacterFlyweightFactory.GetFlyweightCount()} flyweights");
        
        // In real scenarios, measure actual memory using profilers
    }
}
```

## Summary
The Flyweight pattern is like having a wardrobe where you own one shirt design but can wear it at different times and places. Instead of buying 100 identical shirts, you have one shirt (flyweight) and 100 occasions to wear it (contexts).

It's perfect for scenarios where you have thousands of similar objects but most of their data is the same. Think of it as the "share the template, customize the details" pattern - you share the expensive, common parts and only store the unique, lightweight parts separately.

The key insight is that memory efficiency often comes from recognizing what can be shared versus what must be unique, then designing your system to take advantage of that distinction.
