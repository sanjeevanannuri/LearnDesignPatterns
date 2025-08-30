# 🎯 Design Patterns Practice (.NET 9)

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-green.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](http://makeapullrequest.com)

> **Master all 23 Gang of Four design patterns with comprehensive C# examples and real-world use cases!**

A complete, production-ready implementation of design patterns using modern C# 12 and .NET 9, perfect for learning, teaching, and professional reference.

## 🌟 What Makes This Special?

### 📚 **Complete Coverage**
- **All 23 Gang of Four patterns** implemented with modern C#
- **Real-world examples** from industry applications
- **Multiple use cases** for each pattern (3+ scenarios per pattern)
- **Production-ready code** that you can use in actual projects

### 🎯 **Learning-Focused**
- **Real-world analogies** (coffee shops, remote controls, traffic lights)
- **Problem-solution approach** showing why each pattern exists
- **When to use vs when to avoid** with honest assessments
- **Progressive complexity** from simple to advanced implementations

### 🚀 **Modern & Professional**
- **Latest C# 12 features** (records, pattern matching, nullable types)
- **.NET 9 optimizations** and best practices
- **Enterprise-grade examples** (e-commerce, banking, gaming)
- **Comprehensive testing** guidance and examples

## 📖 Quick Start

```bash
# Clone and run
git clone https://github.com/sanjeevanannuri/LearnDesignPatterns.git
cd LearnDesignPatterns
dotnet run --project DesignPatternsPractice.App

# Choose any pattern from the interactive menu!
```

📺 **Watch the Video Tutorial**: [Design Patterns Explained](https://youtu.be/fx08IcKMCNE) - Complete walkthrough of all patterns with practical examples!

## 🏗️ All 23 Patterns Covered

<details>
<summary><b>🔨 Creational Patterns (5)</b> - Object Creation</summary>

| Pattern | What It Does | Real Example |
|---------|-------------|--------------|
| **[Singleton](DesignPatternsPractice.App/DesignPatterns/Creational/Singleton/)** | One instance globally | Database connection pool |
| **[Factory Method](DesignPatternsPractice.App/DesignPatterns/Creational/FactoryMethod/)** | Create without specifying class | UI component factory |
| **[Abstract Factory](DesignPatternsPractice.App/DesignPatterns/Creational/AbstractFactory/)** | Create families of objects | Cross-platform UI themes |
| **[Builder](DesignPatternsPractice.App/DesignPatterns/Creational/Builder/)** | Build complex objects step-by-step | SQL query builder |
| **[Prototype](DesignPatternsPractice.App/DesignPatterns/Creational/Prototype/)** | Clone existing objects | Document templates |

</details>

<details>
<summary><b>🔧 Structural Patterns (7)</b> - Object Composition</summary>

| Pattern | What It Does | Real Example |
|---------|-------------|--------------|
| **[Adapter](DesignPatternsPractice.App/DesignPatterns/Structural/Adapter/)** | Make incompatible interfaces work | Payment gateway integration |
| **[Bridge](DesignPatternsPractice.App/DesignPatterns/Structural/Bridge/)** | Separate abstraction from implementation | Remote control for devices |
| **[Composite](DesignPatternsPractice.App/DesignPatterns/Structural/Composite/)** | Tree structures of objects | File system (files & folders) |
| **[Decorator](DesignPatternsPractice.App/DesignPatterns/Structural/Decorator/)** | Add features dynamically | Coffee shop add-ons |
| **[Facade](DesignPatternsPractice.App/DesignPatterns/Structural/Facade/)** | Simplified interface to complex system | Home theater system |
| **[Flyweight](DesignPatternsPractice.App/DesignPatterns/Structural/Flyweight/)** | Share data efficiently | Text editor characters |
| **[Proxy](DesignPatternsPractice.App/DesignPatterns/Structural/Proxy/)** | Control access to objects | Virtual image loading |

</details>

<details>
<summary><b>🎭 Behavioral Patterns (11)</b> - Object Interaction</summary>

| Pattern | What It Does | Real Example |
|---------|-------------|--------------|
| **[Chain of Responsibility](DesignPatternsPractice.App/DesignPatterns/Behavioral/ChainOfResponsibility/)** | Pass requests through handler chain | Help desk escalation |
| **[Command](DesignPatternsPractice.App/DesignPatterns/Behavioral/Command/)** | Encapsulate requests as objects | Text editor undo/redo |
| **[Interpreter](DesignPatternsPractice.App/DesignPatterns/Behavioral/Interpreter/)** | Define language grammar | Mathematical expressions |
| **[Iterator](DesignPatternsPractice.App/DesignPatterns/Behavioral/Iterator/)** | Access elements sequentially | Custom collection traversal |
| **[Mediator](DesignPatternsPractice.App/DesignPatterns/Behavioral/Mediator/)** | Centralize complex communications | Air traffic control |
| **[Memento](DesignPatternsPractice.App/DesignPatterns/Behavioral/Memento/)** | Capture and restore state | Game save system |
| **[Observer](DesignPatternsPractice.App/DesignPatterns/Behavioral/Observer/)** | Notify multiple objects of changes | Stock price updates |
| **[State](DesignPatternsPractice.App/DesignPatterns/Behavioral/State/)** | Change behavior based on state | Vending machine |
| **[Strategy](DesignPatternsPractice.App/DesignPatterns/Behavioral/Strategy/)** | Switch algorithms at runtime | Payment methods |
| **[Template Method](DesignPatternsPractice.App/DesignPatterns/Behavioral/TemplateMethod/)** | Define algorithm skeleton | Data processing pipeline |
| **[Visitor](DesignPatternsPractice.App/DesignPatterns/Behavioral/Visitor/)** | Add operations without changing classes | File system operations |

</details>

## 🎯 Real-World Impact

### Before Design Patterns ❌
```csharp
// Rigid, hard to maintain, impossible to extend
public class OrderProcessor
{
    public void ProcessOrder(Order order)
    {
        // Hardcoded email sending
        var smtp = new SmtpClient();
        smtp.Send(new MailMessage(order.Email, "Order Confirmed"));
        
        // Hardcoded payment processing
        var creditCard = new CreditCardProcessor();
        creditCard.Charge(order.Amount);
        
        // No flexibility, no testing, no extensibility
    }
}
```

### After Design Patterns ✅
```csharp
// Flexible, maintainable, easily extensible
public class OrderProcessor
{
    private readonly INotificationStrategy _notification; // Strategy Pattern
    private readonly IPaymentProcessor _payment;          // Strategy Pattern
    private readonly List<IOrderObserver> _observers;     // Observer Pattern
    
    public async Task ProcessOrderAsync(Order order)
    {
        // Execute with undo capability
        var command = new ProcessOrderCommand(order, _payment, _notification);
        await _commandInvoker.ExecuteAsync(command); // Command Pattern
        
        // Notify all interested parties
        _observers.ForEach(o => o.OnOrderProcessed(order)); // Observer Pattern
    }
}

// Now you can easily:
// - Switch payment methods (Strategy)
// - Add new notification types (Observer)
// - Undo operations (Command)
// - Test with mocks (Dependency Injection)
// - Scale horizontally (Command Queue)
```

## 🏆 Industry Examples

<details>
<summary><b>🛒 E-Commerce Platform</b></summary>

```csharp
// Real patterns used in production e-commerce:

// Strategy: Payment Processing
services.AddScoped<IPaymentStrategy, CreditCardStrategy>();
services.AddScoped<IPaymentStrategy, PayPalStrategy>();
services.AddScoped<IPaymentStrategy, CryptoStrategy>();

// Observer: Order Status Updates
order.Subscribe(new EmailNotifier());
order.Subscribe(new SMSNotifier());
order.Subscribe(new InventoryUpdater());
order.Subscribe(new AnalyticsTracker());

// Command: Shopping Cart Operations
var cart = new ShoppingCart();
cart.Execute(new AddItemCommand(product));
cart.Execute(new ApplyCouponCommand(coupon));
cart.Undo(); // Remove last operation

// State: Order Workflow
order.State = new PendingState();    // Customer can modify
order.State = new ProcessingState(); // Being prepared
order.State = new ShippedState();    // In transit
order.State = new DeliveredState();  // Complete
```

</details>

<details>
<summary><b>🏦 Banking System</b></summary>

```csharp
// Real patterns used in banking applications:

// Chain of Responsibility: Transaction Approval
var approvalChain = new AmountHandler()
    .SetNext(new RiskAssessmentHandler())
    .SetNext(new ComplianceHandler())
    .SetNext(new FinalApprovalHandler());

// Memento: Transaction Rollback
var transaction = new BankTransaction(account, amount);
var memento = transaction.CreateSnapshot();
// ... if something goes wrong ...
transaction.RestoreSnapshot(memento);

// Proxy: Security & Access Control
var secureAccount = new SecurityProxy(realAccount);
secureAccount.Withdraw(amount); // Checks permissions first

// Template Method: Different Account Types
public abstract class AccountProcessor
{
    public void ProcessTransaction() // Template
    {
        ValidateAccount();     // Same for all
        CalculateFees();       // Different per type
        UpdateBalance();       // Same for all
        SendNotification();    // Different per type
    }
}
```

</details>

## 🚀 Modern C# Features Showcased

- **Records & Pattern Matching**
- **Nullable Reference Types**
- **Async/Await Patterns**
- **Generic Constraints**
- **Extension Methods**
- **Dependency Injection**
- **File-scoped Namespaces**
- **Global Using Statements**

## 📚 Learning Paths

### 🌱 **Beginner Path**
1. **Start Here**: [Singleton](DesignPatternsPractice.App/DesignPatterns/Creational/Singleton/) → [Factory Method](DesignPatternsPractice.App/DesignPatterns/Creational/FactoryMethod/)
2. **Then Try**: [Adapter](DesignPatternsPractice.App/DesignPatterns/Structural/Adapter/) → [Decorator](DesignPatternsPractice.App/DesignPatterns/Structural/Decorator/)
3. **Finally**: [Observer](DesignPatternsPractice.App/DesignPatterns/Behavioral/Observer/) → [Strategy](DesignPatternsPractice.App/DesignPatterns/Behavioral/Strategy/)

### 🔥 **Professional Path**
1. **Master All Creational** for object creation flexibility
2. **Study All Structural** for system architecture
3. **Deep Dive Behavioral** for complex interactions

### Each Pattern Includes:
- 📖 **Comprehensive README** with real-world analogies
- 💻 **Production-ready C# code** with modern features
- 🌍 **Multiple examples** from different industries
- ✅ **Best practices** and anti-patterns to avoid
- 🧪 **Testing strategies** and mock examples
- 🎯 **When to use** and when to choose alternatives

## 🤝 Contributing

We love contributions! Whether you're:
- 🐛 **Reporting bugs**
- 💡 **Suggesting features** 
- 📖 **Improving documentation**
- 🔧 **Adding examples**
- 🌍 **Translating content**

See our [Contributing Guide](CONTRIBUTING.md) to get started!

## 📄 License

MIT License - feel free to use this in your projects, courses, or wherever you need!

## 🌟 Star History

If this project helped you master design patterns, please consider giving it a star! ⭐

## 🏅 Why This Repository?

✅ **Complete & Comprehensive** - All 23 patterns with real examples  
✅ **Modern & Current** - Latest C# 12 and .NET 9 features  
✅ **Production-Ready** - Code you can actually use in projects  
✅ **Learning-Focused** - Built specifically for education  
✅ **Industry-Relevant** - Examples from real business domains  
✅ **Well-Documented** - Every pattern thoroughly explained  
✅ **Community-Driven** - Open source with active contributions  

---

**Ready to become a design patterns expert?** 🚀 Clone the repo and start exploring!

*"Design patterns represent solutions to problems that arise when developing software within a particular context."* - Gang of Four
