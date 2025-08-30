namespace DesignPatterns.Behavioral.TemplateMethod
{
    // Abstract class defining the template method
    public abstract class DataProcessor
    {
        // Template method - defines the algorithm skeleton
        public void ProcessData()
        {
            ReadData();
            ProcessDataCore();
            ValidateData();
            SaveData();
            NotifyComplete();
        }

        // Concrete methods (common steps)
        protected virtual void ReadData()
        {
            Console.WriteLine("Reading data from source...");
        }

        protected virtual void ValidateData()
        {
            Console.WriteLine("Validating processed data...");
        }

        protected virtual void NotifyComplete()
        {
            Console.WriteLine("Data processing completed successfully");
        }

        // Abstract methods (steps that vary)
        protected abstract void ProcessDataCore();
        protected abstract void SaveData();
    }

    // Concrete implementations
    public class CsvDataProcessor : DataProcessor
    {
        protected override void ReadData()
        {
            Console.WriteLine("Reading data from CSV file...");
        }

        protected override void ProcessDataCore()
        {
            Console.WriteLine("Parsing CSV format and cleaning data...");
            Console.WriteLine("Converting CSV records to objects...");
        }

        protected override void SaveData()
        {
            Console.WriteLine("Saving processed data to CSV file...");
        }
    }

    public class XmlDataProcessor : DataProcessor
    {
        protected override void ReadData()
        {
            Console.WriteLine("Reading data from XML file...");
        }

        protected override void ProcessDataCore()
        {
            Console.WriteLine("Parsing XML schema and validating structure...");
            Console.WriteLine("Extracting data from XML nodes...");
        }

        protected override void SaveData()
        {
            Console.WriteLine("Saving processed data to XML file...");
        }
    }

    public class JsonDataProcessor : DataProcessor
    {
        protected override void ProcessDataCore()
        {
            Console.WriteLine("Parsing JSON structure and deserializing objects...");
            Console.WriteLine("Applying business rules to JSON data...");
        }

        protected override void SaveData()
        {
            Console.WriteLine("Serializing data to JSON format...");
            Console.WriteLine("Saving to JSON file...");
        }

        protected override void NotifyComplete()
        {
            Console.WriteLine("JSON processing completed - sending notification email");
        }
    }

    // Real-world example: Coffee making process
    public abstract class CoffeeMaker
    {
        // Template method
        public void MakeCoffee()
        {
            BoilWater();
            BrewCoffeeGrinds();
            PourInCup();
            if (CustomerWantsCondiments())
            {
                AddCondiments();
            }
            Console.WriteLine("Coffee is ready!");
        }

        protected void BoilWater()
        {
            Console.WriteLine("Boiling water to 100°C");
        }

        protected void PourInCup()
        {
            Console.WriteLine("Pouring coffee into cup");
        }

        // Abstract methods - subclasses must implement
        protected abstract void BrewCoffeeGrinds();
        protected abstract void AddCondiments();

        // Hook method - subclasses can override if needed
        protected virtual bool CustomerWantsCondiments()
        {
            return true;
        }
    }

    public class Coffee : CoffeeMaker
    {
        protected override void BrewCoffeeGrinds()
        {
            Console.WriteLine("Dripping coffee through filter for 5 minutes");
        }

        protected override void AddCondiments()
        {
            Console.WriteLine("Adding sugar and milk");
        }
    }

    public class Tea : CoffeeMaker
    {
        protected override void BrewCoffeeGrinds()
        {
            Console.WriteLine("Steeping tea bag in hot water for 3 minutes");
        }

        protected override void AddCondiments()
        {
            Console.WriteLine("Adding lemon and honey");
        }

        protected override bool CustomerWantsCondiments()
        {
            // Tea drinkers might not want condiments
            return false;
        }
    }

    public class Espresso : CoffeeMaker
    {
        protected override void BrewCoffeeGrinds()
        {
            Console.WriteLine("Forcing hot water through finely ground coffee beans");
            Console.WriteLine("Extracting espresso in 25-30 seconds");
        }

        protected override void AddCondiments()
        {
            Console.WriteLine("Adding a thin layer of crema on top");
        }
    }

    // Game development example
    public abstract class Game
    {
        // Template method
        public void Play()
        {
            Initialize();
            StartPlay();
            while (!IsGameOver())
            {
                MakeMove();
                CheckGameState();
            }
            EndGame();
            DisplayResults();
        }

        // Common methods
        protected virtual void Initialize()
        {
            Console.WriteLine("Initializing game...");
        }

        protected virtual void StartPlay()
        {
            Console.WriteLine("Game started!");
        }

        protected virtual void EndGame()
        {
            Console.WriteLine("Game ended!");
        }

        // Abstract methods - game-specific
        protected abstract void MakeMove();
        protected abstract bool IsGameOver();
        protected abstract void CheckGameState();
        protected abstract void DisplayResults();
    }

    public class Chess : Game
    {
        private int _moveCount = 0;
        private readonly int _maxMoves = 3; // Simplified for demo

        protected override void Initialize()
        {
            base.Initialize();
            Console.WriteLine("Setting up chess board with pieces");
        }

        protected override void MakeMove()
        {
            _moveCount++;
            Console.WriteLine($"Chess move #{_moveCount}: Moving piece");
        }

        protected override bool IsGameOver()
        {
            return _moveCount >= _maxMoves;
        }

        protected override void CheckGameState()
        {
            Console.WriteLine("Checking for check, checkmate, or stalemate");
        }

        protected override void DisplayResults()
        {
            Console.WriteLine("Chess game results: Game completed after " + _moveCount + " moves");
        }
    }

    public class TicTacToe : Game
    {
        private int _moveCount = 0;
        private readonly int _maxMoves = 4; // Simplified for demo

        protected override void Initialize()
        {
            base.Initialize();
            Console.WriteLine("Creating 3x3 grid");
        }

        protected override void MakeMove()
        {
            _moveCount++;
            Console.WriteLine($"TicTacToe move #{_moveCount}: Placing X or O");
        }

        protected override bool IsGameOver()
        {
            return _moveCount >= _maxMoves;
        }

        protected override void CheckGameState()
        {
            Console.WriteLine("Checking for three in a row");
        }

        protected override void DisplayResults()
        {
            Console.WriteLine("TicTacToe results: Game completed in " + _moveCount + " moves");
        }
    }

    // Report generation example
    public abstract class ReportGenerator
    {
        // Template method
        public void GenerateReport()
        {
            GatherData();
            ProcessData();
            FormatReport();
            if (ShouldSaveToFile())
            {
                SaveToFile();
            }
            if (ShouldEmailReport())
            {
                EmailReport();
            }
            Console.WriteLine("Report generation completed");
        }

        protected virtual void GatherData()
        {
            Console.WriteLine("Gathering data from database...");
        }

        protected virtual void ProcessData()
        {
            Console.WriteLine("Processing and analyzing data...");
        }

        // Abstract methods
        protected abstract void FormatReport();
        protected abstract void SaveToFile();

        // Hook methods
        protected virtual bool ShouldSaveToFile()
        {
            return true;
        }

        protected virtual bool ShouldEmailReport()
        {
            return false;
        }

        protected virtual void EmailReport()
        {
            Console.WriteLine("Emailing report to stakeholders");
        }
    }

    public class PdfReport : ReportGenerator
    {
        protected override void FormatReport()
        {
            Console.WriteLine("Formatting report as PDF with tables and charts");
        }

        protected override void SaveToFile()
        {
            Console.WriteLine("Saving PDF report to file system");
        }

        protected override bool ShouldEmailReport()
        {
            return true; // PDFs are often emailed
        }
    }

    public class ExcelReport : ReportGenerator
    {
        protected override void FormatReport()
        {
            Console.WriteLine("Formatting report as Excel spreadsheet with formulas");
        }

        protected override void SaveToFile()
        {
            Console.WriteLine("Saving Excel report with multiple worksheets");
        }
    }

    public class HtmlReport : ReportGenerator
    {
        protected override void GatherData()
        {
            base.GatherData();
            Console.WriteLine("Gathering additional web analytics data...");
        }

        protected override void FormatReport()
        {
            Console.WriteLine("Formatting report as interactive HTML with CSS styling");
        }

        protected override void SaveToFile()
        {
            Console.WriteLine("Saving HTML report to web server directory");
        }

        protected override bool ShouldSaveToFile()
        {
            return true;
        }

        protected override bool ShouldEmailReport()
        {
            return true; // HTML reports can be emailed as links
        }

        protected override void EmailReport()
        {
            Console.WriteLine("Emailing HTML report link to stakeholders");
        }
    }
}
