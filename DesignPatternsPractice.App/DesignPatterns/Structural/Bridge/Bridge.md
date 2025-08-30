# Bridge Pattern

## Overview
The Bridge pattern separates an abstraction from its implementation so both can change independently. Think of it like a TV remote control - the remote (abstraction) works with any TV brand (implementation) because they communicate through a standard interface, not by being tightly coupled together.

## Problem It Solves
Imagine you're building a notification system that can send messages through different channels (Email, SMS, Push) and you want different types of notifications (Alert, Info, Warning):
- Without Bridge: You'd need 9 classes (AlertEmail, AlertSMS, AlertPush, InfoEmail, InfoSMS, etc.)
- With different urgencies: 18 classes (UrgentAlertEmail, NormalAlertEmail, etc.)
- This explodes exponentially - the "Cartesian Product Problem"

Without Bridge, you get class explosion:
```csharp
// BAD: Class explosion
public class AlertEmailNotification { }
public class AlertSMSNotification { }
public class AlertPushNotification { }
public class InfoEmailNotification { }
public class InfoSMSNotification { }
public class InfoPushNotification { }
public class WarningEmailNotification { }
public class WarningSMSNotification { }
public class WarningPushNotification { }
// And it keeps growing...
```

## Real-World Analogy
Think of a **TV and Remote Control**:
1. **Remote Control** (Abstraction): Has buttons for power, volume, channel
2. **TV** (Implementation): Different brands (Sony, Samsung, LG) implement these features differently
3. **Standard Interface**: Infrared signals that any TV understands
4. **Flexibility**: You can use any remote with any TV that supports the standard
5. **Independence**: TV manufacturers can change internal workings without affecting remotes

Or consider **Cars and Engines**:
- Car (abstraction) has start, accelerate, brake methods
- Engine (implementation) can be gasoline, diesel, electric, or hybrid
- Car doesn't need to know engine details, just the interface
- You can put different engines in the same car model

## Implementation Details

### Basic Structure
```csharp
// Implementation interface
public interface IImplementation
{
    void OperationImpl();
}

// Concrete implementations
public class ConcreteImplementationA : IImplementation
{
    public void OperationImpl()
    {
        Console.WriteLine("Implementation A");
    }
}

public class ConcreteImplementationB : IImplementation
{
    public void OperationImpl()
    {
        Console.WriteLine("Implementation B");
    }
}

// Abstraction
public abstract class Abstraction
{
    protected IImplementation _implementation;

    public Abstraction(IImplementation implementation)
    {
        _implementation = implementation;
    }

    public virtual void Operation()
    {
        _implementation.OperationImpl();
    }
}

// Refined abstraction
public class RefinedAbstraction : Abstraction
{
    public RefinedAbstraction(IImplementation implementation) 
        : base(implementation)
    {
    }

    public override void Operation()
    {
        Console.WriteLine("Refined operation:");
        _implementation.OperationImpl();
    }
}
```

### Key Components
1. **Abstraction**: Defines the abstraction's interface and maintains reference to implementation
2. **Refined Abstraction**: Extends the abstraction
3. **Implementation**: Interface for implementation classes
4. **Concrete Implementation**: Specific implementations

## Example from Our Code
```csharp
// Implementation interface - the "bridge"
public interface IDevice
{
    bool IsEnabled();
    void Enable();
    void Disable();
    int GetVolume();
    void SetVolume(int volume);
    int GetChannel();
    void SetChannel(int channel);
}

// Concrete implementations
public class TV : IDevice
{
    private bool _on = false;
    private int _volume = 30;
    private int _channel = 1;

    public bool IsEnabled() => _on;
    public void Enable() => _on = true;
    public void Disable() => _on = false;
    public int GetVolume() => _volume;
    public void SetVolume(int volume) => _volume = volume;
    public int GetChannel() => _channel;
    public void SetChannel(int channel) => _channel = channel;
}

public class Radio : IDevice
{
    private bool _on = false;
    private int _volume = 10;
    private int _channel = 1;

    public bool IsEnabled() => _on;
    public void Enable() => _on = true;
    public void Disable() => _on = false;
    public int GetVolume() => _volume;
    public void SetVolume(int volume) => _volume = volume;
    public int GetChannel() => _channel;
    public void SetChannel(int channel) => _channel = channel;
}

// Abstraction - the remote control
public class RemoteControl
{
    protected IDevice _device;

    public RemoteControl(IDevice device)
    {
        _device = device;
    }

    public void TogglePower()
    {
        if (_device.IsEnabled())
        {
            _device.Disable();
            Console.WriteLine("Device turned OFF");
        }
        else
        {
            _device.Enable();
            Console.WriteLine("Device turned ON");
        }
    }

    public void VolumeDown()
    {
        _device.SetVolume(_device.GetVolume() - 10);
        Console.WriteLine($"Volume: {_device.GetVolume()}");
    }

    public void VolumeUp()
    {
        _device.SetVolume(_device.GetVolume() + 10);
        Console.WriteLine($"Volume: {_device.GetVolume()}");
    }

    public void ChannelDown()
    {
        _device.SetChannel(_device.GetChannel() - 1);
        Console.WriteLine($"Channel: {_device.GetChannel()}");
    }

    public void ChannelUp()
    {
        _device.SetChannel(_device.GetChannel() + 1);
        Console.WriteLine($"Channel: {_device.GetChannel()}");
    }
}

// Refined abstraction - advanced remote
public class AdvancedRemoteControl : RemoteControl
{
    public AdvancedRemoteControl(IDevice device) : base(device)
    {
    }

    public void Mute()
    {
        _device.SetVolume(0);
        Console.WriteLine("Device muted");
    }

    public void SetChannel(int channel)
    {
        _device.SetChannel(channel);
        Console.WriteLine($"Channel set to: {channel}");
    }
}

// Usage
var tv = new TV();
var radio = new Radio();

var tvRemote = new RemoteControl(tv);
var radioRemote = new AdvancedRemoteControl(radio);

tvRemote.TogglePower();  // Works with TV
tvRemote.VolumeUp();

radioRemote.TogglePower(); // Works with Radio
radioRemote.Mute();        // Advanced feature
```

## Real-World Examples

### 1. **Drawing Applications**
```csharp
// Implementation - different rendering engines
public interface IRenderEngine
{
    void DrawLine(int x1, int y1, int x2, int y2);
    void DrawCircle(int x, int y, int radius);
    void DrawRectangle(int x, int y, int width, int height);
}

public class DirectXRenderer : IRenderEngine
{
    public void DrawLine(int x1, int y1, int x2, int y2)
        => Console.WriteLine($"DirectX: Drawing line from ({x1},{y1}) to ({x2},{y2})");

    public void DrawCircle(int x, int y, int radius)
        => Console.WriteLine($"DirectX: Drawing circle at ({x},{y}) with radius {radius}");

    public void DrawRectangle(int x, int y, int width, int height)
        => Console.WriteLine($"DirectX: Drawing rectangle at ({x},{y}) size {width}x{height}");
}

public class OpenGLRenderer : IRenderEngine
{
    public void DrawLine(int x1, int y1, int x2, int y2)
        => Console.WriteLine($"OpenGL: Line from ({x1},{y1}) to ({x2},{y2})");

    public void DrawCircle(int x, int y, int radius)
        => Console.WriteLine($"OpenGL: Circle at ({x},{y}) radius {radius}");

    public void DrawRectangle(int x, int y, int width, int height)
        => Console.WriteLine($"OpenGL: Rectangle at ({x},{y}) size {width}x{height}");
}

// Abstraction - shapes
public abstract class Shape
{
    protected IRenderEngine _renderer;

    public Shape(IRenderEngine renderer)
    {
        _renderer = renderer;
    }

    public abstract void Draw();
    public abstract void Move(int x, int y);
}

// Refined abstractions
public class Circle : Shape
{
    private int _x, _y, _radius;

    public Circle(int x, int y, int radius, IRenderEngine renderer) 
        : base(renderer)
    {
        _x = x;
        _y = y;
        _radius = radius;
    }

    public override void Draw()
    {
        _renderer.DrawCircle(_x, _y, _radius);
    }

    public override void Move(int x, int y)
    {
        _x = x;
        _y = y;
        Console.WriteLine($"Circle moved to ({x}, {y})");
    }
}

public class Rectangle : Shape
{
    private int _x, _y, _width, _height;

    public Rectangle(int x, int y, int width, int height, IRenderEngine renderer) 
        : base(renderer)
    {
        _x = x;
        _y = y;
        _width = width;
        _height = height;
    }

    public override void Draw()
    {
        _renderer.DrawRectangle(_x, _y, _width, _height);
    }

    public override void Move(int x, int y)
    {
        _x = x;
        _y = y;
        Console.WriteLine($"Rectangle moved to ({x}, {y})");
    }
}

// Usage - any shape can use any renderer
var directX = new DirectXRenderer();
var openGL = new OpenGLRenderer();

var circle1 = new Circle(10, 10, 5, directX);
var circle2 = new Circle(20, 20, 8, openGL);

circle1.Draw(); // DirectX rendering
circle2.Draw(); // OpenGL rendering
```

### 2. **Database Connections**
```csharp
// Implementation - different database providers
public interface IDatabaseProvider
{
    void Connect(string connectionString);
    void Disconnect();
    void ExecuteQuery(string sql);
    void ExecuteCommand(string sql);
}

public class SqlServerProvider : IDatabaseProvider
{
    public void Connect(string connectionString)
        => Console.WriteLine($"Connecting to SQL Server: {connectionString}");

    public void Disconnect()
        => Console.WriteLine("Disconnecting from SQL Server");

    public void ExecuteQuery(string sql)
        => Console.WriteLine($"SQL Server Query: {sql}");

    public void ExecuteCommand(string sql)
        => Console.WriteLine($"SQL Server Command: {sql}");
}

public class OracleProvider : IDatabaseProvider
{
    public void Connect(string connectionString)
        => Console.WriteLine($"Connecting to Oracle: {connectionString}");

    public void Disconnect()
        => Console.WriteLine("Disconnecting from Oracle");

    public void ExecuteQuery(string sql)
        => Console.WriteLine($"Oracle Query: {sql}");

    public void ExecuteCommand(string sql)
        => Console.WriteLine($"Oracle Command: {sql}");
}

// Abstraction - database operations
public abstract class DatabaseManager
{
    protected IDatabaseProvider _provider;

    public DatabaseManager(IDatabaseProvider provider)
    {
        _provider = provider;
    }

    public virtual void Connect(string connectionString)
    {
        _provider.Connect(connectionString);
    }

    public virtual void Disconnect()
    {
        _provider.Disconnect();
    }

    public abstract void SaveUser(string username, string email);
    public abstract List<string> GetUsers();
}

// Refined abstraction
public class UserManager : DatabaseManager
{
    public UserManager(IDatabaseProvider provider) : base(provider)
    {
    }

    public override void SaveUser(string username, string email)
    {
        var sql = $"INSERT INTO Users (Username, Email) VALUES ('{username}', '{email}')";
        _provider.ExecuteCommand(sql);
    }

    public override List<string> GetUsers()
    {
        var sql = "SELECT Username FROM Users";
        _provider.ExecuteQuery(sql);
        return new List<string> { "user1", "user2" }; // Simulated result
    }
}

// Usage - same user manager works with different databases
var sqlServer = new SqlServerProvider();
var oracle = new OracleProvider();

var userManager1 = new UserManager(sqlServer);
var userManager2 = new UserManager(oracle);

userManager1.Connect("Server=sql;Database=MyApp");
userManager1.SaveUser("john", "john@email.com");

userManager2.Connect("Server=oracle;Database=MyApp");
userManager2.SaveUser("jane", "jane@email.com");
```

### 3. **Message Sending System**
```csharp
// Implementation - different messaging channels
public interface IMessageSender
{
    void SendMessage(string message, string recipient);
}

public class EmailSender : IMessageSender
{
    public void SendMessage(string message, string recipient)
    {
        Console.WriteLine($"Email to {recipient}: {message}");
    }
}

public class SMSSender : IMessageSender
{
    public void SendMessage(string message, string recipient)
    {
        Console.WriteLine($"SMS to {recipient}: {message}");
    }
}

public class PushNotificationSender : IMessageSender
{
    public void SendMessage(string message, string recipient)
    {
        Console.WriteLine($"Push notification to {recipient}: {message}");
    }
}

// Abstraction - different types of messages
public abstract class Message
{
    protected IMessageSender _sender;
    protected string _content;

    public Message(IMessageSender sender)
    {
        _sender = sender;
    }

    public abstract void Send(string recipient);
}

// Refined abstractions
public class AlertMessage : Message
{
    public AlertMessage(IMessageSender sender) : base(sender)
    {
        _content = "🚨 ALERT: ";
    }

    public override void Send(string recipient)
    {
        _sender.SendMessage(_content + "Critical system issue detected!", recipient);
    }
}

public class InfoMessage : Message
{
    public InfoMessage(IMessageSender sender) : base(sender)
    {
        _content = "ℹ️ INFO: ";
    }

    public override void Send(string recipient)
    {
        _sender.SendMessage(_content + "System update completed successfully.", recipient);
    }
}

public class WelcomeMessage : Message
{
    private string _userName;

    public WelcomeMessage(string userName, IMessageSender sender) : base(sender)
    {
        _userName = userName;
    }

    public override void Send(string recipient)
    {
        _sender.SendMessage($"Welcome {_userName}! Thanks for joining our service.", recipient);
    }
}

// Usage - any message type can use any sender
var emailSender = new EmailSender();
var smsSender = new SMSSender();
var pushSender = new PushNotificationSender();

var alertEmail = new AlertMessage(emailSender);
var alertSMS = new AlertMessage(smsSender);
var welcomeEmail = new WelcomeMessage("John", emailSender);
var welcomePush = new WelcomeMessage("Jane", pushSender);

alertEmail.Send("admin@company.com");
alertSMS.Send("+1234567890");
welcomeEmail.Send("john@email.com");
welcomePush.Send("jane_device_id");
```

## Benefits
- **Decoupling**: Abstraction and implementation can change independently
- **Extensibility**: Easy to add new abstractions or implementations
- **Runtime Binding**: Can switch implementations at runtime
- **Single Responsibility**: Each class has one reason to change
- **Avoids Class Explosion**: N abstractions + M implementations = N + M classes (not N × M)

## Drawbacks
- **Complexity**: Adds another layer of abstraction
- **Indirection**: May impact performance slightly
- **Learning Curve**: Can be harder to understand initially
- **Overkill**: May be unnecessary for simple scenarios

## When to Use
✅ **Use When:**
- You want to avoid permanent binding between abstraction and implementation
- Both abstractions and implementations should be extensible through subclassing
- Changes in implementation shouldn't affect clients
- You want to share implementation among multiple objects
- You have a "Cartesian product" complexity problem

❌ **Avoid When:**
- You have only one implementation
- Abstraction and implementation are unlikely to change
- The relationship is simple and stable
- Performance is critical and indirection is costly

## Bridge vs Other Patterns

| Pattern | Purpose | Key Difference |
|---------|---------|----------------|
| **Bridge** | Separates abstraction from implementation | Both sides can vary independently |
| **Adapter** | Makes incompatible interfaces work together | Focuses on interface compatibility |
| **Strategy** | Encapsulates algorithms | Focuses on behavior/algorithm selection |
| **State** | Changes behavior based on state | Focuses on state-dependent behavior |

## Best Practices
1. **Clear Separation**: Keep abstraction and implementation truly separate
2. **Stable Interface**: Implementation interface should be stable
3. **Composition over Inheritance**: Prefer composition for flexibility
4. **Factory Pattern**: Use factories to create appropriate combinations
5. **Documentation**: Clearly document the bridge relationship
6. **Testing**: Test all abstraction-implementation combinations

## Common Mistakes
1. **Tight Coupling**: Making abstraction depend on specific implementation details
2. **Leaky Abstraction**: Exposing implementation details through abstraction
3. **Overuse**: Using Bridge when simple inheritance would suffice
4. **Complex Interface**: Making the implementation interface too complex

## Modern C# Features
```csharp
// Using dependency injection with Bridge
public class NotificationService
{
    private readonly IMessageSender _sender;

    public NotificationService(IMessageSender sender)
    {
        _sender = sender;
    }

    public async Task SendAlertAsync(string message, string recipient)
    {
        await Task.Run(() => _sender.SendMessage($"ALERT: {message}", recipient));
    }
}

// Register in DI container
services.AddScoped<IMessageSender, EmailSender>();
services.AddScoped<NotificationService>();

// Using generic constraints
public abstract class Message<T> where T : IMessageSender
{
    protected T _sender;

    public Message(T sender)
    {
        _sender = sender;
    }

    public abstract Task SendAsync(string recipient);
}
```

## Testing Bridge Pattern
```csharp
[Test]
public void RemoteControl_TogglePower_CallsDeviceCorrectly()
{
    // Arrange
    var mockDevice = new Mock<IDevice>();
    mockDevice.Setup(d => d.IsEnabled()).Returns(false);
    var remote = new RemoteControl(mockDevice.Object);

    // Act
    remote.TogglePower();

    // Assert
    mockDevice.Verify(d => d.Enable(), Times.Once);
}
```

## Summary
The Bridge pattern is like having a universal remote control that works with any device brand. It separates "what" you want to do (abstraction) from "how" it gets done (implementation), allowing both to evolve independently.

Think of it as the solution to the "multiplication problem" - instead of creating every possible combination of features, you create separate hierarchies that connect through a bridge interface. Like having different cars that can use different engines without redesigning the entire car for each engine type.

The key insight is that sometimes the best way to handle complexity is to separate concerns and connect them through a stable interface rather than trying to build one monolithic solution.
