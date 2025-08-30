# Mediator Pattern

## Overview
The Mediator pattern defines how a set of objects interact with each other by keeping objects from referring to each other explicitly and letting them communicate through a central mediator. Think of it like an **air traffic control tower** - instead of pilots talking directly to each other (which would be chaotic), they all communicate through the control tower that coordinates all interactions safely.

## Problem It Solves
Imagine you're building a complex UI with many interactive components:
- Dialog boxes with buttons, text fields, checkboxes, dropdowns
- Components need to interact with each other (enabling/disabling, updating values)
- Direct component-to-component communication creates tight coupling
- Adding new components or interactions becomes increasingly difficult

Without Mediator pattern, you get a tangled web of dependencies:
```csharp
// BAD: Direct coupling between components
public class LoginButton
{
    private UsernameTextBox _username;
    private PasswordTextBox _password;
    private RememberMeCheckbox _rememberMe;

    public void OnClick()
    {
        if (_username.IsValid() && _password.IsValid())
        {
            _rememberMe.SavePreference();
            // Directly manipulating other components
        }
    }
}
```

This creates a maintenance nightmare where every component knows about every other component.

## Real-World Analogy
Think of an **air traffic control system**:
1. **Pilots** (Colleagues): Each aircraft with its own behavior and state
2. **Control Tower** (Mediator): Coordinates all communication and decisions
3. **No Direct Communication**: Planes don't talk directly to each other
4. **Centralized Logic**: All coordination logic is in one place
5. **Safety**: Prevents conflicts through centralized management

Or consider a **chat room**:
- **Users** (Colleagues): Each person wanting to communicate
- **Chat Server** (Mediator): Handles message routing and room management
- **No Direct Connection**: Users don't connect directly to each other
- **Central Rules**: Server enforces rules, handles permissions, logging
- **Scalability**: Easy to add new users or features

## Implementation Details

### Basic Structure
```csharp
// Mediator interface
public interface IMediator
{
    void Notify(object sender, string eventType);
}

// Colleague base class
public abstract class Colleague
{
    protected IMediator _mediator;

    public Colleague(IMediator mediator)
    {
        _mediator = mediator;
    }
}

// Concrete mediator
public class ConcreteMediator : IMediator
{
    private Component1 _component1;
    private Component2 _component2;

    public ConcreteMediator(Component1 component1, Component2 component2)
    {
        _component1 = component1;
        _component2 = component2;
        _component1.SetMediator(this);
        _component2.SetMediator(this);
    }

    public void Notify(object sender, string eventType)
    {
        if (eventType == "A")
        {
            Console.WriteLine("Mediator reacts on A and triggers:");
            _component2.DoC();
        }
        else if (eventType == "D")
        {
            Console.WriteLine("Mediator reacts on D and triggers:");
            _component1.DoB();
        }
    }
}

// Concrete colleagues
public class Component1 : Colleague
{
    public void DoA()
    {
        Console.WriteLine("Component 1 does A.");
        _mediator.Notify(this, "A");
    }

    public void DoB()
    {
        Console.WriteLine("Component 1 does B.");
    }
}

public class Component2 : Colleague
{
    public void DoC()
    {
        Console.WriteLine("Component 2 does C.");
    }

    public void DoD()
    {
        Console.WriteLine("Component 2 does D.");
        _mediator.Notify(this, "D");
    }
}
```

### Key Components
1. **Mediator**: Interface defining communication contract
2. **ConcreteMediator**: Implements coordination logic
3. **Colleague**: Base class for components that communicate
4. **ConcreteColleague**: Specific components that interact through mediator

## Example from Our Code
```csharp
// Mediator interface for chat room
public interface IChatRoomMediator
{
    void SendMessage(string message, User user);
    void AddUser(User user);
    void RemoveUser(User user);
    void NotifyUserJoined(User user);
    void NotifyUserLeft(User user);
}

// User base class (Colleague)
public abstract class User
{
    protected IChatRoomMediator _mediator;
    public string Name { get; protected set; }
    public bool IsOnline { get; protected set; }
    protected List<string> _messageHistory;

    public User(IChatRoomMediator mediator, string name)
    {
        _mediator = mediator;
        Name = name;
        IsOnline = false;
        _messageHistory = new List<string>();
    }

    public virtual void Send(string message)
    {
        Console.WriteLine($"👤 {Name} sends: {message}");
        _mediator.SendMessage(message, this);
    }

    public virtual void Receive(string message, User sender)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        var formattedMessage = $"[{timestamp}] {sender.Name}: {message}";
        _messageHistory.Add(formattedMessage);
        Console.WriteLine($"📨 {Name} receives: {formattedMessage}");
    }

    public virtual void Join()
    {
        IsOnline = true;
        _mediator.AddUser(this);
        _mediator.NotifyUserJoined(this);
        Console.WriteLine($"✅ {Name} joined the chat room");
    }

    public virtual void Leave()
    {
        IsOnline = false;
        _mediator.RemoveUser(this);
        _mediator.NotifyUserLeft(this);
        Console.WriteLine($"❌ {Name} left the chat room");
    }

    public virtual void ShowMessageHistory()
    {
        Console.WriteLine($"\n📝 {Name}'s Message History:");
        if (_messageHistory.Count == 0)
        {
            Console.WriteLine("   No messages yet");
        }
        else
        {
            foreach (var message in _messageHistory.TakeLast(10)) // Show last 10 messages
            {
                Console.WriteLine($"   {message}");
            }
        }
    }

    public virtual void OnUserJoined(User user)
    {
        Console.WriteLine($"🔔 {Name} sees: {user.Name} joined the room");
    }

    public virtual void OnUserLeft(User user)
    {
        Console.WriteLine($"🔔 {Name} sees: {user.Name} left the room");
    }
}

// Concrete colleagues - different user types
public class RegularUser : User
{
    public RegularUser(IChatRoomMediator mediator, string name) : base(mediator, name) { }

    public override void Send(string message)
    {
        if (IsOnline)
        {
            base.Send(message);
        }
        else
        {
            Console.WriteLine($"❌ {Name} cannot send message - not online");
        }
    }
}

public class ModeratorUser : User
{
    private List<string> _mutedUsers;

    public ModeratorUser(IChatRoomMediator mediator, string name) : base(mediator, name)
    {
        _mutedUsers = new List<string>();
    }

    public void MuteUser(string username)
    {
        if (!_mutedUsers.Contains(username))
        {
            _mutedUsers.Add(username);
            Console.WriteLine($"🔇 Moderator {Name} muted user: {username}");
            _mediator.SendMessage($"🔇 {username} has been muted by moderator", this);
        }
    }

    public void UnmuteUser(string username)
    {
        if (_mutedUsers.Remove(username))
        {
            Console.WriteLine($"🔊 Moderator {Name} unmuted user: {username}");
            _mediator.SendMessage($"🔊 {username} has been unmuted by moderator", this);
        }
    }

    public bool IsUserMuted(string username)
    {
        return _mutedUsers.Contains(username);
    }

    public override void Send(string message)
    {
        if (IsOnline)
        {
            Console.WriteLine($"👑 [MODERATOR] {Name} sends: {message}");
            _mediator.SendMessage($"[MODERATOR] {message}", this);
        }
        else
        {
            Console.WriteLine($"❌ Moderator {Name} cannot send message - not online");
        }
    }
}

public class BotUser : User
{
    private Dictionary<string, string> _autoResponses;
    private List<string> _triggers;

    public BotUser(IChatRoomMediator mediator, string name) : base(mediator, name)
    {
        _autoResponses = new Dictionary<string, string>
        {
            { "hello", "Hi there! 👋 How can I help you?" },
            { "help", "I'm here to assist! Type 'commands' to see what I can do." },
            { "commands", "Available commands: hello, help, time, joke, weather" },
            { "time", $"Current time is: {DateTime.Now:HH:mm:ss}" },
            { "joke", "Why don't scientists trust atoms? Because they make up everything! 😄" },
            { "weather", "It's always sunny in the digital world! ☀️" }
        };
        _triggers = _autoResponses.Keys.ToList();
    }

    public override void Receive(string message, User sender)
    {
        base.Receive(message, sender);

        // Auto-respond to triggers
        if (sender != this && IsOnline)
        {
            var lowerMessage = message.ToLower();
            foreach (var trigger in _triggers)
            {
                if (lowerMessage.Contains(trigger))
                {
                    // Delay response slightly to simulate processing
                    Task.Delay(500).ContinueWith(_ => {
                        Send(_autoResponses[trigger]);
                    });
                    break;
                }
            }
        }
    }

    public override void Send(string message)
    {
        if (IsOnline)
        {
            Console.WriteLine($"🤖 [BOT] {Name} sends: {message}");
            _mediator.SendMessage($"[BOT] {message}", this);
        }
    }
}

// Concrete mediator - Chat Room
public class ChatRoom : IChatRoomMediator
{
    private List<User> _users;
    private List<string> _chatLog;
    private string _roomName;

    public ChatRoom(string roomName)
    {
        _roomName = roomName;
        _users = new List<User>();
        _chatLog = new List<string>();
        Console.WriteLine($"🏠 Chat room '{roomName}' created");
    }

    public void SendMessage(string message, User sender)
    {
        if (!_users.Contains(sender))
        {
            Console.WriteLine($"❌ User {sender.Name} is not in the chat room");
            return;
        }

        // Check if sender is muted by any moderator
        var moderators = _users.OfType<ModeratorUser>();
        foreach (var mod in moderators)
        {
            if (mod.IsUserMuted(sender.Name))
            {
                Console.WriteLine($"🔇 {sender.Name}'s message blocked - user is muted");
                return;
            }
        }

        // Log the message
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        var logEntry = $"[{timestamp}] {sender.Name}: {message}";
        _chatLog.Add(logEntry);

        // Send to all users except sender
        foreach (var user in _users.Where(u => u != sender && u.IsOnline))
        {
            user.Receive(message, sender);
        }

        Console.WriteLine($"📢 Message broadcasted to {_users.Count(u => u != sender && u.IsOnline)} users");
    }

    public void AddUser(User user)
    {
        if (!_users.Contains(user))
        {
            _users.Add(user);
            Console.WriteLine($"➕ {user.Name} added to chat room '{_roomName}'");
        }
    }

    public void RemoveUser(User user)
    {
        if (_users.Remove(user))
        {
            Console.WriteLine($"➖ {user.Name} removed from chat room '{_roomName}'");
        }
    }

    public void NotifyUserJoined(User newUser)
    {
        var joinMessage = $"🎉 {newUser.Name} joined the chat room";
        _chatLog.Add($"[{DateTime.Now:HH:mm:ss}] SYSTEM: {joinMessage}");

        foreach (var user in _users.Where(u => u != newUser && u.IsOnline))
        {
            user.OnUserJoined(newUser);
        }
    }

    public void NotifyUserLeft(User leftUser)
    {
        var leaveMessage = $"👋 {leftUser.Name} left the chat room";
        _chatLog.Add($"[{DateTime.Now:HH:mm:ss}] SYSTEM: {leaveMessage}");

        foreach (var user in _users.Where(u => u != leftUser && u.IsOnline))
        {
            user.OnUserLeft(leftUser);
        }
    }

    public void ShowChatRoomInfo()
    {
        Console.WriteLine($"\n🏠 Chat Room: {_roomName}");
        Console.WriteLine($"👥 Online Users ({_users.Count(u => u.IsOnline)}):");
        
        foreach (var user in _users.Where(u => u.IsOnline))
        {
            var userType = user switch
            {
                ModeratorUser => "👑 Moderator",
                BotUser => "🤖 Bot",
                _ => "👤 User"
            };
            Console.WriteLine($"   {userType}: {user.Name}");
        }

        Console.WriteLine($"📝 Total Messages: {_chatLog.Count}");
    }

    public void ShowRecentChatLog(int messageCount = 5)
    {
        Console.WriteLine($"\n📋 Recent Chat Log (last {messageCount} messages):");
        var recentMessages = _chatLog.TakeLast(messageCount);
        
        foreach (var message in recentMessages)
        {
            Console.WriteLine($"   {message}");
        }
    }
}

// Usage - demonstrating mediator pattern
var chatRoom = new ChatRoom("General Discussion");

Console.WriteLine("=== Chat Room with Mediator Pattern ===");

// Create users
var alice = new RegularUser(chatRoom, "Alice");
var bob = new RegularUser(chatRoom, "Bob");
var charlie = new ModeratorUser(chatRoom, "Charlie");
var helpBot = new BotUser(chatRoom, "HelpBot");

// Users join the chat room
alice.Join();
bob.Join();
charlie.Join();
helpBot.Join();

chatRoom.ShowChatRoomInfo();

// Simulate conversation
Console.WriteLine("\n=== Conversation Simulation ===");

alice.Send("Hello everyone!");
bob.Send("Hi Alice! How are you?");
alice.Send("I'm doing great, thanks for asking!");

// Bot responds to "help" trigger
bob.Send("I need some help with commands");

Thread.Sleep(1000); // Wait for bot response

charlie.Send("Welcome to the chat room, everyone!");

// Moderator actions
charlie.MuteUser("Bob");
bob.Send("Why can't I send messages?"); // This should be blocked

charlie.UnmuteUser("Bob");
bob.Send("Thanks for unmuting me!");

// Bot responds to "time" trigger
alice.Send("What time is it?");

Thread.Sleep(1000); // Wait for bot response

// Show chat room status
chatRoom.ShowChatRoomInfo();
chatRoom.ShowRecentChatLog(8);

// Users leave
bob.Leave();
alice.ShowMessageHistory();

Console.WriteLine("\n=== Final Chat Room Status ===");
chatRoom.ShowChatRoomInfo();
```

## Real-World Examples

### 1. **Smart Home Automation System**
```csharp
// Smart home mediator interface
public interface ISmartHomeMediator
{
    void RegisterDevice(ISmartDevice device);
    void DeviceStateChanged(ISmartDevice device, string state, object value);
    void ExecuteScene(string sceneName);
    void SetSecurityMode(bool enabled);
}

// Smart device interface (Colleague)
public interface ISmartDevice
{
    string DeviceId { get; }
    string DeviceName { get; }
    string DeviceType { get; }
    bool IsOnline { get; }
    void UpdateState(string state, object value);
    void ReceiveCommand(string command, object parameter);
}

// Base smart device
public abstract class SmartDevice : ISmartDevice
{
    protected ISmartHomeMediator _mediator;
    
    public string DeviceId { get; protected set; }
    public string DeviceName { get; protected set; }
    public string DeviceType { get; protected set; }
    public bool IsOnline { get; protected set; }

    protected SmartDevice(ISmartHomeMediator mediator, string deviceId, string deviceName, string deviceType)
    {
        _mediator = mediator;
        DeviceId = deviceId;
        DeviceName = deviceName;
        DeviceType = deviceType;
        IsOnline = true;
    }

    public abstract void UpdateState(string state, object value);
    public abstract void ReceiveCommand(string command, object parameter);

    protected void NotifyStateChange(string state, object value)
    {
        Console.WriteLine($"🏠 {DeviceName} ({DeviceType}): {state} = {value}");
        _mediator?.DeviceStateChanged(this, state, value);
    }
}

// Concrete smart devices
public class SmartLight : SmartDevice
{
    public bool IsOn { get; private set; }
    public int Brightness { get; private set; } = 100;
    public string Color { get; private set; } = "white";

    public SmartLight(ISmartHomeMediator mediator, string deviceId, string deviceName)
        : base(mediator, deviceId, deviceName, "Light") { }

    public override void UpdateState(string state, object value)
    {
        switch (state.ToLower())
        {
            case "power":
                IsOn = (bool)value;
                NotifyStateChange("Power", IsOn ? "ON" : "OFF");
                break;
            case "brightness":
                if (IsOn)
                {
                    Brightness = Math.Max(1, Math.Min(100, (int)value));
                    NotifyStateChange("Brightness", $"{Brightness}%");
                }
                break;
            case "color":
                if (IsOn)
                {
                    Color = value.ToString();
                    NotifyStateChange("Color", Color);
                }
                break;
        }
    }

    public override void ReceiveCommand(string command, object parameter)
    {
        switch (command.ToLower())
        {
            case "turn_on":
                UpdateState("power", true);
                break;
            case "turn_off":
                UpdateState("power", false);
                break;
            case "set_brightness":
                UpdateState("brightness", parameter);
                break;
            case "set_color":
                UpdateState("color", parameter);
                break;
        }
    }
}

public class SmartThermostat : SmartDevice
{
    public int Temperature { get; private set; } = 72;
    public string Mode { get; private set; } = "auto";
    public bool IsHeating { get; private set; }
    public bool IsCooling { get; private set; }

    public SmartThermostat(ISmartHomeMediator mediator, string deviceId, string deviceName)
        : base(mediator, deviceId, deviceName, "Thermostat") { }

    public override void UpdateState(string state, object value)
    {
        switch (state.ToLower())
        {
            case "temperature":
                Temperature = Math.Max(40, Math.Min(90, (int)value));
                NotifyStateChange("Temperature", $"{Temperature}°F");
                break;
            case "mode":
                Mode = value.ToString().ToLower();
                NotifyStateChange("Mode", Mode);
                break;
            case "heating":
                IsHeating = (bool)value;
                IsCooling = false;
                NotifyStateChange("Status", IsHeating ? "Heating" : "Idle");
                break;
            case "cooling":
                IsCooling = (bool)value;
                IsHeating = false;
                NotifyStateChange("Status", IsCooling ? "Cooling" : "Idle");
                break;
        }
    }

    public override void ReceiveCommand(string command, object parameter)
    {
        switch (command.ToLower())
        {
            case "set_temperature":
                UpdateState("temperature", parameter);
                break;
            case "set_mode":
                UpdateState("mode", parameter);
                break;
        }
    }
}

public class SmartSecurityCamera : SmartDevice
{
    public bool IsRecording { get; private set; }
    public bool MotionDetected { get; private set; }
    public string LastMotionTime { get; private set; }

    public SmartSecurityCamera(ISmartHomeMediator mediator, string deviceId, string deviceName)
        : base(mediator, deviceId, deviceName, "Security Camera") { }

    public override void UpdateState(string state, object value)
    {
        switch (state.ToLower())
        {
            case "recording":
                IsRecording = (bool)value;
                NotifyStateChange("Recording", IsRecording ? "ON" : "OFF");
                break;
            case "motion":
                MotionDetected = (bool)value;
                if (MotionDetected)
                {
                    LastMotionTime = DateTime.Now.ToString("HH:mm:ss");
                    NotifyStateChange("Motion Detected", LastMotionTime);
                }
                break;
        }
    }

    public override void ReceiveCommand(string command, object parameter)
    {
        switch (command.ToLower())
        {
            case "start_recording":
                UpdateState("recording", true);
                break;
            case "stop_recording":
                UpdateState("recording", false);
                break;
            case "simulate_motion":
                UpdateState("motion", true);
                Task.Delay(2000).ContinueWith(_ => UpdateState("motion", false));
                break;
        }
    }
}

// Smart home hub (Concrete Mediator)
public class SmartHomeHub : ISmartHomeMediator
{
    private List<ISmartDevice> _devices;
    private Dictionary<string, List<(string deviceId, string command, object parameter)>> _scenes;
    private bool _securityMode;
    private bool _awayMode;

    public SmartHomeHub()
    {
        _devices = new List<ISmartDevice>();
        _scenes = new Dictionary<string, List<(string, string, object)>>();
        _securityMode = false;
        _awayMode = false;
        InitializeScenes();
    }

    public void RegisterDevice(ISmartDevice device)
    {
        _devices.Add(device);
        Console.WriteLine($"🔗 Device registered: {device.DeviceName} ({device.DeviceType})");
    }

    public void DeviceStateChanged(ISmartDevice device, string state, object value)
    {
        // Smart home automation logic based on device state changes
        
        if (device.DeviceType == "Security Camera" && state == "Motion Detected")
        {
            HandleMotionDetection(device);
        }
        else if (device.DeviceType == "Thermostat" && state == "Temperature")
        {
            HandleTemperatureChange(device, (string)value);
        }
        else if (device.DeviceType == "Light" && state == "Power" && value.ToString() == "ON")
        {
            HandleLightTurnedOn(device);
        }
    }

    public void ExecuteScene(string sceneName)
    {
        if (_scenes.ContainsKey(sceneName))
        {
            Console.WriteLine($"🎬 Executing scene: {sceneName}");
            var commands = _scenes[sceneName];
            
            foreach (var (deviceId, command, parameter) in commands)
            {
                var device = _devices.FirstOrDefault(d => d.DeviceId == deviceId);
                if (device != null && device.IsOnline)
                {
                    device.ReceiveCommand(command, parameter);
                }
            }
        }
        else
        {
            Console.WriteLine($"❌ Scene '{sceneName}' not found");
        }
    }

    public void SetSecurityMode(bool enabled)
    {
        _securityMode = enabled;
        Console.WriteLine($"🔒 Security mode: {(enabled ? "ENABLED" : "DISABLED")}");
        
        if (enabled)
        {
            // Turn on all security cameras
            foreach (var camera in _devices.OfType<SmartSecurityCamera>())
            {
                camera.ReceiveCommand("start_recording", null);
            }
        }
    }

    public void SetAwayMode(bool enabled)
    {
        _awayMode = enabled;
        Console.WriteLine($"🏃 Away mode: {(enabled ? "ENABLED" : "DISABLED")}");
        
        if (enabled)
        {
            ExecuteScene("Away");
            SetSecurityMode(true);
        }
        else
        {
            ExecuteScene("Welcome Home");
            SetSecurityMode(false);
        }
    }

    private void HandleMotionDetection(ISmartDevice camera)
    {
        Console.WriteLine($"🚨 Motion detected by {camera.DeviceName}!");
        
        if (_securityMode || _awayMode)
        {
            // Security response
            Console.WriteLine("🔔 Security alert triggered!");
            
            // Turn on nearby lights
            var lights = _devices.OfType<SmartLight>();
            foreach (var light in lights)
            {
                light.ReceiveCommand("turn_on", null);
                light.ReceiveCommand("set_brightness", 100);
            }
        }
        else
        {
            // Normal response - gentle lighting
            var entryLight = _devices.OfType<SmartLight>().FirstOrDefault(l => l.DeviceName.Contains("Entry"));
            entryLight?.ReceiveCommand("turn_on", null);
            entryLight?.ReceiveCommand("set_brightness", 50);
        }
    }

    private void HandleTemperatureChange(ISmartDevice thermostat, string tempValue)
    {
        // Energy saving logic
        if (tempValue.Contains("°F"))
        {
            var temp = int.Parse(tempValue.Replace("°F", ""));
            
            if (temp > 78 && !_awayMode)
            {
                Console.WriteLine("💡 Energy saving tip: Consider raising thermostat setting");
                
                // Dim lights to reduce heat
                foreach (var light in _devices.OfType<SmartLight>().Where(l => l.IsOn))
                {
                    light.ReceiveCommand("set_brightness", 70);
                }
            }
        }
    }

    private void HandleLightTurnedOn(ISmartDevice light)
    {
        if (_awayMode)
        {
            Console.WriteLine($"⚠️ Light turned on while in away mode: {light.DeviceName}");
        }
    }

    private void InitializeScenes()
    {
        // Good Morning scene
        _scenes["Good Morning"] = new List<(string, string, object)>
        {
            ("light_1", "turn_on", null),
            ("light_1", "set_brightness", 80),
            ("light_2", "turn_on", null),
            ("light_2", "set_brightness", 60),
            ("thermostat_1", "set_temperature", 75)
        };

        // Movie Night scene
        _scenes["Movie Night"] = new List<(string, string, object)>
        {
            ("light_1", "turn_on", null),
            ("light_1", "set_brightness", 20),
            ("light_1", "set_color", "red"),
            ("light_2", "turn_off", null),
            ("thermostat_1", "set_temperature", 72)
        };

        // Bedtime scene
        _scenes["Bedtime"] = new List<(string, string, object)>
        {
            ("light_1", "turn_off", null),
            ("light_2", "turn_off", null),
            ("thermostat_1", "set_temperature", 68)
        };

        // Away scene
        _scenes["Away"] = new List<(string, string, object)>
        {
            ("light_1", "turn_off", null),
            ("light_2", "turn_off", null),
            ("thermostat_1", "set_temperature", 65)
        };

        // Welcome Home scene
        _scenes["Welcome Home"] = new List<(string, string, object)>
        {
            ("light_1", "turn_on", null),
            ("light_1", "set_brightness", 100),
            ("thermostat_1", "set_temperature", 72)
        };
    }

    public void ShowSystemStatus()
    {
        Console.WriteLine($"\n🏠 Smart Home System Status:");
        Console.WriteLine($"   Security Mode: {(_securityMode ? "ON" : "OFF")}");
        Console.WriteLine($"   Away Mode: {(_awayMode ? "ON" : "OFF")}");
        Console.WriteLine($"   Connected Devices: {_devices.Count}");
        Console.WriteLine($"   Available Scenes: {string.Join(", ", _scenes.Keys)}");
        
        Console.WriteLine($"\n📱 Device Status:");
        foreach (var device in _devices)
        {
            var status = device.IsOnline ? "🟢 Online" : "🔴 Offline";
            Console.WriteLine($"   {status} - {device.DeviceName} ({device.DeviceType})");
        }
    }
}

// Usage
var smartHome = new SmartHomeHub();

Console.WriteLine("\n=== Smart Home Automation System ===");

// Register devices
var livingRoomLight = new SmartLight(smartHome, "light_1", "Living Room Light");
var bedroomLight = new SmartLight(smartHome, "light_2", "Bedroom Light");
var mainThermostat = new SmartThermostat(smartHome, "thermostat_1", "Main Thermostat");
var frontCamera = new SmartSecurityCamera(smartHome, "camera_1", "Front Door Camera");

smartHome.RegisterDevice(livingRoomLight);
smartHome.RegisterDevice(bedroomLight);
smartHome.RegisterDevice(mainThermostat);
smartHome.RegisterDevice(frontCamera);

smartHome.ShowSystemStatus();

// Test individual device control
Console.WriteLine("\n=== Individual Device Control ===");
livingRoomLight.ReceiveCommand("turn_on", null);
livingRoomLight.ReceiveCommand("set_color", "blue");
mainThermostat.ReceiveCommand("set_temperature", 75);

// Test scenes
Console.WriteLine("\n=== Scene Execution ===");
smartHome.ExecuteScene("Good Morning");
Thread.Sleep(1000);

smartHome.ExecuteScene("Movie Night");
Thread.Sleep(1000);

// Test automation
Console.WriteLine("\n=== Automation Testing ===");
smartHome.SetAwayMode(true);
Thread.Sleep(1000);

frontCamera.ReceiveCommand("simulate_motion", null);
Thread.Sleep(3000);

smartHome.SetAwayMode(false);
smartHome.ShowSystemStatus();
```

### 2. **Trading Platform System**
```csharp
// Trading mediator interface
public interface ITradingMediator
{
    void RegisterTrader(ITrader trader);
    void RegisterBroker(IBroker broker);
    void PlaceOrder(Order order, ITrader trader);
    void ExecuteTrade(Trade trade);
    void NotifyPriceChange(string symbol, decimal price);
    void NotifyTradeExecution(Trade trade);
}

// Market participant interfaces
public interface ITrader
{
    string TraderId { get; }
    string Name { get; }
    decimal Balance { get; }
    void ReceivePriceUpdate(string symbol, decimal price);
    void ReceiveTradeConfirmation(Trade trade);
    void ReceiveOrderUpdate(Order order, OrderStatus status);
}

public interface IBroker
{
    string BrokerId { get; }
    void ProcessOrder(Order order);
    void NotifyOrderFilled(Order order, Trade trade);
}

// Trading entities
public enum OrderType { Buy, Sell }
public enum OrderStatus { Pending, PartiallyFilled, Filled, Cancelled }

public class Order
{
    public string OrderId { get; set; } = Guid.NewGuid().ToString();
    public string Symbol { get; set; }
    public OrderType Type { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string TraderId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
}

public class Trade
{
    public string TradeId { get; set; } = Guid.NewGuid().ToString();
    public string Symbol { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string BuyerId { get; set; }
    public string SellerId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

// Concrete traders
public class RetailTrader : ITrader
{
    private ITradingMediator _mediator;
    private Dictionary<string, decimal> _portfolio;
    private List<Order> _activeOrders;

    public string TraderId { get; }
    public string Name { get; }
    public decimal Balance { get; private set; }

    public RetailTrader(ITradingMediator mediator, string traderId, string name, decimal initialBalance)
    {
        _mediator = mediator;
        TraderId = traderId;
        Name = name;
        Balance = initialBalance;
        _portfolio = new Dictionary<string, decimal>();
        _activeOrders = new List<Order>();
    }

    public void PlaceBuyOrder(string symbol, int quantity, decimal price)
    {
        var totalCost = quantity * price;
        if (Balance >= totalCost)
        {
            var order = new Order
            {
                Symbol = symbol,
                Type = OrderType.Buy,
                Quantity = quantity,
                Price = price,
                TraderId = TraderId
            };

            _activeOrders.Add(order);
            Console.WriteLine($"📈 {Name} placed BUY order: {quantity} {symbol} @ ${price}");
            _mediator.PlaceOrder(order, this);
        }
        else
        {
            Console.WriteLine($"❌ {Name} insufficient funds for order");
        }
    }

    public void PlaceSellOrder(string symbol, int quantity, decimal price)
    {
        if (_portfolio.ContainsKey(symbol) && _portfolio[symbol] >= quantity)
        {
            var order = new Order
            {
                Symbol = symbol,
                Type = OrderType.Sell,
                Quantity = quantity,
                Price = price,
                TraderId = TraderId
            };

            _activeOrders.Add(order);
            Console.WriteLine($"📉 {Name} placed SELL order: {quantity} {symbol} @ ${price}");
            _mediator.PlaceOrder(order, this);
        }
        else
        {
            Console.WriteLine($"❌ {Name} insufficient shares for sale");
        }
    }

    public void ReceivePriceUpdate(string symbol, decimal price)
    {
        // React to price changes
        if (_portfolio.ContainsKey(symbol))
        {
            Console.WriteLine($"💰 {Name} sees {symbol} price: ${price}");
        }
    }

    public void ReceiveTradeConfirmation(Trade trade)
    {
        if (trade.BuyerId == TraderId)
        {
            // Bought shares
            Balance -= trade.Quantity * trade.Price;
            _portfolio[trade.Symbol] = _portfolio.GetValueOrDefault(trade.Symbol, 0) + trade.Quantity;
            Console.WriteLine($"✅ {Name} bought {trade.Quantity} {trade.Symbol} @ ${trade.Price}");
        }
        else if (trade.SellerId == TraderId)
        {
            // Sold shares
            Balance += trade.Quantity * trade.Price;
            _portfolio[trade.Symbol] -= trade.Quantity;
            Console.WriteLine($"✅ {Name} sold {trade.Quantity} {trade.Symbol} @ ${trade.Price}");
        }
    }

    public void ReceiveOrderUpdate(Order order, OrderStatus status)
    {
        var activeOrder = _activeOrders.FirstOrDefault(o => o.OrderId == order.OrderId);
        if (activeOrder != null)
        {
            activeOrder.Status = status;
            Console.WriteLine($"📋 {Name} order update: {order.Symbol} - {status}");
            
            if (status == OrderStatus.Filled || status == OrderStatus.Cancelled)
            {
                _activeOrders.Remove(activeOrder);
            }
        }
    }

    public void ShowPortfolio()
    {
        Console.WriteLine($"\n💼 {Name}'s Portfolio:");
        Console.WriteLine($"   Cash Balance: ${Balance:F2}");
        Console.WriteLine($"   Holdings:");
        foreach (var holding in _portfolio.Where(h => h.Value > 0))
        {
            Console.WriteLine($"     {holding.Key}: {holding.Value} shares");
        }
        Console.WriteLine($"   Active Orders: {_activeOrders.Count}");
    }
}

public class InstitutionalTrader : ITrader
{
    private ITradingMediator _mediator;
    private Dictionary<string, decimal> _portfolio;
    private decimal _riskLimit;

    public string TraderId { get; }
    public string Name { get; }
    public decimal Balance { get; private set; }

    public InstitutionalTrader(ITradingMediator mediator, string traderId, string name, decimal initialBalance)
    {
        _mediator = mediator;
        TraderId = traderId;
        Name = name;
        Balance = initialBalance;
        _portfolio = new Dictionary<string, decimal>();
        _riskLimit = initialBalance * 0.1m; // 10% risk limit
    }

    public void PlaceLargeOrder(string symbol, int quantity, decimal price, OrderType type)
    {
        // Split large orders into smaller chunks
        var chunkSize = 100;
        var chunks = (int)Math.Ceiling((double)quantity / chunkSize);

        Console.WriteLine($"🏦 {Name} splitting large order into {chunks} chunks");

        for (int i = 0; i < chunks; i++)
        {
            var chunkQuantity = Math.Min(chunkSize, quantity - (i * chunkSize));
            var order = new Order
            {
                Symbol = symbol,
                Type = type,
                Quantity = chunkQuantity,
                Price = price,
                TraderId = TraderId
            };

            Console.WriteLine($"📊 {Name} placing chunk {i + 1}/{chunks}: {chunkQuantity} {symbol}");
            _mediator.PlaceOrder(order, this);

            // Delay between chunks to avoid market impact
            if (i < chunks - 1)
                Thread.Sleep(100);
        }
    }

    public void ReceivePriceUpdate(string symbol, decimal price)
    {
        // Institutional traders might have algorithmic responses
        Console.WriteLine($"🏛️ {Name} analyzing {symbol} price: ${price}");
    }

    public void ReceiveTradeConfirmation(Trade trade)
    {
        if (trade.BuyerId == TraderId)
        {
            Balance -= trade.Quantity * trade.Price;
            _portfolio[trade.Symbol] = _portfolio.GetValueOrDefault(trade.Symbol, 0) + trade.Quantity;
            Console.WriteLine($"🏦 {Name} institutional buy: {trade.Quantity} {trade.Symbol} @ ${trade.Price}");
        }
        else if (trade.SellerId == TraderId)
        {
            Balance += trade.Quantity * trade.Price;
            _portfolio[trade.Symbol] -= trade.Quantity;
            Console.WriteLine($"🏦 {Name} institutional sell: {trade.Quantity} {trade.Symbol} @ ${trade.Price}");
        }
    }

    public void ReceiveOrderUpdate(Order order, OrderStatus status)
    {
        Console.WriteLine($"🏛️ {Name} institutional order update: {order.Symbol} - {status}");
    }
}

// Market maker broker
public class MarketMakerBroker : IBroker
{
    private Dictionary<string, List<Order>> _orderBook;
    private Dictionary<string, decimal> _currentPrices;
    private ITradingMediator _mediator;

    public string BrokerId { get; }

    public MarketMakerBroker(ITradingMediator mediator, string brokerId)
    {
        _mediator = mediator;
        BrokerId = brokerId;
        _orderBook = new Dictionary<string, List<Order>>();
        _currentPrices = new Dictionary<string, decimal>
        {
            { "AAPL", 150.00m },
            { "GOOGL", 2500.00m },
            { "MSFT", 300.00m },
            { "TSLA", 800.00m }
        };
    }

    public void ProcessOrder(Order order)
    {
        Console.WriteLine($"🏢 Broker processing: {order.Type} {order.Quantity} {order.Symbol} @ ${order.Price}");

        if (!_orderBook.ContainsKey(order.Symbol))
        {
            _orderBook[order.Symbol] = new List<Order>();
        }

        _orderBook[order.Symbol].Add(order);

        // Try to match orders
        TryMatchOrders(order.Symbol);
    }

    public void NotifyOrderFilled(Order order, Trade trade)
    {
        Console.WriteLine($"📋 Order {order.OrderId} filled via trade {trade.TradeId}");
    }

    private void TryMatchOrders(string symbol)
    {
        var orders = _orderBook[symbol];
        var buyOrders = orders.Where(o => o.Type == OrderType.Buy && o.Status == OrderStatus.Pending)
                              .OrderByDescending(o => o.Price) // Highest price first
                              .ToList();
        
        var sellOrders = orders.Where(o => o.Type == OrderType.Sell && o.Status == OrderStatus.Pending)
                               .OrderBy(o => o.Price) // Lowest price first
                               .ToList();

        while (buyOrders.Any() && sellOrders.Any())
        {
            var buyOrder = buyOrders.First();
            var sellOrder = sellOrders.First();

            if (buyOrder.Price >= sellOrder.Price)
            {
                // Match found
                var tradeQuantity = Math.Min(buyOrder.Quantity, sellOrder.Quantity);
                var tradePrice = sellOrder.Price; // Use sell price

                var trade = new Trade
                {
                    Symbol = symbol,
                    Quantity = tradeQuantity,
                    Price = tradePrice,
                    BuyerId = buyOrder.TraderId,
                    SellerId = sellOrder.TraderId
                };

                Console.WriteLine($"💱 Trade executed: {tradeQuantity} {symbol} @ ${tradePrice}");

                // Update order quantities
                buyOrder.Quantity -= tradeQuantity;
                sellOrder.Quantity -= tradeQuantity;

                // Update order statuses
                if (buyOrder.Quantity == 0)
                {
                    buyOrder.Status = OrderStatus.Filled;
                    buyOrders.Remove(buyOrder);
                }

                if (sellOrder.Quantity == 0)
                {
                    sellOrder.Status = OrderStatus.Filled;
                    sellOrders.Remove(sellOrder);
                }

                // Update current price
                _currentPrices[symbol] = tradePrice;

                // Notify mediator
                _mediator.ExecuteTrade(trade);
                _mediator.NotifyPriceChange(symbol, tradePrice);
            }
            else
            {
                break; // No more matches possible
            }
        }
    }

    public void ShowOrderBook(string symbol)
    {
        if (_orderBook.ContainsKey(symbol))
        {
            Console.WriteLine($"\n📊 Order Book for {symbol}:");
            var orders = _orderBook[symbol].Where(o => o.Status == OrderStatus.Pending);
            
            var buyOrders = orders.Where(o => o.Type == OrderType.Buy).OrderByDescending(o => o.Price);
            var sellOrders = orders.Where(o => o.Type == OrderType.Sell).OrderBy(o => o.Price);

            Console.WriteLine("  BUY ORDERS:");
            foreach (var order in buyOrders.Take(5))
            {
                Console.WriteLine($"    {order.Quantity} @ ${order.Price}");
            }

            Console.WriteLine("  SELL ORDERS:");
            foreach (var order in sellOrders.Take(5))
            {
                Console.WriteLine($"    {order.Quantity} @ ${order.Price}");
            }
        }
    }
}

// Trading platform (Concrete Mediator)
public class TradingPlatform : ITradingMediator
{
    private List<ITrader> _traders;
    private List<IBroker> _brokers;
    private List<Trade> _tradeHistory;
    private Dictionary<string, decimal> _marketPrices;

    public TradingPlatform()
    {
        _traders = new List<ITrader>();
        _brokers = new List<IBroker>();
        _tradeHistory = new List<Trade>();
        _marketPrices = new Dictionary<string, decimal>();
    }

    public void RegisterTrader(ITrader trader)
    {
        _traders.Add(trader);
        Console.WriteLine($"👤 Trader registered: {trader.Name} ({trader.TraderId})");
    }

    public void RegisterBroker(IBroker broker)
    {
        _brokers.Add(broker);
        Console.WriteLine($"🏢 Broker registered: {broker.BrokerId}");
    }

    public void PlaceOrder(Order order, ITrader trader)
    {
        Console.WriteLine($"📝 Order placed by {trader.Name}: {order.Type} {order.Quantity} {order.Symbol}");
        
        // Route to first available broker (in real system, this would be more sophisticated)
        var broker = _brokers.FirstOrDefault();
        if (broker != null)
        {
            broker.ProcessOrder(order);
        }
        else
        {
            Console.WriteLine("❌ No brokers available to process order");
        }
    }

    public void ExecuteTrade(Trade trade)
    {
        _tradeHistory.Add(trade);
        Console.WriteLine($"✅ Trade executed: {trade.Symbol} - {trade.Quantity} @ ${trade.Price}");

        // Notify relevant traders
        var buyer = _traders.FirstOrDefault(t => t.TraderId == trade.BuyerId);
        var seller = _traders.FirstOrDefault(t => t.TraderId == trade.SellerId);

        buyer?.ReceiveTradeConfirmation(trade);
        seller?.ReceiveTradeConfirmation(trade);
    }

    public void NotifyPriceChange(string symbol, decimal price)
    {
        _marketPrices[symbol] = price;
        Console.WriteLine($"📈 Price update: {symbol} = ${price}");

        // Notify all traders
        foreach (var trader in _traders)
        {
            trader.ReceivePriceUpdate(symbol, price);
        }
    }

    public void NotifyTradeExecution(Trade trade)
    {
        Console.WriteLine($"🔔 Trade notification: {trade.Symbol} traded at ${trade.Price}");
    }

    public void ShowMarketSummary()
    {
        Console.WriteLine($"\n📊 Market Summary:");
        Console.WriteLine($"   Active Traders: {_traders.Count}");
        Console.WriteLine($"   Active Brokers: {_brokers.Count}");
        Console.WriteLine($"   Total Trades: {_tradeHistory.Count}");
        
        Console.WriteLine($"   Current Prices:");
        foreach (var price in _marketPrices)
        {
            Console.WriteLine($"     {price.Key}: ${price.Value}");
        }

        if (_tradeHistory.Any())
        {
            Console.WriteLine($"   Recent Trades:");
            foreach (var trade in _tradeHistory.TakeLast(3))
            {
                Console.WriteLine($"     {trade.Symbol}: {trade.Quantity} @ ${trade.Price} at {trade.Timestamp:HH:mm:ss}");
            }
        }
    }
}

// Usage
var platform = new TradingPlatform();

Console.WriteLine("\n=== Trading Platform System ===");

// Register brokers
var marketMaker = new MarketMakerBroker(platform, "MM001");
platform.RegisterBroker(marketMaker);

// Register traders
var alice = new RetailTrader(platform, "T001", "Alice", 10000m);
var bob = new RetailTrader(platform, "T002", "Bob", 15000m);
var institutional = new InstitutionalTrader(platform, "I001", "BigBank Capital", 1000000m);

platform.RegisterTrader(alice);
platform.RegisterTrader(bob);
platform.RegisterTrader(institutional);

// Simulate trading
Console.WriteLine("\n=== Trading Simulation ===");

alice.PlaceBuyOrder("AAPL", 50, 149.50m);
bob.PlaceSellOrder("AAPL", 30, 150.50m);

Thread.Sleep(500);

institutional.PlaceLargeOrder("AAPL", 500, 150.00m, OrderType.Buy);

Thread.Sleep(1000);

alice.PlaceSellOrder("AAPL", 20, 151.00m);

Thread.Sleep(500);

// Show results
marketMaker.ShowOrderBook("AAPL");
alice.ShowPortfolio();
bob.ShowPortfolio();
platform.ShowMarketSummary();
```

## Benefits
- **Reduced Coupling**: Objects don't need to know about each other directly
- **Centralized Control**: Complex interactions managed in one place
- **Easier Maintenance**: Changes to interaction logic happen in one place
- **Reusable Components**: Colleagues can be reused in different contexts
- **Protocol Independence**: Can change communication protocol without affecting colleagues

## Drawbacks
- **Complexity**: Mediator can become overly complex
- **Single Point of Failure**: All communication goes through one object
- **Performance**: Additional indirection may impact performance
- **God Object**: Risk of mediator becoming a monolithic controller

## When to Use
✅ **Use When:**
- Objects communicate in complex but well-defined ways
- Reusing objects is difficult due to many dependencies
- Behavior distributed between classes should be customizable
- You want to centralize complex communications and control logic

❌ **Avoid When:**
- Simple object interactions that don't benefit from centralization
- Performance is critical and indirection is costly
- The mediator would become too complex
- Objects need tight coupling for efficiency

## Mediator vs Other Patterns

| Pattern | Purpose | Key Difference |
|---------|---------|----------------|
| **Mediator** | Centralizes communication | Focuses on interaction coordination |
| **Observer** | Notifies multiple objects | Focuses on event broadcasting |
| **Command** | Encapsulates requests | Focuses on request encapsulation |
| **Facade** | Simplifies interface | Focuses on interface simplification |

## Best Practices
1. **Keep Mediator Focused**: Don't let it become a god object
2. **Use Events**: Leverage event-driven communication when possible
3. **Interface Segregation**: Create specific mediator interfaces for different contexts
4. **Stateless When Possible**: Minimize state in mediator to reduce complexity
5. **Documentation**: Clearly document interaction protocols

## Common Mistakes
1. **Overly Complex Mediator**: Putting too much logic in one mediator
2. **Tight Coupling to Mediator**: Making colleagues too dependent on specific mediator
3. **Not Using Interfaces**: Direct coupling to concrete mediator classes
4. **State Management**: Poor management of mediator state

## Modern C# Features
```csharp
// Using events for loose coupling
public class EventDrivenMediator
{
    public event Action<string, object> ComponentInteraction;

    public void Notify(string eventType, object data)
    {
        ComponentInteraction?.Invoke(eventType, data);
    }
}

// Using dependency injection
public interface INotificationService
{
    Task SendNotificationAsync(string message, string recipient);
}

public class MediatorWithDI
{
    private readonly INotificationService _notificationService;

    public MediatorWithDI(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task HandleUserAction(string action, string userId)
    {
        await _notificationService.SendNotificationAsync($"User performed: {action}", userId);
    }
}

// Using MediatR library pattern
public class Request : IRequest<Response>
{
    public string Data { get; set; }
}

public class Response
{
    public string Result { get; set; }
}

public class Handler : IRequestHandler<Request, Response>
{
    public Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Result = $"Processed: {request.Data}" });
    }
}
```

## Testing Mediators
```csharp
[Test]
public void Mediator_Notify_CallsCorrectComponents()
{
    // Arrange
    var component1 = new Mock<IComponent>();
    var component2 = new Mock<IComponent>();
    var mediator = new ConcreteMediator(component1.Object, component2.Object);

    // Act
    mediator.Notify(component1.Object, "TestEvent");

    // Assert
    component2.Verify(c => c.React(), Times.Once);
    component1.Verify(c => c.React(), Times.Never);
}
```

## Summary
The Mediator pattern is like having a skilled conductor for an orchestra - instead of musicians trying to coordinate directly with each other (which would be chaos), they all follow the conductor who orchestrates the entire performance. The conductor knows when each section should play, how loud, and how the different parts should work together harmoniously.

In software, the mediator serves as the central coordinator that manages complex interactions between objects, preventing them from becoming tightly coupled while ensuring they work together effectively. It's perfect for systems where many objects need to interact in sophisticated ways, like chat systems, trading platforms, or smart home automation.

The key insight is that sometimes the best way to manage complexity is to centralize it in one place rather than distribute it across many objects, making the system easier to understand, maintain, and extend.
