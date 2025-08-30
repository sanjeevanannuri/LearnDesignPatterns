using DesignPatterns.Creational.Singleton;
using DesignPatterns.Creational.FactoryMethod;
using DesignPatterns.Creational.AbstractFactory;
using DesignPatterns.Creational.Builder;
using DesignPatterns.Creational.Prototype;
using DesignPatterns.Structural.Adapter;
using DesignPatterns.Structural.Bridge;
using DesignPatterns.Structural.Composite;
using DesignPatterns.Structural.Decorator;
using DesignPatterns.Structural.Facade;
using DesignPatterns.Structural.Flyweight;
using DesignPatterns.Structural.Proxy;
using DesignPatterns.Behavioral.ChainOfResponsibility;
using DesignPatterns.Behavioral.Command;
using DesignPatterns.Behavioral.Observer;
using DesignPatterns.Behavioral.Strategy;
using DesignPatterns.Behavioral.State;
using DesignPatterns.Behavioral.TemplateMethod;
using DesignPatterns.Behavioral.Visitor;
using DesignPatterns.Behavioral.Mediator;
using DesignPatterns.Behavioral.Memento;
using DesignPatterns.Behavioral.Iterator;
using DesignPatterns.Behavioral.Interpreter;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

// Singleton pattern demo
Console.WriteLine("--- Singleton Pattern Demo ---");
var singleton1 = Singleton.Instance;
var singleton2 = Singleton.Instance;

singleton1.Message = "Singleton instance updated!";
Console.WriteLine(singleton2.Message); // Should print the updated message

// Factory Method pattern demo
Console.WriteLine("\n--- Factory Method Pattern Demo ---");
Creator creatorA = new ConcreteCreatorA();
Creator creatorB = new ConcreteCreatorB();
IProduct productA = creatorA.FactoryMethod();
IProduct productB = creatorB.FactoryMethod();
Console.WriteLine(productA.Operation());
Console.WriteLine(productB.Operation());

// Abstract Factory pattern demo
Console.WriteLine("\n--- Abstract Factory Pattern Demo ---");
IGUIFactory factory = new WindowsFactory();
IButton button = factory.CreateButton();
ICheckbox checkbox = factory.CreateCheckbox();
Console.WriteLine(button.Paint());
Console.WriteLine(checkbox.Paint());

factory = new MacFactory();
button = factory.CreateButton();
checkbox = factory.CreateCheckbox();
Console.WriteLine(button.Paint());
Console.WriteLine(checkbox.Paint());

// Builder pattern demo
Console.WriteLine("\n--- Builder Pattern Demo ---");
var director = new DesignPatterns.Creational.Builder.Director();
var builder = new WoodenHouseBuilder();
director.Construct(builder);
House house = builder.GetResult();
Console.WriteLine(house);

// Prototype pattern demo
Console.WriteLine("\n--- Prototype Pattern Demo ---");
var manager = new DocumentManager();

// Create and register prototypes
var reportTemplate = new DesignPatterns.Creational.Prototype.Document("Report Template", "This is a report template");
reportTemplate.Tags.Add("Report");
reportTemplate.Tags.Add("Template");
manager.RegisterPrototype("report", reportTemplate);

var letterTemplate = new DesignPatterns.Creational.Prototype.Document("Letter Template", "This is a letter template");
letterTemplate.Tags.Add("Letter");
letterTemplate.Tags.Add("Template");
manager.RegisterPrototype("letter", letterTemplate);

// Clone documents from prototypes
var report1 = manager.CreateDocument("report");
report1.Title = "Monthly Report";
report1.Content = "Monthly sales report content";

var report2 = manager.CreateDocument("report");
report2.Title = "Quarterly Report";

Console.WriteLine("Original template: " + reportTemplate);
Console.WriteLine("Cloned report 1: " + report1);
Console.WriteLine("Cloned report 2: " + report2);

// === STRUCTURAL PATTERNS ===

// Adapter pattern demo
Console.WriteLine("\n--- Adapter Pattern Demo ---");
var adaptee = new Adaptee();
ITarget target = new Adapter(adaptee);
Console.WriteLine(target.GetRequest());

// Real-world example: Payment adapter
Console.WriteLine("\nPayment Adapter Example:");
var payPalApi = new PayPalApi();
IPaymentProcessor paymentProcessor = new PayPalAdapter(payPalApi);
Console.WriteLine(paymentProcessor.ProcessPayment(100.50m));

// Bridge pattern demo
Console.WriteLine("\n--- Bridge Pattern Demo ---");
var tv = new TV();
var radio = new Radio();

var remoteControl = new DesignPatterns.Structural.Bridge.RemoteControl(tv);
Console.WriteLine("TV Remote: " + remoteControl.TogglePower());
Console.WriteLine("TV Remote: " + remoteControl.VolumeUp());

var advancedRemote = new AdvancedRemoteControl(radio);
Console.WriteLine("Radio Advanced Remote: " + advancedRemote.TogglePower());
Console.WriteLine("Radio Advanced Remote: " + advancedRemote.VolumeUp());
Console.WriteLine("Radio Advanced Remote: " + advancedRemote.Mute());

// Composite pattern demo
Console.WriteLine("\n--- Composite Pattern Demo ---");
var root = new DesignPatterns.Structural.Composite.Directory("Root");
var documents = new DesignPatterns.Structural.Composite.Directory("Documents");
var pictures = new DesignPatterns.Structural.Composite.Directory("Pictures");

var file1 = new DesignPatterns.Structural.Composite.File("resume.pdf", 500);
var file2 = new DesignPatterns.Structural.Composite.File("photo1.jpg", 1200);
var file3 = new DesignPatterns.Structural.Composite.File("photo2.jpg", 800);
var file4 = new DesignPatterns.Structural.Composite.File("config.txt", 50);

documents.Add(file1);
documents.Add(file4);
pictures.Add(file2);
pictures.Add(file3);

root.Add(documents);
root.Add(pictures);

Console.WriteLine("File system structure:");
root.Display();
Console.WriteLine($"\nTotal size: {root.GetSize()}KB");

// Decorator pattern demo
Console.WriteLine("\n--- Decorator Pattern Demo ---");
ICoffee coffee = new SimpleCoffee();
Console.WriteLine($"{coffee.GetDescription()} costs ${coffee.GetCost()}");

coffee = new MilkDecorator(coffee);
Console.WriteLine($"{coffee.GetDescription()} costs ${coffee.GetCost()}");

coffee = new SugarDecorator(coffee);
Console.WriteLine($"{coffee.GetDescription()} costs ${coffee.GetCost()}");

coffee = new WhipDecorator(coffee);
Console.WriteLine($"{coffee.GetDescription()} costs ${coffee.GetCost()}");

// Create another coffee with different decorators
ICoffee fancyCoffee = new VanillaDecorator(new WhipDecorator(new MilkDecorator(new SimpleCoffee())));
Console.WriteLine($"\nFancy: {fancyCoffee.GetDescription()} costs ${fancyCoffee.GetCost()}");

// Facade pattern demo
Console.WriteLine("\n--- Facade Pattern Demo ---");
var computer = new ComputerFacade();
computer.StartComputer();

Console.WriteLine("\nHome Theater Example:");
var homeTheater = new HomeTheaterFacade();
homeTheater.WatchMovie("The Matrix");
homeTheater.EndMovie();

// Flyweight pattern demo
Console.WriteLine("\n--- Flyweight Pattern Demo ---");
var document = new DesignPatterns.Structural.Flyweight.Document();
document.AddCharacter('H', 12, "Black", "Arial");
document.AddCharacter('e', 12, "Black", "Arial");
document.AddCharacter('l', 12, "Black", "Arial");
document.AddCharacter('l', 12, "Black", "Arial");  // Reuses existing 'l' flyweight
document.AddCharacter('o', 12, "Black", "Arial");
document.AddCharacter(' ', 12, "Black", "Arial");
document.AddCharacter('W', 14, "Blue", "Times");
document.AddCharacter('o', 14, "Blue", "Times");   // Reuses existing 'o' flyweight
document.AddCharacter('r', 14, "Blue", "Times");
document.AddCharacter('l', 14, "Blue", "Times");   // Reuses existing 'l' flyweight
document.AddCharacter('d', 14, "Blue", "Times");
document.Display();

Console.WriteLine("\nForest Example:");
var forest = new Forest();
forest.PlantTree(10, 20, "Oak", "oak_sprite.png");
forest.PlantTree(15, 25, "Pine", "pine_sprite.png");
forest.PlantTree(20, 30, "Oak", "oak_sprite.png");    // Reuses Oak tree type
forest.PlantTree(25, 35, "Birch", "birch_sprite.png");
forest.PlantTree(30, 40, "Pine", "pine_sprite.png");   // Reuses Pine tree type
forest.PlantTree(35, 45, "Oak", "oak_sprite.png");     // Reuses Oak tree type
forest.Render("Summer");

// Proxy pattern demo
Console.WriteLine("\n--- Proxy Pattern Demo ---");
Console.WriteLine("1. Virtual Proxy (Lazy Loading):");
IImage image1 = new ProxyImage("photo1.jpg");
IImage image2 = new ProxyImage("photo2.jpg");

Console.WriteLine("Images created (not loaded yet)");
Console.WriteLine("Displaying image1 for first time:");
image1.Display();
Console.WriteLine("Displaying image1 again (already loaded):");
image1.Display();

Console.WriteLine("\n2. Protection Proxy (Access Control):");
var account = new BankAccount("12345", 1000);
var ownerProxy = new BankAccountProxy(account, "Owner");
var guestProxy = new BankAccountProxy(account, "Guest");

Console.WriteLine("Owner operations:");
Console.WriteLine($"Balance: ${ownerProxy.GetBalance()}");
ownerProxy.Withdraw(100);

Console.WriteLine("\nGuest operations:");
Console.WriteLine($"Balance: ${guestProxy.GetBalance()}");
guestProxy.Withdraw(50);

Console.WriteLine("\n3. Caching Proxy:");
var databaseService = new DatabaseService();
var cachingProxy = new CachingProxy(databaseService);

Console.WriteLine("First request (will hit database):");
Console.WriteLine(cachingProxy.GetData("user123"));
Console.WriteLine("\nSecond request (will use cache):");
Console.WriteLine(cachingProxy.GetData("user123"));
Console.WriteLine("\nThird request with different key (will hit database):");
Console.WriteLine(cachingProxy.GetData("user456"));

// === BEHAVIORAL PATTERNS ===

// Chain of Responsibility pattern demo
Console.WriteLine("\n--- Chain of Responsibility Pattern Demo ---");
Console.WriteLine("1. Expense Approval System:");

// Set up the chain
var teamLeader = new TeamLeader();
var expenseManager = new Manager();
var expenseDirector = new DesignPatterns.Behavioral.ChainOfResponsibility.Director();
var ceo = new CEO();

teamLeader.SetNext(expenseManager);
expenseManager.SetNext(expenseDirector);
expenseDirector.SetNext(ceo);

// Test different expense amounts
var expense1 = new ExpenseRequest("John", 500, "Office supplies");
var expense2 = new ExpenseRequest("Jane", 3000, "New laptop");
var expense3 = new ExpenseRequest("Bob", 15000, "Team training");
var expense4 = new ExpenseRequest("Alice", 50000, "Office renovation");

teamLeader.HandleRequest(expense1);
Console.WriteLine();
teamLeader.HandleRequest(expense2);
Console.WriteLine();
teamLeader.HandleRequest(expense3);
Console.WriteLine();
teamLeader.HandleRequest(expense4);

Console.WriteLine("\n2. Support Ticket System:");

// Set up support chain
var level1 = new Level1Support();
var level2 = new Level2Support();
var level3 = new Level3Support();
var management = new ManagementSupport();

level1.SetNext(level2);
level2.SetNext(level3);
level3.SetNext(management);

// Test different ticket levels
var ticket1 = new SupportTicket("Customer A", "Password reset", TicketLevel.Level1);
var ticket2 = new SupportTicket("Customer B", "Software installation issue", TicketLevel.Level2);
var ticket3 = new SupportTicket("Customer C", "System integration problem", TicketLevel.Level3);
var ticket4 = new SupportTicket("Customer D", "Critical system outage", TicketLevel.Management);

level1.HandleTicket(ticket1);
Console.WriteLine();
level1.HandleTicket(ticket2);
Console.WriteLine();
level1.HandleTicket(ticket3);
Console.WriteLine();
level1.HandleTicket(ticket4);

// Command pattern demo
Console.WriteLine("\n--- Command Pattern Demo ---");
Console.WriteLine("1. Remote Control System:");

// Create receivers
var livingRoomLight = new DesignPatterns.Behavioral.Command.Light("Living Room");
var kitchenLight = new DesignPatterns.Behavioral.Command.Light("Kitchen");
var stereo = new DesignPatterns.Behavioral.Command.Stereo("Living Room");

// Create commands
var livingRoomLightOn = new LightOnCommand(livingRoomLight);
var livingRoomLightOff = new LightOffCommand(livingRoomLight);
var kitchenLightOn = new LightOnCommand(kitchenLight);
var kitchenLightOff = new LightOffCommand(kitchenLight);
var stereoOnWithVolume = new StereoOnWithVolumeCommand(stereo);
var stereoOff = new StereoOffCommand(stereo);

// Create remote control and set commands
var remote = new DesignPatterns.Behavioral.Command.RemoteControl();
remote.SetCommand(0, livingRoomLightOn, livingRoomLightOff);
remote.SetCommand(1, kitchenLightOn, kitchenLightOff);
remote.SetCommand(2, stereoOnWithVolume, stereoOff);

Console.WriteLine(remote);

// Test commands
Console.WriteLine("Testing individual commands:");
remote.OnButtonPressed(0);  // Living room light on
remote.OffButtonPressed(0); // Living room light off
remote.OnButtonPressed(1);  // Kitchen light on
remote.OnButtonPressed(2);  // Stereo on with volume
remote.UndoButtonPressed(); // Undo stereo command

Console.WriteLine("\n2. Macro Command (Party Mode):");
var partyOn = new ICommand[] { livingRoomLightOn, kitchenLightOn, stereoOnWithVolume };
var partyOff = new ICommand[] { livingRoomLightOff, kitchenLightOff, stereoOff };
var partyOnMacro = new MacroCommand(partyOn);
var partyOffMacro = new MacroCommand(partyOff);

remote.SetCommand(3, partyOnMacro, partyOffMacro);

Console.WriteLine("Party mode ON:");
remote.OnButtonPressed(3);
Console.WriteLine("\nParty mode OFF:");
remote.OffButtonPressed(3);

Console.WriteLine("\n3. Text Editor Commands:");
var commandEditor = new DesignPatterns.Behavioral.Command.TextEditor();
var writeHello = new WriteCommand(commandEditor, "Hello ");
var writeWorld = new WriteCommand(commandEditor, "World!");
var deleteCommand = new DeleteCommand(commandEditor, 6);

Console.WriteLine("Executing commands:");
writeHello.Execute();
writeWorld.Execute();
deleteCommand.Execute();

Console.WriteLine("\nUndoing commands:");
deleteCommand.Undo();
writeWorld.Undo();
writeHello.Undo();

// Observer pattern demo
Console.WriteLine("\n--- Observer Pattern Demo ---");
Console.WriteLine("1. Stock Market Monitoring:");

// Create stock
var appleStock = new DesignPatterns.Behavioral.Observer.Stock("AAPL", 150.00m);

// Create observers
var trader1 = new StockTrader("John", 140.00m, 160.00m);
var trader2 = new StockTrader("Jane", 145.00m, 155.00m);
var display = new StockDisplay("Market Display");

// Attach observers
appleStock.Attach(trader1);
appleStock.Attach(trader2);
appleStock.Attach(display);

// Simulate price changes
appleStock.SetPrice(142.00m); // Should trigger buy signals
Console.WriteLine();
appleStock.SetPrice(158.00m); // Should trigger sell signals
Console.WriteLine();
appleStock.SetPrice(149.00m); // Should trigger hold signals

// Detach one observer
Console.WriteLine();
appleStock.Detach(trader1);
appleStock.SetPrice(165.00m); // Only trader2 and display should respond

Console.WriteLine("\n2. Newsletter Subscription System:");

var techNewsletter = new DesignPatterns.Behavioral.Observer.Newsletter();

// Create subscribers
var emailSub1 = new EmailSubscriber("john@example.com");
var emailSub2 = new EmailSubscriber("jane@example.com");
var smsSub1 = new SmsSubscriber("555-1234");

// Subscribe
techNewsletter.Attach(emailSub1);
techNewsletter.Attach(emailSub2);
techNewsletter.Attach(smsSub1);

// Publish articles
techNewsletter.PublishArticle("New AI Breakthrough Announced");
techNewsletter.PublishArticle("Quantum Computing Update");

// Unsubscribe one user
techNewsletter.Detach(emailSub1);
techNewsletter.PublishArticle("Tech Conference Next Week");

Console.WriteLine("\n3. Weather Station Monitoring:");

var weatherStation = new DesignPatterns.Behavioral.Observer.WeatherStation();

// Create weather displays
var currentDisplay = new CurrentConditionsDisplay();
var statisticsDisplay = new StatisticsDisplay();
var forecastDisplay = new ForecastDisplay();

// Register displays
weatherStation.Attach(currentDisplay);
weatherStation.Attach(statisticsDisplay);
weatherStation.Attach(forecastDisplay);

// Simulate weather updates
weatherStation.SetMeasurements(80, 65, 30.4f);
weatherStation.SetMeasurements(82, 70, 29.2f);
weatherStation.SetMeasurements(78, 90, 29.2f);

// Strategy pattern demo
Console.WriteLine("\n--- Strategy Pattern Demo ---");
Console.WriteLine("1. Payment Processing System:");

// Create shopping cart
var cart = new ShoppingCart();
cart.AddItem("Laptop");
cart.AddItem("Mouse");
cart.AddItem("Keyboard");

// Try different payment strategies
Console.WriteLine("\nPaying with Credit Card:");
cart.SetPaymentStrategy(new CreditCardPayment("1234567890123456", "John Doe", "123", "12/25"));
cart.Checkout(1500.00m);

Console.WriteLine("\nPaying with PayPal:");
cart.SetPaymentStrategy(new PayPalPayment("john@example.com", "password123"));
cart.Checkout(1500.00m);

Console.WriteLine("\nPaying with Bitcoin:");
cart.SetPaymentStrategy(new BitcoinPayment("1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa"));
cart.Checkout(1500.00m);

Console.WriteLine("\n2. Sorting Algorithms:");

// Create different datasets
var smallList = new List<int> { 64, 34, 25, 12, 22, 11, 90 };
var mediumList = new List<int> { 64, 34, 25, 12, 22, 11, 90, 5, 77, 30, 88, 66 };

var sortContext = new SortContext();

// Test Bubble Sort
sortContext.SetSortStrategy(new BubbleSortStrategy());
sortContext.SortNumbers(new List<int>(smallList));

// Test Quick Sort
sortContext.SetSortStrategy(new QuickSortStrategy());
sortContext.SortNumbers(new List<int>(mediumList));

// Test Merge Sort
sortContext.SetSortStrategy(new MergeSortStrategy());
sortContext.SortNumbers(new List<int>(mediumList));

Console.WriteLine("3. File Compression:");

var files = new List<string> { "document.txt", "image.jpg", "video.mp4", "archive.pdf" };
var compressionContext = new CompressionContext();

Console.WriteLine("Using ZIP compression:");
compressionContext.SetCompressionStrategy(new ZipCompression());
compressionContext.CompressFiles(files);

Console.WriteLine("\nUsing RAR compression:");
compressionContext.SetCompressionStrategy(new RarCompression());
compressionContext.CompressFiles(files);

// State pattern demo
Console.WriteLine("\n--- State Pattern Demo ---");

Console.WriteLine("1. Vending Machine State Management:");
var vendingMachine = new VendingMachine(productStock: 3);
Console.WriteLine($"Initial state: {vendingMachine.CurrentState}");

// Try to select product without coins
vendingMachine.SelectProduct();

// Insert coin and select product
vendingMachine.InsertCoin();
vendingMachine.SelectProduct();
vendingMachine.DispenseProduct();

Console.WriteLine($"Final state: {vendingMachine.CurrentState}");

Console.WriteLine("\n2. Document Workflow Management:");
var stateDocument = new DesignPatterns.Behavioral.State.Document("Initial content");
Console.WriteLine($"Document state: {stateDocument.CurrentState}");

// Edit document in draft
stateDocument.Edit();

// Send for review
stateDocument.Review();

// Try to edit during review (should go back to draft)
stateDocument.Edit();

// Review again and approve
stateDocument.Review();
stateDocument.Approve();

// Publish
stateDocument.Publish();

// Try to edit published document
stateDocument.Edit();

Console.WriteLine("\n3. Audio Player State:");
var audioPlayer = new AudioPlayer();
Console.WriteLine($"Player state: {audioPlayer.CurrentState}");

// Try to play without loading track
audioPlayer.Play();

// Load track and play
audioPlayer.LoadTrack("Song 1.mp3");
audioPlayer.Play();

// Pause and resume
audioPlayer.Pause();
audioPlayer.Play();

// Stop
audioPlayer.Stop();

Console.WriteLine($"Final player state: {audioPlayer.CurrentState}");

// Template Method pattern demo
Console.WriteLine("\n--- Template Method Pattern Demo ---");

Console.WriteLine("1. Data Processing Workflow:");
Console.WriteLine("Processing CSV data:");
var csvProcessor = new CsvDataProcessor();
csvProcessor.ProcessData();

Console.WriteLine("\nProcessing XML data:");
var xmlProcessor = new XmlDataProcessor();
xmlProcessor.ProcessData();

Console.WriteLine("\nProcessing JSON data:");
var jsonProcessor = new JsonDataProcessor();
jsonProcessor.ProcessData();

Console.WriteLine("\n2. Beverage Making Process:");
Console.WriteLine("Making Coffee:");
var coffeeTemplate = new DesignPatterns.Behavioral.TemplateMethod.Coffee();
coffeeTemplate.MakeCoffee();

Console.WriteLine("\nMaking Tea:");
var tea = new Tea();
tea.MakeCoffee();

Console.WriteLine("\nMaking Espresso:");
var espresso = new Espresso();
espresso.MakeCoffee();

Console.WriteLine("\n3. Game Playing Template:");
Console.WriteLine("Playing Chess:");
var chess = new Chess();
chess.Play();

Console.WriteLine("\nPlaying TicTacToe:");
var ticTacToe = new TicTacToe();
ticTacToe.Play();

Console.WriteLine("\n4. Report Generation:");
Console.WriteLine("Generating PDF Report:");
var pdfReport = new PdfReport();
pdfReport.GenerateReport();

Console.WriteLine("\nGenerating Excel Report:");
var excelReport = new ExcelReport();
excelReport.GenerateReport();

Console.WriteLine("\nGenerating HTML Report:");
var htmlReport = new HtmlReport();
htmlReport.GenerateReport();

// Visitor pattern demo
Console.WriteLine("\n--- Visitor Pattern Demo ---");

Console.WriteLine("1. Shape Processing with Different Visitors:");
var shapes = new List<IShape>
{
    new Circle(5),
    new Rectangle(4, 6),
    new Triangle(3, 8)
};

var areaCalculator = new AreaCalculator();
var perimeterCalculator = new PerimeterCalculator();
var renderer = new ShapeRenderer();

Console.WriteLine("Calculating areas:");
foreach (var shape in shapes)
{
    shape.Accept(areaCalculator);
}
Console.WriteLine($"Total area: {areaCalculator.TotalArea:F2}");

Console.WriteLine("\nCalculating perimeters:");
foreach (var shape in shapes)
{
    shape.Accept(perimeterCalculator);
}
Console.WriteLine($"Total perimeter: {perimeterCalculator.TotalPerimeter:F2}");

Console.WriteLine("\nRendering shapes:");
foreach (var shape in shapes)
{
    shape.Accept(renderer);
}

Console.WriteLine("\n2. File System Analysis:");
var fileRoot = new DesignPatterns.Behavioral.Visitor.Directory("Root");
var docs = new DesignPatterns.Behavioral.Visitor.Directory("Documents");
var pics = new DesignPatterns.Behavioral.Visitor.Directory("Pictures");

docs.Add(new DesignPatterns.Behavioral.Visitor.File("resume.pdf", 500000));
docs.Add(new DesignPatterns.Behavioral.Visitor.File("report.docx", 250000));
pics.Add(new DesignPatterns.Behavioral.Visitor.File("photo1.jpg", 1200000));
pics.Add(new DesignPatterns.Behavioral.Visitor.File("photo2.png", 800000));

fileRoot.Add(docs);
fileRoot.Add(pics);

var sizeCalculator = new SizeCalculator();
var fileCounter = new FileCounter();
var searchVisitor = new SearchVisitor("photo");

Console.WriteLine("File system structure and sizes:");
fileRoot.Accept(sizeCalculator);
Console.WriteLine($"Total size: {sizeCalculator.TotalSize} bytes");

Console.WriteLine("\nFile statistics:");
fileRoot.Accept(fileCounter);
fileCounter.PrintStatistics();

Console.WriteLine("\nSearching for 'photo':");
fileRoot.Accept(searchVisitor);

Console.WriteLine("\n3. Expression Tree Evaluation:");
// Expression: (5 + 3) * (10 - 6)
var expr = new Multiplication(
    new Addition(new Number(5), new Number(3)),
    new Subtraction(new Number(10), new Number(6))
);

var evaluator = new EvaluationVisitor();
var printer = new PrintVisitor();

Console.Write("Expression: ");
expr.Accept(printer);
Console.WriteLine();

Console.WriteLine("Evaluation:");
var result = expr.Accept(evaluator);
Console.WriteLine($"Result: {result}");

// Mediator pattern demo
Console.WriteLine("\n--- Mediator Pattern Demo ---");

Console.WriteLine("1. Chat Room Communication:");
var chatRoom = new ChatRoom();
var user1 = new ChatUser(chatRoom, "Alice");
var user2 = new ChatUser(chatRoom, "Bob");
var admin = new AdminUser(chatRoom, "Admin");

chatRoom.AddUser(user1);
chatRoom.AddUser(user2);
chatRoom.AddUser(admin);

user1.Send("Hello everyone!");
user2.Send("Hi Alice!");
admin.Send("Welcome to the chat room!");

Console.WriteLine("\n2. Air Traffic Control:");
var atc = new AirTrafficControl();
var flight1 = new PassengerAircraft(atc, "AA123");
var flight2 = new CargoAircraft(atc, "CARGO456");

atc.RegisterAircraft(flight1);
atc.RegisterAircraft(flight2);

flight1.RequestLanding();
flight2.RequestTakeoff();

Console.WriteLine("\n3. UI Dialog Interaction:");
var loginDialog = new LoginDialog();
loginDialog.SimulateUserInteraction();

// Memento pattern demo
Console.WriteLine("\n--- Memento Pattern Demo ---");

Console.WriteLine("1. Text Editor with Undo:");
var mementoEditor = new DesignPatterns.Behavioral.Memento.TextEditor();
var editorHistory = new EditorHistory(mementoEditor);

editorHistory.Backup();
mementoEditor.Write("Hello ");
editorHistory.Backup();
mementoEditor.Write("World!");
editorHistory.Backup();
mementoEditor.Delete(6);
editorHistory.ShowHistory();
editorHistory.Undo();
editorHistory.Undo();

Console.WriteLine("\n2. Game Save System:");
var character = new GameCharacter("Hero");
var saveManager = new GameSaveManager();

saveManager.SaveGame(character, "Slot1");
character.LevelUp();
character.GainExperience(100);
character.MoveTo("Dark Forest");
character.AddItem("Magic Sword");

saveManager.SaveGame(character, "Slot2");
character.TakeDamage(50);
character.MoveTo("Dragon's Lair");

Console.WriteLine("\nCurrent character state:");
Console.WriteLine($"Level {character.Level}, Health: {character.Health}, Location: {character.Location}");

saveManager.ShowSaveSlots();
saveManager.LoadGame(character, "Slot1");

Console.WriteLine("\n3. Calculator with History:");
var calculator = new Calculator();
var calcHistory = new CalculatorHistory(calculator);

calcHistory.SaveState();
calculator.Add(10);
calcHistory.SaveState();
calculator.Multiply(3);
calcHistory.SaveState();
calculator.Subtract(5);

Console.WriteLine($"Calculator history count: {calcHistory.GetHistoryCount()}");
calcHistory.Undo();
calcHistory.Undo();

// Iterator pattern demo
Console.WriteLine("\n--- Iterator Pattern Demo ---");

Console.WriteLine("1. Book Collection Iteration:");
var bookCollection = new BookCollection();
bookCollection.AddBook(new Book("1984", "George Orwell", "Dystopian", 1949));
bookCollection.AddBook(new Book("To Kill a Mockingbird", "Harper Lee", "Drama", 1960));
bookCollection.AddBook(new Book("Dune", "Frank Herbert", "Sci-Fi", 1965));
bookCollection.AddBook(new Book("Foundation", "Isaac Asimov", "Sci-Fi", 1951));

Console.WriteLine("\nIterating forward:");
var iterator = bookCollection.CreateIterator();
while (iterator.HasNext())
{
    Console.WriteLine($"  {iterator.Next()}");
}

Console.WriteLine("\nIterating backward:");
var reverseIterator = bookCollection.CreateReverseIterator();
while (reverseIterator.HasNext())
{
    Console.WriteLine($"  {reverseIterator.Next()}");
}

Console.WriteLine("\nIterating Sci-Fi books only:");
var sciFiIterator = bookCollection.CreateGenreIterator("Sci-Fi");
while (sciFiIterator.HasNext())
{
    Console.WriteLine($"  {sciFiIterator.Next()}");
}

Console.WriteLine("\n2. Tree Traversal:");
var tree = new Tree<string>();
var treeRoot = new TreeNode<string>("Root");
var child1 = new TreeNode<string>("Child1");
var child2 = new TreeNode<string>("Child2");
var grandchild1 = new TreeNode<string>("Grandchild1");
var grandchild2 = new TreeNode<string>("Grandchild2");

child1.AddChild(grandchild1);
child2.AddChild(grandchild2);
treeRoot.AddChild(child1);
treeRoot.AddChild(child2);
tree.Root = treeRoot;

Console.WriteLine("Depth-first traversal:");
var depthIterator = tree.CreateIterator();
while (depthIterator.HasNext())
{
    Console.WriteLine($"  {depthIterator.Next()}");
}

Console.WriteLine("Breadth-first traversal:");
var breadthIterator = tree.CreateBreadthFirstIterator();
while (breadthIterator.HasNext())
{
    Console.WriteLine($"  {breadthIterator.Next()}");
}

Console.WriteLine("\n3. Custom Sequences:");
Console.WriteLine("Even numbers 0-10:");
var evenNumbers = new NumberSequence(0, 10, 2);
foreach (var num in evenNumbers)
{
    Console.Write($"{num} ");
}
Console.WriteLine();

Console.WriteLine("First 10 Fibonacci numbers:");
var fibonacci = new FibonacciSequence(10);
foreach (var num in fibonacci)
{
    Console.Write($"{num} ");
}
Console.WriteLine();

// Interpreter pattern demo
Console.WriteLine("\n--- Interpreter Pattern Demo ---");

Console.WriteLine("1. Mathematical Expression Evaluation:");
var context = new Context();
context.SetVariable("x", 10);
context.SetVariable("y", 5);

// Parse and evaluate: x y + 3 *  (postfix notation)
var expression = ExpressionParser.ParseExpression("x y + 3 *");
var mathResult = expression.Interpret(context);
Console.WriteLine($"Final result: {mathResult}");

Console.WriteLine("\n2. Boolean Expression Evaluation:");
var boolContext = new BooleanContext();
boolContext.SetVariable("isLoggedIn", true);
boolContext.SetVariable("hasPermission", false);

var boolExpr = new OrExpression(
    new BooleanVariable("isLoggedIn"),
    new AndExpression(
        new BooleanVariable("hasPermission"),
        new BooleanConstant(true)
    )
);

var boolResult = boolExpr.Interpret(boolContext);
Console.WriteLine($"Access granted: {boolResult}");

Console.WriteLine("\n3. Simple Script Interpretation:");
var script = @"
// Simple script example
x = 10
y = 20
result = x + y
print result
message = ""Hello""
print message
";

var scriptContext = new ScriptContext();
var commands = ScriptInterpreter.ParseScript(script);

Console.WriteLine("Executing script:");
foreach (var command in commands)
{
    command.Execute(scriptContext);
}
