# 🎯 Design Patterns Practice (.NET 9)

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-green.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](http://makeapullrequest.com)

A comprehensive implementation of all 23 Gang of Four design patterns in C# using .NET 9, with real-world examples and detailed explanations.

## 🧠 What Are Design Patterns?

Design patterns are **proven solutions to recurring problems** in software design. Think of them as blueprints or templates that help you solve common programming challenges elegantly and efficiently.

### 🏠 Real-World Analogy
Just like architects use proven building patterns (blueprints for doors, windows, foundations), software developers use design patterns to solve common coding problems. You wouldn't reinvent how to build a door every time you construct a house - similarly, you shouldn't reinvent solutions for problems that have already been solved elegantly.

### 💡 Why Design Patterns Matter

```csharp
// ❌ Without Patterns: Tightly coupled, hard to maintain
public class OrderProcessor
{
    public void ProcessOrder(Order order)
    {
        // Send email directly (tight coupling)
        var emailService = new EmailService();
        emailService.SendConfirmation(order.CustomerEmail);
        
        // Process payment directly (no flexibility)
        var creditCard = new CreditCardProcessor();
        creditCard.Charge(order.Amount);
        
        // No way to undo, extend, or modify behavior
    }
}

// ✅ With Patterns: Flexible, extensible, maintainable
public class OrderProcessor
{
    private readonly INotificationService _notification; // Strategy Pattern
    private readonly IPaymentProcessor _payment;         // Strategy Pattern
    private readonly ICommandInvoker _commandInvoker;    // Command Pattern
    
    public void ProcessOrder(Order order)
    {
        var commands = new List<ICommand>
        {
            new SendNotificationCommand(_notification, order),
            new ProcessPaymentCommand(_payment, order),
            new UpdateInventoryCommand(order)
        };
        
        _commandInvoker.ExecuteCommands(commands); // Can undo, log, queue
    }
}
```

## 🏗️ Pattern Categories

### **Creational Patterns** - Object Creation
*"How do we create objects efficiently and flexibly?"*

| Pattern | Real-World Example | When to Use |
|---------|-------------------|-------------|
| **Singleton** | Database connection pool | One instance needed globally |
| **Factory Method** | UI component creation | Create objects without specifying exact classes |
| **Abstract Factory** | Cross-platform UI (Windows/Mac) | Create families of related objects |
| **Builder** | SQL query construction | Create complex objects step by step |
| **Prototype** | Document templates | Create objects by cloning existing ones |

### **Structural Patterns** - Object Composition
*"How do we compose objects to form larger structures?"*

| Pattern | Real-World Example | When to Use |
|---------|-------------------|-------------|
| **Adapter** | Payment gateway integration | Make incompatible interfaces work together |
| **Bridge** | Remote control for different devices | Separate abstraction from implementation |
| **Composite** | File system (files & folders) | Treat individual and composite objects uniformly |
| **Decorator** | Coffee shop add-ons | Add features to objects dynamically |
| **Facade** | Computer startup process | Provide simple interface to complex subsystem |
| **Flyweight** | Text editor character rendering | Share data efficiently among many objects |
| **Proxy** | Virtual image loading | Control access to another object |

### **Behavioral Patterns** - Object Interaction
*"How do objects communicate and distribute responsibilities?"*

| Pattern | Real-World Example | When to Use |
|---------|-------------------|-------------|
| **Observer** | News subscription system | Notify multiple objects about state changes |
| **Strategy** | Payment processing methods | Switch algorithms at runtime |
| **Command** | Remote control buttons | Encapsulate requests as objects |
| **State** | Vending machine states | Change behavior based on internal state |
| **Chain of Responsibility** | Help desk escalation | Pass requests through handler chain |
| **Mediator** | Air traffic control | Centralize complex communications |
| **Template Method** | Data processing pipeline | Define algorithm skeleton, vary steps |
| **Visitor** | File system operations | Add operations without changing classes |
| **Memento** | Text editor undo/redo | Capture and restore object state |
| **Iterator** | Collection traversal | Access elements sequentially |
| **Interpreter** | SQL query parsing | Define grammar for a language |

## 🚀 Real-World Benefits

### 1. **Code Reusability**
```csharp
// ✅ Strategy Pattern: Swap payment methods easily
public class ShoppingCart
{
    private IPaymentStrategy _paymentMethod;
    
    public void SetPaymentMethod(IPaymentStrategy strategy) => _paymentMethod = strategy;
    public void Checkout(decimal amount) => _paymentMethod.Pay(amount);
}

// Usage: Same cart, different payment methods
var cart = new ShoppingCart();
cart.SetPaymentMethod(new CreditCardPayment("1234-5678"));
cart.SetPaymentMethod(new PayPalPayment("user@email.com"));
cart.SetPaymentMethod(new DigitalWalletPayment("wallet123"));
```

### 2. **Maintainability**
```csharp
// ✅ Observer Pattern: Add new notification types without changing existing code
public class UserRegistration
{
    private readonly List<IUserRegistrationObserver> _observers = new();
    
    public void RegisterUser(User user)
    {
        // Register user logic...
        NotifyObservers(user);
    }
    
    // Easy to add: Email, SMS, Slack, Analytics, Audit, etc.
    public void Subscribe(IUserRegistrationObserver observer) => _observers.Add(observer);
}
```

### 3. **Testability**
```csharp
// ✅ Dependency Injection + Strategy Pattern
public class EmailService
{
    private readonly IEmailProvider _provider; // Can mock for testing
    
    public EmailService(IEmailProvider provider) => _provider = provider;
    public Task SendAsync(Email email) => _provider.SendAsync(email);
}

// Easy unit testing
[Test]
public async Task SendEmail_Should_CallProvider()
{
    var mockProvider = new Mock<IEmailProvider>();
    var service = new EmailService(mockProvider.Object);
    
    await service.SendAsync(new Email("test@test.com", "Subject", "Body"));
    
    mockProvider.Verify(p => p.SendAsync(It.IsAny<Email>()), Times.Once);
}
```

### 4. **Scalability**
```csharp
// ✅ Command Pattern: Queue, retry, and scale operations
public class TaskProcessor
{
    private readonly IQueue<ICommand> _commandQueue;
    
    public async Task ProcessAsync(ICommand command)
    {
        await _commandQueue.EnqueueAsync(command); // Scale with queues
    }
}

// Easy horizontal scaling
var commands = new[]
{
    new SendEmailCommand(email),
    new ProcessPaymentCommand(payment),
    new UpdateInventoryCommand(product)
};

await Task.WhenAll(commands.Select(cmd => processor.ProcessAsync(cmd)));
```

## 📁 Project Structure

```
DesignPatternsPractice.App/
├── DesignPatterns/
│   ├── Creational/
│   │   ├── Singleton/ (README.md + SingletonExample.cs)
│   │   ├── FactoryMethod/ (README.md + FactoryMethodExample.cs)
│   │   ├── AbstractFactory/ (README.md + AbstractFactoryExample.cs)
│   │   ├── Builder/ (README.md + BuilderExample.cs)
│   │   └── Prototype/ (README.md + PrototypeExample.cs)
│   ├── Structural/
│   │   ├── Adapter/ (README.md + AdapterExample.cs)
│   │   ├── Bridge/ (README.md + BridgeExample.cs)
│   │   ├── Composite/ (README.md + CompositeExample.cs)
│   │   ├── Decorator/ (README.md + DecoratorExample.cs)
│   │   ├── Facade/ (README.md + FacadeExample.cs)
│   │   ├── Flyweight/ (README.md + FlyweightExample.cs)
│   │   └── Proxy/ (README.md + ProxyExample.cs)
│   └── Behavioral/
│       ├── ChainOfResponsibility/ (README.md + ChainOfResponsibilityExample.cs)
│       ├── Command/ (README.md + CommandExample.cs)
│       ├── Interpreter/ (README.md + InterpreterExample.cs)
│       ├── Iterator/ (README.md + IteratorExample.cs)
│       ├── Mediator/ (README.md + MediatorExample.cs)
│       ├── Memento/ (README.md + MementoExample.cs)
│       ├── Observer/ (README.md + ObserverExample.cs)
│       ├── State/ (README.md + StateExample.cs)
│       ├── Strategy/ (README.md + StrategyExample.cs)
│       ├── TemplateMethod/ (README.md + TemplateMethodExample.cs)
│       └── Visitor/ (README.md + VisitorExample.cs)
├── Program.cs (Demonstrates all patterns)
└── README.md (This file)
```

## 🛠️ Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Visual Studio 2022, VS Code, or JetBrains Rider
- Git

### Installation
```bash
# Clone the repository
git clone https://github.com/sanjeevanannuri/LearnDesignPatterns.git
cd LearnDesignPatterns

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run all pattern demonstrations
dotnet run --project DesignPatternsPractice.App
```

### Running Individual Patterns
```bash
# Run the project and follow the interactive menu
dotnet run --project DesignPatternsPractice.App

# Or examine specific pattern implementations
# Each pattern folder contains detailed README.md and working examples
```

## 📚 Learning Path

### **For Beginners**
1. Start with **Creational Patterns** (Singleton, Factory Method)
2. Move to **Structural Patterns** (Adapter, Decorator)
3. Study **Behavioral Patterns** (Observer, Strategy)
4. Practice with the provided examples

### **For Experienced Developers**
1. Review all patterns for completeness
2. Focus on **modern C# implementations**
3. Study **real-world applications** in each README
4. Adapt patterns to your current projects

### Each Pattern Includes:
- 📖 **Detailed README** with real-world analogies
- 💻 **Working C# Code** with modern .NET 9 features
- 🌍 **Multiple Examples** showing different use cases
- ✅ **Best Practices** and when to avoid the pattern
- 🧪 **Testing Examples** and implementation tips
- 🏗️ **UML Diagrams** for visual learners

## 🎯 When to Use Design Patterns

### ✅ **Use Patterns When:**
- You have **recurring design problems**
- Code needs to be **flexible and extensible**
- Working in **teams** (common vocabulary)
- Building **large-scale applications**
- Need **proven solutions** to complex problems

### ❌ **Avoid Patterns When:**
- **Over-engineering** simple solutions
- Adding **unnecessary complexity**
- **Forcing patterns** where they don't fit
- **Premature optimization** concerns

## 🏆 Industry Applications

### **E-Commerce Platform**
```csharp
// Observer: Order status updates → Email, SMS, Push notifications
// Strategy: Payment processing → Credit Card, PayPal, Crypto
// Factory: Product creation → Physical, Digital, Subscription
// Command: Shopping cart operations → Add, Remove, Undo
// State: Order workflow → Pending → Paid → Shipped → Delivered
```

### **Banking System**
```csharp
// Singleton: Configuration, Audit logger
// Chain of Responsibility: Transaction approval → Amount → Risk → Compliance
// Memento: Transaction rollback and audit trails
// Proxy: Security access control and rate limiting
// Template Method: Account processing → Savings, Checking, Credit
```

### **Game Development**
```csharp
// State: Character states → Idle, Running, Jumping, Attacking
// Command: Player input → Move, Attack, Cast Spell (with replay)
// Observer: Achievement system → XP gain, Item collection
// Flyweight: Sprite rendering → Share textures, optimize memory
// Visitor: Game object processing → AI, Physics, Rendering
```

### **Enterprise Software**
```csharp
// Facade: API Gateway → Simplify microservice interactions
// Adapter: Legacy system integration → Old database, External APIs
// Decorator: Security layers → Authentication, Authorization, Logging
// Mediator: Event bus → Decouple service communications
// Builder: Complex configuration → Multi-environment setups
```

## 🚀 Modern C# Features Used

This project showcases modern C# and .NET 9 features:

- **Records** for immutable data structures
- **Pattern Matching** with switch expressions
- **Nullable Reference Types** for better null safety
- **Async/Await** for non-blocking operations
- **Generic Constraints** for type safety
- **Extension Methods** for fluent APIs
- **Dependency Injection** for loose coupling
- **Top-level Programs** for cleaner entry points
- **File-scoped Namespaces** for reduced nesting
- **Global Using Statements** for common imports

## 🧪 Testing

The project includes comprehensive examples of testing design patterns:

```bash
# Run all tests (if test project is added)
dotnet test

# Each pattern includes testing guidance in its README
# Focus on testing behavior, not implementation details
```

## 🤝 Contributing

We welcome contributions! Here's how you can help:

### Ways to Contribute
- 🐛 **Bug Reports** - Found an issue? Open an issue with details
- 💡 **Feature Requests** - Suggest improvements or new examples
- 📖 **Documentation** - Improve explanations or add more real-world examples
- 🔧 **Code** - Submit pull requests with enhancements
- 🌍 **Translations** - Help translate documentation to other languages

### Development Guidelines
1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Commit** your changes (`git commit -m 'Add amazing feature'`)
4. **Push** to the branch (`git push origin feature/amazing-feature`)
5. **Open** a Pull Request

### Code Standards
- Follow existing code style and conventions
- Include comprehensive documentation
- Add unit tests for new functionality
- Ensure all examples compile and run
- Use modern C# features appropriately

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.

## 🙏 Acknowledgments

- **Gang of Four** - For the original Design Patterns book
- **Microsoft** - For the excellent .NET platform
- **Community** - For feedback and contributions
- **Martin Fowler** - For architectural insights
- **Robert C. Martin** - For clean code principles

## 📚 Additional Resources

### Books
- [Design Patterns: Elements of Reusable Object-Oriented Software](https://www.amazon.com/Design-Patterns-Elements-Reusable-Object-Oriented/dp/0201633612) - The original Gang of Four book
- [Head First Design Patterns](https://www.oreilly.com/library/view/head-first-design/0596007124/) - Beginner-friendly approach
- [Clean Code](https://www.amazon.com/Clean-Code-Handbook-Software-Craftsmanship/dp/0132350882) - Robert C. Martin
- [Refactoring](https://martinfowler.com/books/refactoring.html) - Martin Fowler

### Online Resources
- [Refactoring Guru](https://refactoring.guru/design-patterns) - Visual explanations and examples
- [Microsoft .NET Documentation](https://docs.microsoft.com/en-us/dotnet/) - Official .NET guidance
- [C# Design Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/) - Microsoft's recommendations
- [Gang of Four Patterns](https://en.wikipedia.org/wiki/Design_Patterns) - Wikipedia reference



## 📊 Project Stats

- **23 Design Patterns** - Complete Gang of Four implementation
- **100+ Code Examples** - Real-world scenarios and use cases
- **Comprehensive Documentation** - Every pattern explained in detail
- **.NET 9** - Latest C# features and best practices
- **Production Ready** - Patterns used in enterprise applications

## 🌟 Star History

If this project helped you learn design patterns, please consider giving it a star! ⭐

## 📞 Contact & Support

- **Issues** - [GitHub Issues](https://github.com/sanjeevanannuri/LearnDesignPatterns/issues)
- **Discussions** - [GitHub Discussions](https://github.com/sanjeevanannuri/LearnDesignPatterns/discussions)
- **Repository** - [LearnDesignPatterns](https://github.com/sanjeevanannuri/LearnDesignPatterns)

---

**Happy coding!** 🎉 Master these patterns, and you'll write more maintainable, flexible, and professional code.

*"Patterns are solutions to problems that occur over and over again in object-oriented design."* - Gang of Four
