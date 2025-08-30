# Facade Pattern

## Overview
The Facade pattern provides a simplified interface to a complex subsystem. Think of it like a hotel concierge - instead of you having to directly contact housekeeping, room service, maintenance, and security separately, you just call the front desk and they coordinate everything for you.

## Problem It Solves
Imagine you're building a home theater system with multiple components:
- DVD Player, Projector, Screen, Amplifier, Speakers, Lights, Popcorn Maker
- To watch a movie, you need to: turn on projector, lower screen, set amplifier, adjust speakers, dim lights, start popcorn maker, put DVD in player
- Each component has its own complex interface
- Users just want to "watch a movie" without managing all these details

Without Facade, client code becomes complex:
```csharp
// BAD: Client must know about all subsystems
public void WatchMovie()
{
    popcornMaker.TurnOn();
    popcornMaker.Pop();
    lights.Dim(10);
    screen.Down();
    projector.TurnOn();
    projector.SetInput(dvdPlayer);
    projector.WideScreenMode();
    amplifier.TurnOn();
    amplifier.SetDvd(dvdPlayer);
    amplifier.SetSurroundSound();
    amplifier.SetVolume(5);
    dvdPlayer.TurnOn();
    dvdPlayer.Play("Raiders of the Lost Ark");
}
```

This is error-prone and couples the client to many subsystems.

## Real-World Analogy
Think of a **restaurant**:
1. **Complex Kitchen** (Subsystem): Multiple chefs, prep cooks, dishwashers, equipment
2. **Waiter** (Facade): You tell the waiter "I'll have the salmon"
3. **Coordination**: Waiter coordinates with kitchen, knows the process
4. **Simple Interface**: You don't need to know about food prep, cooking times, plating
5. **Abstraction**: You get your meal without managing the complexity

Or consider **starting a car**:
- **Complex Systems**: Engine, fuel injection, ignition, electrical, transmission
- **Simple Interface**: Turn the key or press a button
- **Hidden Complexity**: All subsystems work together seamlessly
- **User Experience**: Just "start the car"

## Implementation Details

### Basic Structure
```csharp
// Subsystem classes - complex internal components
public class SubsystemA
{
    public void OperationA1() => Console.WriteLine("SubsystemA OperationA1");
    public void OperationA2() => Console.WriteLine("SubsystemA OperationA2");
}

public class SubsystemB
{
    public void OperationB1() => Console.WriteLine("SubsystemB OperationB1");
    public void OperationB2() => Console.WriteLine("SubsystemB OperationB2");
}

public class SubsystemC
{
    public void OperationC1() => Console.WriteLine("SubsystemC OperationC1");
    public void OperationC2() => Console.WriteLine("SubsystemC OperationC2");
}

// Facade - provides simple interface to complex subsystem
public class Facade
{
    private SubsystemA _subsystemA;
    private SubsystemB _subsystemB;
    private SubsystemC _subsystemC;

    public Facade()
    {
        _subsystemA = new SubsystemA();
        _subsystemB = new SubsystemB();
        _subsystemC = new SubsystemC();
    }

    public void SimpleOperation()
    {
        Console.WriteLine("Facade: Simple operation");
        _subsystemA.OperationA1();
        _subsystemB.OperationB1();
        _subsystemC.OperationC1();
    }

    public void ComplexOperation()
    {
        Console.WriteLine("Facade: Complex operation");
        _subsystemA.OperationA1();
        _subsystemA.OperationA2();
        _subsystemB.OperationB2();
        _subsystemC.OperationC1();
        _subsystemC.OperationC2();
    }
}
```

### Key Components
1. **Facade**: Provides simple methods that coordinate subsystem operations
2. **Subsystem Classes**: Complex components that perform the actual work
3. **Client**: Uses the facade instead of interacting with subsystems directly

## Example from Our Code
```csharp
// Subsystem classes - home theater components
public class DVDPlayer
{
    public void TurnOn() => Console.WriteLine("DVD Player turned on");
    public void TurnOff() => Console.WriteLine("DVD Player turned off");
    public void Play(string movie) => Console.WriteLine($"Playing '{movie}'");
    public void Stop() => Console.WriteLine("DVD Player stopped");
    public void Eject() => Console.WriteLine("DVD ejected");
}

public class Projector
{
    public void TurnOn() => Console.WriteLine("Projector turned on");
    public void TurnOff() => Console.WriteLine("Projector turned off");
    public void WideScreenMode() => Console.WriteLine("Projector set to widescreen mode");
    public void SetInput(DVDPlayer dvdPlayer) => Console.WriteLine("Projector input set to DVD player");
}

public class Screen
{
    public void Up() => Console.WriteLine("Screen going up");
    public void Down() => Console.WriteLine("Screen going down");
}

public class Amplifier
{
    public void TurnOn() => Console.WriteLine("Amplifier turned on");
    public void TurnOff() => Console.WriteLine("Amplifier turned off");
    public void SetDvd(DVDPlayer dvdPlayer) => Console.WriteLine("Amplifier input set to DVD");
    public void SetVolume(int level) => Console.WriteLine($"Amplifier volume set to {level}");
    public void SetSurroundSound() => Console.WriteLine("Amplifier set to surround sound");
}

public class TheaterLights
{
    public void TurnOn() => Console.WriteLine("Theater lights turned on");
    public void TurnOff() => Console.WriteLine("Theater lights turned off");
    public void Dim(int level) => Console.WriteLine($"Theater lights dimmed to {level}%");
}

public class PopcornMaker
{
    public void TurnOn() => Console.WriteLine("Popcorn maker turned on");
    public void TurnOff() => Console.WriteLine("Popcorn maker turned off");
    public void Pop() => Console.WriteLine("Popping popcorn!");
}

// Facade - simplifies home theater operations
public class HomeTheaterFacade
{
    private DVDPlayer _dvdPlayer;
    private Projector _projector;
    private Screen _screen;
    private Amplifier _amplifier;
    private TheaterLights _lights;
    private PopcornMaker _popcornMaker;

    public HomeTheaterFacade(DVDPlayer dvdPlayer, Projector projector, Screen screen,
                           Amplifier amplifier, TheaterLights lights, PopcornMaker popcornMaker)
    {
        _dvdPlayer = dvdPlayer;
        _projector = projector;
        _screen = screen;
        _amplifier = amplifier;
        _lights = lights;
        _popcornMaker = popcornMaker;
    }

    public void WatchMovie(string movie)
    {
        Console.WriteLine("Get ready to watch a movie...");
        
        _popcornMaker.TurnOn();
        _popcornMaker.Pop();
        _lights.Dim(10);
        _screen.Down();
        _projector.TurnOn();
        _projector.WideScreenMode();
        _projector.SetInput(_dvdPlayer);
        _amplifier.TurnOn();
        _amplifier.SetDvd(_dvdPlayer);
        _amplifier.SetSurroundSound();
        _amplifier.SetVolume(5);
        _dvdPlayer.TurnOn();
        _dvdPlayer.Play(movie);
        
        Console.WriteLine("Movie is now playing. Enjoy!");
    }

    public void EndMovie()
    {
        Console.WriteLine("Shutting down movie theater...");
        
        _popcornMaker.TurnOff();
        _lights.TurnOn();
        _screen.Up();
        _projector.TurnOff();
        _amplifier.TurnOff();
        _dvdPlayer.Stop();
        _dvdPlayer.Eject();
        _dvdPlayer.TurnOff();
        
        Console.WriteLine("Movie theater is shut down.");
    }

    public void ListenToRadio(string station)
    {
        Console.WriteLine("Setting up for radio listening...");
        
        _amplifier.TurnOn();
        _amplifier.SetVolume(8);
        // Radio setup would go here
        
        Console.WriteLine($"Now playing radio station: {station}");
    }
}

// Usage - simple interface for complex operations
var dvdPlayer = new DVDPlayer();
var projector = new Projector();
var screen = new Screen();
var amplifier = new Amplifier();
var lights = new TheaterLights();
var popcornMaker = new PopcornMaker();

var homeTheater = new HomeTheaterFacade(dvdPlayer, projector, screen, 
                                       amplifier, lights, popcornMaker);

// Simple method calls hide complex coordination
homeTheater.WatchMovie("Raiders of the Lost Ark");
Console.WriteLine();
homeTheater.EndMovie();
```

## Real-World Examples

### 1. **Banking System**
```csharp
// Subsystem classes - internal banking components
public class AccountService
{
    public bool ValidateAccount(string accountNumber)
    {
        Console.WriteLine($"Validating account: {accountNumber}");
        return accountNumber.Length >= 10; // Simple validation
    }

    public decimal GetBalance(string accountNumber)
    {
        Console.WriteLine($"Getting balance for: {accountNumber}");
        return 1000.50m; // Mock balance
    }

    public void DebitAccount(string accountNumber, decimal amount)
    {
        Console.WriteLine($"Debiting ${amount} from account: {accountNumber}");
    }

    public void CreditAccount(string accountNumber, decimal amount)
    {
        Console.WriteLine($"Crediting ${amount} to account: {accountNumber}");
    }
}

public class SecurityService
{
    public bool AuthenticateUser(string userId, string pin)
    {
        Console.WriteLine($"Authenticating user: {userId}");
        return pin == "1234"; // Simple authentication
    }

    public bool AuthorizeTransaction(string userId, decimal amount)
    {
        Console.WriteLine($"Authorizing transaction of ${amount} for user: {userId}");
        return amount <= 5000; // Simple authorization
    }
}

public class NotificationService
{
    public void SendSMS(string phoneNumber, string message)
    {
        Console.WriteLine($"SMS to {phoneNumber}: {message}");
    }

    public void SendEmail(string email, string subject, string message)
    {
        Console.WriteLine($"Email to {email} - {subject}: {message}");
    }
}

public class TransactionService
{
    public string GenerateTransactionId()
    {
        return $"TXN{DateTime.Now.Ticks}";
    }

    public void LogTransaction(string transactionId, string details)
    {
        Console.WriteLine($"Logging transaction {transactionId}: {details}");
    }
}

// Facade - simplified banking operations
public class BankingFacade
{
    private AccountService _accountService;
    private SecurityService _securityService;
    private NotificationService _notificationService;
    private TransactionService _transactionService;

    public BankingFacade()
    {
        _accountService = new AccountService();
        _securityService = new SecurityService();
        _notificationService = new NotificationService();
        _transactionService = new TransactionService();
    }

    public bool TransferMoney(string fromAccount, string toAccount, decimal amount,
                             string userId, string pin, string phoneNumber)
    {
        Console.WriteLine("=== Starting Money Transfer ===");

        // Step 1: Authenticate user
        if (!_securityService.AuthenticateUser(userId, pin))
        {
            Console.WriteLine("Authentication failed!");
            return false;
        }

        // Step 2: Validate accounts
        if (!_accountService.ValidateAccount(fromAccount) || 
            !_accountService.ValidateAccount(toAccount))
        {
            Console.WriteLine("Invalid account number!");
            return false;
        }

        // Step 3: Check balance
        if (_accountService.GetBalance(fromAccount) < amount)
        {
            Console.WriteLine("Insufficient funds!");
            return false;
        }

        // Step 4: Authorize transaction
        if (!_securityService.AuthorizeTransaction(userId, amount))
        {
            Console.WriteLine("Transaction not authorized!");
            return false;
        }

        // Step 5: Process transfer
        var transactionId = _transactionService.GenerateTransactionId();
        _accountService.DebitAccount(fromAccount, amount);
        _accountService.CreditAccount(toAccount, amount);

        // Step 6: Log and notify
        _transactionService.LogTransaction(transactionId, 
            $"Transfer ${amount} from {fromAccount} to {toAccount}");
        _notificationService.SendSMS(phoneNumber, 
            $"Transfer of ${amount} completed. Transaction ID: {transactionId}");

        Console.WriteLine("=== Transfer Completed Successfully ===");
        return true;
    }

    public decimal CheckBalance(string accountNumber, string userId, string pin)
    {
        Console.WriteLine("=== Checking Account Balance ===");

        if (!_securityService.AuthenticateUser(userId, pin))
        {
            Console.WriteLine("Authentication failed!");
            return -1;
        }

        if (!_accountService.ValidateAccount(accountNumber))
        {
            Console.WriteLine("Invalid account number!");
            return -1;
        }

        var balance = _accountService.GetBalance(accountNumber);
        Console.WriteLine($"Account balance: ${balance}");
        return balance;
    }
}

// Usage - complex banking operations made simple
var bank = new BankingFacade();

// Simple method call handles complex process
bank.TransferMoney("1234567890", "0987654321", 500.00m, 
                  "user123", "1234", "+1234567890");

Console.WriteLine();

bank.CheckBalance("1234567890", "user123", "1234");
```

### 2. **E-commerce Order Processing**
```csharp
// Subsystem classes
public class InventoryService
{
    public bool CheckAvailability(string productId, int quantity)
    {
        Console.WriteLine($"Checking inventory for {productId}, quantity: {quantity}");
        return true; // Mock availability
    }

    public void ReserveItems(string productId, int quantity)
    {
        Console.WriteLine($"Reserved {quantity} units of {productId}");
    }

    public void UpdateInventory(string productId, int quantity)
    {
        Console.WriteLine($"Updated inventory: -{quantity} units of {productId}");
    }
}

public class PaymentService
{
    public bool ProcessPayment(string cardNumber, decimal amount)
    {
        Console.WriteLine($"Processing payment of ${amount} on card ending in {cardNumber.Substring(12)}");
        return true; // Mock successful payment
    }

    public string GetTransactionId()
    {
        return $"PAY{DateTime.Now.Ticks}";
    }
}

public class ShippingService
{
    public decimal CalculateShippingCost(string address, decimal weight)
    {
        Console.WriteLine($"Calculating shipping cost to {address} for {weight}lbs");
        return 9.99m; // Mock shipping cost
    }

    public string ScheduleShipment(string address, List<string> items)
    {
        Console.WriteLine($"Scheduling shipment to {address} for {items.Count} items");
        return $"SHIP{DateTime.Now.Ticks}";
    }
}

public class CustomerService
{
    public void SendOrderConfirmation(string email, string orderId)
    {
        Console.WriteLine($"Sending order confirmation to {email} for order {orderId}");
    }

    public void SendShippingNotification(string email, string trackingNumber)
    {
        Console.WriteLine($"Sending shipping notification to {email}, tracking: {trackingNumber}");
    }
}

public class OrderService
{
    public string CreateOrder(List<string> items, string customerId)
    {
        var orderId = $"ORD{DateTime.Now.Ticks}";
        Console.WriteLine($"Created order {orderId} for customer {customerId}");
        return orderId;
    }

    public void UpdateOrderStatus(string orderId, string status)
    {
        Console.WriteLine($"Order {orderId} status updated to: {status}");
    }
}

// Facade - simplified order processing
public class OrderProcessingFacade
{
    private InventoryService _inventoryService;
    private PaymentService _paymentService;
    private ShippingService _shippingService;
    private CustomerService _customerService;
    private OrderService _orderService;

    public OrderProcessingFacade()
    {
        _inventoryService = new InventoryService();
        _paymentService = new PaymentService();
        _shippingService = new ShippingService();
        _customerService = new CustomerService();
        _orderService = new OrderService();
    }

    public bool PlaceOrder(List<string> productIds, List<int> quantities, 
                          string customerId, string email, string cardNumber, 
                          string shippingAddress)
    {
        Console.WriteLine("=== Processing Order ===");

        try
        {
            // Step 1: Check inventory
            for (int i = 0; i < productIds.Count; i++)
            {
                if (!_inventoryService.CheckAvailability(productIds[i], quantities[i]))
                {
                    Console.WriteLine($"Product {productIds[i]} not available");
                    return false;
                }
            }

            // Step 2: Calculate totals
            decimal itemTotal = productIds.Count * 25.99m; // Mock pricing
            decimal shippingCost = _shippingService.CalculateShippingCost(shippingAddress, 2.5m);
            decimal totalAmount = itemTotal + shippingCost;

            Console.WriteLine($"Order total: ${totalAmount} (Items: ${itemTotal}, Shipping: ${shippingCost})");

            // Step 3: Process payment
            if (!_paymentService.ProcessPayment(cardNumber, totalAmount))
            {
                Console.WriteLine("Payment processing failed");
                return false;
            }

            // Step 4: Create order
            var orderId = _orderService.CreateOrder(productIds, customerId);

            // Step 5: Reserve inventory
            for (int i = 0; i < productIds.Count; i++)
            {
                _inventoryService.ReserveItems(productIds[i], quantities[i]);
            }

            // Step 6: Schedule shipping
            var trackingNumber = _shippingService.ScheduleShipment(shippingAddress, productIds);

            // Step 7: Update inventory
            for (int i = 0; i < productIds.Count; i++)
            {
                _inventoryService.UpdateInventory(productIds[i], quantities[i]);
            }

            // Step 8: Send notifications
            _customerService.SendOrderConfirmation(email, orderId);
            _customerService.SendShippingNotification(email, trackingNumber);

            // Step 9: Update order status
            _orderService.UpdateOrderStatus(orderId, "Shipped");

            Console.WriteLine("=== Order Processed Successfully ===");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Order processing failed: {ex.Message}");
            return false;
        }
    }
}

// Usage - complex e-commerce process simplified
var orderProcessor = new OrderProcessingFacade();

var productIds = new List<string> { "LAPTOP001", "MOUSE002" };
var quantities = new List<int> { 1, 2 };

orderProcessor.PlaceOrder(productIds, quantities, "CUST123", 
                         "customer@email.com", "1234567890123456", 
                         "123 Main St, City, State");
```

### 3. **File Conversion System**
```csharp
// Subsystem classes - different file processors
public class PDFProcessor
{
    public void LoadPDF(string filePath) => Console.WriteLine($"Loading PDF: {filePath}");
    public string ExtractText() => "PDF text content";
    public void ConvertToFormat(string format) => Console.WriteLine($"Converting PDF to {format}");
}

public class WordProcessor
{
    public void LoadDocument(string filePath) => Console.WriteLine($"Loading Word doc: {filePath}");
    public string GetContent() => "Word document content";
    public void SaveAs(string format, string outputPath) => Console.WriteLine($"Saving as {format}: {outputPath}");
}

public class ImageProcessor
{
    public void LoadImage(string filePath) => Console.WriteLine($"Loading image: {filePath}");
    public void Resize(int width, int height) => Console.WriteLine($"Resizing to {width}x{height}");
    public void ChangeFormat(string format) => Console.WriteLine($"Converting image to {format}");
    public void Compress(int quality) => Console.WriteLine($"Compressing with quality: {quality}%");
}

public class CompressionService
{
    public void CompressFile(string filePath) => Console.WriteLine($"Compressing file: {filePath}");
    public void DecompressFile(string filePath) => Console.WriteLine($"Decompressing file: {filePath}");
}

public class FileValidator
{
    public bool ValidateFormat(string filePath, string expectedFormat)
    {
        Console.WriteLine($"Validating {filePath} format: {expectedFormat}");
        return true;
    }

    public bool CheckFileSize(string filePath, long maxSize)
    {
        Console.WriteLine($"Checking file size for: {filePath}");
        return true;
    }
}

// Facade - simplified file conversion
public class FileConversionFacade
{
    private PDFProcessor _pdfProcessor;
    private WordProcessor _wordProcessor;
    private ImageProcessor _imageProcessor;
    private CompressionService _compressionService;
    private FileValidator _validator;

    public FileConversionFacade()
    {
        _pdfProcessor = new PDFProcessor();
        _wordProcessor = new WordProcessor();
        _imageProcessor = new ImageProcessor();
        _compressionService = new CompressionService();
        _validator = new FileValidator();
    }

    public bool ConvertDocumentToPDF(string inputPath, string outputPath)
    {
        Console.WriteLine("=== Converting Document to PDF ===");

        if (!_validator.ValidateFormat(inputPath, "docx"))
            return false;

        _wordProcessor.LoadDocument(inputPath);
        var content = _wordProcessor.GetContent();
        _wordProcessor.SaveAs("pdf", outputPath);

        Console.WriteLine("Document converted to PDF successfully");
        return true;
    }

    public bool OptimizeImageForWeb(string inputPath, string outputPath)
    {
        Console.WriteLine("=== Optimizing Image for Web ===");

        if (!_validator.CheckFileSize(inputPath, 10 * 1024 * 1024)) // 10MB max
            return false;

        _imageProcessor.LoadImage(inputPath);
        _imageProcessor.Resize(800, 600);
        _imageProcessor.ChangeFormat("jpeg");
        _imageProcessor.Compress(85);

        Console.WriteLine("Image optimized for web successfully");
        return true;
    }

    public bool CreateCompressedArchive(List<string> filePaths, string archivePath)
    {
        Console.WriteLine("=== Creating Compressed Archive ===");

        foreach (var filePath in filePaths)
        {
            if (!_validator.ValidateFormat(filePath, "any"))
                continue;

            _compressionService.CompressFile(filePath);
        }

        Console.WriteLine($"Archive created: {archivePath}");
        return true;
    }
}

// Usage - complex file operations made simple
var converter = new FileConversionFacade();

converter.ConvertDocumentToPDF("document.docx", "document.pdf");
Console.WriteLine();

converter.OptimizeImageForWeb("large-image.png", "web-image.jpg");
Console.WriteLine();

var files = new List<string> { "file1.txt", "file2.pdf", "file3.jpg" };
converter.CreateCompressedArchive(files, "archive.zip");
```

## Benefits
- **Simplified Interface**: Hides complex subsystem interactions
- **Decoupling**: Clients don't depend on subsystem classes
- **Ease of Use**: Makes complex systems more accessible
- **Layered Architecture**: Promotes separation of concerns
- **Maintenance**: Changes to subsystems don't affect clients

## Drawbacks
- **Limited Functionality**: May not expose all subsystem capabilities
- **Additional Layer**: Adds another abstraction layer
- **God Object Risk**: Facade can become too large and complex
- **Tight Coupling**: Facade tightly coupled to subsystems

## When to Use
✅ **Use When:**
- You want to provide a simple interface to a complex subsystem
- There are many dependencies between clients and implementation classes
- You want to layer your subsystems
- You need to hide complexity from clients
- You want to minimize coupling between subsystem and clients

❌ **Avoid When:**
- The subsystem is already simple
- Clients need access to individual subsystem components
- You're just wrapping a single class
- The facade would be more complex than direct subsystem use

## Facade vs Other Patterns

| Pattern | Purpose | Key Difference |
|---------|---------|----------------|
| **Facade** | Simplifies complex subsystem | Provides unified interface |
| **Adapter** | Makes incompatible interfaces compatible | Focuses on interface conversion |
| **Proxy** | Controls access to an object | Focuses on access control |
| **Mediator** | Defines communication between objects | Focuses on object interaction |

## Best Practices
1. **Keep It Simple**: Facade should be simpler than subsystem
2. **Don't Hide Everything**: Provide access to advanced features when needed
3. **Single Responsibility**: Each facade method should have one clear purpose
4. **Minimal Dependencies**: Keep facade dependencies manageable
5. **Documentation**: Clearly document what the facade does
6. **Error Handling**: Provide meaningful error messages to clients

## Common Mistakes
1. **God Object**: Making facade too large and responsible for everything
2. **Leaky Abstraction**: Exposing subsystem details through facade
3. **Over-Simplification**: Hiding functionality that clients actually need
4. **Tight Coupling**: Making facade changes require client changes

## Modern C# Features
```csharp
// Using dependency injection with facade
public class ModernFacade
{
    private readonly ISubsystemA _subsystemA;
    private readonly ISubsystemB _subsystemB;

    public ModernFacade(ISubsystemA subsystemA, ISubsystemB subsystemB)
    {
        _subsystemA = subsystemA;
        _subsystemB = subsystemB;
    }

    public async Task<bool> PerformOperationAsync()
    {
        try
        {
            await _subsystemA.InitializeAsync();
            var result = await _subsystemB.ProcessAsync();
            return result.IsSuccess;
        }
        catch (Exception ex)
        {
            // Log error and return meaningful result
            return false;
        }
    }
}

// Register in DI container
services.AddScoped<ISubsystemA, SubsystemA>();
services.AddScoped<ISubsystemB, SubsystemB>();
services.AddScoped<ModernFacade>();

// Using extension methods for simple facades
public static class DatabaseFacadeExtensions
{
    public static async Task<User> GetUserWithOrdersAsync(this IDbContext context, int userId)
    {
        return await context.Users
            .Include(u => u.Orders)
            .ThenInclude(o => o.Items)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
}
```

## Testing Facades
```csharp
[Test]
public void HomeTheaterFacade_WatchMovie_CallsAllRequiredComponents()
{
    // Arrange
    var mockDvdPlayer = new Mock<DVDPlayer>();
    var mockProjector = new Mock<Projector>();
    var mockScreen = new Mock<Screen>();
    var facade = new HomeTheaterFacade(mockDvdPlayer.Object, 
                                      mockProjector.Object, 
                                      mockScreen.Object, 
                                      /* other mocks */);

    // Act
    facade.WatchMovie("Test Movie");

    // Assert
    mockDvdPlayer.Verify(d => d.TurnOn(), Times.Once);
    mockDvdPlayer.Verify(d => d.Play("Test Movie"), Times.Once);
    mockProjector.Verify(p => p.TurnOn(), Times.Once);
    mockScreen.Verify(s => s.Down(), Times.Once);
}
```

## Advanced Patterns

### Configurable Facade
```csharp
public class ConfigurableFacade
{
    private readonly Dictionary<string, ISubsystem> _subsystems;

    public ConfigurableFacade(IConfiguration config)
    {
        _subsystems = LoadSubsystems(config);
    }

    public async Task<T> ExecuteOperationAsync<T>(string operationName)
    {
        var subsystem = _subsystems[operationName];
        return await subsystem.ExecuteAsync<T>();
    }
}
```

### Facade with Caching
```csharp
public class CachingFacade
{
    private readonly IMemoryCache _cache;
    private readonly ComplexSubsystem _subsystem;

    public async Task<string> GetDataAsync(string key)
    {
        if (_cache.TryGetValue(key, out string cached))
        {
            return cached;
        }

        var result = await _subsystem.FetchDataAsync(key);
        _cache.Set(key, result, TimeSpan.FromMinutes(5));
        return result;
    }
}
```

## Summary
The Facade pattern is like having a universal remote control for your entire entertainment system - one simple interface that handles all the complexity behind the scenes. Instead of managing multiple remotes and knowing the intricate details of each device, you just press "Watch Movie" and everything works together seamlessly.

It's perfect when you have complex subsystems that clients shouldn't need to understand in detail. Think of it as the "easy button" for complex operations - you hide the complexity and provide simple, meaningful operations that clients actually want to perform.

The key insight is that sometimes the best interface is the one that hides complexity rather than exposing it, making systems more usable and maintainable.
