# Command Pattern

## Overview
The Command pattern encapsulates a request as an object, allowing you to parameterize clients with different requests, queue operations, log requests, and support undo operations. Think of it like a restaurant order - instead of the customer directly telling the chef what to cook, they write their order on a ticket (command object) that contains all the information needed to fulfill the request.

## Problem It Solves
Imagine you're building a text editor with various operations like cut, copy, paste, undo, and macros:
- Different UI elements (buttons, menu items, keyboard shortcuts) need to trigger the same operations
- You want to support undo/redo functionality
- You need to queue operations or execute them later
- You want to log all operations for auditing

Without Command pattern, you'd have tight coupling:
```csharp
// BAD: Direct coupling between UI and operations
public class TextEditor
{
    public void OnCutButtonClick() { Cut(); }
    public void OnCopyButtonClick() { Copy(); }
    public void OnPasteButtonClick() { Paste(); }
    
    // Hard to add undo, queuing, or logging
    private void Cut() { /* cut logic */ }
    private void Copy() { /* copy logic */ }
    private void Paste() { /* paste logic */ }
}
```

This makes it difficult to add features like undo, macro recording, or operation queuing.

## Real-World Analogy
Think of a **restaurant ordering system**:
1. **Customer** (Client): Decides what they want to eat
2. **Order Ticket** (Command): Contains all details - table number, items, special instructions
3. **Waiter** (Invoker): Takes the order and passes it to the kitchen
4. **Chef** (Receiver): Reads the ticket and prepares the food
5. **Order Queue**: Multiple orders can be queued and processed in order
6. **Order History**: All orders are logged for billing and inventory

Or consider a **TV Remote Control**:
- **Buttons** (Invokers): Power, Volume, Channel buttons
- **Commands**: Each button press creates a command (PowerOnCommand, VolumeUpCommand)
- **TV** (Receiver): The device that actually performs the operations
- **Programmable**: You can program buttons to execute different commands

## Implementation Details

### Basic Structure
```csharp
// Command interface
public interface ICommand
{
    void Execute();
    void Undo(); // Optional for reversible commands
}

// Receiver - knows how to perform the operations
public class Receiver
{
    public void Action()
    {
        Console.WriteLine("Receiver: Performing action");
    }
}

// Concrete command
public class ConcreteCommand : ICommand
{
    private Receiver _receiver;
    private string _state;

    public ConcreteCommand(Receiver receiver)
    {
        _receiver = receiver;
    }

    public void Execute()
    {
        Console.WriteLine("ConcreteCommand: Executing command");
        _receiver.Action();
    }

    public void Undo()
    {
        Console.WriteLine("ConcreteCommand: Undoing command");
        // Restore previous state
    }
}

// Invoker - asks command to carry out request
public class Invoker
{
    private ICommand _command;

    public void SetCommand(ICommand command)
    {
        _command = command;
    }

    public void ExecuteCommand()
    {
        _command?.Execute();
    }
}
```

### Key Components
1. **Command**: Interface for executing operations
2. **ConcreteCommand**: Implements Command and encapsulates receiver actions
3. **Receiver**: Knows how to perform the actual work
4. **Invoker**: Asks command to carry out the request
5. **Client**: Creates ConcreteCommand and sets its receiver

## Example from Our Code
```csharp
// Command interface
public interface ICommand
{
    void Execute();
    void Undo();
    string Description { get; }
}

// Receiver - Text Editor that performs actual operations
public class TextEditor
{
    private StringBuilder _content;
    private List<string> _clipboard;

    public TextEditor()
    {
        _content = new StringBuilder();
        _clipboard = new List<string>();
    }

    public string Content => _content.ToString();

    public void InsertText(string text, int position)
    {
        if (position >= 0 && position <= _content.Length)
        {
            _content.Insert(position, text);
            Console.WriteLine($"Inserted '{text}' at position {position}");
        }
    }

    public string DeleteText(int position, int length)
    {
        if (position >= 0 && position + length <= _content.Length)
        {
            var deletedText = _content.ToString(position, length);
            _content.Remove(position, length);
            Console.WriteLine($"Deleted '{deletedText}' from position {position}");
            return deletedText;
        }
        return string.Empty;
    }

    public void CopyToClipboard(string text)
    {
        _clipboard.Clear();
        _clipboard.Add(text);
        Console.WriteLine($"Copied '{text}' to clipboard");
    }

    public string GetClipboardContent()
    {
        return _clipboard.Count > 0 ? _clipboard[0] : string.Empty;
    }

    public void DisplayContent()
    {
        Console.WriteLine($"Editor Content: '{_content}'");
    }
}

// Concrete Commands
public class InsertTextCommand : ICommand
{
    private TextEditor _editor;
    private string _text;
    private int _position;

    public InsertTextCommand(TextEditor editor, string text, int position)
    {
        _editor = editor;
        _text = text;
        _position = position;
    }

    public string Description => $"Insert '{_text}' at position {_position}";

    public void Execute()
    {
        _editor.InsertText(_text, _position);
    }

    public void Undo()
    {
        _editor.DeleteText(_position, _text.Length);
    }
}

public class DeleteTextCommand : ICommand
{
    private TextEditor _editor;
    private int _position;
    private int _length;
    private string _deletedText;

    public DeleteTextCommand(TextEditor editor, int position, int length)
    {
        _editor = editor;
        _position = position;
        _length = length;
    }

    public string Description => $"Delete {_length} characters from position {_position}";

    public void Execute()
    {
        _deletedText = _editor.DeleteText(_position, _length);
    }

    public void Undo()
    {
        if (!string.IsNullOrEmpty(_deletedText))
        {
            _editor.InsertText(_deletedText, _position);
        }
    }
}

public class CopyCommand : ICommand
{
    private TextEditor _editor;
    private int _position;
    private int _length;

    public CopyCommand(TextEditor editor, int position, int length)
    {
        _editor = editor;
        _position = position;
        _length = length;
    }

    public string Description => $"Copy {_length} characters from position {_position}";

    public void Execute()
    {
        if (_position >= 0 && _position + _length <= _editor.Content.Length)
        {
            var textToCopy = _editor.Content.Substring(_position, _length);
            _editor.CopyToClipboard(textToCopy);
        }
    }

    public void Undo()
    {
        // Copy operation doesn't need undo
        Console.WriteLine("Copy operation cannot be undone");
    }
}

public class PasteCommand : ICommand
{
    private TextEditor _editor;
    private int _position;
    private string _pastedText;

    public PasteCommand(TextEditor editor, int position)
    {
        _editor = editor;
        _position = position;
    }

    public string Description => $"Paste at position {_position}";

    public void Execute()
    {
        _pastedText = _editor.GetClipboardContent();
        if (!string.IsNullOrEmpty(_pastedText))
        {
            _editor.InsertText(_pastedText, _position);
        }
    }

    public void Undo()
    {
        if (!string.IsNullOrEmpty(_pastedText))
        {
            _editor.DeleteText(_position, _pastedText.Length);
        }
    }
}

// Macro Command - composite command
public class MacroCommand : ICommand
{
    private List<ICommand> _commands;

    public MacroCommand()
    {
        _commands = new List<ICommand>();
    }

    public string Description => $"Macro with {_commands.Count} commands";

    public void AddCommand(ICommand command)
    {
        _commands.Add(command);
    }

    public void Execute()
    {
        Console.WriteLine($"Executing macro with {_commands.Count} commands:");
        foreach (var command in _commands)
        {
            Console.WriteLine($"  - {command.Description}");
            command.Execute();
        }
    }

    public void Undo()
    {
        Console.WriteLine("Undoing macro commands in reverse order:");
        for (int i = _commands.Count - 1; i >= 0; i--)
        {
            Console.WriteLine($"  - Undoing: {_commands[i].Description}");
            _commands[i].Undo();
        }
    }
}

// Invoker - Command Manager with undo/redo support
public class CommandManager
{
    private Stack<ICommand> _undoStack;
    private Stack<ICommand> _redoStack;

    public CommandManager()
    {
        _undoStack = new Stack<ICommand>();
        _redoStack = new Stack<ICommand>();
    }

    public void ExecuteCommand(ICommand command)
    {
        Console.WriteLine($"\nExecuting: {command.Description}");
        command.Execute();
        
        _undoStack.Push(command);
        _redoStack.Clear(); // Clear redo stack when new command is executed
        
        Console.WriteLine($"Command executed. Undo stack size: {_undoStack.Count}");
    }

    public void Undo()
    {
        if (_undoStack.Count > 0)
        {
            var command = _undoStack.Pop();
            Console.WriteLine($"\nUndoing: {command.Description}");
            command.Undo();
            _redoStack.Push(command);
            Console.WriteLine($"Undo completed. Redo stack size: {_redoStack.Count}");
        }
        else
        {
            Console.WriteLine("Nothing to undo");
        }
    }

    public void Redo()
    {
        if (_redoStack.Count > 0)
        {
            var command = _redoStack.Pop();
            Console.WriteLine($"\nRedoing: {command.Description}");
            command.Execute();
            _undoStack.Push(command);
            Console.WriteLine($"Redo completed. Undo stack size: {_undoStack.Count}");
        }
        else
        {
            Console.WriteLine("Nothing to redo");
        }
    }

    public void ShowHistory()
    {
        Console.WriteLine($"\nCommand History:");
        Console.WriteLine($"Undo stack ({_undoStack.Count} commands):");
        var undoArray = _undoStack.ToArray();
        for (int i = 0; i < undoArray.Length; i++)
        {
            Console.WriteLine($"  {i + 1}. {undoArray[i].Description}");
        }

        Console.WriteLine($"Redo stack ({_redoStack.Count} commands):");
        var redoArray = _redoStack.ToArray();
        for (int i = 0; i < redoArray.Length; i++)
        {
            Console.WriteLine($"  {i + 1}. {redoArray[i].Description}");
        }
    }
}

// Usage - demonstrating command pattern
var editor = new TextEditor();
var commandManager = new CommandManager();

Console.WriteLine("=== Text Editor with Command Pattern ===");
editor.DisplayContent();

// Execute various commands
var insertHello = new InsertTextCommand(editor, "Hello ", 0);
commandManager.ExecuteCommand(insertHello);
editor.DisplayContent();

var insertWorld = new InsertTextCommand(editor, "World!", 6);
commandManager.ExecuteCommand(insertWorld);
editor.DisplayContent();

var copyCommand = new CopyCommand(editor, 0, 5); // Copy "Hello"
commandManager.ExecuteCommand(copyCommand);

var pasteCommand = new PasteCommand(editor, 12); // Paste at end
commandManager.ExecuteCommand(pasteCommand);
editor.DisplayContent();

// Create and execute a macro
var macro = new MacroCommand();
macro.AddCommand(new InsertTextCommand(editor, " How", 17));
macro.AddCommand(new InsertTextCommand(editor, " are", 21));
macro.AddCommand(new InsertTextCommand(editor, " you?", 25));

commandManager.ExecuteCommand(macro);
editor.DisplayContent();

// Test undo/redo
commandManager.ShowHistory();

commandManager.Undo(); // Undo macro
editor.DisplayContent();

commandManager.Undo(); // Undo paste
editor.DisplayContent();

commandManager.Redo(); // Redo paste
editor.DisplayContent();

commandManager.ShowHistory();
```

## Real-World Examples

### 1. **Remote Control System**
```csharp
// Receiver interfaces and implementations
public interface IDevice
{
    void TurnOn();
    void TurnOff();
    bool IsOn { get; }
}

public class Television : IDevice
{
    public bool IsOn { get; private set; }
    public int Volume { get; private set; } = 50;
    public int Channel { get; private set; } = 1;

    public void TurnOn()
    {
        IsOn = true;
        Console.WriteLine("📺 TV turned ON");
    }

    public void TurnOff()
    {
        IsOn = false;
        Console.WriteLine("📺 TV turned OFF");
    }

    public void SetVolume(int volume)
    {
        Volume = Math.Max(0, Math.Min(100, volume));
        Console.WriteLine($"📺 TV volume set to {Volume}");
    }

    public void SetChannel(int channel)
    {
        if (IsOn)
        {
            Channel = channel;
            Console.WriteLine($"📺 TV channel set to {Channel}");
        }
    }
}

public class SoundSystem : IDevice
{
    public bool IsOn { get; private set; }
    public int Volume { get; private set; } = 30;

    public void TurnOn()
    {
        IsOn = true;
        Console.WriteLine("🔊 Sound System turned ON");
    }

    public void TurnOff()
    {
        IsOn = false;
        Console.WriteLine("🔊 Sound System turned OFF");
    }

    public void SetVolume(int volume)
    {
        Volume = Math.Max(0, Math.Min(100, volume));
        Console.WriteLine($"🔊 Sound System volume set to {Volume}");
    }
}

// Commands for device operations
public class PowerOnCommand : ICommand
{
    private IDevice _device;

    public PowerOnCommand(IDevice device)
    {
        _device = device;
    }

    public string Description => $"Turn on {_device.GetType().Name}";

    public void Execute()
    {
        _device.TurnOn();
    }

    public void Undo()
    {
        _device.TurnOff();
    }
}

public class PowerOffCommand : ICommand
{
    private IDevice _device;

    public PowerOffCommand(IDevice device)
    {
        _device = device;
    }

    public string Description => $"Turn off {_device.GetType().Name}";

    public void Execute()
    {
        _device.TurnOff();
    }

    public void Undo()
    {
        _device.TurnOn();
    }
}

public class VolumeCommand : ICommand
{
    private Television _tv;
    private SoundSystem _soundSystem;
    private int _newVolume;
    private int _previousTvVolume;
    private int _previousSoundVolume;

    public VolumeCommand(Television tv, SoundSystem soundSystem, int newVolume)
    {
        _tv = tv;
        _soundSystem = soundSystem;
        _newVolume = newVolume;
    }

    public string Description => $"Set volume to {_newVolume}";

    public void Execute()
    {
        _previousTvVolume = _tv.Volume;
        _previousSoundVolume = _soundSystem.Volume;
        
        _tv.SetVolume(_newVolume);
        _soundSystem.SetVolume(_newVolume);
    }

    public void Undo()
    {
        _tv.SetVolume(_previousTvVolume);
        _soundSystem.SetVolume(_previousSoundVolume);
    }
}

// No Operation Command (Null Object Pattern)
public class NoOpCommand : ICommand
{
    public string Description => "No operation";

    public void Execute()
    {
        Console.WriteLine("No operation assigned to this button");
    }

    public void Undo()
    {
        // Nothing to undo
    }
}

// Remote Control - Invoker
public class RemoteControl
{
    private ICommand[] _onCommands;
    private ICommand[] _offCommands;
    private ICommand _lastCommand;

    public RemoteControl()
    {
        _onCommands = new ICommand[5];
        _offCommands = new ICommand[5];
        
        var noOpCommand = new NoOpCommand();
        for (int i = 0; i < 5; i++)
        {
            _onCommands[i] = noOpCommand;
            _offCommands[i] = noOpCommand;
        }
        _lastCommand = noOpCommand;
    }

    public void SetCommand(int slot, ICommand onCommand, ICommand offCommand)
    {
        _onCommands[slot] = onCommand;
        _offCommands[slot] = offCommand;
    }

    public void PressOnButton(int slot)
    {
        if (slot >= 0 && slot < _onCommands.Length)
        {
            Console.WriteLine($"🔘 Pressing ON button {slot}");
            _onCommands[slot].Execute();
            _lastCommand = _onCommands[slot];
        }
    }

    public void PressOffButton(int slot)
    {
        if (slot >= 0 && slot < _offCommands.Length)
        {
            Console.WriteLine($"🔘 Pressing OFF button {slot}");
            _offCommands[slot].Execute();
            _lastCommand = _offCommands[slot];
        }
    }

    public void PressUndoButton()
    {
        Console.WriteLine("🔄 Pressing UNDO button");
        _lastCommand.Undo();
    }

    public void ShowConfiguration()
    {
        Console.WriteLine("\n📱 Remote Control Configuration:");
        for (int i = 0; i < _onCommands.Length; i++)
        {
            Console.WriteLine($"Slot {i}: ON=[{_onCommands[i].Description}], OFF=[{_offCommands[i].Description}]");
        }
    }
}

// Usage
var tv = new Television();
var soundSystem = new SoundSystem();
var remote = new RemoteControl();

// Set up commands
remote.SetCommand(0, new PowerOnCommand(tv), new PowerOffCommand(tv));
remote.SetCommand(1, new PowerOnCommand(soundSystem), new PowerOffCommand(soundSystem));

// Create macro for "Movie Mode"
var movieMacro = new MacroCommand();
movieMacro.AddCommand(new PowerOnCommand(tv));
movieMacro.AddCommand(new PowerOnCommand(soundSystem));
movieMacro.AddCommand(new VolumeCommand(tv, soundSystem, 80));

var movieOffMacro = new MacroCommand();
movieOffMacro.AddCommand(new PowerOffCommand(tv));
movieOffMacro.AddCommand(new PowerOffCommand(soundSystem));

remote.SetCommand(2, movieMacro, movieOffMacro);

remote.ShowConfiguration();

// Test remote control
remote.PressOnButton(0);   // Turn on TV
remote.PressOnButton(1);   // Turn on Sound System
remote.PressOffButton(0);  // Turn off TV
remote.PressUndoButton();  // Undo (turn TV back on)

Console.WriteLine("\n--- Testing Movie Mode ---");
remote.PressOnButton(2);   // Movie mode on
remote.PressOffButton(2);  // Movie mode off
```

### 2. **Stock Trading System**
```csharp
// Stock class (Receiver)
public class Stock
{
    public string Symbol { get; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }

    public Stock(string symbol, decimal price, int quantity)
    {
        Symbol = symbol;
        Price = price;
        Quantity = quantity;
    }

    public void Buy(int quantity)
    {
        Quantity += quantity;
        Console.WriteLine($"📈 Bought {quantity} shares of {Symbol}. Total: {Quantity}");
    }

    public void Sell(int quantity)
    {
        if (Quantity >= quantity)
        {
            Quantity -= quantity;
            Console.WriteLine($"📉 Sold {quantity} shares of {Symbol}. Remaining: {Quantity}");
        }
        else
        {
            Console.WriteLine($"❌ Cannot sell {quantity} shares of {Symbol}. Only have {Quantity}");
        }
    }

    public void UpdatePrice(decimal newPrice)
    {
        var oldPrice = Price;
        Price = newPrice;
        Console.WriteLine($"💰 {Symbol} price updated: ${oldPrice} → ${newPrice}");
    }
}

// Trading Commands
public class BuyStockCommand : ICommand
{
    private Stock _stock;
    private int _quantity;

    public BuyStockCommand(Stock stock, int quantity)
    {
        _stock = stock;
        _quantity = quantity;
    }

    public string Description => $"Buy {_quantity} shares of {_stock.Symbol}";

    public void Execute()
    {
        _stock.Buy(_quantity);
    }

    public void Undo()
    {
        _stock.Sell(_quantity);
    }
}

public class SellStockCommand : ICommand
{
    private Stock _stock;
    private int _quantity;

    public SellStockCommand(Stock stock, int quantity)
    {
        _stock = stock;
        _quantity = quantity;
    }

    public string Description => $"Sell {_quantity} shares of {_stock.Symbol}";

    public void Execute()
    {
        _stock.Sell(_quantity);
    }

    public void Undo()
    {
        _stock.Buy(_quantity);
    }
}

// Stock Broker (Invoker)
public class StockBroker
{
    private List<ICommand> _orders = new List<ICommand>();

    public void TakeOrder(ICommand order)
    {
        _orders.Add(order);
        Console.WriteLine($"📋 Order received: {order.Description}");
    }

    public void PlaceOrders()
    {
        Console.WriteLine($"\n💼 Executing {_orders.Count} orders:");
        foreach (var order in _orders)
        {
            Console.WriteLine($"   Executing: {order.Description}");
            order.Execute();
        }
        _orders.Clear();
        Console.WriteLine("✅ All orders executed\n");
    }

    public void CancelOrders()
    {
        Console.WriteLine($"❌ Canceling {_orders.Count} pending orders");
        _orders.Clear();
    }
}

// Portfolio Management Strategy
public class PortfolioRebalanceCommand : ICommand
{
    private List<ICommand> _trades;
    private string _strategyName;

    public PortfolioRebalanceCommand(string strategyName)
    {
        _strategyName = strategyName;
        _trades = new List<ICommand>();
    }

    public string Description => $"Portfolio rebalance: {_strategyName}";

    public void AddTrade(ICommand trade)
    {
        _trades.Add(trade);
    }

    public void Execute()
    {
        Console.WriteLine($"🔄 Executing portfolio rebalance strategy: {_strategyName}");
        foreach (var trade in _trades)
        {
            Console.WriteLine($"   - {trade.Description}");
            trade.Execute();
        }
        Console.WriteLine($"✅ Portfolio rebalance completed");
    }

    public void Undo()
    {
        Console.WriteLine($"🔄 Undoing portfolio rebalance: {_strategyName}");
        for (int i = _trades.Count - 1; i >= 0; i--)
        {
            Console.WriteLine($"   - Undoing: {_trades[i].Description}");
            _trades[i].Undo();
        }
        Console.WriteLine($"✅ Portfolio rebalance undone");
    }
}

// Usage
var appleStock = new Stock("AAPL", 150.00m, 0);
var googleStock = new Stock("GOOGL", 2500.00m, 0);
var broker = new StockBroker();

// Individual orders
broker.TakeOrder(new BuyStockCommand(appleStock, 100));
broker.TakeOrder(new BuyStockCommand(googleStock, 10));
broker.TakeOrder(new SellStockCommand(appleStock, 20));

broker.PlaceOrders();

// Portfolio rebalancing strategy
var rebalance = new PortfolioRebalanceCommand("Conservative Growth");
rebalance.AddTrade(new SellStockCommand(appleStock, 30));
rebalance.AddTrade(new BuyStockCommand(googleStock, 5));

var commandManager = new CommandManager();
commandManager.ExecuteCommand(rebalance);

Console.WriteLine("\n--- Undoing rebalance ---");
commandManager.Undo();
```

### 3. **Smart Home Automation**
```csharp
// Smart Home Devices (Receivers)
public class SmartLight
{
    public string Location { get; }
    public bool IsOn { get; private set; }
    public int Brightness { get; private set; } = 100;

    public SmartLight(string location)
    {
        Location = location;
    }

    public void TurnOn()
    {
        IsOn = true;
        Console.WriteLine($"💡 {Location} light turned ON (brightness: {Brightness}%)");
    }

    public void TurnOff()
    {
        IsOn = false;
        Console.WriteLine($"💡 {Location} light turned OFF");
    }

    public void SetBrightness(int brightness)
    {
        Brightness = Math.Max(1, Math.Min(100, brightness));
        Console.WriteLine($"💡 {Location} light brightness set to {Brightness}%");
    }
}

public class SmartThermostat
{
    public int Temperature { get; private set; } = 72;

    public void SetTemperature(int temperature)
    {
        Temperature = temperature;
        Console.WriteLine($"🌡️ Thermostat set to {Temperature}°F");
    }
}

public class SmartSpeaker
{
    public int Volume { get; private set; } = 50;
    public string CurrentSong { get; private set; } = "";

    public void PlayMusic(string song)
    {
        CurrentSong = song;
        Console.WriteLine($"🎵 Playing: {song} (Volume: {Volume}%)");
    }

    public void StopMusic()
    {
        CurrentSong = "";
        Console.WriteLine("🎵 Music stopped");
    }

    public void SetVolume(int volume)
    {
        Volume = Math.Max(0, Math.Min(100, volume));
        Console.WriteLine($"🎵 Speaker volume set to {Volume}%");
    }
}

// Smart Home Commands
public class LightCommand : ICommand
{
    private SmartLight _light;
    private bool _turnOn;
    private int _brightness;
    private bool _previousState;
    private int _previousBrightness;

    public LightCommand(SmartLight light, bool turnOn, int brightness = 100)
    {
        _light = light;
        _turnOn = turnOn;
        _brightness = brightness;
    }

    public string Description => $"{(_turnOn ? "Turn on" : "Turn off")} {_light.Location} light";

    public void Execute()
    {
        _previousState = _light.IsOn;
        _previousBrightness = _light.Brightness;

        if (_turnOn)
        {
            _light.SetBrightness(_brightness);
            _light.TurnOn();
        }
        else
        {
            _light.TurnOff();
        }
    }

    public void Undo()
    {
        if (_previousState)
        {
            _light.SetBrightness(_previousBrightness);
            _light.TurnOn();
        }
        else
        {
            _light.TurnOff();
        }
    }
}

public class ThermostatCommand : ICommand
{
    private SmartThermostat _thermostat;
    private int _temperature;
    private int _previousTemperature;

    public ThermostatCommand(SmartThermostat thermostat, int temperature)
    {
        _thermostat = thermostat;
        _temperature = temperature;
    }

    public string Description => $"Set thermostat to {_temperature}°F";

    public void Execute()
    {
        _previousTemperature = _thermostat.Temperature;
        _thermostat.SetTemperature(_temperature);
    }

    public void Undo()
    {
        _thermostat.SetTemperature(_previousTemperature);
    }
}

public class MusicCommand : ICommand
{
    private SmartSpeaker _speaker;
    private string _song;
    private int _volume;
    private string _previousSong;
    private int _previousVolume;

    public MusicCommand(SmartSpeaker speaker, string song, int volume)
    {
        _speaker = speaker;
        _song = song;
        _volume = volume;
    }

    public string Description => $"Play '{_song}' at volume {_volume}%";

    public void Execute()
    {
        _previousSong = _speaker.CurrentSong;
        _previousVolume = _speaker.Volume;

        _speaker.SetVolume(_volume);
        if (!string.IsNullOrEmpty(_song))
        {
            _speaker.PlayMusic(_song);
        }
        else
        {
            _speaker.StopMusic();
        }
    }

    public void Undo()
    {
        _speaker.SetVolume(_previousVolume);
        if (!string.IsNullOrEmpty(_previousSong))
        {
            _speaker.PlayMusic(_previousSong);
        }
        else
        {
            _speaker.StopMusic();
        }
    }
}

// Scene Commands (Macro Commands)
public class HomeSceneCommand : ICommand
{
    private List<ICommand> _commands;
    private string _sceneName;

    public HomeSceneCommand(string sceneName)
    {
        _sceneName = sceneName;
        _commands = new List<ICommand>();
    }

    public string Description => $"Activate '{_sceneName}' scene";

    public void AddCommand(ICommand command)
    {
        _commands.Add(command);
    }

    public void Execute()
    {
        Console.WriteLine($"🏠 Activating '{_sceneName}' scene:");
        foreach (var command in _commands)
        {
            Console.WriteLine($"   • {command.Description}");
            command.Execute();
        }
        Console.WriteLine($"✅ '{_sceneName}' scene activated");
    }

    public void Undo()
    {
        Console.WriteLine($"🏠 Deactivating '{_sceneName}' scene:");
        for (int i = _commands.Count - 1; i >= 0; i--)
        {
            Console.WriteLine($"   • Undoing: {_commands[i].Description}");
            _commands[i].Undo();
        }
        Console.WriteLine($"✅ '{_sceneName}' scene deactivated");
    }
}

// Smart Home Controller
public class SmartHomeController
{
    private CommandManager _commandManager;
    private Dictionary<string, ICommand> _scenes;

    public SmartHomeController()
    {
        _commandManager = new CommandManager();
        _scenes = new Dictionary<string, ICommand>();
    }

    public void AddScene(string name, ICommand scene)
    {
        _scenes[name] = scene;
        Console.WriteLine($"📱 Scene '{name}' added to controller");
    }

    public void ActivateScene(string sceneName)
    {
        if (_scenes.ContainsKey(sceneName))
        {
            _commandManager.ExecuteCommand(_scenes[sceneName]);
        }
        else
        {
            Console.WriteLine($"❌ Scene '{sceneName}' not found");
        }
    }

    public void UndoLastCommand()
    {
        _commandManager.Undo();
    }

    public void RedoLastCommand()
    {
        _commandManager.Redo();
    }
}

// Usage
var livingRoomLight = new SmartLight("Living Room");
var bedroomLight = new SmartLight("Bedroom");
var thermostat = new SmartThermostat();
var speaker = new SmartSpeaker();

var controller = new SmartHomeController();

// Create "Good Morning" scene
var goodMorning = new HomeSceneCommand("Good Morning");
goodMorning.AddCommand(new LightCommand(livingRoomLight, true, 80));
goodMorning.AddCommand(new LightCommand(bedroomLight, true, 60));
goodMorning.AddCommand(new ThermostatCommand(thermostat, 75));
goodMorning.AddCommand(new MusicCommand(speaker, "Morning Playlist", 30));

// Create "Movie Night" scene
var movieNight = new HomeSceneCommand("Movie Night");
movieNight.AddCommand(new LightCommand(livingRoomLight, true, 20));
movieNight.AddCommand(new LightCommand(bedroomLight, false));
movieNight.AddCommand(new ThermostatCommand(thermostat, 70));
movieNight.AddCommand(new MusicCommand(speaker, "", 0)); // Stop music

// Create "Bedtime" scene
var bedtime = new HomeSceneCommand("Bedtime");
bedtime.AddCommand(new LightCommand(livingRoomLight, false));
bedtime.AddCommand(new LightCommand(bedroomLight, false));
bedtime.AddCommand(new ThermostatCommand(thermostat, 68));
bedtime.AddCommand(new MusicCommand(speaker, "Sleep Sounds", 15));

controller.AddScene("Good Morning", goodMorning);
controller.AddScene("Movie Night", movieNight);
controller.AddScene("Bedtime", bedtime);

// Test scenes
Console.WriteLine("\n=== Smart Home Automation Demo ===");

controller.ActivateScene("Good Morning");
Thread.Sleep(1000);

controller.ActivateScene("Movie Night");
Thread.Sleep(1000);

controller.ActivateScene("Bedtime");
Thread.Sleep(1000);

Console.WriteLine("\n--- Testing Undo/Redo ---");
controller.UndoLastCommand(); // Undo bedtime
controller.UndoLastCommand(); // Undo movie night
controller.RedoLastCommand(); // Redo movie night
```

## Benefits
- **Decoupling**: Invoker is decoupled from receiver
- **Undo/Redo**: Easy to implement undo and redo functionality
- **Macro Commands**: Can combine multiple commands into one
- **Logging**: Can log all commands for auditing
- **Queuing**: Commands can be queued and executed later
- **Remote Execution**: Commands can be serialized and sent over network

## Drawbacks
- **Complexity**: Adds more classes and objects
- **Memory Usage**: Each command object takes memory
- **Indirection**: Additional layer between invoker and receiver
- **Overhead**: May be overkill for simple operations

## When to Use
✅ **Use When:**
- You need to parameterize objects with operations
- You want to queue, log, or support undo operations
- You need to support macros (composite commands)
- You want to decouple invoker from receiver
- Operations need to be triggered at different times

❌ **Avoid When:**
- Simple operations that don't need queuing or undo
- Memory is severely constrained
- Real-time systems where command overhead matters
- Operations are tightly coupled to their context

## Command vs Other Patterns

| Pattern | Purpose | Key Difference |
|---------|---------|----------------|
| **Command** | Encapsulates request as object | Focuses on request encapsulation |
| **Strategy** | Encapsulates algorithms | Focuses on algorithm selection |
| **Observer** | Notifies multiple objects | Focuses on event notification |
| **Chain of Responsibility** | Passes request along chain | Focuses on request delegation |

## Best Practices
1. **Immutable Commands**: Make command objects immutable when possible
2. **Small Commands**: Keep commands focused on single operations
3. **Error Handling**: Handle errors gracefully in Execute() and Undo()
4. **Resource Management**: Properly manage resources in command objects
5. **Command History**: Limit history size to prevent memory issues
6. **Serialization**: Make commands serializable if needed for persistence

## Common Mistakes
1. **Heavy Commands**: Putting too much logic in command objects
2. **State Management**: Not properly managing state for undo operations
3. **Memory Leaks**: Keeping references to large objects in command history
4. **Exception Handling**: Not handling exceptions in undo operations

## Modern C# Features
```csharp
// Using Action delegates for simple commands
public class ActionCommand : ICommand
{
    private readonly Action _execute;
    private readonly Action _undo;

    public ActionCommand(Action execute, Action undo)
    {
        _execute = execute;
        _undo = undo;
    }

    public void Execute() => _execute();
    public void Undo() => _undo();
}

// Usage
var command = new ActionCommand(
    execute: () => Console.WriteLine("Doing something"),
    undo: () => Console.WriteLine("Undoing something")
);

// Using async commands
public interface IAsyncCommand
{
    Task ExecuteAsync();
    Task UndoAsync();
}

public class AsyncCommand : IAsyncCommand
{
    private readonly Func<Task> _execute;
    private readonly Func<Task> _undo;

    public AsyncCommand(Func<Task> execute, Func<Task> undo)
    {
        _execute = execute;
        _undo = undo;
    }

    public async Task ExecuteAsync() => await _execute();
    public async Task UndoAsync() => await _undo();
}

// Using records for command data
public record CreateUserCommand(string Name, string Email, string Role) : ICommand
{
    public void Execute()
    {
        // Create user logic
    }

    public void Undo()
    {
        // Delete user logic
    }
}
```

## Testing Commands
```csharp
[Test]
public void InsertTextCommand_Execute_InsertsTextAtCorrectPosition()
{
    // Arrange
    var editor = new TextEditor();
    var command = new InsertTextCommand(editor, "Hello", 0);

    // Act
    command.Execute();

    // Assert
    Assert.AreEqual("Hello", editor.Content);
}

[Test]
public void InsertTextCommand_Undo_RemovesInsertedText()
{
    // Arrange
    var editor = new TextEditor();
    var command = new InsertTextCommand(editor, "Hello", 0);
    command.Execute();

    // Act
    command.Undo();

    // Assert
    Assert.AreEqual("", editor.Content);
}
```

## Summary
The Command pattern is like having a universal remote control for your entire application - each button press creates a command object that knows exactly what to do and how to undo it. Instead of tight coupling between the buttons (invokers) and the devices (receivers), you have command objects that act as intermediaries.

It's perfect for implementing undo/redo functionality, creating macro operations, queuing operations for later execution, and building flexible user interfaces where the same action can be triggered from multiple places.

The key insight is that by turning requests into objects, you gain tremendous flexibility in how those requests are handled, stored, queued, logged, and potentially undone.
