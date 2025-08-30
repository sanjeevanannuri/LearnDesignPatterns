namespace DesignPatterns.Structural.Proxy
{
    // Subject interface
    public interface IImage
    {
        void Display();
    }

    // Real Subject - expensive to create
    public class RealImage : IImage
    {
        private readonly string _filename;

        public RealImage(string filename)
        {
            _filename = filename;
            LoadImageFromDisk();
        }

        private void LoadImageFromDisk()
        {
            Console.WriteLine($"Loading image from disk: {_filename}");
            // Simulate expensive operation
            Thread.Sleep(1000);
        }

        public void Display()
        {
            Console.WriteLine($"Displaying image: {_filename}");
        }
    }

    // Proxy - controls access to RealImage
    public class ProxyImage : IImage
    {
        private readonly string _filename;
        private RealImage? _realImage;

        public ProxyImage(string filename)
        {
            _filename = filename;
        }

        public void Display()
        {
            // Lazy initialization - only create RealImage when needed
            if (_realImage == null)
            {
                _realImage = new RealImage(_filename);
            }
            _realImage.Display();
        }
    }

    // Real-world example: Protected Resource Access
    public interface IBankAccount
    {
        void Withdraw(decimal amount);
        void Deposit(decimal amount);
        decimal GetBalance();
    }

    public class BankAccount : IBankAccount
    {
        private decimal _balance;
        private readonly string _accountNumber;

        public BankAccount(string accountNumber, decimal initialBalance)
        {
            _accountNumber = accountNumber;
            _balance = initialBalance;
        }

        public void Withdraw(decimal amount)
        {
            if (_balance >= amount)
            {
                _balance -= amount;
                Console.WriteLine($"Withdrew ${amount}. New balance: ${_balance}");
            }
            else
            {
                Console.WriteLine($"Insufficient funds. Current balance: ${_balance}");
            }
        }

        public void Deposit(decimal amount)
        {
            _balance += amount;
            Console.WriteLine($"Deposited ${amount}. New balance: ${_balance}");
        }

        public decimal GetBalance()
        {
            return _balance;
        }
    }

    // Protection Proxy
    public class BankAccountProxy : IBankAccount
    {
        private readonly BankAccount _bankAccount;
        private readonly string _userRole;

        public BankAccountProxy(BankAccount bankAccount, string userRole)
        {
            _bankAccount = bankAccount;
            _userRole = userRole;
        }

        public void Withdraw(decimal amount)
        {
            if (_userRole == "Owner" || _userRole == "Admin")
            {
                _bankAccount.Withdraw(amount);
            }
            else
            {
                Console.WriteLine("Access denied: You don't have permission to withdraw money.");
            }
        }

        public void Deposit(decimal amount)
        {
            if (_userRole == "Owner" || _userRole == "Admin" || _userRole == "Depositor")
            {
                _bankAccount.Deposit(amount);
            }
            else
            {
                Console.WriteLine("Access denied: You don't have permission to deposit money.");
            }
        }

        public decimal GetBalance()
        {
            if (_userRole == "Owner" || _userRole == "Admin")
            {
                return _bankAccount.GetBalance();
            }
            else
            {
                Console.WriteLine("Access denied: You don't have permission to view balance.");
                return 0;
            }
        }
    }

    // Caching Proxy example
    public interface IDataService
    {
        string GetData(string key);
    }

    public class DatabaseService : IDataService
    {
        public string GetData(string key)
        {
            Console.WriteLine($"Fetching data from database for key: {key}");
            // Simulate expensive database operation
            Thread.Sleep(500);
            return $"Data for {key} from database";
        }
    }

    public class CachingProxy : IDataService
    {
        private readonly DatabaseService _databaseService;
        private readonly Dictionary<string, string> _cache;

        public CachingProxy(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _cache = new Dictionary<string, string>();
        }

        public string GetData(string key)
        {
            if (_cache.ContainsKey(key))
            {
                Console.WriteLine($"Returning cached data for key: {key}");
                return _cache[key];
            }

            string data = _databaseService.GetData(key);
            _cache[key] = data;
            Console.WriteLine($"Data cached for key: {key}");
            return data;
        }
    }
}
