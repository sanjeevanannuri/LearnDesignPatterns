# Chain of Responsibility Pattern

## Overview
The Chain of Responsibility pattern passes requests along a chain of handlers. Upon receiving a request, each handler decides whether to process it or pass it to the next handler in the chain. Think of it like a customer service escalation system - your issue goes from first-level support to a supervisor to a manager until someone who can handle it takes responsibility.

## Problem It Solves
Imagine you're building an expense approval system where different amounts require different levels of authorization:
- Amounts under $100: Team Lead can approve
- Amounts $100-$1000: Manager can approve  
- Amounts $1000-$10000: Director can approve
- Amounts over $10000: CEO must approve

Without Chain of Responsibility, you'd have complex conditional logic:
```csharp
// BAD: Complex conditional logic
public bool ApproveExpense(decimal amount, string requester)
{
    if (amount < 100)
    {
        if (IsTeamLead(requester))
            return ApproveAsTeamLead(amount);
        else if (IsManager(requester))
            return ApproveAsManager(amount);
        // ... more conditions
    }
    else if (amount < 1000)
    {
        if (IsManager(requester) || IsDirector(requester) || IsCEO(requester))
            return ApproveAsManager(amount);
        // ... more conditions
    }
    // This gets very complex and hard to maintain
}
```

This approach violates the Open/Closed Principle and becomes unwieldy.

## Real-World Analogy
Think of a **technical support ticket system**:
1. **Level 1 Support** (First Handler): Handles common issues like password resets
2. **Level 2 Support** (Second Handler): Handles software troubleshooting  
3. **Engineering Team** (Third Handler): Handles complex technical issues
4. **Management** (Final Handler): Handles escalations and policy decisions

Each level tries to resolve the issue. If they can't, they pass it up the chain. The customer doesn't need to know the internal structure - they just submit a ticket.

Or consider **exception handling in programming**:
- **Try Block**: Attempts to handle the operation
- **Catch Block 1**: Handles specific exceptions (FileNotFoundException)
- **Catch Block 2**: Handles broader exceptions (IOException)  
- **Catch Block 3**: Handles all exceptions (Exception)

## Implementation Details

### Basic Structure
```csharp
// Handler interface
public abstract class Handler
{
    protected Handler _nextHandler;

    public void SetNext(Handler handler)
    {
        _nextHandler = handler;
    }

    public abstract void HandleRequest(Request request);
}

// Concrete handlers
public class ConcreteHandlerA : Handler
{
    public override void HandleRequest(Request request)
    {
        if (CanHandle(request))
        {
            // Handle the request
            Console.WriteLine($"ConcreteHandlerA handling {request.Type}");
        }
        else if (_nextHandler != null)
        {
            // Pass to next handler
            _nextHandler.HandleRequest(request);
        }
        else
        {
            Console.WriteLine("No handler available for this request");
        }
    }

    private bool CanHandle(Request request)
    {
        return request.Type == "TypeA";
    }
}

public class ConcreteHandlerB : Handler
{
    public override void HandleRequest(Request request)
    {
        if (CanHandle(request))
        {
            Console.WriteLine($"ConcreteHandlerB handling {request.Type}");
        }
        else if (_nextHandler != null)
        {
            _nextHandler.HandleRequest(request);
        }
        else
        {
            Console.WriteLine("No handler available for this request");
        }
    }

    private bool CanHandle(Request request)
    {
        return request.Type == "TypeB";
    }
}

// Request class
public class Request
{
    public string Type { get; set; }
    public object Data { get; set; }
}
```

### Key Components
1. **Handler**: Abstract base class defining interface for handling requests
2. **ConcreteHandler**: Implements the handling logic for specific requests
3. **Client**: Initiates requests to the first handler in the chain
4. **Request**: Contains the data being processed through the chain

## Example from Our Code
```csharp
// Request class - expense approval request
public class ExpenseRequest
{
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public string Requester { get; set; }
    public DateTime RequestDate { get; set; }

    public ExpenseRequest(decimal amount, string description, string requester)
    {
        Amount = amount;
        Description = description;
        Requester = requester;
        RequestDate = DateTime.Now;
    }

    public override string ToString()
    {
        return $"Expense: ${Amount} for '{Description}' by {Requester}";
    }
}

// Abstract handler
public abstract class ExpenseHandler
{
    protected ExpenseHandler _nextHandler;
    protected string _handlerName;
    protected decimal _approvalLimit;

    public ExpenseHandler(string handlerName, decimal approvalLimit)
    {
        _handlerName = handlerName;
        _approvalLimit = approvalLimit;
    }

    public void SetNext(ExpenseHandler handler)
    {
        _nextHandler = handler;
    }

    public abstract void ProcessRequest(ExpenseRequest request);

    protected virtual void ApproveRequest(ExpenseRequest request)
    {
        Console.WriteLine($"✅ {_handlerName} APPROVED: {request}");
    }

    protected virtual void PassToNext(ExpenseRequest request)
    {
        if (_nextHandler != null)
        {
            Console.WriteLine($"⬆️ {_handlerName} passing to next level: {request}");
            _nextHandler.ProcessRequest(request);
        }
        else
        {
            Console.WriteLine($"❌ {_handlerName} REJECTED - No higher authority available: {request}");
        }
    }
}

// Concrete handlers
public class TeamLeadHandler : ExpenseHandler
{
    public TeamLeadHandler() : base("Team Lead", 100) { }

    public override void ProcessRequest(ExpenseRequest request)
    {
        Console.WriteLine($"📝 {_handlerName} reviewing: {request}");

        if (request.Amount <= _approvalLimit)
        {
            ApproveRequest(request);
        }
        else
        {
            PassToNext(request);
        }
    }
}

public class ManagerHandler : ExpenseHandler
{
    public ManagerHandler() : base("Manager", 1000) { }

    public override void ProcessRequest(ExpenseRequest request)
    {
        Console.WriteLine($"📋 {_handlerName} reviewing: {request}");

        if (request.Amount <= _approvalLimit)
        {
            // Additional validation for managers
            if (IsValidManagerExpense(request))
            {
                ApproveRequest(request);
            }
            else
            {
                Console.WriteLine($"❌ {_handlerName} REJECTED - Invalid expense category: {request}");
            }
        }
        else
        {
            PassToNext(request);
        }
    }

    private bool IsValidManagerExpense(ExpenseRequest request)
    {
        // Simulate business rules
        return !request.Description.ToLower().Contains("personal");
    }
}

public class DirectorHandler : ExpenseHandler
{
    public DirectorHandler() : base("Director", 10000) { }

    public override void ProcessRequest(ExpenseRequest request)
    {
        Console.WriteLine($"📊 {_handlerName} reviewing: {request}");

        if (request.Amount <= _approvalLimit)
        {
            // Directors require justification for large expenses
            if (request.Amount > 5000)
            {
                Console.WriteLine($"🔍 {_handlerName} requesting additional justification for large expense");
            }
            ApproveRequest(request);
        }
        else
        {
            PassToNext(request);
        }
    }
}

public class CEOHandler : ExpenseHandler
{
    public CEOHandler() : base("CEO", decimal.MaxValue) { }

    public override void ProcessRequest(ExpenseRequest request)
    {
        Console.WriteLine($"👔 {_handlerName} reviewing: {request}");

        // CEO can approve any amount but may have special conditions
        if (request.Amount > 50000)
        {
            Console.WriteLine($"🏛️ {_handlerName} - Large expense requires board approval");
            Console.WriteLine($"📄 {_handlerName} preparing board presentation for: {request}");
        }

        ApproveRequest(request);
    }
}

// Usage - building the chain
var teamLead = new TeamLeadHandler();
var manager = new ManagerHandler();
var director = new DirectorHandler();
var ceo = new CEOHandler();

// Build the chain
teamLead.SetNext(manager);
manager.SetNext(director);
director.SetNext(ceo);

// Test different expense amounts
var expenses = new[]
{
    new ExpenseRequest(50, "Office supplies", "John"),
    new ExpenseRequest(500, "Software license", "Alice"),
    new ExpenseRequest(5000, "Team training program", "Bob"),
    new ExpenseRequest(25000, "New server hardware", "Carol"),
    new ExpenseRequest(75000, "Office renovation", "David")
};

foreach (var expense in expenses)
{
    Console.WriteLine($"\n{'='*60}");
    teamLead.ProcessRequest(expense);
}
```

## Real-World Examples

### 1. **Logging System with Different Levels**
```csharp
// Log levels enum
public enum LogLevel
{
    Debug = 1,
    Info = 2,
    Warning = 3,
    Error = 4,
    Fatal = 5
}

// Log entry class
public class LogEntry
{
    public LogLevel Level { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
    public string Source { get; set; }

    public LogEntry(LogLevel level, string message, string source = "")
    {
        Level = level;
        Message = message;
        Timestamp = DateTime.Now;
        Source = source;
    }

    public override string ToString()
    {
        return $"[{Timestamp:HH:mm:ss}] {Level}: {Message} (Source: {Source})";
    }
}

// Abstract logger handler
public abstract class Logger
{
    protected Logger _nextLogger;
    protected LogLevel _logLevel;
    protected string _loggerName;

    public Logger(LogLevel logLevel, string loggerName)
    {
        _logLevel = logLevel;
        _loggerName = loggerName;
    }

    public void SetNext(Logger logger)
    {
        _nextLogger = logger;
    }

    public void Log(LogEntry entry)
    {
        if (entry.Level >= _logLevel)
        {
            WriteLog(entry);
        }

        // Always pass to next logger (unlike some chain implementations)
        _nextLogger?.Log(entry);
    }

    protected abstract void WriteLog(LogEntry entry);
}

// Concrete loggers
public class ConsoleLogger : Logger
{
    public ConsoleLogger(LogLevel logLevel) : base(logLevel, "Console") { }

    protected override void WriteLog(LogEntry entry)
    {
        var color = entry.Level switch
        {
            LogLevel.Debug => ConsoleColor.Gray,
            LogLevel.Info => ConsoleColor.White,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Fatal => ConsoleColor.Magenta,
            _ => ConsoleColor.White
        };

        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine($"[{_loggerName}] {entry}");
        Console.ForegroundColor = originalColor;
    }
}

public class FileLogger : Logger
{
    private string _filePath;

    public FileLogger(LogLevel logLevel, string filePath) : base(logLevel, "File")
    {
        _filePath = filePath;
    }

    protected override void WriteLog(LogEntry entry)
    {
        var logText = $"[{_loggerName}] {entry}";
        File.AppendAllText(_filePath, logText + Environment.NewLine);
        Console.WriteLine($"📁 Logged to file: {_filePath}");
    }
}

public class EmailLogger : Logger
{
    private string _emailAddress;

    public EmailLogger(LogLevel logLevel, string emailAddress) : base(logLevel, "Email")
    {
        _emailAddress = emailAddress;
    }

    protected override void WriteLog(LogEntry entry)
    {
        // Simulate sending email for critical errors
        Console.WriteLine($"📧 Sending email alert to {_emailAddress}: {entry}");
    }
}

public class DatabaseLogger : Logger
{
    private string _connectionString;

    public DatabaseLogger(LogLevel logLevel, string connectionString) : base(logLevel, "Database")
    {
        _connectionString = connectionString;
    }

    protected override void WriteLog(LogEntry entry)
    {
        // Simulate database logging
        Console.WriteLine($"💾 Logging to database: {entry}");
    }
}

// Usage - setting up logging chain
var consoleLogger = new ConsoleLogger(LogLevel.Debug);
var fileLogger = new FileLogger(LogLevel.Info, "app.log");
var emailLogger = new EmailLogger(LogLevel.Error, "admin@company.com");
var dbLogger = new DatabaseLogger(LogLevel.Warning, "Server=localhost;Database=Logs");

// Build chain
consoleLogger.SetNext(fileLogger);
fileLogger.SetNext(dbLogger);
dbLogger.SetNext(emailLogger);

// Test logging
var logEntries = new[]
{
    new LogEntry(LogLevel.Debug, "Application started", "Main"),
    new LogEntry(LogLevel.Info, "User logged in", "Auth"),
    new LogEntry(LogLevel.Warning, "Low disk space", "System"),
    new LogEntry(LogLevel.Error, "Database connection failed", "DataAccess"),
    new LogEntry(LogLevel.Fatal, "System crash detected", "Core")
};

foreach (var entry in logEntries)
{
    Console.WriteLine($"\n--- Processing Log Entry ---");
    consoleLogger.Log(entry);
}
```

### 2. **HTTP Request Processing Pipeline**
```csharp
// HTTP request class
public class HttpRequest
{
    public string Method { get; set; }
    public string Path { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public string Body { get; set; }
    public string UserAgent { get; set; }
    public string IPAddress { get; set; }

    public HttpRequest()
    {
        Headers = new Dictionary<string, string>();
    }

    public override string ToString()
    {
        return $"{Method} {Path} from {IPAddress}";
    }
}

// HTTP response class
public class HttpResponse
{
    public int StatusCode { get; set; }
    public string StatusMessage { get; set; }
    public string Body { get; set; }
    public bool IsCompleted { get; set; }

    public HttpResponse()
    {
        StatusCode = 200;
        StatusMessage = "OK";
    }
}

// Abstract middleware handler
public abstract class HttpMiddleware
{
    protected HttpMiddleware _next;
    protected string _middlewareName;

    public HttpMiddleware(string middlewareName)
    {
        _middlewareName = middlewareName;
    }

    public void SetNext(HttpMiddleware middleware)
    {
        _next = middleware;
    }

    public virtual HttpResponse Handle(HttpRequest request)
    {
        Console.WriteLine($"🔄 {_middlewareName} processing: {request}");

        var response = ProcessRequest(request);

        if (!response.IsCompleted && _next != null)
        {
            return _next.Handle(request);
        }

        return response;
    }

    protected abstract HttpResponse ProcessRequest(HttpRequest request);
}

// Concrete middleware
public class AuthenticationMiddleware : HttpMiddleware
{
    public AuthenticationMiddleware() : base("Authentication") { }

    protected override HttpResponse ProcessRequest(HttpRequest request)
    {
        // Check for authentication
        if (!request.Headers.ContainsKey("Authorization"))
        {
            if (request.Path.StartsWith("/api/"))
            {
                return new HttpResponse
                {
                    StatusCode = 401,
                    StatusMessage = "Unauthorized",
                    Body = "Missing authentication token",
                    IsCompleted = true
                };
            }
        }

        Console.WriteLine($"✅ {_middlewareName}: Request authenticated");
        return new HttpResponse { IsCompleted = false }; // Continue processing
    }
}

public class RateLimitingMiddleware : HttpMiddleware
{
    private Dictionary<string, (DateTime lastRequest, int requestCount)> _clientRequests;
    private readonly int _maxRequestsPerMinute = 60;

    public RateLimitingMiddleware() : base("Rate Limiting")
    {
        _clientRequests = new Dictionary<string, (DateTime, int)>();
    }

    protected override HttpResponse ProcessRequest(HttpRequest request)
    {
        var clientIP = request.IPAddress;
        var now = DateTime.Now;

        if (_clientRequests.ContainsKey(clientIP))
        {
            var (lastRequest, count) = _clientRequests[clientIP];
            
            if (now - lastRequest < TimeSpan.FromMinutes(1))
            {
                if (count >= _maxRequestsPerMinute)
                {
                    return new HttpResponse
                    {
                        StatusCode = 429,
                        StatusMessage = "Too Many Requests",
                        Body = "Rate limit exceeded",
                        IsCompleted = true
                    };
                }
                _clientRequests[clientIP] = (lastRequest, count + 1);
            }
            else
            {
                _clientRequests[clientIP] = (now, 1);
            }
        }
        else
        {
            _clientRequests[clientIP] = (now, 1);
        }

        Console.WriteLine($"✅ {_middlewareName}: Rate limit check passed");
        return new HttpResponse { IsCompleted = false };
    }
}

public class SecurityMiddleware : HttpMiddleware
{
    private readonly List<string> _blockedIPs = new() { "192.168.1.100", "10.0.0.50" };
    private readonly List<string> _maliciousPatterns = new() { "<script>", "DROP TABLE", "SELECT *" };

    public SecurityMiddleware() : base("Security") { }

    protected override HttpResponse ProcessRequest(HttpRequest request)
    {
        // Check blocked IPs
        if (_blockedIPs.Contains(request.IPAddress))
        {
            return new HttpResponse
            {
                StatusCode = 403,
                StatusMessage = "Forbidden",
                Body = "IP address blocked",
                IsCompleted = true
            };
        }

        // Check for malicious patterns
        var fullRequest = $"{request.Path} {request.Body}".ToLower();
        foreach (var pattern in _maliciousPatterns)
        {
            if (fullRequest.Contains(pattern.ToLower()))
            {
                Console.WriteLine($"🚨 {_middlewareName}: Malicious pattern detected: {pattern}");
                return new HttpResponse
                {
                    StatusCode = 400,
                    StatusMessage = "Bad Request",
                    Body = "Request contains malicious content",
                    IsCompleted = true
                };
            }
        }

        Console.WriteLine($"✅ {_middlewareName}: Security check passed");
        return new HttpResponse { IsCompleted = false };
    }
}

public class ApplicationMiddleware : HttpMiddleware
{
    public ApplicationMiddleware() : base("Application") { }

    protected override HttpResponse ProcessRequest(HttpRequest request)
    {
        // Simulate application processing
        var response = new HttpResponse { IsCompleted = true };

        if (request.Path == "/api/users")
        {
            response.Body = "{'users': ['John', 'Jane', 'Bob']}";
        }
        else if (request.Path == "/api/orders")
        {
            response.Body = "{'orders': [{'id': 1, 'total': 99.99}]}";
        }
        else if (request.Path == "/")
        {
            response.Body = "<html><body>Welcome to our API</body></html>";
        }
        else
        {
            response.StatusCode = 404;
            response.StatusMessage = "Not Found";
            response.Body = "Resource not found";
        }

        Console.WriteLine($"🎯 {_middlewareName}: Request processed");
        return response;
    }
}

// Usage - building HTTP pipeline
var auth = new AuthenticationMiddleware();
var rateLimiting = new RateLimitingMiddleware();
var security = new SecurityMiddleware();
var application = new ApplicationMiddleware();

// Build pipeline
auth.SetNext(rateLimiting);
rateLimiting.SetNext(security);
security.SetNext(application);

// Test requests
var requests = new[]
{
    new HttpRequest { Method = "GET", Path = "/", IPAddress = "192.168.1.1", UserAgent = "Chrome" },
    new HttpRequest { Method = "GET", Path = "/api/users", IPAddress = "192.168.1.2", 
                     Headers = { ["Authorization"] = "Bearer token123" } },
    new HttpRequest { Method = "POST", Path = "/api/data", IPAddress = "192.168.1.100", 
                     Body = "DROP TABLE users;" }, // Blocked IP with malicious content
    new HttpRequest { Method = "GET", Path = "/api/orders", IPAddress = "192.168.1.3" } // Missing auth
};

foreach (var request in requests)
{
    Console.WriteLine($"\n{'='*70}");
    Console.WriteLine($"Processing: {request}");
    Console.WriteLine($"{'='*70}");
    
    var response = auth.Handle(request);
    Console.WriteLine($"📤 Response: {response.StatusCode} {response.StatusMessage}");
    if (!string.IsNullOrEmpty(response.Body))
    {
        Console.WriteLine($"📄 Body: {response.Body}");
    }
}
```

### 3. **Event Processing Chain**
```csharp
// Event types
public enum EventType
{
    UserAction,
    SystemAlert,
    DataChange,
    SecurityEvent,
    Performance
}

// Event class
public class SystemEvent
{
    public EventType Type { get; set; }
    public string Source { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object> Data { get; set; }
    public int Severity { get; set; } // 1-10, 10 being most severe

    public SystemEvent()
    {
        Timestamp = DateTime.Now;
        Data = new Dictionary<string, object>();
    }

    public override string ToString()
    {
        return $"{Type} from {Source}: {Message} (Severity: {Severity})";
    }
}

// Abstract event processor
public abstract class EventProcessor
{
    protected EventProcessor _nextProcessor;
    protected string _processorName;

    public EventProcessor(string processorName)
    {
        _processorName = processorName;
    }

    public void SetNext(EventProcessor processor)
    {
        _nextProcessor = processor;
    }

    public virtual void ProcessEvent(SystemEvent systemEvent)
    {
        if (CanProcess(systemEvent))
        {
            HandleEvent(systemEvent);
        }

        // Always pass to next processor for additional processing
        _nextProcessor?.ProcessEvent(systemEvent);
    }

    protected abstract bool CanProcess(SystemEvent systemEvent);
    protected abstract void HandleEvent(SystemEvent systemEvent);
}

// Concrete processors
public class FilterProcessor : EventProcessor
{
    private readonly EventType[] _allowedTypes;
    private readonly int _minimumSeverity;

    public FilterProcessor(EventType[] allowedTypes, int minimumSeverity) 
        : base("Filter")
    {
        _allowedTypes = allowedTypes;
        _minimumSeverity = minimumSeverity;
    }

    protected override bool CanProcess(SystemEvent systemEvent)
    {
        return _allowedTypes.Contains(systemEvent.Type) && 
               systemEvent.Severity >= _minimumSeverity;
    }

    protected override void HandleEvent(SystemEvent systemEvent)
    {
        Console.WriteLine($"🔍 {_processorName}: Event passed filter - {systemEvent}");
    }
}

public class LoggingProcessor : EventProcessor
{
    public LoggingProcessor() : base("Logging") { }

    protected override bool CanProcess(SystemEvent systemEvent)
    {
        return true; // Log all events
    }

    protected override void HandleEvent(SystemEvent systemEvent)
    {
        var logLevel = systemEvent.Severity >= 8 ? "ERROR" : 
                      systemEvent.Severity >= 6 ? "WARN" : "INFO";
        
        Console.WriteLine($"📝 {_processorName}: [{logLevel}] {systemEvent}");
        
        // Simulate writing to log file
        File.AppendAllText("events.log", $"[{systemEvent.Timestamp}] {logLevel}: {systemEvent}\n");
    }
}

public class AlertProcessor : EventProcessor
{
    public AlertProcessor() : base("Alert") { }

    protected override bool CanProcess(SystemEvent systemEvent)
    {
        return systemEvent.Severity >= 7; // Only high severity events
    }

    protected override void HandleEvent(SystemEvent systemEvent)
    {
        Console.WriteLine($"🚨 {_processorName}: HIGH SEVERITY ALERT - {systemEvent}");
        
        if (systemEvent.Type == EventType.SecurityEvent)
        {
            Console.WriteLine($"📧 {_processorName}: Sending security team notification");
        }
        
        if (systemEvent.Severity >= 9)
        {
            Console.WriteLine($"📱 {_processorName}: Sending SMS alert to on-call engineer");
        }
    }
}

public class MetricsProcessor : EventProcessor
{
    private Dictionary<EventType, int> _eventCounts;

    public MetricsProcessor() : base("Metrics")
    {
        _eventCounts = new Dictionary<EventType, int>();
    }

    protected override bool CanProcess(SystemEvent systemEvent)
    {
        return true; // Count all events
    }

    protected override void HandleEvent(SystemEvent systemEvent)
    {
        if (!_eventCounts.ContainsKey(systemEvent.Type))
        {
            _eventCounts[systemEvent.Type] = 0;
        }
        
        _eventCounts[systemEvent.Type]++;
        
        Console.WriteLine($"📊 {_processorName}: Event count updated - {systemEvent.Type}: {_eventCounts[systemEvent.Type]}");
    }

    public void PrintMetrics()
    {
        Console.WriteLine("\n📈 Event Metrics:");
        foreach (var kvp in _eventCounts)
        {
            Console.WriteLine($"  {kvp.Key}: {kvp.Value} events");
        }
    }
}

// Usage - building event processing pipeline
var filter = new FilterProcessor(
    new[] { EventType.SystemAlert, EventType.SecurityEvent, EventType.Performance }, 
    3);
var logging = new LoggingProcessor();
var alerts = new AlertProcessor();
var metrics = new MetricsProcessor();

// Build chain
filter.SetNext(logging);
logging.SetNext(alerts);
alerts.SetNext(metrics);

// Test events
var events = new[]
{
    new SystemEvent 
    { 
        Type = EventType.UserAction, 
        Source = "WebApp", 
        Message = "User logged in", 
        Severity = 2 
    },
    new SystemEvent 
    { 
        Type = EventType.SystemAlert, 
        Source = "Database", 
        Message = "Connection pool exhausted", 
        Severity = 8 
    },
    new SystemEvent 
    { 
        Type = EventType.SecurityEvent, 
        Source = "AuthService", 
        Message = "Multiple failed login attempts", 
        Severity = 7 
    },
    new SystemEvent 
    { 
        Type = EventType.Performance, 
        Source = "API", 
        Message = "Response time exceeded threshold", 
        Severity = 6 
    },
    new SystemEvent 
    { 
        Type = EventType.SecurityEvent, 
        Source = "Firewall", 
        Message = "Potential DDoS attack detected", 
        Severity = 10 
    }
};

foreach (var evt in events)
{
    Console.WriteLine($"\n{'='*80}");
    Console.WriteLine($"Processing Event: {evt}");
    Console.WriteLine($"{'='*80}");
    
    filter.ProcessEvent(evt);
}

Console.WriteLine();
metrics.PrintMetrics();
```

## Benefits
- **Decoupling**: Sender doesn't need to know which handler will process the request
- **Flexibility**: Can add, remove, or reorder handlers dynamically
- **Single Responsibility**: Each handler has one specific responsibility
- **Open/Closed Principle**: Easy to add new handlers without modifying existing code
- **Runtime Configuration**: Chain can be configured at runtime

## Drawbacks
- **No Guarantee**: Request might not be handled if no handler can process it
- **Performance**: Request travels through multiple handlers
- **Debugging**: Can be hard to follow the request flow
- **Configuration**: Setting up the chain correctly can be complex

## When to Use
✅ **Use When:**
- Multiple objects can handle a request, but you don't know which one in advance
- You want to decouple sender from receiver
- You want to handle requests in a specific order
- The set of handlers can change dynamically
- You want to implement a approval/escalation workflow

❌ **Avoid When:**
- You know exactly which handler should process each request
- Performance is critical and the chain is long
- The order of handlers doesn't matter
- You have a simple conditional that works well

## Chain of Responsibility vs Other Patterns

| Pattern | Purpose | Key Difference |
|---------|---------|----------------|
| **Chain of Responsibility** | Passes request along handler chain | Focuses on request delegation |
| **Command** | Encapsulates requests as objects | Focuses on request encapsulation |
| **Observer** | Notifies multiple objects of changes | Focuses on event notification |
| **Strategy** | Selects algorithm at runtime | Focuses on algorithm selection |

## Best Practices
1. **Clear Responsibility**: Each handler should have a clear, single responsibility
2. **Handle or Pass**: Each handler should either handle the request or pass it on
3. **Default Handler**: Consider a default handler at the end of the chain
4. **Immutable Requests**: Make requests immutable to prevent side effects
5. **Error Handling**: Handle cases where no handler can process the request
6. **Performance**: Keep handlers lightweight for better performance

## Common Mistakes
1. **Circular Chains**: Creating loops in the handler chain
2. **No Terminal Handler**: Not handling the case where no handler processes the request
3. **Heavy Handlers**: Making handlers too complex or slow
4. **Order Dependency**: Making handlers depend on a specific order when they shouldn't

## Modern C# Features
```csharp
// Using delegates for simple chains
public class DelegateChain
{
    private Func<HttpRequest, HttpResponse> _pipeline;

    public DelegateChain()
    {
        _pipeline = request => new HttpResponse { StatusCode = 404 };
    }

    public void Use(Func<HttpRequest, Func<HttpRequest, HttpResponse>, HttpResponse> middleware)
    {
        var next = _pipeline;
        _pipeline = request => middleware(request, next);
    }

    public HttpResponse Handle(HttpRequest request)
    {
        return _pipeline(request);
    }
}

// Usage
var chain = new DelegateChain();
chain.Use((req, next) => {
    Console.WriteLine("Auth middleware");
    return req.Headers.ContainsKey("auth") ? next(req) : new HttpResponse { StatusCode = 401 };
});
chain.Use((req, next) => {
    Console.WriteLine("Logging middleware");
    return next(req);
});

// Using async patterns
public abstract class AsyncHandler
{
    protected AsyncHandler _next;

    public async Task<Response> HandleAsync(Request request)
    {
        var response = await ProcessAsync(request);
        
        if (!response.IsCompleted && _next != null)
        {
            return await _next.HandleAsync(request);
        }
        
        return response;
    }

    protected abstract Task<Response> ProcessAsync(Request request);
}
```

## Testing Chain of Responsibility
```csharp
[Test]
public void ExpenseHandler_SmallAmount_ApprovedByTeamLead()
{
    // Arrange
    var teamLead = new TeamLeadHandler();
    var manager = new ManagerHandler();
    teamLead.SetNext(manager);
    
    var request = new ExpenseRequest(50, "Office supplies", "John");

    // Act
    teamLead.ProcessRequest(request);

    // Assert - would verify through logging or state changes
    // In real implementation, you'd capture the approval status
}

[Test]
public void ExpenseHandler_LargeAmount_EscalatesToCEO()
{
    // Arrange
    var chain = BuildFullChain(); // Helper method to build complete chain
    var request = new ExpenseRequest(100000, "Major purchase", "Alice");

    // Act & Assert
    Assert.DoesNotThrow(() => chain.ProcessRequest(request));
}
```

## Summary
The Chain of Responsibility pattern is like a customer service escalation system - your request starts at the first level and moves up the chain until someone who can handle it takes responsibility. Each handler in the chain has the chance to process the request or pass it along to the next level.

It's perfect for approval workflows, request processing pipelines, error handling hierarchies, and any scenario where you have multiple potential handlers for a request but don't know which one should handle it in advance.

The key insight is that sometimes the best way to handle complex decision-making is to break it into a series of simple, focused handlers that each make their own determination about whether they can help or if the request needs to go elsewhere.
