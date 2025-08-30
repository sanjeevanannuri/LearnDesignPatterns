# Proxy Pattern

## Overview
The Proxy pattern provides a placeholder or surrogate for another object to control access to it. Think of it like a personal assistant - instead of you handling every call, meeting request, or document directly, your assistant filters requests, schedules appropriately, and sometimes handles things without bothering you at all.

## Problem It Solves
Imagine you have an expensive resource that you want to control access to:
- Large images that take time to load from disk
- Database connections that are costly to establish
- Remote services that might be unavailable
- Sensitive objects that require access control

Without Proxy, you face these issues:
```csharp
// BAD: Direct access to expensive resources
public class ExpensiveImage
{
    private byte[] _imageData;
    
    public ExpensiveImage(string filename)
    {
        LoadFromDisk(filename); // Always loads immediately, even if never displayed
    }
    
    private void LoadFromDisk(string filename)
    {
        // Expensive operation - loads large file
        Thread.Sleep(2000); // Simulate slow loading
        _imageData = File.ReadAllBytes(filename);
    }
    
    public void Display()
    {
        Console.WriteLine("Displaying image...");
    }
}

// Client loads all images upfront
var images = new List<ExpensiveImage>
{
    new ExpensiveImage("photo1.jpg"), // Loads immediately
    new ExpensiveImage("photo2.jpg"), // Loads immediately
    new ExpensiveImage("photo3.jpg")  // Loads immediately
};
// User might never view some images, but we loaded them all!
```

## Real-World Analogy
Think of a **bank safe deposit box**:
1. **Safe Deposit Box** (Real Subject): Contains your valuable items
2. **Bank Employee** (Proxy): Controls access, checks your ID, logs visits
3. **Access Control**: You can't just walk into the vault - you go through the employee
4. **Logging**: Bank records when you access your box
5. **Protection**: Employee ensures only authorized people access specific boxes

Or consider **internet browsing with a proxy server**:
- **Web Server** (Real Subject): Contains the actual website
- **Proxy Server** (Proxy): Sits between you and the web server
- **Caching**: Proxy might have cached copy of the website
- **Filtering**: Proxy can block access to certain sites
- **Logging**: Proxy records what sites you visit

## Implementation Details

### Basic Structure
```csharp
// Subject interface - common interface for RealSubject and Proxy
public interface ISubject
{
    void Request();
}

// Real subject - the actual object that does the work
public class RealSubject : ISubject
{
    public void Request()
    {
        Console.WriteLine("RealSubject: Handling request");
    }
}

// Proxy - controls access to the real subject
public class Proxy : ISubject
{
    private RealSubject _realSubject;

    public void Request()
    {
        // Pre-processing
        if (CheckAccess())
        {
            // Lazy initialization
            if (_realSubject == null)
            {
                _realSubject = new RealSubject();
            }

            _realSubject.Request();

            // Post-processing
            LogAccess();
        }
    }

    private bool CheckAccess()
    {
        Console.WriteLine("Proxy: Checking access prior to firing a real request");
        return true;
    }

    private void LogAccess()
    {
        Console.WriteLine("Proxy: Logging the time of request");
    }
}
```

### Key Components
1. **Subject**: Interface that makes Proxy and RealSubject interchangeable
2. **RealSubject**: The actual object that performs the real work
3. **Proxy**: Controls access to RealSubject, may add additional behavior
4. **Client**: Uses objects through the Subject interface

## Example from Our Code
```csharp
// Subject interface
public interface IImage
{
    void Display();
    string GetInfo();
}

// Real subject - actual image that's expensive to load
public class RealImage : IImage
{
    private string _filename;
    private byte[] _imageData;
    private DateTime _loadTime;

    public RealImage(string filename)
    {
        _filename = filename;
        LoadFromDisk();
    }

    private void LoadFromDisk()
    {
        Console.WriteLine($"Loading image from disk: {_filename}");
        
        // Simulate expensive loading operation
        Thread.Sleep(1000);
        
        // Simulate loading image data
        _imageData = new byte[1024 * 1024]; // 1MB of fake image data
        _loadTime = DateTime.Now;
        
        Console.WriteLine($"Image {_filename} loaded successfully");
    }

    public void Display()
    {
        Console.WriteLine($"Displaying image: {_filename}");
        Console.WriteLine($"Image size: {_imageData.Length} bytes");
    }

    public string GetInfo()
    {
        return $"Real image: {_filename}, loaded at {_loadTime}, size: {_imageData.Length} bytes";
    }
}

// Proxy - controls access and provides lazy loading
public class ImageProxy : IImage
{
    private string _filename;
    private RealImage _realImage;
    private bool _accessAllowed;
    private List<DateTime> _accessLog;

    public ImageProxy(string filename, bool accessAllowed = true)
    {
        _filename = filename;
        _accessAllowed = accessAllowed;
        _accessLog = new List<DateTime>();
        
        Console.WriteLine($"Image proxy created for: {filename}");
    }

    public void Display()
    {
        Console.WriteLine($"Proxy: Request to display {_filename}");
        
        // Access control
        if (!_accessAllowed)
        {
            Console.WriteLine("Access denied: You don't have permission to view this image");
            return;
        }

        // Log access
        _accessLog.Add(DateTime.Now);

        // Lazy loading - only create real image when needed
        if (_realImage == null)
        {
            Console.WriteLine("Proxy: Creating real image (lazy loading)");
            _realImage = new RealImage(_filename);
        }

        // Delegate to real image
        _realImage.Display();
        
        Console.WriteLine($"Proxy: Image access logged. Total accesses: {_accessLog.Count}");
    }

    public string GetInfo()
    {
        // Can provide info without loading the actual image
        if (_realImage == null)
        {
            return $"Proxy image: {_filename} (not yet loaded)";
        }
        else
        {
            return $"Proxy -> {_realImage.GetInfo()}";
        }
    }

    // Additional proxy functionality
    public void GrantAccess()
    {
        _accessAllowed = true;
        Console.WriteLine($"Access granted for {_filename}");
    }

    public void RevokeAccess()
    {
        _accessAllowed = false;
        Console.WriteLine($"Access revoked for {_filename}");
    }

    public List<DateTime> GetAccessLog()
    {
        return new List<DateTime>(_accessLog);
    }
}

// Usage - demonstrating proxy benefits
Console.WriteLine("=== Creating Image Proxies ===");
var image1 = new ImageProxy("vacation_photo.jpg");
var image2 = new ImageProxy("secret_document.jpg", false); // No access initially
var image3 = new ImageProxy("family_photo.jpg");

Console.WriteLine("\n=== Getting Info (No Loading) ===");
Console.WriteLine(image1.GetInfo());
Console.WriteLine(image2.GetInfo());
Console.WriteLine(image3.GetInfo());

Console.WriteLine("\n=== First Display Attempt ===");
image1.Display(); // Will load the real image
image2.Display(); // Access denied
image3.Display(); // Will load the real image

Console.WriteLine("\n=== Second Display Attempt ===");
image1.Display(); // Uses already loaded image
image3.Display(); // Uses already loaded image

Console.WriteLine("\n=== Granting Access and Trying Again ===");
image2.GrantAccess();
image2.Display(); // Now loads and displays

Console.WriteLine("\n=== Final Info ===");
Console.WriteLine(image1.GetInfo());
Console.WriteLine(image2.GetInfo());
Console.WriteLine(image3.GetInfo());
```

## Types of Proxies

### 1. **Virtual Proxy (Lazy Loading)**
Controls creation of expensive objects:
```csharp
public class VirtualProxy : IExpensiveObject
{
    private ExpensiveObject _realObject;
    private string _initializationData;

    public VirtualProxy(string data)
    {
        _initializationData = data;
        // Don't create real object yet
    }

    public void DoWork()
    {
        if (_realObject == null)
        {
            Console.WriteLine("Creating expensive object...");
            _realObject = new ExpensiveObject(_initializationData);
        }
        
        _realObject.DoWork();
    }
}
```

### 2. **Protection Proxy (Access Control)**
Controls access based on permissions:
```csharp
public class ProtectionProxy : ISecureService
{
    private SecureService _service;
    private string _currentUser;

    public ProtectionProxy(string user)
    {
        _currentUser = user;
        _service = new SecureService();
    }

    public void ReadData()
    {
        if (HasReadPermission(_currentUser))
        {
            _service.ReadData();
        }
        else
        {
            throw new UnauthorizedAccessException("Read access denied");
        }
    }

    public void WriteData(string data)
    {
        if (HasWritePermission(_currentUser))
        {
            _service.WriteData(data);
        }
        else
        {
            throw new UnauthorizedAccessException("Write access denied");
        }
    }

    private bool HasReadPermission(string user) => user != "guest";
    private bool HasWritePermission(string user) => user == "admin";
}
```

### 3. **Remote Proxy**
Represents objects in different address spaces:
```csharp
public class RemoteServiceProxy : IRemoteService
{
    private string _serviceUrl;
    private HttpClient _httpClient;

    public RemoteServiceProxy(string serviceUrl)
    {
        _serviceUrl = serviceUrl;
        _httpClient = new HttpClient();
    }

    public async Task<string> GetDataAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_serviceUrl}/api/data/{id}");
            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Remote service error: {ex.Message}");
            return "Service unavailable";
        }
    }

    public async Task<bool> SendDataAsync(string data)
    {
        try
        {
            var content = new StringContent(data);
            var response = await _httpClient.PostAsync($"{_serviceUrl}/api/data", content);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Remote service error: {ex.Message}");
            return false;
        }
    }
}
```

### 4. **Caching Proxy**
Caches results to improve performance:
```csharp
public class CachingProxy : IDataService
{
    private DataService _dataService;
    private Dictionary<string, (object Data, DateTime CacheTime)> _cache;
    private TimeSpan _cacheExpiration;

    public CachingProxy(TimeSpan cacheExpiration)
    {
        _dataService = new DataService();
        _cache = new Dictionary<string, (object, DateTime)>();
        _cacheExpiration = cacheExpiration;
    }

    public T GetData<T>(string key)
    {
        // Check cache first
        if (_cache.TryGetValue(key, out var cached))
        {
            if (DateTime.Now - cached.CacheTime < _cacheExpiration)
            {
                Console.WriteLine($"Cache hit for key: {key}");
                return (T)cached.Data;
            }
            else
            {
                Console.WriteLine($"Cache expired for key: {key}");
                _cache.Remove(key);
            }
        }

        // Cache miss - get from real service
        Console.WriteLine($"Cache miss for key: {key}");
        var data = _dataService.GetData<T>(key);
        
        // Store in cache
        _cache[key] = (data, DateTime.Now);
        
        return data;
    }
}
```

## Real-World Examples

### 1. **Database Connection Proxy**
```csharp
// Subject interface
public interface IDatabaseConnection
{
    void Connect();
    void Disconnect();
    void ExecuteQuery(string sql);
    bool IsConnected { get; }
}

// Real subject - actual database connection
public class DatabaseConnection : IDatabaseConnection
{
    private string _connectionString;
    private bool _isConnected;

    public DatabaseConnection(string connectionString)
    {
        _connectionString = connectionString;
    }

    public bool IsConnected => _isConnected;

    public void Connect()
    {
        Console.WriteLine($"Connecting to database: {_connectionString}");
        Thread.Sleep(500); // Simulate connection time
        _isConnected = true;
        Console.WriteLine("Database connected");
    }

    public void Disconnect()
    {
        if (_isConnected)
        {
            Console.WriteLine("Disconnecting from database");
            _isConnected = false;
        }
    }

    public void ExecuteQuery(string sql)
    {
        if (!_isConnected)
        {
            throw new InvalidOperationException("Not connected to database");
        }
        
        Console.WriteLine($"Executing query: {sql}");
        Thread.Sleep(100); // Simulate query execution
    }
}

// Proxy - adds connection pooling and logging
public class DatabaseConnectionProxy : IDatabaseConnection
{
    private DatabaseConnection _connection;
    private string _connectionString;
    private string _userId;
    private List<string> _queryLog;
    private DateTime _lastActivity;

    public DatabaseConnectionProxy(string connectionString, string userId)
    {
        _connectionString = connectionString;
        _userId = userId;
        _queryLog = new List<string>();
    }

    public bool IsConnected => _connection?.IsConnected ?? false;

    public void Connect()
    {
        if (_connection == null)
        {
            Console.WriteLine($"Proxy: Creating new database connection for user {_userId}");
            _connection = new DatabaseConnection(_connectionString);
        }

        if (!_connection.IsConnected)
        {
            _connection.Connect();
        }
        
        _lastActivity = DateTime.Now;
    }

    public void Disconnect()
    {
        // Don't immediately disconnect - keep connection in pool
        Console.WriteLine("Proxy: Connection returned to pool (not actually disconnected)");
        _lastActivity = DateTime.Now;
    }

    public void ExecuteQuery(string sql)
    {
        // Auto-connect if needed
        if (!IsConnected)
        {
            Connect();
        }

        // Log query for auditing
        var logEntry = $"{DateTime.Now}: User {_userId} executed: {sql}";
        _queryLog.Add(logEntry);
        Console.WriteLine($"Proxy: Logging query - {logEntry}");

        // Validate query (security check)
        if (sql.ToUpper().Contains("DROP") || sql.ToUpper().Contains("DELETE"))
        {
            Console.WriteLine("Proxy: Dangerous query detected - additional security check required");
            // Could prompt for confirmation, require additional auth, etc.
        }

        _connection.ExecuteQuery(sql);
        _lastActivity = DateTime.Now;
    }

    public List<string> GetQueryLog()
    {
        return new List<string>(_queryLog);
    }

    public TimeSpan GetIdleTime()
    {
        return DateTime.Now - _lastActivity;
    }
}

// Usage
var dbProxy = new DatabaseConnectionProxy("Server=localhost;Database=MyApp", "john_doe");

dbProxy.ExecuteQuery("SELECT * FROM Users"); // Auto-connects
dbProxy.ExecuteQuery("SELECT * FROM Orders WHERE UserId = 123");
dbProxy.ExecuteQuery("UPDATE Users SET LastLogin = NOW() WHERE Id = 123");
dbProxy.Disconnect(); // Doesn't actually disconnect

Console.WriteLine("\nQuery Log:");
foreach (var logEntry in dbProxy.GetQueryLog())
{
    Console.WriteLine($"  {logEntry}");
}
```

### 2. **Web Service Proxy with Circuit Breaker**
```csharp
public interface IPaymentService
{
    Task<bool> ProcessPaymentAsync(decimal amount, string cardNumber);
    Task<string> GetTransactionStatusAsync(string transactionId);
}

public class PaymentService : IPaymentService
{
    public async Task<bool> ProcessPaymentAsync(decimal amount, string cardNumber)
    {
        Console.WriteLine($"Processing payment: ${amount} on card {cardNumber}");
        await Task.Delay(500); // Simulate processing time
        
        // Simulate occasional failures
        if (Random.Shared.Next(10) < 2) // 20% failure rate
        {
            throw new HttpRequestException("Payment service temporarily unavailable");
        }
        
        return true;
    }

    public async Task<string> GetTransactionStatusAsync(string transactionId)
    {
        await Task.Delay(200);
        return $"Transaction {transactionId}: Completed";
    }
}

public class PaymentServiceProxy : IPaymentService
{
    private readonly PaymentService _paymentService;
    private int _failureCount;
    private DateTime _lastFailureTime;
    private readonly int _failureThreshold = 3;
    private readonly TimeSpan _circuitBreakerTimeout = TimeSpan.FromMinutes(1);
    private bool _circuitOpen;

    public PaymentServiceProxy()
    {
        _paymentService = new PaymentService();
    }

    public async Task<bool> ProcessPaymentAsync(decimal amount, string cardNumber)
    {
        // Circuit breaker logic
        if (_circuitOpen)
        {
            if (DateTime.Now - _lastFailureTime > _circuitBreakerTimeout)
            {
                Console.WriteLine("Proxy: Attempting to close circuit breaker");
                _circuitOpen = false;
                _failureCount = 0;
            }
            else
            {
                Console.WriteLine("Proxy: Circuit breaker is OPEN - payment service unavailable");
                return false;
            }
        }

        try
        {
            Console.WriteLine("Proxy: Forwarding payment request to service");
            var result = await _paymentService.ProcessPaymentAsync(amount, cardNumber);
            
            // Reset failure count on success
            _failureCount = 0;
            return result;
        }
        catch (Exception ex)
        {
            _failureCount++;
            _lastFailureTime = DateTime.Now;
            
            Console.WriteLine($"Proxy: Service failure #{_failureCount}: {ex.Message}");
            
            if (_failureCount >= _failureThreshold)
            {
                _circuitOpen = true;
                Console.WriteLine("Proxy: Circuit breaker OPENED due to repeated failures");
            }
            
            return false;
        }
    }

    public async Task<string> GetTransactionStatusAsync(string transactionId)
    {
        if (_circuitOpen)
        {
            return $"Service unavailable - Transaction {transactionId}: Status unknown";
        }

        try
        {
            return await _paymentService.GetTransactionStatusAsync(transactionId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Proxy: Status check failed: {ex.Message}");
            return $"Transaction {transactionId}: Status check failed";
        }
    }
}

// Usage
var paymentProxy = new PaymentServiceProxy();

// Test multiple payments to demonstrate circuit breaker
for (int i = 1; i <= 10; i++)
{
    Console.WriteLine($"\n--- Payment Attempt {i} ---");
    var success = await paymentProxy.ProcessPaymentAsync(99.99m, "****1234");
    Console.WriteLine($"Payment result: {(success ? "SUCCESS" : "FAILED")}");
    
    await Task.Delay(100); // Brief pause between attempts
}
```

### 3. **Smart Reference Proxy**
```csharp
public interface IDocument
{
    string GetContent();
    void SetContent(string content);
    void Save();
}

public class Document : IDocument
{
    private string _filename;
    private string _content;
    private bool _isDirty;

    public Document(string filename)
    {
        _filename = filename;
        LoadFromFile();
    }

    private void LoadFromFile()
    {
        Console.WriteLine($"Loading document: {_filename}");
        // Simulate file loading
        _content = $"Content of {_filename}";
        _isDirty = false;
    }

    public string GetContent()
    {
        return _content;
    }

    public void SetContent(string content)
    {
        _content = content;
        _isDirty = true;
    }

    public void Save()
    {
        if (_isDirty)
        {
            Console.WriteLine($"Saving document: {_filename}");
            // Simulate file saving
            _isDirty = false;
        }
    }
}

public class SmartDocumentProxy : IDocument
{
    private Document _document;
    private string _filename;
    private int _referenceCount;
    private readonly object _lock = new object();

    public SmartDocumentProxy(string filename)
    {
        _filename = filename;
    }

    public string GetContent()
    {
        lock (_lock)
        {
            EnsureDocumentLoaded();
            _referenceCount++;
            
            Console.WriteLine($"Proxy: Reference count for {_filename}: {_referenceCount}");
            return _document.GetContent();
        }
    }

    public void SetContent(string content)
    {
        lock (_lock)
        {
            EnsureDocumentLoaded();
            _document.SetContent(content);
        }
    }

    public void Save()
    {
        lock (_lock)
        {
            if (_document != null)
            {
                _document.Save();
            }
        }
    }

    private void EnsureDocumentLoaded()
    {
        if (_document == null)
        {
            Console.WriteLine($"Proxy: Loading document {_filename} for first time");
            _document = new Document(_filename);
        }
    }

    public void Release()
    {
        lock (_lock)
        {
            _referenceCount--;
            Console.WriteLine($"Proxy: Reference count for {_filename}: {_referenceCount}");
            
            if (_referenceCount <= 0 && _document != null)
            {
                Console.WriteLine($"Proxy: No more references - cleaning up {_filename}");
                _document.Save(); // Auto-save before cleanup
                _document = null; // Allow garbage collection
            }
        }
    }
}

// Usage
var docProxy = new SmartDocumentProxy("important_document.txt");

// Multiple references to same document
var content1 = docProxy.GetContent(); // Loads document, ref count = 1
var content2 = docProxy.GetContent(); // Uses loaded document, ref count = 2

docProxy.SetContent("Updated content");

docProxy.Release(); // ref count = 1
docProxy.Release(); // ref count = 0, document saved and cleaned up
```

## Benefits
- **Controlled Access**: Can add authentication, authorization, logging
- **Lazy Loading**: Create expensive objects only when needed
- **Caching**: Cache results to improve performance
- **Remote Access**: Handle network communication transparently
- **Reference Management**: Track object usage and lifecycle

## Drawbacks
- **Added Complexity**: Another layer of indirection
- **Performance Impact**: Additional method calls and checks
- **Maintenance**: More code to maintain and debug
- **Transparency Issues**: May not be completely transparent to clients

## When to Use
✅ **Use When:**
- You need to control access to an object
- You want to add functionality without changing the object
- You need lazy loading of expensive resources
- You want to cache expensive operations
- You need to handle remote objects locally
- You want to add reference counting or logging

❌ **Avoid When:**
- The overhead outweighs the benefits
- Direct access is simpler and sufficient
- You're not adding meaningful functionality
- The proxy becomes more complex than the original object

## Proxy vs Other Patterns

| Pattern | Purpose | Key Difference |
|---------|---------|----------------|
| **Proxy** | Controls access to an object | Same interface, controls access |
| **Adapter** | Makes incompatible interfaces work | Changes interface |
| **Decorator** | Adds behavior dynamically | Enhances functionality |
| **Facade** | Simplifies complex subsystem | Provides unified interface |

## Best Practices
1. **Preserve Interface**: Proxy should implement same interface as real subject
2. **Transparent Operation**: Client shouldn't know it's using a proxy
3. **Lazy Initialization**: Create real object only when needed
4. **Error Handling**: Handle cases where real object is unavailable
5. **Thread Safety**: Consider concurrent access to proxy
6. **Resource Management**: Properly manage lifecycle of real objects

## Common Mistakes
1. **Leaky Abstraction**: Exposing proxy-specific details to clients
2. **Performance Issues**: Adding unnecessary overhead
3. **State Synchronization**: Not keeping proxy and real object in sync
4. **Exception Handling**: Not properly handling exceptions from real object

## Modern C# Features
```csharp
// Using DispatchProxy for dynamic proxies
public class DynamicLoggingProxy<T> : DispatchProxy where T : class
{
    private T _target;

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        Console.WriteLine($"Calling method: {targetMethod?.Name}");
        
        try
        {
            var result = targetMethod?.Invoke(_target, args);
            Console.WriteLine($"Method {targetMethod?.Name} completed successfully");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Method {targetMethod?.Name} threw exception: {ex.Message}");
            throw;
        }
    }

    public static T Create(T target)
    {
        var proxy = Create<T, DynamicLoggingProxy<T>>() as DynamicLoggingProxy<T>;
        proxy._target = target;
        return proxy as T;
    }
}

// Using async patterns
public class AsyncCachingProxy : IAsyncDataService
{
    private readonly IAsyncDataService _service;
    private readonly IMemoryCache _cache;

    public async Task<T> GetDataAsync<T>(string key)
    {
        if (_cache.TryGetValue(key, out T cached))
        {
            return cached;
        }

        var data = await _service.GetDataAsync<T>(key);
        _cache.Set(key, data, TimeSpan.FromMinutes(5));
        return data;
    }
}
```

## Testing Proxies
```csharp
[Test]
public void ImageProxy_Display_LoadsRealImageOnFirstCall()
{
    // Arrange
    var proxy = new ImageProxy("test.jpg");
    
    // Act
    proxy.Display();
    
    // Assert - verify real image was created and used
    Assert.That(proxy.GetInfo(), Contains.Substring("loaded at"));
}

[Test]
public void ProtectionProxy_RestrictedAccess_ThrowsException()
{
    // Arrange
    var proxy = new ProtectionProxy("guest");
    
    // Act & Assert
    Assert.Throws<UnauthorizedAccessException>(() => proxy.WriteData("test"));
}
```

## Summary
The Proxy pattern is like having a security guard at a building entrance - they control who gets in, keep logs of visitors, and sometimes handle requests without bothering the people inside. The proxy stands between the client and the real object, providing the same interface but adding valuable services like access control, lazy loading, caching, or remote access.

It's perfect when you need to add cross-cutting concerns (logging, security, caching) without modifying the original object, or when you want to control expensive operations like network calls or resource loading.

The key insight is that sometimes the best way to improve an object is to put a smart intermediary in front of it rather than changing the object itself.
