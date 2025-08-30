namespace DesignPatterns.Behavioral.Strategy
{
    // Strategy interface
    public interface IPaymentStrategy
    {
        void Pay(decimal amount);
        bool ValidatePaymentDetails();
    }

    // Concrete Strategies
    public class CreditCardPayment : IPaymentStrategy
    {
        private readonly string _cardNumber;
        private readonly string _name;
        private readonly string _cvv;
        private readonly string _dateOfExpiry;

        public CreditCardPayment(string cardNumber, string name, string cvv, string dateOfExpiry)
        {
            _cardNumber = cardNumber;
            _name = name;
            _cvv = cvv;
            _dateOfExpiry = dateOfExpiry;
        }

        public bool ValidatePaymentDetails()
        {
            // Simulate validation
            Console.WriteLine("Validating credit card details...");
            return !string.IsNullOrEmpty(_cardNumber) && !string.IsNullOrEmpty(_cvv);
        }

        public void Pay(decimal amount)
        {
            if (ValidatePaymentDetails())
            {
                Console.WriteLine($"${amount} paid using Credit Card ending in {_cardNumber.Substring(_cardNumber.Length - 4)}");
            }
            else
            {
                Console.WriteLine("Credit card payment failed: Invalid details");
            }
        }
    }

    public class PayPalPayment : IPaymentStrategy
    {
        private readonly string _email;
        private readonly string _password;

        public PayPalPayment(string email, string password)
        {
            _email = email;
            _password = password;
        }

        public bool ValidatePaymentDetails()
        {
            Console.WriteLine("Validating PayPal credentials...");
            return !string.IsNullOrEmpty(_email) && !string.IsNullOrEmpty(_password);
        }

        public void Pay(decimal amount)
        {
            if (ValidatePaymentDetails())
            {
                Console.WriteLine($"${amount} paid using PayPal account: {_email}");
            }
            else
            {
                Console.WriteLine("PayPal payment failed: Invalid credentials");
            }
        }
    }

    public class BitcoinPayment : IPaymentStrategy
    {
        private readonly string _walletAddress;

        public BitcoinPayment(string walletAddress)
        {
            _walletAddress = walletAddress;
        }

        public bool ValidatePaymentDetails()
        {
            Console.WriteLine("Validating Bitcoin wallet address...");
            return !string.IsNullOrEmpty(_walletAddress) && _walletAddress.Length > 10;
        }

        public void Pay(decimal amount)
        {
            if (ValidatePaymentDetails())
            {
                Console.WriteLine($"${amount} paid using Bitcoin to address: {_walletAddress.Substring(0, 8)}...");
            }
            else
            {
                Console.WriteLine("Bitcoin payment failed: Invalid wallet address");
            }
        }
    }

    // Context
    public class ShoppingCart
    {
        private readonly List<string> _items = new();
        private IPaymentStrategy? _paymentStrategy;

        public void AddItem(string item)
        {
            _items.Add(item);
            Console.WriteLine($"Added '{item}' to cart");
        }

        public void SetPaymentStrategy(IPaymentStrategy paymentStrategy)
        {
            _paymentStrategy = paymentStrategy;
        }

        public void Checkout(decimal amount)
        {
            if (_paymentStrategy == null)
            {
                Console.WriteLine("Please select a payment method");
                return;
            }

            Console.WriteLine($"\nProcessing checkout for {_items.Count} items...");
            _paymentStrategy.Pay(amount);
        }
    }

    // Real-world example: Sorting Strategies
    public interface ISortStrategy<T>
    {
        void Sort(List<T> list);
        string GetAlgorithmName();
    }

    public class BubbleSortStrategy : ISortStrategy<int>
    {
        public string GetAlgorithmName() => "Bubble Sort";

        public void Sort(List<int> list)
        {
            Console.WriteLine($"Sorting using {GetAlgorithmName()}...");
            int n = list.Count;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (list[j] > list[j + 1])
                    {
                        (list[j], list[j + 1]) = (list[j + 1], list[j]);
                    }
                }
            }
        }
    }

    public class QuickSortStrategy : ISortStrategy<int>
    {
        public string GetAlgorithmName() => "Quick Sort";

        public void Sort(List<int> list)
        {
            Console.WriteLine($"Sorting using {GetAlgorithmName()}...");
            QuickSort(list, 0, list.Count - 1);
        }

        private void QuickSort(List<int> list, int low, int high)
        {
            if (low < high)
            {
                int pi = Partition(list, low, high);
                QuickSort(list, low, pi - 1);
                QuickSort(list, pi + 1, high);
            }
        }

        private int Partition(List<int> list, int low, int high)
        {
            int pivot = list[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (list[j] < pivot)
                {
                    i++;
                    (list[i], list[j]) = (list[j], list[i]);
                }
            }
            (list[i + 1], list[high]) = (list[high], list[i + 1]);
            return i + 1;
        }
    }

    public class MergeSortStrategy : ISortStrategy<int>
    {
        public string GetAlgorithmName() => "Merge Sort";

        public void Sort(List<int> list)
        {
            Console.WriteLine($"Sorting using {GetAlgorithmName()}...");
            var temp = new int[list.Count];
            MergeSort(list, temp, 0, list.Count - 1);
        }

        private void MergeSort(List<int> list, int[] temp, int left, int right)
        {
            if (left < right)
            {
                int mid = (left + right) / 2;
                MergeSort(list, temp, left, mid);
                MergeSort(list, temp, mid + 1, right);
                Merge(list, temp, left, mid, right);
            }
        }

        private void Merge(List<int> list, int[] temp, int left, int mid, int right)
        {
            for (int i = left; i <= right; i++)
                temp[i] = list[i];

            int i1 = left, i2 = mid + 1, k = left;

            while (i1 <= mid && i2 <= right)
            {
                if (temp[i1] <= temp[i2])
                    list[k++] = temp[i1++];
                else
                    list[k++] = temp[i2++];
            }

            while (i1 <= mid)
                list[k++] = temp[i1++];
        }
    }

    public class SortContext
    {
        private ISortStrategy<int>? _sortStrategy;

        public void SetSortStrategy(ISortStrategy<int> sortStrategy)
        {
            _sortStrategy = sortStrategy;
        }

        public void SortNumbers(List<int> numbers)
        {
            if (_sortStrategy == null)
            {
                Console.WriteLine("No sorting strategy set");
                return;
            }

            Console.WriteLine($"Before sorting: [{string.Join(", ", numbers)}]");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _sortStrategy.Sort(numbers);
            stopwatch.Stop();
            Console.WriteLine($"After sorting: [{string.Join(", ", numbers)}]");
            Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds}ms\n");
        }
    }

    // Compression strategies example
    public interface ICompressionStrategy
    {
        void CompressFiles(List<string> files);
        string GetCompressionType();
    }

    public class ZipCompression : ICompressionStrategy
    {
        public string GetCompressionType() => "ZIP";

        public void CompressFiles(List<string> files)
        {
            Console.WriteLine($"Compressing {files.Count} files using {GetCompressionType()} compression...");
            foreach (var file in files)
            {
                Console.WriteLine($"  Compressing {file} with ZIP algorithm");
            }
            Console.WriteLine("ZIP compression completed");
        }
    }

    public class RarCompression : ICompressionStrategy
    {
        public string GetCompressionType() => "RAR";

        public void CompressFiles(List<string> files)
        {
            Console.WriteLine($"Compressing {files.Count} files using {GetCompressionType()} compression...");
            foreach (var file in files)
            {
                Console.WriteLine($"  Compressing {file} with RAR algorithm");
            }
            Console.WriteLine("RAR compression completed");
        }
    }

    public class CompressionContext
    {
        private ICompressionStrategy? _compressionStrategy;

        public void SetCompressionStrategy(ICompressionStrategy compressionStrategy)
        {
            _compressionStrategy = compressionStrategy;
        }

        public void CompressFiles(List<string> files)
        {
            if (_compressionStrategy == null)
            {
                Console.WriteLine("No compression strategy set");
                return;
            }

            _compressionStrategy.CompressFiles(files);
        }
    }
}
