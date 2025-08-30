namespace DesignPatterns.Behavioral.Observer
{
    // Subject interface
    public interface ISubject
    {
        void Attach(IObserver observer);
        void Detach(IObserver observer);
        void Notify();
    }

    // Observer interface
    public interface IObserver
    {
        void Update(ISubject subject);
    }

    // Concrete Subject - Stock
    public class Stock : ISubject
    {
        private readonly List<IObserver> _observers = new();
        private string _symbol;
        private decimal _price;

        public Stock(string symbol, decimal price)
        {
            _symbol = symbol;
            _price = price;
        }

        public string Symbol => _symbol;
        public decimal Price => _price;

        public void SetPrice(decimal price)
        {
            _price = price;
            Console.WriteLine($"Stock {_symbol} price updated to ${_price}");
            Notify();
        }

        public void Attach(IObserver observer)
        {
            _observers.Add(observer);
            Console.WriteLine($"Observer attached to {_symbol}");
        }

        public void Detach(IObserver observer)
        {
            _observers.Remove(observer);
            Console.WriteLine($"Observer detached from {_symbol}");
        }

        public void Notify()
        {
            foreach (var observer in _observers)
            {
                observer.Update(this);
            }
        }
    }

    // Concrete Observers
    public class StockTrader : IObserver
    {
        private readonly string _name;
        private readonly decimal _buyThreshold;
        private readonly decimal _sellThreshold;

        public StockTrader(string name, decimal buyThreshold, decimal sellThreshold)
        {
            _name = name;
            _buyThreshold = buyThreshold;
            _sellThreshold = sellThreshold;
        }

        public void Update(ISubject subject)
        {
            if (subject is Stock stock)
            {
                if (stock.Price <= _buyThreshold)
                {
                    Console.WriteLine($"  {_name}: BUY signal for {stock.Symbol} at ${stock.Price}");
                }
                else if (stock.Price >= _sellThreshold)
                {
                    Console.WriteLine($"  {_name}: SELL signal for {stock.Symbol} at ${stock.Price}");
                }
                else
                {
                    Console.WriteLine($"  {_name}: HOLD {stock.Symbol} at ${stock.Price}");
                }
            }
        }
    }

    public class StockDisplay : IObserver
    {
        private readonly string _displayName;

        public StockDisplay(string displayName)
        {
            _displayName = displayName;
        }

        public void Update(ISubject subject)
        {
            if (subject is Stock stock)
            {
                Console.WriteLine($"  {_displayName}: {stock.Symbol} - ${stock.Price}");
            }
        }
    }

    // Real-world example: Newsletter Subscription
    public class Newsletter : ISubject
    {
        private readonly List<IObserver> _subscribers = new();
        private string _latestArticle = "";

        public string LatestArticle => _latestArticle;

        public void PublishArticle(string article)
        {
            _latestArticle = article;
            Console.WriteLine($"\nNewsletter published: '{article}'");
            Notify();
        }

        public void Attach(IObserver observer)
        {
            _subscribers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            _subscribers.Remove(observer);
        }

        public void Notify()
        {
            foreach (var subscriber in _subscribers)
            {
                subscriber.Update(this);
            }
        }
    }

    public class EmailSubscriber : IObserver
    {
        private readonly string _email;

        public EmailSubscriber(string email)
        {
            _email = email;
        }

        public void Update(ISubject subject)
        {
            if (subject is Newsletter newsletter)
            {
                Console.WriteLine($"  Email sent to {_email}: '{newsletter.LatestArticle}'");
            }
        }
    }

    public class SmsSubscriber : IObserver
    {
        private readonly string _phoneNumber;

        public SmsSubscriber(string phoneNumber)
        {
            _phoneNumber = phoneNumber;
        }

        public void Update(ISubject subject)
        {
            if (subject is Newsletter newsletter)
            {
                Console.WriteLine($"  SMS sent to {_phoneNumber}: '{newsletter.LatestArticle}'");
            }
        }
    }

    // Weather Station example
    public class WeatherStation : ISubject
    {
        private readonly List<IObserver> _observers = new();
        private float _temperature;
        private float _humidity;
        private float _pressure;

        public float Temperature => _temperature;
        public float Humidity => _humidity;
        public float Pressure => _pressure;

        public void SetMeasurements(float temperature, float humidity, float pressure)
        {
            _temperature = temperature;
            _humidity = humidity;
            _pressure = pressure;
            Console.WriteLine($"\nWeather updated: {_temperature}°F, {_humidity}% humidity, {_pressure} pressure");
            Notify();
        }

        public void Attach(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (var observer in _observers)
            {
                observer.Update(this);
            }
        }
    }

    public class CurrentConditionsDisplay : IObserver
    {
        public void Update(ISubject subject)
        {
            if (subject is WeatherStation station)
            {
                Console.WriteLine($"  Current conditions: {station.Temperature}°F and {station.Humidity}% humidity");
            }
        }
    }

    public class StatisticsDisplay : IObserver
    {
        private readonly List<float> _temperatures = new();

        public void Update(ISubject subject)
        {
            if (subject is WeatherStation station)
            {
                _temperatures.Add(station.Temperature);
                float avg = _temperatures.Average();
                float min = _temperatures.Min();
                float max = _temperatures.Max();
                Console.WriteLine($"  Avg/Max/Min temperature: {avg:F1}°F/{max}°F/{min}°F");
            }
        }
    }

    public class ForecastDisplay : IObserver
    {
        private float _lastPressure = 0;

        public void Update(ISubject subject)
        {
            if (subject is WeatherStation station)
            {
                string forecast = station.Pressure > _lastPressure ? "Improving weather on the way!" :
                                station.Pressure < _lastPressure ? "Watch out for cooler, rainy weather" :
                                "More of the same";
                Console.WriteLine($"  Forecast: {forecast}");
                _lastPressure = station.Pressure;
            }
        }
    }
}
