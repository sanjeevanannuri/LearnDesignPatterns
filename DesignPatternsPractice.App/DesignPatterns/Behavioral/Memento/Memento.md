# Memento Pattern

## Overview
The Memento pattern captures and stores an object's internal state so it can be restored later without violating encapsulation. Think of it like a **save game feature** in video games - you can save your progress at any point and restore to that exact state later, or like **Ctrl+Z (undo)** in text editors that remembers previous versions of your document.

## Problem It Solves
Imagine you're building a text editor with undo/redo functionality:
- You need to save the state of the document at various points
- Users should be able to undo changes and return to previous states
- You can't expose the internal structure of your objects
- You need to manage potentially many saved states efficiently

Without Memento pattern, you might violate encapsulation:
```csharp
// BAD: Exposing internal state
public class TextEditor
{
    public string Content { get; set; } // Direct access violates encapsulation
    public int CursorPosition { get; set; } // Anyone can modify this
    
    // Client code must know internal structure
    public void SaveState() { /* How to save without exposing internals? */ }
}
```

This approach breaks encapsulation and makes the code fragile to changes.

## Real-World Analogy
Think of **photo editing software**:
1. **Original Photo** (Originator): The image you're editing
2. **History Panel** (Caretaker): Shows list of edit steps you can return to
3. **Saved States** (Mementos): Each step in your editing history
4. **Restore**: Click any step to return the photo to that exact state
5. **Privacy**: The saved states only contain what's needed to restore, not internal implementation

Or consider **database transactions**:
- **Database** (Originator): Current state of data
- **Transaction Log** (Caretaker): Manages rollback points
- **Savepoints** (Mementos): Specific states you can rollback to
- **Rollback**: Restore database to a previous consistent state

## Implementation Details

### Basic Structure
```csharp
// Memento - stores state of Originator
public class Memento
{
    private readonly string _state;
    private readonly DateTime _timestamp;

    public Memento(string state)
    {
        _state = state;
        _timestamp = DateTime.Now;
    }

    public string GetState() => _state;
    public DateTime GetTimestamp() => _timestamp;
}

// Originator - creates and uses mementos
public class Originator
{
    private string _state;

    public void SetState(string state)
    {
        Console.WriteLine($"Originator: Setting state to {state}");
        _state = state;
    }

    public Memento SaveStateToMemento()
    {
        Console.WriteLine($"Originator: Saving state to Memento");
        return new Memento(_state);
    }

    public void RestoreStateFromMemento(Memento memento)
    {
        _state = memento.GetState();
        Console.WriteLine($"Originator: State restored from Memento: {_state}");
    }

    public void ShowState()
    {
        Console.WriteLine($"Originator: Current state is {_state}");
    }
}

// Caretaker - manages mementos but doesn't operate on them
public class Caretaker
{
    private readonly List<Memento> _mementos = new();

    public void AddMemento(Memento memento)
    {
        _mementos.Add(memento);
    }

    public Memento GetMemento(int index)
    {
        return _mementos[index];
    }

    public int Count => _mementos.Count;
}
```

### Key Components
1. **Memento**: Stores the internal state of Originator
2. **Originator**: Creates mementos and restores its state from them
3. **Caretaker**: Manages mementos but doesn't examine their contents

## Example from Our Code
```csharp
// Document memento - stores complete document state
public class DocumentMemento
{
    public string Content { get; private set; }
    public int CursorPosition { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string Description { get; private set; }

    public DocumentMemento(string content, int cursorPosition, string description)
    {
        Content = content;
        CursorPosition = cursorPosition;
        Timestamp = DateTime.Now;
        Description = description;
    }

    public override string ToString()
    {
        var preview = Content.Length > 30 ? Content.Substring(0, 30) + "..." : Content;
        return $"[{Timestamp:HH:mm:ss}] {Description}: \"{preview}\" (cursor: {CursorPosition})";
    }
}

// Text document - the originator
public class TextDocument
{
    private string _content;
    private int _cursorPosition;
    private string _filename;

    public TextDocument(string filename)
    {
        _filename = filename;
        _content = "";
        _cursorPosition = 0;
    }

    public string Content => _content;
    public int CursorPosition => _cursorPosition;
    public string Filename => _filename;

    public void InsertText(string text)
    {
        _content = _content.Insert(_cursorPosition, text);
        _cursorPosition += text.Length;
        Console.WriteLine($"📝 Inserted '{text}' at position {_cursorPosition - text.Length}");
        Console.WriteLine($"   Content: \"{_content}\"");
    }

    public void DeleteText(int length)
    {
        if (_cursorPosition >= length)
        {
            var deletedText = _content.Substring(_cursorPosition - length, length);
            _content = _content.Remove(_cursorPosition - length, length);
            _cursorPosition -= length;
            Console.WriteLine($"🗑️ Deleted '{deletedText}'");
            Console.WriteLine($"   Content: \"{_content}\"");
        }
    }

    public void MoveCursor(int position)
    {
        if (position >= 0 && position <= _content.Length)
        {
            _cursorPosition = position;
            Console.WriteLine($"↔️ Moved cursor to position {_cursorPosition}");
        }
    }

    public void ReplaceText(int start, int length, string newText)
    {
        if (start >= 0 && start + length <= _content.Length)
        {
            var oldText = _content.Substring(start, length);
            _content = _content.Remove(start, length).Insert(start, newText);
            _cursorPosition = start + newText.Length;
            Console.WriteLine($"🔄 Replaced '{oldText}' with '{newText}'");
            Console.WriteLine($"   Content: \"{_content}\"");
        }
    }

    // Create memento with current state
    public DocumentMemento CreateMemento(string description)
    {
        Console.WriteLine($"💾 Creating memento: {description}");
        return new DocumentMemento(_content, _cursorPosition, description);
    }

    // Restore state from memento
    public void RestoreFromMemento(DocumentMemento memento)
    {
        _content = memento.Content;
        _cursorPosition = memento.CursorPosition;
        Console.WriteLine($"⏪ Restored from memento: {memento.Description}");
        Console.WriteLine($"   Content: \"{_content}\"");
        Console.WriteLine($"   Cursor position: {_cursorPosition}");
    }

    public void ShowStatus()
    {
        Console.WriteLine($"\n📄 Document Status:");
        Console.WriteLine($"   Filename: {_filename}");
        Console.WriteLine($"   Content: \"{_content}\"");
        Console.WriteLine($"   Length: {_content.Length} characters");
        Console.WriteLine($"   Cursor: {_cursorPosition}");
    }
}

// Document history manager - the caretaker
public class DocumentHistory
{
    private List<DocumentMemento> _history;
    private int _currentIndex;
    private int _maxHistorySize;

    public DocumentHistory(int maxHistorySize = 20)
    {
        _history = new List<DocumentMemento>();
        _currentIndex = -1;
        _maxHistorySize = maxHistorySize;
    }

    public void SaveState(DocumentMemento memento)
    {
        // Remove any redo history when saving a new state
        if (_currentIndex < _history.Count - 1)
        {
            _history.RemoveRange(_currentIndex + 1, _history.Count - _currentIndex - 1);
        }

        _history.Add(memento);
        _currentIndex++;

        // Maintain history size limit
        if (_history.Count > _maxHistorySize)
        {
            _history.RemoveAt(0);
            _currentIndex--;
        }

        Console.WriteLine($"💾 State saved. History size: {_history.Count}, Current: {_currentIndex + 1}");
    }

    public DocumentMemento Undo()
    {
        if (CanUndo())
        {
            var memento = _history[_currentIndex - 1];
            _currentIndex--;
            Console.WriteLine($"⏪ Undo to: {memento.Description}");
            return memento;
        }
        
        Console.WriteLine("❌ Nothing to undo");
        return null;
    }

    public DocumentMemento Redo()
    {
        if (CanRedo())
        {
            _currentIndex++;
            var memento = _history[_currentIndex];
            Console.WriteLine($"⏩ Redo to: {memento.Description}");
            return memento;
        }
        
        Console.WriteLine("❌ Nothing to redo");
        return null;
    }

    public bool CanUndo() => _currentIndex > 0;
    public bool CanRedo() => _currentIndex < _history.Count - 1;

    public DocumentMemento GetState(int index)
    {
        if (index >= 0 && index < _history.Count)
        {
            var memento = _history[index];
            _currentIndex = index;
            Console.WriteLine($"⏯️ Jumped to state: {memento.Description}");
            return memento;
        }
        return null;
    }

    public void ShowHistory()
    {
        Console.WriteLine($"\n📚 Document History ({_history.Count} states):");
        for (int i = 0; i < _history.Count; i++)
        {
            var marker = i == _currentIndex ? "➤ " : "  ";
            Console.WriteLine($"{marker}{i + 1}. {_history[i]}");
        }
        
        Console.WriteLine($"\nUndo available: {CanUndo()}");
        Console.WriteLine($"Redo available: {CanRedo()}");
    }

    public void ClearHistory()
    {
        _history.Clear();
        _currentIndex = -1;
        Console.WriteLine("🗑️ History cleared");
    }

    public int Count => _history.Count;
    public int CurrentIndex => _currentIndex;
}

// Text editor with undo/redo functionality
public class TextEditor
{
    private TextDocument _document;
    private DocumentHistory _history;

    public TextEditor(string filename)
    {
        _document = new TextDocument(filename);
        _history = new DocumentHistory();
        
        // Save initial state
        SaveState("Initial document");
    }

    public void Type(string text)
    {
        _document.InsertText(text);
        SaveState($"Typed '{text}'");
    }

    public void Backspace(int characters = 1)
    {
        _document.DeleteText(characters);
        SaveState($"Deleted {characters} character(s)");
    }

    public void Replace(int start, int length, string newText)
    {
        _document.ReplaceText(start, length, newText);
        SaveState($"Replaced text with '{newText}'");
    }

    public void MoveCursor(int position)
    {
        _document.MoveCursor(position);
        // Note: Cursor movements typically don't create new history states
        // unless they're part of a larger operation
    }

    public void Undo()
    {
        var memento = _history.Undo();
        if (memento != null)
        {
            _document.RestoreFromMemento(memento);
        }
    }

    public void Redo()
    {
        var memento = _history.Redo();
        if (memento != null)
        {
            _document.RestoreFromMemento(memento);
        }
    }

    public void GoToState(int stateIndex)
    {
        var memento = _history.GetState(stateIndex);
        if (memento != null)
        {
            _document.RestoreFromMemento(memento);
        }
    }

    public void SaveState(string description)
    {
        var memento = _document.CreateMemento(description);
        _history.SaveState(memento);
    }

    public void ShowDocument()
    {
        _document.ShowStatus();
    }

    public void ShowHistory()
    {
        _history.ShowHistory();
    }

    public void ClearHistory()
    {
        _history.ClearHistory();
        SaveState("History cleared");
    }
}

// Usage - demonstrating memento pattern
var editor = new TextEditor("MyDocument.txt");

Console.WriteLine("=== Text Editor with Memento Pattern ===");

editor.ShowDocument();

// Perform various editing operations
editor.Type("Hello ");
editor.Type("World!");
editor.Type(" This is a test.");

editor.ShowHistory();

// Try undo operations
editor.Undo();
editor.Undo();

editor.ShowDocument();

// Try redo
editor.Redo();

// More editing
editor.Type(" How are you?");
editor.Replace(6, 5, "Universe");

editor.ShowHistory();

// Jump to specific state
Console.WriteLine("\n--- Jumping to state 3 ---");
editor.GoToState(2); // Zero-based index

// Continue editing from that point
editor.Type(" Nice to meet you!");

editor.ShowHistory();
editor.ShowDocument();

// Test undo/redo after jumping
editor.Undo();
editor.Undo();
editor.Redo();

Console.WriteLine("\n=== Final State ===");
editor.ShowDocument();
editor.ShowHistory();
```

## Real-World Examples

### 1. **Game Save System**
```csharp
// Game state memento
public class GameStateMemento
{
    public int Level { get; private set; }
    public int Score { get; private set; }
    public int Lives { get; private set; }
    public Vector3 PlayerPosition { get; private set; }
    public Dictionary<string, object> Inventory { get; private set; }
    public DateTime SaveTime { get; private set; }
    public string SaveName { get; private set; }

    public GameStateMemento(int level, int score, int lives, Vector3 position, 
                           Dictionary<string, object> inventory, string saveName)
    {
        Level = level;
        Score = score;
        Lives = lives;
        PlayerPosition = position;
        Inventory = new Dictionary<string, object>(inventory);
        SaveTime = DateTime.Now;
        SaveName = saveName;
    }

    public override string ToString()
    {
        return $"{SaveName} - Level {Level}, Score {Score:N0}, Lives {Lives} [{SaveTime:MM/dd HH:mm}]";
    }
}

// Player position helper
public struct Vector3
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public Vector3(float x, float y, float z)
    {
        X = x; Y = y; Z = z;
    }

    public override string ToString() => $"({X:F1}, {Y:F1}, {Z:F1})";
}

// Game state - the originator
public class GameState
{
    private int _level;
    private int _score;
    private int _lives;
    private Vector3 _playerPosition;
    private Dictionary<string, object> _inventory;
    private bool _isGameOver;

    public GameState()
    {
        _level = 1;
        _score = 0;
        _lives = 3;
        _playerPosition = new Vector3(0, 0, 0);
        _inventory = new Dictionary<string, object>();
        _isGameOver = false;
    }

    // Game properties
    public int Level => _level;
    public int Score => _score;
    public int Lives => _lives;
    public Vector3 PlayerPosition => _playerPosition;
    public bool IsGameOver => _isGameOver;

    // Game actions
    public void MovePlayer(float x, float y, float z)
    {
        _playerPosition = new Vector3(x, y, z);
        Console.WriteLine($"🎮 Player moved to {_playerPosition}");
    }

    public void AddScore(int points)
    {
        _score += points;
        Console.WriteLine($"⭐ Score increased by {points}. Total: {_score:N0}");
        
        // Level up every 1000 points
        if (_score / 1000 > _level - 1)
        {
            LevelUp();
        }
    }

    public void LoseLife()
    {
        if (_lives > 0)
        {
            _lives--;
            Console.WriteLine($"💔 Life lost! Lives remaining: {_lives}");
            
            if (_lives == 0)
            {
                _isGameOver = true;
                Console.WriteLine("💀 Game Over!");
            }
        }
    }

    public void GainLife()
    {
        _lives++;
        Console.WriteLine($"❤️ Extra life gained! Lives: {_lives}");
    }

    public void LevelUp()
    {
        _level++;
        Console.WriteLine($"🎉 Level up! Now on level {_level}");
        GainLife(); // Bonus life for reaching new level
    }

    public void AddInventoryItem(string itemName, object item)
    {
        _inventory[itemName] = item;
        Console.WriteLine($"🎒 Added to inventory: {itemName}");
    }

    public void UseInventoryItem(string itemName)
    {
        if (_inventory.Remove(itemName))
        {
            Console.WriteLine($"🎒 Used item: {itemName}");
        }
        else
        {
            Console.WriteLine($"❌ Item not found: {itemName}");
        }
    }

    // Memento operations
    public GameStateMemento CreateSaveGame(string saveName)
    {
        Console.WriteLine($"💾 Creating save game: {saveName}");
        return new GameStateMemento(_level, _score, _lives, _playerPosition, _inventory, saveName);
    }

    public void LoadSaveGame(GameStateMemento memento)
    {
        _level = memento.Level;
        _score = memento.Score;
        _lives = memento.Lives;
        _playerPosition = memento.PlayerPosition;
        _inventory = new Dictionary<string, object>(memento.Inventory);
        _isGameOver = false;
        
        Console.WriteLine($"📁 Loaded save game: {memento.SaveName}");
        ShowGameState();
    }

    public void ShowGameState()
    {
        Console.WriteLine($"\n🎮 Current Game State:");
        Console.WriteLine($"   Level: {_level}");
        Console.WriteLine($"   Score: {_score:N0}");
        Console.WriteLine($"   Lives: {_lives}");
        Console.WriteLine($"   Position: {_playerPosition}");
        Console.WriteLine($"   Inventory: {_inventory.Count} items");
        Console.WriteLine($"   Game Over: {_isGameOver}");
        
        if (_inventory.Count > 0)
        {
            Console.WriteLine("   Items:");
            foreach (var item in _inventory)
            {
                Console.WriteLine($"     - {item.Key}: {item.Value}");
            }
        }
    }
}

// Save game manager - the caretaker
public class SaveGameManager
{
    private Dictionary<string, GameStateMemento> _saveSlots;
    private List<GameStateMemento> _autoSaves;
    private int _maxAutoSaves;

    public SaveGameManager(int maxAutoSaves = 5)
    {
        _saveSlots = new Dictionary<string, GameStateMemento>();
        _autoSaves = new List<GameStateMemento>();
        _maxAutoSaves = maxAutoSaves;
    }

    public void SaveGame(GameStateMemento memento, string slotName)
    {
        _saveSlots[slotName] = memento;
        Console.WriteLine($"💾 Game saved to slot: {slotName}");
    }

    public GameStateMemento LoadGame(string slotName)
    {
        if (_saveSlots.TryGetValue(slotName, out var memento))
        {
            Console.WriteLine($"📁 Loading game from slot: {slotName}");
            return memento;
        }
        
        Console.WriteLine($"❌ Save slot '{slotName}' not found");
        return null;
    }

    public void AutoSave(GameStateMemento memento)
    {
        _autoSaves.Add(memento);
        
        // Maintain auto-save limit
        if (_autoSaves.Count > _maxAutoSaves)
        {
            _autoSaves.RemoveAt(0);
        }
        
        Console.WriteLine($"🔄 Auto-saved. Auto-saves: {_autoSaves.Count}/{_maxAutoSaves}");
    }

    public GameStateMemento GetAutoSave(int index)
    {
        if (index >= 0 && index < _autoSaves.Count)
        {
            var autoSave = _autoSaves[_autoSaves.Count - 1 - index]; // Most recent first
            Console.WriteLine($"📁 Loading auto-save {index + 1}: {autoSave.SaveName}");
            return autoSave;
        }
        
        Console.WriteLine($"❌ Auto-save {index + 1} not found");
        return null;
    }

    public void DeleteSave(string slotName)
    {
        if (_saveSlots.Remove(slotName))
        {
            Console.WriteLine($"🗑️ Deleted save: {slotName}");
        }
        else
        {
            Console.WriteLine($"❌ Save slot '{slotName}' not found");
        }
    }

    public void ShowSaveSlots()
    {
        Console.WriteLine($"\n💾 Save Slots ({_saveSlots.Count}):");
        if (_saveSlots.Count == 0)
        {
            Console.WriteLine("   No saved games");
        }
        else
        {
            foreach (var slot in _saveSlots.OrderBy(s => s.Key))
            {
                Console.WriteLine($"   {slot.Key}: {slot.Value}");
            }
        }
    }

    public void ShowAutoSaves()
    {
        Console.WriteLine($"\n🔄 Auto-Saves ({_autoSaves.Count}/{_maxAutoSaves}):");
        if (_autoSaves.Count == 0)
        {
            Console.WriteLine("   No auto-saves");
        }
        else
        {
            for (int i = 0; i < _autoSaves.Count; i++)
            {
                var autoSave = _autoSaves[_autoSaves.Count - 1 - i];
                Console.WriteLine($"   {i + 1}. {autoSave}");
            }
        }
    }
}

// Game with save/load functionality
public class Game
{
    private GameState _gameState;
    private SaveGameManager _saveManager;
    private int _autoSaveCounter;

    public Game()
    {
        _gameState = new GameState();
        _saveManager = new SaveGameManager();
        _autoSaveCounter = 0;
    }

    public void PlayGame()
    {
        Console.WriteLine("🎮 Starting new game...");
        _gameState.ShowGameState();
        
        // Auto-save at start
        AutoSave("Game Start");
    }

    public void SimulateGameplay()
    {
        Console.WriteLine("\n--- Simulating gameplay ---");
        
        // Player actions
        _gameState.MovePlayer(10, 5, 0);
        _gameState.AddInventoryItem("Health Potion", "Restores 50 HP");
        _gameState.AddScore(150);
        
        AutoSave("After collecting items");
        
        _gameState.MovePlayer(25, 10, 5);
        _gameState.AddScore(300);
        _gameState.AddInventoryItem("Magic Sword", "Damage +20");
        
        AutoSave("Found magic sword");
        
        _gameState.AddScore(600); // This should trigger level up
        _gameState.MovePlayer(50, 20, 10);
        
        AutoSave("Level 2 reached");
        
        // Simulate taking damage
        _gameState.LoseLife();
        _gameState.UseInventoryItem("Health Potion");
        
        AutoSave("After combat");
    }

    public void SaveToSlot(string slotName)
    {
        var saveGame = _gameState.CreateSaveGame(slotName);
        _saveManager.SaveGame(saveGame, slotName);
    }

    public void LoadFromSlot(string slotName)
    {
        var saveGame = _saveManager.LoadGame(slotName);
        if (saveGame != null)
        {
            _gameState.LoadSaveGame(saveGame);
        }
    }

    public void LoadAutoSave(int autoSaveIndex)
    {
        var autoSave = _saveManager.GetAutoSave(autoSaveIndex);
        if (autoSave != null)
        {
            _gameState.LoadSaveGame(autoSave);
        }
    }

    private void AutoSave(string description)
    {
        _autoSaveCounter++;
        var autoSave = _gameState.CreateSaveGame($"Auto-save {_autoSaveCounter}: {description}");
        _saveManager.AutoSave(autoSave);
    }

    public void ShowGameState() => _gameState.ShowGameState();
    public void ShowSaves() => _saveManager.ShowSaveSlots();
    public void ShowAutoSaves() => _saveManager.ShowAutoSaves();
}

// Usage
var game = new Game();

Console.WriteLine("\n=== Game Save System ===");

game.PlayGame();
game.SimulateGameplay();

// Manual save
game.SaveToSlot("QuickSave");
game.SaveToSlot("BeforeBoss");

// Continue playing
game.SimulateGameplay();

// Show all saves
game.ShowSaves();
game.ShowAutoSaves();

// Load previous save
Console.WriteLine("\n--- Loading previous save ---");
game.LoadFromSlot("BeforeBoss");

// Load auto-save
Console.WriteLine("\n--- Loading auto-save ---");
game.LoadAutoSave(2); // Load 3rd most recent auto-save

game.ShowGameState();
```

### 2. **Drawing Application with Undo/Redo**
```csharp
// Drawing shape
public class Shape
{
    public string Type { get; set; }
    public Point Position { get; set; }
    public Size Size { get; set; }
    public string Color { get; set; }
    public int Layer { get; set; }

    public Shape(string type, Point position, Size size, string color, int layer = 0)
    {
        Type = type;
        Position = position;
        Size = size;
        Color = color;
        Layer = layer;
    }

    public override string ToString()
    {
        return $"{Color} {Type} at {Position} size {Size} (layer {Layer})";
    }
}

public struct Point
{
    public int X { get; set; }
    public int Y { get; set; }
    
    public Point(int x, int y) { X = x; Y = y; }
    public override string ToString() => $"({X}, {Y})";
}

public struct Size
{
    public int Width { get; set; }
    public int Height { get; set; }
    
    public Size(int width, int height) { Width = width; Height = height; }
    public override string ToString() => $"{Width}x{Height}";
}

// Canvas memento
public class CanvasMemento
{
    public List<Shape> Shapes { get; private set; }
    public string BackgroundColor { get; private set; }
    public Size CanvasSize { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string OperationDescription { get; private set; }

    public CanvasMemento(List<Shape> shapes, string backgroundColor, Size canvasSize, string description)
    {
        Shapes = shapes.Select(s => new Shape(s.Type, s.Position, s.Size, s.Color, s.Layer)).ToList();
        BackgroundColor = backgroundColor;
        CanvasSize = canvasSize;
        OperationDescription = description;
        Timestamp = DateTime.Now;
    }

    public override string ToString()
    {
        return $"[{Timestamp:HH:mm:ss}] {OperationDescription} ({Shapes.Count} shapes)";
    }
}

// Drawing canvas - the originator
public class DrawingCanvas
{
    private List<Shape> _shapes;
    private string _backgroundColor;
    private Size _canvasSize;
    private int _nextLayer;

    public DrawingCanvas(Size canvasSize, string backgroundColor = "White")
    {
        _shapes = new List<Shape>();
        _backgroundColor = backgroundColor;
        _canvasSize = canvasSize;
        _nextLayer = 0;
    }

    public IReadOnlyList<Shape> Shapes => _shapes.AsReadOnly();
    public string BackgroundColor => _backgroundColor;
    public Size CanvasSize => _canvasSize;

    public void AddShape(Shape shape)
    {
        shape.Layer = _nextLayer++;
        _shapes.Add(shape);
        Console.WriteLine($"➕ Added shape: {shape}");
    }

    public void RemoveShape(int index)
    {
        if (index >= 0 && index < _shapes.Count)
        {
            var shape = _shapes[index];
            _shapes.RemoveAt(index);
            Console.WriteLine($"➖ Removed shape: {shape}");
        }
    }

    public void MoveShape(int index, Point newPosition)
    {
        if (index >= 0 && index < _shapes.Count)
        {
            var oldPosition = _shapes[index].Position;
            _shapes[index].Position = newPosition;
            Console.WriteLine($"↔️ Moved shape from {oldPosition} to {newPosition}");
        }
    }

    public void ResizeShape(int index, Size newSize)
    {
        if (index >= 0 && index < _shapes.Count)
        {
            var oldSize = _shapes[index].Size;
            _shapes[index].Size = newSize;
            Console.WriteLine($"📏 Resized shape from {oldSize} to {newSize}");
        }
    }

    public void ChangeShapeColor(int index, string newColor)
    {
        if (index >= 0 && index < _shapes.Count)
        {
            var oldColor = _shapes[index].Color;
            _shapes[index].Color = newColor;
            Console.WriteLine($"🎨 Changed shape color from {oldColor} to {newColor}");
        }
    }

    public void SetBackgroundColor(string color)
    {
        var oldColor = _backgroundColor;
        _backgroundColor = color;
        Console.WriteLine($"🎨 Changed background from {oldColor} to {color}");
    }

    public void Clear()
    {
        var count = _shapes.Count;
        _shapes.Clear();
        _nextLayer = 0;
        Console.WriteLine($"🗑️ Cleared canvas ({count} shapes removed)");
    }

    // Memento operations
    public CanvasMemento CreateMemento(string description)
    {
        return new CanvasMemento(_shapes, _backgroundColor, _canvasSize, description);
    }

    public void RestoreFromMemento(CanvasMemento memento)
    {
        _shapes = memento.Shapes.ToList();
        _backgroundColor = memento.BackgroundColor;
        _canvasSize = memento.CanvasSize;
        _nextLayer = _shapes.Count > 0 ? _shapes.Max(s => s.Layer) + 1 : 0;
        
        Console.WriteLine($"⏪ Restored canvas: {memento.OperationDescription}");
    }

    public void ShowCanvas()
    {
        Console.WriteLine($"\n🎨 Canvas ({_canvasSize.Width}x{_canvasSize.Height}, {_backgroundColor} background):");
        if (_shapes.Count == 0)
        {
            Console.WriteLine("   Empty canvas");
        }
        else
        {
            var sortedShapes = _shapes.OrderBy(s => s.Layer);
            foreach (var shape in sortedShapes)
            {
                Console.WriteLine($"   {shape}");
            }
        }
    }
}

// Drawing history manager
public class DrawingHistory
{
    private List<CanvasMemento> _history;
    private int _currentIndex;
    private int _maxHistorySize;

    public DrawingHistory(int maxHistorySize = 50)
    {
        _history = new List<CanvasMemento>();
        _currentIndex = -1;
        _maxHistorySize = maxHistorySize;
    }

    public void SaveState(CanvasMemento memento)
    {
        // Remove redo history when saving new state
        if (_currentIndex < _history.Count - 1)
        {
            _history.RemoveRange(_currentIndex + 1, _history.Count - _currentIndex - 1);
        }

        _history.Add(memento);
        _currentIndex++;

        // Maintain size limit
        if (_history.Count > _maxHistorySize)
        {
            _history.RemoveAt(0);
            _currentIndex--;
        }

        Console.WriteLine($"💾 State saved: {memento.OperationDescription}");
    }

    public CanvasMemento Undo()
    {
        if (CanUndo())
        {
            var memento = _history[_currentIndex - 1];
            _currentIndex--;
            Console.WriteLine($"⏪ Undo: {memento.OperationDescription}");
            return memento;
        }
        
        Console.WriteLine("❌ Nothing to undo");
        return null;
    }

    public CanvasMemento Redo()
    {
        if (CanRedo())
        {
            _currentIndex++;
            var memento = _history[_currentIndex];
            Console.WriteLine($"⏩ Redo: {memento.OperationDescription}");
            return memento;
        }
        
        Console.WriteLine("❌ Nothing to redo");
        return null;
    }

    public bool CanUndo() => _currentIndex > 0;
    public bool CanRedo() => _currentIndex < _history.Count - 1;

    public void ShowHistory(int maxItems = 10)
    {
        Console.WriteLine($"\n📚 Drawing History ({_history.Count} states):");
        
        var startIndex = Math.Max(0, _currentIndex - maxItems / 2);
        var endIndex = Math.Min(_history.Count - 1, startIndex + maxItems - 1);
        
        for (int i = startIndex; i <= endIndex; i++)
        {
            var marker = i == _currentIndex ? "➤ " : "  ";
            Console.WriteLine($"{marker}{i + 1}. {_history[i]}");
        }
        
        Console.WriteLine($"\nCan Undo: {CanUndo()}, Can Redo: {CanRedo()}");
    }
}

// Drawing application
public class DrawingApplication
{
    private DrawingCanvas _canvas;
    private DrawingHistory _history;

    public DrawingApplication(Size canvasSize)
    {
        _canvas = new DrawingCanvas(canvasSize);
        _history = new DrawingHistory();
        
        // Save initial state
        SaveState("New canvas created");
    }

    public void DrawRectangle(Point position, Size size, string color)
    {
        var rectangle = new Shape("Rectangle", position, size, color);
        _canvas.AddShape(rectangle);
        SaveState($"Drew {color} rectangle");
    }

    public void DrawCircle(Point center, int radius, string color)
    {
        var circle = new Shape("Circle", center, new Size(radius * 2, radius * 2), color);
        _canvas.AddShape(circle);
        SaveState($"Drew {color} circle");
    }

    public void DrawLine(Point start, Point end, string color)
    {
        var line = new Shape("Line", start, new Size(Math.Abs(end.X - start.X), Math.Abs(end.Y - start.Y)), color);
        _canvas.AddShape(line);
        SaveState($"Drew {color} line");
    }

    public void DeleteShape(int index)
    {
        if (index >= 0 && index < _canvas.Shapes.Count)
        {
            var shape = _canvas.Shapes[index];
            _canvas.RemoveShape(index);
            SaveState($"Deleted {shape.Type}");
        }
    }

    public void MoveShape(int index, Point newPosition)
    {
        if (index >= 0 && index < _canvas.Shapes.Count)
        {
            _canvas.MoveShape(index, newPosition);
            SaveState($"Moved {_canvas.Shapes[index].Type}");
        }
    }

    public void ChangeBackground(string color)
    {
        _canvas.SetBackgroundColor(color);
        SaveState($"Changed background to {color}");
    }

    public void ClearCanvas()
    {
        _canvas.Clear();
        SaveState("Cleared canvas");
    }

    public void Undo()
    {
        var memento = _history.Undo();
        if (memento != null)
        {
            _canvas.RestoreFromMemento(memento);
        }
    }

    public void Redo()
    {
        var memento = _history.Redo();
        if (memento != null)
        {
            _canvas.RestoreFromMemento(memento);
        }
    }

    private void SaveState(string description)
    {
        var memento = _canvas.CreateMemento(description);
        _history.SaveState(memento);
    }

    public void ShowCanvas() => _canvas.ShowCanvas();
    public void ShowHistory() => _history.ShowHistory();
}

// Usage
var drawingApp = new DrawingApplication(new Size(800, 600));

Console.WriteLine("\n=== Drawing Application with Undo/Redo ===");

drawingApp.ShowCanvas();

// Draw some shapes
drawingApp.DrawRectangle(new Point(10, 10), new Size(100, 50), "Red");
drawingApp.DrawCircle(new Point(200, 100), 30, "Blue");
drawingApp.DrawLine(new Point(0, 0), new Point(100, 100), "Black");

drawingApp.ShowCanvas();

// Make some changes
drawingApp.ChangeBackground("LightGray");
drawingApp.DrawRectangle(new Point(300, 200), new Size(80, 80), "Green");

drawingApp.ShowHistory();

// Test undo/redo
Console.WriteLine("\n--- Testing Undo/Redo ---");
drawingApp.Undo();
drawingApp.Undo();
drawingApp.ShowCanvas();

drawingApp.Redo();
drawingApp.ShowCanvas();

// More operations
drawingApp.MoveShape(0, new Point(50, 50));
drawingApp.DeleteShape(1);

drawingApp.ShowHistory();
drawingApp.ShowCanvas();
```

## Benefits
- **Encapsulation**: Internal state is hidden from external objects
- **Undo/Redo**: Easy implementation of undo and redo functionality
- **Snapshot**: Capture complete state at any point in time
- **Recovery**: Restore to previous known good states
- **Debugging**: Save states for debugging and testing

## Drawbacks
- **Memory Usage**: Storing many states can consume significant memory
- **Performance**: Creating and restoring mementos can be expensive
- **Complexity**: Managing many mementos requires careful caretaker design
- **Deep Copy Issues**: May need deep copying for complex object states

## When to Use
✅ **Use When:**
- You need undo/redo functionality
- You want to save checkpoints or snapshots
- You need to restore objects to previous states
- You want to implement save/load functionality
- You need to maintain state history for debugging

❌ **Avoid When:**
- Memory usage is critically constrained
- Object state changes are very frequent
- The cost of copying state is prohibitive
- Simple state management is sufficient

## Memento vs Other Patterns

| Pattern | Purpose | Key Difference |
|---------|---------|----------------|
| **Memento** | Saves and restores object state | Focuses on state preservation |
| **Command** | Encapsulates operations | Focuses on operation encapsulation |
| **Prototype** | Creates copies of objects | Focuses on object cloning |
| **Snapshot** | Captures current state | Similar but less formal structure |

## Best Practices
1. **Limit Memento Size**: Only store essential state information
2. **Manage Memory**: Implement size limits and cleanup old mementos
3. **Immutable Mementos**: Make memento objects immutable
4. **Efficient Copying**: Use efficient copying mechanisms when possible
5. **Clear Naming**: Use descriptive names for saved states

## Common Mistakes
1. **Storing Too Much**: Including unnecessary data in mementos
2. **Memory Leaks**: Not cleaning up old mementos
3. **Shallow Copying**: Not properly copying reference types
4. **Breaking Encapsulation**: Exposing internal state through mementos

## Modern C# Features
```csharp
// Using records for immutable mementos
public record DocumentState(string Content, int CursorPosition, DateTime Timestamp)
{
    public static DocumentState Create(string content, int cursor) =>
        new(content, cursor, DateTime.Now);
}

// Using with expressions for efficient copying
public class ModernDocument
{
    private DocumentState _state;

    public DocumentState CreateMemento() => _state;
    
    public void RestoreFromMemento(DocumentState memento) => _state = memento;
    
    public void UpdateContent(string newContent) =>
        _state = _state with { Content = newContent };
}

// Using JSON serialization for complex objects
public class SerializableMemento
{
    private readonly string _serializedState;

    public SerializableMemento(object state)
    {
        _serializedState = JsonSerializer.Serialize(state);
    }

    public T Restore<T>() =>
        JsonSerializer.Deserialize<T>(_serializedState);
}

// Using weak references for memory management
public class MemoryEfficientCaretaker
{
    private readonly List<WeakReference> _mementos = new();

    public void AddMemento(object memento)
    {
        _mementos.Add(new WeakReference(memento));
        CleanupDeadReferences();
    }

    private void CleanupDeadReferences()
    {
        _mementos.RemoveAll(wr => !wr.IsAlive);
    }
}
```

## Testing Mementos
```csharp
[Test]
public void Memento_RestoreState_RestoresCorrectValues()
{
    // Arrange
    var document = new TextDocument("test.txt");
    document.InsertText("Hello World");
    var memento = document.CreateMemento("Initial state");
    
    document.InsertText(" More text");
    
    // Act
    document.RestoreFromMemento(memento);
    
    // Assert
    Assert.AreEqual("Hello World", document.Content);
}

[Test]
public void History_Undo_RestoresPreviousState()
{
    // Arrange
    var history = new DocumentHistory();
    var memento1 = new DocumentMemento("State 1", 0, "First");
    var memento2 = new DocumentMemento("State 2", 0, "Second");
    
    history.SaveState(memento1);
    history.SaveState(memento2);
    
    // Act
    var restored = history.Undo();
    
    // Assert
    Assert.AreEqual("State 1", restored.Content);
    Assert.IsTrue(history.CanRedo());
}
```

## Summary
The Memento pattern is like having a time machine for your objects - it lets you capture complete snapshots of an object's state and restore to any previous point without breaking encapsulation. Just like saving your progress in a video game or using Ctrl+Z in a text editor, the Memento pattern provides a clean way to implement undo/redo functionality and state management.

The pattern is perfect when you need to provide users with the ability to revert changes, implement save/load functionality, or maintain state history for debugging purposes. It's especially valuable in applications like text editors, drawing programs, games, and any system where users expect to be able to undo their actions.

The key insight is that by separating state capture (Memento), state management (Caretaker), and the object itself (Originator), you can implement powerful state management features while keeping your object's internal structure completely private and protected.
