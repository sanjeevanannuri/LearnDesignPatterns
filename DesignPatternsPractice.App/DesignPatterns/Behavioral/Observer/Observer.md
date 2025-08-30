# Observer Pattern

## Overview
The Observer pattern defines a one-to-many dependency between objects so that when one object changes state, all its dependents are notified and updated automatically. Think of it like a **newsletter subscription** - when the publisher (subject) releases new content, all subscribers (observers) automatically receive the update.

## Problem It Solves
Imagine you're building a stock trading application:
- Multiple displays need to show stock price updates (price chart, ticker, portfolio value)
- When a stock price changes, all displays should update automatically
- You don't want tight coupling between the stock data and the displays
- New types of displays should be easy to add without modifying existing code

Without Observer pattern, you might have tight coupling:
```csharp
// BAD: Tight coupling
public class Stock
{
    private decimal _price;
    private PriceChart _chart;      // Tight coupling to specific displays
    private TickerDisplay _ticker;  // Hard to add new display types
    private Portfolio _portfolio;   // Violates open/closed principle

    public void SetPrice(decimal price)
    {
        _price = price;
        // Manually update each display - rigid and hard to maintain
        _chart.UpdatePrice(price);
        _ticker.UpdatePrice(price);
        _portfolio.UpdatePrice(price);
    }
}
```

This approach creates tight coupling and makes it difficult to add new observers.

## Real-World Analogy
Think of **social media notifications**:
1. **Content Creator** (Subject): Posts new content
2. **Followers** (Observers): Want to be notified of new posts
3. **Platform** (Notification System): Manages the subscription relationship
4. **Automatic Updates**: When creator posts, all followers get notified
5. **Subscribe/Unsubscribe**: Followers can choose to follow or unfollow

Or consider **weather monitoring**:
- **Weather Station** (Subject): Measures temperature, humidity, pressure
- **Displays** (Observers): Current conditions, forecast, statistics
- **Automatic Updates**: When weather data changes, all displays update
- **Different Views**: Each display shows data in its own format

## Implementation Details

### Basic Structure
```csharp
// Observer interface
public interface IObserver
{
    void Update(ISubject subject);
}

// Subject interface
public interface ISubject
{
    void RegisterObserver(IObserver observer);
    void RemoveObserver(IObserver observer);
    void NotifyObservers();
}

// Concrete subject
public class ConcreteSubject : ISubject
{
    private List<IObserver> _observers = new();
    private string _state;

    public string State
    {
        get => _state;
        set
        {
            _state = value;
            NotifyObservers();
        }
    }

    public void RegisterObserver(IObserver observer)
    {
        _observers.Add(observer);
        Console.WriteLine($"Observer registered. Total observers: {_observers.Count}");
    }

    public void RemoveObserver(IObserver observer)
    {
        _observers.Remove(observer);
        Console.WriteLine($"Observer removed. Total observers: {_observers.Count}");
    }

    public void NotifyObservers()
    {
        Console.WriteLine($"Notifying {_observers.Count} observers...");
        foreach (var observer in _observers)
        {
            observer.Update(this);
        }
    }
}

// Concrete observer
public class ConcreteObserver : IObserver
{
    private string _name;
    private string _observedState;

    public ConcreteObserver(string name)
    {
        _name = name;
    }

    public void Update(ISubject subject)
    {
        if (subject is ConcreteSubject concreteSubject)
        {
            _observedState = concreteSubject.State;
            Console.WriteLine($"Observer {_name} received update: {_observedState}");
        }
    }

    public void Display()
    {
        Console.WriteLine($"Observer {_name}: Current state is {_observedState}");
    }
}
```

### Key Components
1. **Subject (Publisher)**: Maintains list of observers and notifies them of changes
2. **Observer (Subscriber)**: Defines updating interface for objects that should be notified
3. **ConcreteSubject**: Stores state and sends notification when state changes
4. **ConcreteObserver**: Implements the observer updating interface

## Example from Our Code
```csharp
// Stock data - the subject
public class Stock : ISubject
{
    private List<IStockObserver> _observers;
    private string _symbol;
    private decimal _price;
    private decimal _previousPrice;
    private int _volume;
    private DateTime _lastUpdate;

    public Stock(string symbol, decimal initialPrice)
    {
        _observers = new List<IStockObserver>();
        _symbol = symbol;
        _price = initialPrice;
        _previousPrice = initialPrice;
        _volume = 0;
        _lastUpdate = DateTime.Now;
    }

    public string Symbol => _symbol;
    public decimal Price => _price;
    public decimal PreviousPrice => _previousPrice;
    public int Volume => _volume;
    public DateTime LastUpdate => _lastUpdate;
    public decimal PriceChange => _price - _previousPrice;
    public decimal PriceChangePercent => _previousPrice != 0 ? (PriceChange / _previousPrice) * 100 : 0;

    public void RegisterObserver(IStockObserver observer)
    {
        _observers.Add(observer);
        Console.WriteLine($"📈 Observer registered for {_symbol}. Total observers: {_observers.Count}");
        
        // Send current state to new observer
        observer.OnPriceChanged(this);
    }

    public void RemoveObserver(IStockObserver observer)
    {
        _observers.Remove(observer);
        Console.WriteLine($"📉 Observer removed from {_symbol}. Total observers: {_observers.Count}");
    }

    public void NotifyObservers()
    {
        Console.WriteLine($"🔔 Notifying {_observers.Count} observers of {_symbol} change...");
        foreach (var observer in _observers)
        {
            observer.OnPriceChanged(this);
        }
    }

    public void UpdatePrice(decimal newPrice, int volume = 0)
    {
        _previousPrice = _price;
        _price = newPrice;
        _volume += volume;
        _lastUpdate = DateTime.Now;
        
        Console.WriteLine($"💰 {_symbol}: ${_price:F2} (Change: {PriceChange:+0.00;-0.00} / {PriceChangePercent:+0.00;-0.00}%)");
        
        NotifyObservers();
    }

    public void ShowCurrentState()
    {
        Console.WriteLine($"\n📊 Stock: {_symbol}");
        Console.WriteLine($"   Current Price: ${_price:F2}");
        Console.WriteLine($"   Previous Price: ${_previousPrice:F2}");
        Console.WriteLine($"   Change: ${PriceChange:+0.00;-0.00} ({PriceChangePercent:+0.00;-0.00}%)");
        Console.WriteLine($"   Volume: {_volume:N0}");
        Console.WriteLine($"   Last Update: {_lastUpdate:HH:mm:ss}");
        Console.WriteLine($"   Observers: {_observers.Count}");
    }
}

// Stock observer interface
public interface IStockObserver
{
    void OnPriceChanged(Stock stock);
}

// Price display - shows current price
public class PriceDisplay : IStockObserver
{
    private string _displayName;
    
    public PriceDisplay(string displayName)
    {
        _displayName = displayName;
    }

    public void OnPriceChanged(Stock stock)
    {
        var arrow = stock.PriceChange > 0 ? "⬆️" : stock.PriceChange < 0 ? "⬇️" : "➡️";
        var color = stock.PriceChange > 0 ? "🟢" : stock.PriceChange < 0 ? "🔴" : "🟡";
        
        Console.WriteLine($"   {color} [{_displayName}] {stock.Symbol}: ${stock.Price:F2} {arrow}");
    }
}

// Statistics display - shows detailed analytics
public class StatisticsDisplay : IStockObserver
{
    private string _displayName;
    private List<decimal> _priceHistory;
    private int _maxHistorySize;

    public StatisticsDisplay(string displayName, int maxHistorySize = 10)
    {
        _displayName = displayName;
        _priceHistory = new List<decimal>();
        _maxHistorySize = maxHistorySize;
    }

    public void OnPriceChanged(Stock stock)
    {
        // Update price history
        _priceHistory.Add(stock.Price);
        if (_priceHistory.Count > _maxHistorySize)
        {
            _priceHistory.RemoveAt(0);
        }

        // Calculate statistics
        var avg = _priceHistory.Average();
        var min = _priceHistory.Min();
        var max = _priceHistory.Max();
        var volatility = CalculateVolatility();

        Console.WriteLine($"   📊 [{_displayName}] {stock.Symbol} Stats:");
        Console.WriteLine($"      Avg: ${avg:F2} | Range: ${min:F2}-${max:F2} | Vol: {volatility:F1}%");
    }

    private decimal CalculateVolatility()
    {
        if (_priceHistory.Count < 2) return 0;

        var changes = new List<decimal>();
        for (int i = 1; i < _priceHistory.Count; i++)
        {
            var change = Math.Abs(_priceHistory[i] - _priceHistory[i - 1]) / _priceHistory[i - 1];
            changes.Add(change);
        }

        return changes.Average() * 100; // As percentage
    }

    public void ShowDetailedStats(Stock stock)
    {
        Console.WriteLine($"\n📈 Detailed Statistics for {stock.Symbol}:");
        Console.WriteLine($"   Price History ({_priceHistory.Count} points): [{string.Join(", ", _priceHistory.Select(p => $"${p:F2}"))}]");
        Console.WriteLine($"   Average: ${_priceHistory.Average():F2}");
        Console.WriteLine($"   Min: ${_priceHistory.Min():F2}");
        Console.WriteLine($"   Max: ${_priceHistory.Max():F2}");
        Console.WriteLine($"   Volatility: {CalculateVolatility():F2}%");
    }
}

// Alert system - triggers alerts on conditions
public class AlertSystem : IStockObserver
{
    private string _name;
    private decimal _upperThreshold;
    private decimal _lowerThreshold;
    private bool _alertsEnabled;

    public AlertSystem(string name, decimal lowerThreshold, decimal upperThreshold)
    {
        _name = name;
        _lowerThreshold = lowerThreshold;
        _upperThreshold = upperThreshold;
        _alertsEnabled = true;
    }

    public void OnPriceChanged(Stock stock)
    {
        if (!_alertsEnabled) return;

        if (stock.Price >= _upperThreshold)
        {
            Console.WriteLine($"   🚨 [{_name}] HIGH ALERT: {stock.Symbol} reached ${stock.Price:F2} (threshold: ${_upperThreshold:F2})");
        }
        else if (stock.Price <= _lowerThreshold)
        {
            Console.WriteLine($"   🚨 [{_name}] LOW ALERT: {stock.Symbol} dropped to ${stock.Price:F2} (threshold: ${_lowerThreshold:F2})");
        }

        // Check for rapid changes
        if (Math.Abs(stock.PriceChangePercent) > 5)
        {
            Console.WriteLine($"   ⚡ [{_name}] VOLATILITY ALERT: {stock.Symbol} changed {stock.PriceChangePercent:+0.00;-0.00}% rapidly!");
        }
    }

    public void SetThresholds(decimal lower, decimal upper)
    {
        _lowerThreshold = lower;
        _upperThreshold = upper;
        Console.WriteLine($"🎯 [{_name}] Alert thresholds updated: ${lower:F2} - ${upper:F2}");
    }

    public void EnableAlerts() => _alertsEnabled = true;
    public void DisableAlerts() => _alertsEnabled = false;
}

// Portfolio tracker - tracks total value
public class PortfolioTracker : IStockObserver
{
    private string _portfolioName;
    private Dictionary<string, int> _holdings; // symbol -> quantity
    private decimal _totalValue;
    private decimal _previousValue;

    public PortfolioTracker(string portfolioName)
    {
        _portfolioName = portfolioName;
        _holdings = new Dictionary<string, int>();
        _totalValue = 0;
        _previousValue = 0;
    }

    public void AddHolding(string symbol, int quantity)
    {
        _holdings[symbol] = _holdings.GetValueOrDefault(symbol, 0) + quantity;
        Console.WriteLine($"💼 [{_portfolioName}] Added {quantity} shares of {symbol}. Total: {_holdings[symbol]}");
    }

    public void OnPriceChanged(Stock stock)
    {
        if (_holdings.ContainsKey(stock.Symbol))
        {
            var quantity = _holdings[stock.Symbol];
            var oldHoldingValue = quantity * stock.PreviousPrice;
            var newHoldingValue = quantity * stock.Price;
            var changeInValue = newHoldingValue - oldHoldingValue;

            _previousValue = _totalValue;
            _totalValue = _totalValue - oldHoldingValue + newHoldingValue;

            Console.WriteLine($"   💼 [{_portfolioName}] {stock.Symbol} holding: {quantity} shares × ${stock.Price:F2} = ${newHoldingValue:F2}");
            Console.WriteLine($"   💼 Portfolio value change: ${changeInValue:+0.00;-0.00} (Total: ${_totalValue:F2})");
        }
    }

    public void ShowPortfolio()
    {
        Console.WriteLine($"\n💼 Portfolio: {_portfolioName}");
        Console.WriteLine($"   Total Value: ${_totalValue:F2}");
        Console.WriteLine($"   Previous Value: ${_previousValue:F2}");
        Console.WriteLine($"   Change: ${_totalValue - _previousValue:+0.00;-0.00}");
        Console.WriteLine($"   Holdings:");
        
        foreach (var holding in _holdings)
        {
            Console.WriteLine($"     {holding.Key}: {holding.Value} shares");
        }
    }
}

// News publisher - publishes stock-related news
public class NewsPublisher : IStockObserver
{
    private string _publisherName;
    private List<string> _newsItems;

    public NewsPublisher(string publisherName)
    {
        _publisherName = publisherName;
        _newsItems = new List<string>();
    }

    public void OnPriceChanged(Stock stock)
    {
        string newsItem = null;

        // Generate news based on price changes
        if (stock.PriceChangePercent > 10)
        {
            newsItem = $"BREAKING: {stock.Symbol} surges {stock.PriceChangePercent:F1}% to ${stock.Price:F2}";
        }
        else if (stock.PriceChangePercent < -10)
        {
            newsItem = $"ALERT: {stock.Symbol} plunges {Math.Abs(stock.PriceChangePercent):F1}% to ${stock.Price:F2}";
        }
        else if (stock.PriceChangePercent > 5)
        {
            newsItem = $"UPDATE: {stock.Symbol} rises {stock.PriceChangePercent:F1}% in trading";
        }
        else if (stock.PriceChangePercent < -5)
        {
            newsItem = $"UPDATE: {stock.Symbol} falls {Math.Abs(stock.PriceChangePercent):F1}% amid selling";
        }

        if (newsItem != null)
        {
            _newsItems.Add($"[{DateTime.Now:HH:mm:ss}] {newsItem}");
            Console.WriteLine($"   📰 [{_publisherName}] {newsItem}");
        }
    }

    public void ShowRecentNews(int count = 5)
    {
        Console.WriteLine($"\n📰 Recent News from {_publisherName}:");
        var recentNews = _newsItems.TakeLast(count);
        foreach (var news in recentNews)
        {
            Console.WriteLine($"   {news}");
        }
    }
}

// Stock market - manages multiple stocks
public class StockMarket
{
    private Dictionary<string, Stock> _stocks;
    private Random _random;

    public StockMarket()
    {
        _stocks = new Dictionary<string, Stock>();
        _random = new Random();
    }

    public Stock AddStock(string symbol, decimal initialPrice)
    {
        var stock = new Stock(symbol, initialPrice);
        _stocks[symbol] = stock;
        Console.WriteLine($"📈 Added stock {symbol} at ${initialPrice:F2}");
        return stock;
    }

    public Stock GetStock(string symbol)
    {
        return _stocks.GetValueOrDefault(symbol);
    }

    public void SimulateMarketDay()
    {
        Console.WriteLine("\n🏪 Simulating market day...");
        
        foreach (var stock in _stocks.Values)
        {
            // Simulate random price changes
            var changePercent = (_random.NextDouble() - 0.5) * 0.2; // ±10% max change
            var newPrice = stock.Price * (decimal)(1 + changePercent);
            var volume = _random.Next(1000, 10000);
            
            stock.UpdatePrice(Math.Max(0.01m, newPrice), volume);
            
            // Add small delay to see notifications in sequence
            Thread.Sleep(100);
        }
    }

    public void SimulateVolatileDay()
    {
        Console.WriteLine("\n⚡ Simulating volatile trading day...");
        
        foreach (var stock in _stocks.Values)
        {
            // Simulate larger price swings
            var changePercent = (_random.NextDouble() - 0.5) * 0.5; // ±25% max change
            var newPrice = stock.Price * (decimal)(1 + changePercent);
            var volume = _random.Next(5000, 50000);
            
            stock.UpdatePrice(Math.Max(0.01m, newPrice), volume);
            Thread.Sleep(200);
        }
    }

    public void ShowMarketSummary()
    {
        Console.WriteLine("\n📊 Market Summary:");
        foreach (var stock in _stocks.Values)
        {
            stock.ShowCurrentState();
        }
    }
}

// Usage demonstration
var market = new StockMarket();

Console.WriteLine("=== Stock Market Observer Pattern Demo ===");

// Create stocks
var appleStock = market.AddStock("AAPL", 150.00m);
var googleStock = market.AddStock("GOOGL", 2800.00m);
var teslaStock = market.AddStock("TSLA", 800.00m);

// Create various observers
var priceDisplay1 = new PriceDisplay("Main Display");
var priceDisplay2 = new PriceDisplay("Mobile App");
var statsDisplay = new StatisticsDisplay("Analytics Dashboard");
var alertSystem = new AlertSystem("Risk Management", 140.00m, 160.00m);
var portfolio = new PortfolioTracker("My Portfolio");
var newsPublisher = new NewsPublisher("Financial Times");

// Add holdings to portfolio
portfolio.AddHolding("AAPL", 100);
portfolio.AddHolding("GOOGL", 10);
portfolio.AddHolding("TSLA", 50);

// Register observers with Apple stock
appleStock.RegisterObserver(priceDisplay1);
appleStock.RegisterObserver(priceDisplay2);
appleStock.RegisterObserver(statsDisplay);
appleStock.RegisterObserver(alertSystem);
appleStock.RegisterObserver(portfolio);
appleStock.RegisterObserver(newsPublisher);

// Register some observers with other stocks
googleStock.RegisterObserver(priceDisplay1);
googleStock.RegisterObserver(portfolio);
teslaStock.RegisterObserver(priceDisplay1);
teslaStock.RegisterObserver(portfolio);

Console.WriteLine("\n--- Manual Price Updates ---");
appleStock.UpdatePrice(155.50m, 2500);
appleStock.UpdatePrice(148.75m, 3200);
appleStock.UpdatePrice(165.25m, 5000); // Should trigger high alert

Console.WriteLine("\n--- Market Simulation ---");
market.SimulateMarketDay();

Console.WriteLine("\n--- Current Status ---");
portfolio.ShowPortfolio();
statsDisplay.ShowDetailedStats(appleStock);
newsPublisher.ShowRecentNews();

Console.WriteLine("\n--- Removing Observer ---");
appleStock.RemoveObserver(alertSystem);

Console.WriteLine("\n--- Volatile Day Simulation ---");
market.SimulateVolatileDay();

market.ShowMarketSummary();
```

## Real-World Examples

### 1. **Weather Monitoring System**
```csharp
// Weather data - the subject
public class WeatherStation
{
    private List<IWeatherObserver> _observers;
    private float _temperature;
    private float _humidity;
    private float _pressure;
    private DateTime _lastUpdate;

    public WeatherStation()
    {
        _observers = new List<IWeatherObserver>();
        _lastUpdate = DateTime.Now;
    }

    public float Temperature => _temperature;
    public float Humidity => _humidity;
    public float Pressure => _pressure;
    public DateTime LastUpdate => _lastUpdate;

    public void RegisterObserver(IWeatherObserver observer)
    {
        _observers.Add(observer);
        Console.WriteLine($"🌡️ Weather observer registered. Total: {_observers.Count}");
        
        // Send current data to new observer
        observer.OnWeatherUpdate(this);
    }

    public void RemoveObserver(IWeatherObserver observer)
    {
        _observers.Remove(observer);
        Console.WriteLine($"🌡️ Weather observer removed. Total: {_observers.Count}");
    }

    public void NotifyObservers()
    {
        Console.WriteLine($"📡 Broadcasting weather update to {_observers.Count} displays...");
        foreach (var observer in _observers)
        {
            observer.OnWeatherUpdate(this);
        }
    }

    public void SetWeatherData(float temperature, float humidity, float pressure)
    {
        _temperature = temperature;
        _humidity = humidity;
        _pressure = pressure;
        _lastUpdate = DateTime.Now;
        
        Console.WriteLine($"🌤️ Weather updated: {temperature}°F, {humidity}% humidity, {pressure} inHg");
        NotifyObservers();
    }

    public void ShowCurrentWeather()
    {
        Console.WriteLine($"\n🌡️ Current Weather Conditions:");
        Console.WriteLine($"   Temperature: {_temperature}°F");
        Console.WriteLine($"   Humidity: {_humidity}%");
        Console.WriteLine($"   Pressure: {_pressure} inHg");
        Console.WriteLine($"   Last Update: {_lastUpdate:HH:mm:ss}");
        Console.WriteLine($"   Active Displays: {_observers.Count}");
    }
}

// Weather observer interface
public interface IWeatherObserver
{
    void OnWeatherUpdate(WeatherStation station);
}

// Current conditions display
public class CurrentConditionsDisplay : IWeatherObserver
{
    private string _displayName;
    private string _location;

    public CurrentConditionsDisplay(string displayName, string location)
    {
        _displayName = displayName;
        _location = location;
    }

    public void OnWeatherUpdate(WeatherStation station)
    {
        var tempColor = GetTemperatureColor(station.Temperature);
        var humidityStatus = GetHumidityStatus(station.Humidity);
        
        Console.WriteLine($"   📺 [{_displayName}] {_location} Weather:");
        Console.WriteLine($"       {tempColor} {station.Temperature}°F | 💧 {station.Humidity}% ({humidityStatus}) | 🌬️ {station.Pressure} inHg");
    }

    private string GetTemperatureColor(float temp)
    {
        return temp switch
        {
            < 32 => "🥶", // Freezing
            < 50 => "❄️",  // Cold
            < 70 => "🌤️", // Cool
            < 80 => "☀️", // Warm
            _ => "🔥"     // Hot
        };
    }

    private string GetHumidityStatus(float humidity)
    {
        return humidity switch
        {
            < 30 => "Dry",
            < 60 => "Comfortable",
            < 80 => "Humid",
            _ => "Very Humid"
        };
    }
}

// Forecast display
public class ForecastDisplay : IWeatherObserver
{
    private string _displayName;
    private List<WeatherReading> _readings;
    private int _maxReadings;

    public ForecastDisplay(string displayName, int maxReadings = 5)
    {
        _displayName = displayName;
        _readings = new List<WeatherReading>();
        _maxReadings = maxReadings;
    }

    public void OnWeatherUpdate(WeatherStation station)
    {
        // Add new reading
        _readings.Add(new WeatherReading
        {
            Temperature = station.Temperature,
            Humidity = station.Humidity,
            Pressure = station.Pressure,
            Timestamp = station.LastUpdate
        });

        // Maintain reading limit
        if (_readings.Count > _maxReadings)
        {
            _readings.RemoveAt(0);
        }

        // Generate forecast
        var forecast = GenerateForecast();
        Console.WriteLine($"   🔮 [{_displayName}] Forecast: {forecast}");
    }

    private string GenerateForecast()
    {
        if (_readings.Count < 2) return "Insufficient data for forecast";

        var latest = _readings.Last();
        var previous = _readings[_readings.Count - 2];

        var tempTrend = latest.Temperature > previous.Temperature ? "Rising" : "Falling";
        var pressureTrend = latest.Pressure > previous.Pressure ? "Rising" : "Falling";

        return pressureTrend switch
        {
            "Rising" when tempTrend == "Rising" => "Fair weather, warming trend ☀️",
            "Rising" when tempTrend == "Falling" => "Cool and clear 🌤️",
            "Falling" when tempTrend == "Rising" => "Possible storms ahead ⛈️",
            "Falling" when tempTrend == "Falling" => "Cloudy and cooling 🌫️",
            _ => "Conditions stable 🌥️"
        };
    }

    public void ShowForecastHistory()
    {
        Console.WriteLine($"\n🔮 Forecast History ({_displayName}):");
        foreach (var reading in _readings.TakeLast(3))
        {
            Console.WriteLine($"   [{reading.Timestamp:HH:mm}] {reading.Temperature}°F, {reading.Humidity}%, {reading.Pressure}\"");
        }
    }
}

// Weather reading data structure
public class WeatherReading
{
    public float Temperature { get; set; }
    public float Humidity { get; set; }
    public float Pressure { get; set; }
    public DateTime Timestamp { get; set; }
}

// Weather alerts system
public class WeatherAlertsSystem : IWeatherObserver
{
    private string _alertSystemName;
    private float _tempHighThreshold;
    private float _tempLowThreshold;
    private float _humidityThreshold;
    private bool _alertsEnabled;

    public WeatherAlertsSystem(string name, float tempLow, float tempHigh, float humidityHigh)
    {
        _alertSystemName = name;
        _tempLowThreshold = tempLow;
        _tempHighThreshold = tempHigh;
        _humidityThreshold = humidityHigh;
        _alertsEnabled = true;
    }

    public void OnWeatherUpdate(WeatherStation station)
    {
        if (!_alertsEnabled) return;

        CheckTemperatureAlerts(station.Temperature);
        CheckHumidityAlerts(station.Humidity);
        CheckPressureAlerts(station.Pressure);
    }

    private void CheckTemperatureAlerts(float temperature)
    {
        if (temperature >= _tempHighThreshold)
        {
            Console.WriteLine($"   🚨 [{_alertSystemName}] HEAT WARNING: {temperature}°F (threshold: {_tempHighThreshold}°F)");
        }
        else if (temperature <= _tempLowThreshold)
        {
            Console.WriteLine($"   🚨 [{_alertSystemName}] FREEZE WARNING: {temperature}°F (threshold: {_tempLowThreshold}°F)");
        }
    }

    private void CheckHumidityAlerts(float humidity)
    {
        if (humidity >= _humidityThreshold)
        {
            Console.WriteLine($"   🚨 [{_alertSystemName}] HIGH HUMIDITY ALERT: {humidity}% (threshold: {_humidityThreshold}%)");
        }
    }

    private void CheckPressureAlerts(float pressure)
    {
        if (pressure < 29.0f)
        {
            Console.WriteLine($"   🚨 [{_alertSystemName}] STORM WARNING: Low pressure {pressure}\"");
        }
        else if (pressure > 31.0f)
        {
            Console.WriteLine($"   🚨 [{_alertSystemName}] HIGH PRESSURE: {pressure}\" - Clear skies expected");
        }
    }

    public void SetThresholds(float tempLow, float tempHigh, float humidity)
    {
        _tempLowThreshold = tempLow;
        _tempHighThreshold = tempHigh;
        _humidityThreshold = humidity;
        Console.WriteLine($"🎯 [{_alertSystemName}] Thresholds updated: {tempLow}°F-{tempHigh}°F, {humidity}% humidity");
    }

    public void EnableAlerts() => _alertsEnabled = true;
    public void DisableAlerts() => _alertsEnabled = false;
}

// Statistics display
public class WeatherStatisticsDisplay : IWeatherObserver
{
    private string _displayName;
    private List<float> _temperatures;
    private List<float> _humidityReadings;
    private List<float> _pressureReadings;
    private int _maxReadings;

    public WeatherStatisticsDisplay(string displayName, int maxReadings = 24)
    {
        _displayName = displayName;
        _temperatures = new List<float>();
        _humidityReadings = new List<float>();
        _pressureReadings = new List<float>();
        _maxReadings = maxReadings;
    }

    public void OnWeatherUpdate(WeatherStation station)
    {
        AddReading(_temperatures, station.Temperature);
        AddReading(_humidityReadings, station.Humidity);
        AddReading(_pressureReadings, station.Pressure);

        // Display statistics
        ShowStatistics();
    }

    private void AddReading(List<float> readings, float value)
    {
        readings.Add(value);
        if (readings.Count > _maxReadings)
        {
            readings.RemoveAt(0);
        }
    }

    private void ShowStatistics()
    {
        Console.WriteLine($"   📊 [{_displayName}] 24hr Stats:");
        Console.WriteLine($"       Temp: Avg {_temperatures.Average():F1}°F | Range {_temperatures.Min():F1}°F-{_temperatures.Max():F1}°F");
        Console.WriteLine($"       Humidity: Avg {_humidityReadings.Average():F1}% | Range {_humidityReadings.Min():F1}%-{_humidityReadings.Max():F1}%");
        Console.WriteLine($"       Pressure: Avg {_pressureReadings.Average():F2}\" | Range {_pressureReadings.Min():F2}\"-{_pressureReadings.Max():F2}\"");
    }

    public void ShowDetailedStats()
    {
        Console.WriteLine($"\n📊 Detailed Statistics ({_displayName}):");
        Console.WriteLine($"   Readings collected: {_temperatures.Count}/{_maxReadings}");
        Console.WriteLine($"   Temperature: Avg={_temperatures.Average():F1}°F, Min={_temperatures.Min():F1}°F, Max={_temperatures.Max():F1}°F");
        Console.WriteLine($"   Humidity: Avg={_humidityReadings.Average():F1}%, Min={_humidityReadings.Min():F1}%, Max={_humidityReadings.Max():F1}%");
        Console.WriteLine($"   Pressure: Avg={_pressureReadings.Average():F2}\", Min={_pressureReadings.Min():F2}\", Max={_pressureReadings.Max():F2}\"");
    }
}

// Weather monitoring system
public class WeatherMonitoringSystem
{
    private WeatherStation _station;
    private Random _random;

    public WeatherMonitoringSystem()
    {
        _station = new WeatherStation();
        _random = new Random();
    }

    public void RegisterDisplay(IWeatherObserver observer)
    {
        _station.RegisterObserver(observer);
    }

    public void RemoveDisplay(IWeatherObserver observer)
    {
        _station.RemoveObserver(observer);
    }

    public void SimulateWeatherDay()
    {
        Console.WriteLine("\n🌤️ Simulating weather changes throughout the day...");
        
        var baseTemp = 65f;
        var baseHumidity = 50f;
        var basePressure = 30.0f;

        for (int hour = 0; hour < 8; hour++)
        {
            // Simulate realistic weather patterns
            var tempVariation = (float)(_random.NextDouble() - 0.5) * 10;
            var humidityVariation = (float)(_random.NextDouble() - 0.5) * 20;
            var pressureVariation = (float)(_random.NextDouble() - 0.5) * 2;

            var newTemp = baseTemp + tempVariation;
            var newHumidity = Math.Max(0, Math.Min(100, baseHumidity + humidityVariation));
            var newPressure = Math.Max(28, Math.Min(32, basePressure + pressureVariation));

            Console.WriteLine($"\n--- Hour {hour + 1} ---");
            _station.SetWeatherData(newTemp, newHumidity, newPressure);
            
            Thread.Sleep(500); // Pause to see updates
        }
    }

    public void SimulateStorm()
    {
        Console.WriteLine("\n⛈️ Simulating incoming storm...");
        
        // Storm conditions: dropping pressure, rising humidity, temperature drop
        _station.SetWeatherData(60f, 85f, 28.5f);
        Thread.Sleep(1000);
        
        _station.SetWeatherData(55f, 95f, 28.0f);
        Thread.Sleep(1000);
        
        _station.SetWeatherData(50f, 98f, 27.5f);
    }

    public void ShowCurrentWeather()
    {
        _station.ShowCurrentWeather();
    }
}

// Usage
var weatherSystem = new WeatherMonitoringSystem();

Console.WriteLine("\n=== Weather Monitoring System ===");

// Create displays
var mainDisplay = new CurrentConditionsDisplay("Main Dashboard", "Downtown");
var mobileDisplay = new CurrentConditionsDisplay("Mobile App", "Downtown");
var forecastDisplay = new ForecastDisplay("Weather Channel");
var alertSystem = new WeatherAlertsSystem("Emergency System", 32f, 85f, 80f);
var statsDisplay = new WeatherStatisticsDisplay("Analytics System");

// Register displays
weatherSystem.RegisterDisplay(mainDisplay);
weatherSystem.RegisterDisplay(mobileDisplay);
weatherSystem.RegisterDisplay(forecastDisplay);
weatherSystem.RegisterDisplay(alertSystem);
weatherSystem.RegisterDisplay(statsDisplay);

// Initial weather
weatherSystem.ShowCurrentWeather();

// Simulate weather changes
weatherSystem.SimulateWeatherDay();

// Show detailed statistics
statsDisplay.ShowDetailedStats();
forecastDisplay.ShowForecastHistory();

// Simulate storm
weatherSystem.SimulateStorm();
```

### 2. **Newsletter Subscription System**
```csharp
// Newsletter - the subject
public class Newsletter
{
    private List<ISubscriber> _subscribers;
    private string _name;
    private string _latestArticle;
    private DateTime _publishDate;
    private List<string> _articles;

    public Newsletter(string name)
    {
        _subscribers = new List<ISubscriber>();
        _name = name;
        _articles = new List<string>();
    }

    public string Name => _name;
    public string LatestArticle => _latestArticle;
    public DateTime PublishDate => _publishDate;
    public int SubscriberCount => _subscribers.Count;
    public int ArticleCount => _articles.Count;

    public void Subscribe(ISubscriber subscriber)
    {
        _subscribers.Add(subscriber);
        Console.WriteLine($"📧 New subscriber to {_name}. Total subscribers: {_subscribers.Count}");
        
        // Send welcome message with latest article if available
        if (!string.IsNullOrEmpty(_latestArticle))
        {
            subscriber.OnArticlePublished(this, _latestArticle, true);
        }
    }

    public void Unsubscribe(ISubscriber subscriber)
    {
        _subscribers.Remove(subscriber);
        Console.WriteLine($"📧 Subscriber removed from {_name}. Total subscribers: {_subscribers.Count}");
    }

    public void PublishArticle(string article)
    {
        _latestArticle = article;
        _publishDate = DateTime.Now;
        _articles.Add(article);
        
        Console.WriteLine($"📰 [{_name}] Published: \"{article}\"");
        NotifySubscribers();
    }

    private void NotifySubscribers()
    {
        Console.WriteLine($"📨 Notifying {_subscribers.Count} subscribers...");
        foreach (var subscriber in _subscribers)
        {
            subscriber.OnArticlePublished(this, _latestArticle);
        }
    }

    public void ShowNewsletterInfo()
    {
        Console.WriteLine($"\n📰 Newsletter: {_name}");
        Console.WriteLine($"   Latest Article: {_latestArticle ?? "None"}");
        Console.WriteLine($"   Published: {(_publishDate == default ? "Never" : _publishDate.ToString("MM/dd HH:mm"))}");
        Console.WriteLine($"   Total Articles: {_articleCount}");
        Console.WriteLine($"   Subscribers: {_subscribers.Count}");
    }

    public void ShowRecentArticles(int count = 5)
    {
        Console.WriteLine($"\n📚 Recent Articles from {_name}:");
        var recent = _articles.TakeLast(count);
        foreach (var article in recent)
        {
            Console.WriteLine($"   • {article}");
        }
    }
}

// Subscriber interface
public interface ISubscriber
{
    void OnArticlePublished(Newsletter newsletter, string article, bool isWelcome = false);
}

// Email subscriber
public class EmailSubscriber : ISubscriber
{
    private string _email;
    private string _name;
    private bool _receiveDigest;
    private List<string> _receivedArticles;

    public EmailSubscriber(string name, string email, bool receiveDigest = false)
    {
        _name = name;
        _email = email;
        _receiveDigest = receiveDigest;
        _receivedArticles = new List<string>();
    }

    public void OnArticlePublished(Newsletter newsletter, string article, bool isWelcome = false)
    {
        _receivedArticles.Add(article);
        
        if (isWelcome)
        {
            Console.WriteLine($"   📧 [{_email}] Welcome email: Latest from {newsletter.Name}: \"{article}\"");
        }
        else if (_receiveDigest)
        {
            Console.WriteLine($"   📧 [{_email}] Added to digest: \"{TruncateArticle(article)}\"");
        }
        else
        {
            Console.WriteLine($"   📧 [{_email}] Immediate email: \"{TruncateArticle(article)}\" from {newsletter.Name}");
        }
    }

    private string TruncateArticle(string article)
    {
        return article.Length > 50 ? article.Substring(0, 47) + "..." : article;
    }

    public void SendDigest()
    {
        if (_receiveDigest && _receivedArticles.Count > 0)
        {
            Console.WriteLine($"   📧 [{_email}] Weekly digest: {_receivedArticles.Count} new articles");
            _receivedArticles.Clear();
        }
    }

    public void ShowSubscriberInfo()
    {
        Console.WriteLine($"   📧 {_name} ({_email}) - Digest: {_receiveDigest}, Articles received: {_receivedArticles.Count}");
    }
}

// Mobile app subscriber
public class MobileAppSubscriber : ISubscriber
{
    private string _deviceId;
    private string _userName;
    private bool _pushNotifications;
    private int _notificationCount;

    public MobileAppSubscriber(string userName, string deviceId, bool pushNotifications = true)
    {
        _userName = userName;
        _deviceId = deviceId;
        _pushNotifications = pushNotifications;
        _notificationCount = 0;
    }

    public void OnArticlePublished(Newsletter newsletter, string article, bool isWelcome = false)
    {
        if (_pushNotifications)
        {
            _notificationCount++;
            var message = isWelcome ? "Welcome! Check out the latest:" : "New article:";
            Console.WriteLine($"   📱 [{_deviceId}] Push notification: {message} \"{TruncateForMobile(article)}\"");
        }
        else
        {
            Console.WriteLine($"   📱 [{_deviceId}] In-app notification: New article available");
        }
    }

    private string TruncateForMobile(string article)
    {
        return article.Length > 30 ? article.Substring(0, 27) + "..." : article;
    }

    public void TogglePushNotifications()
    {
        _pushNotifications = !_pushNotifications;
        Console.WriteLine($"📱 [{_userName}] Push notifications {(_pushNotifications ? "enabled" : "disabled")}");
    }

    public void ShowSubscriberInfo()
    {
        Console.WriteLine($"   📱 {_userName} ({_deviceId}) - Push: {_pushNotifications}, Notifications: {_notificationCount}");
    }
}

// RSS feed subscriber
public class RSSFeedSubscriber : ISubscriber
{
    private string _feedName;
    private string _feedUrl;
    private List<RSSItem> _feedItems;
    private int _maxItems;

    public RSSFeedSubscriber(string feedName, string feedUrl, int maxItems = 20)
    {
        _feedName = feedName;
        _feedUrl = feedUrl;
        _feedItems = new List<RSSItem>();
        _maxItems = maxItems;
    }

    public void OnArticlePublished(Newsletter newsletter, string article, bool isWelcome = false)
    {
        var rssItem = new RSSItem
        {
            Title = article,
            Source = newsletter.Name,
            PublishDate = DateTime.Now,
            IsWelcome = isWelcome
        };

        _feedItems.Add(rssItem);

        // Maintain feed size
        if (_feedItems.Count > _maxItems)
        {
            _feedItems.RemoveAt(0);
        }

        var prefix = isWelcome ? "Welcome item" : "New item";
        Console.WriteLine($"   📡 [{_feedName}] {prefix} added to RSS: \"{article}\" from {newsletter.Name}");
    }

    public void ShowFeed(int itemCount = 5)
    {
        Console.WriteLine($"\n📡 RSS Feed: {_feedName} ({_feedUrl})");
        var recentItems = _feedItems.TakeLast(itemCount);
        foreach (var item in recentItems)
        {
            var marker = item.IsWelcome ? "👋" : "📰";
            Console.WriteLine($"   {marker} [{item.PublishDate:MM/dd HH:mm}] {item.Title} (from {item.Source})");
        }
    }

    public void ShowSubscriberInfo()
    {
        Console.WriteLine($"   📡 {_feedName} ({_feedUrl}) - Items: {_feedItems.Count}/{_maxItems}");
    }
}

// RSS item structure
public class RSSItem
{
    public string Title { get; set; }
    public string Source { get; set; }
    public DateTime PublishDate { get; set; }
    public bool IsWelcome { get; set; }
}

// Social media subscriber
public class SocialMediaSubscriber : ISubscriber
{
    private string _platform;
    private string _handle;
    private bool _autoPost;
    private List<string> _scheduledPosts;

    public SocialMediaSubscriber(string platform, string handle, bool autoPost = true)
    {
        _platform = platform;
        _handle = handle;
        _autoPost = autoPost;
        _scheduledPosts = new List<string>();
    }

    public void OnArticlePublished(Newsletter newsletter, string article, bool isWelcome = false)
    {
        var hashtag = GetPlatformHashtag(newsletter.Name);
        var post = $"New article: \"{TruncateForSocial(article)}\" {hashtag}";

        if (isWelcome)
        {
            post = $"Welcome! Latest: \"{TruncateForSocial(article)}\" {hashtag}";
        }

        if (_autoPost)
        {
            Console.WriteLine($"   📱 [{_platform}@{_handle}] Auto-posted: {post}");
        }
        else
        {
            _scheduledPosts.Add(post);
            Console.WriteLine($"   📱 [{_platform}@{_handle}] Scheduled post: {TruncateForSocial(post)}");
        }
    }

    private string TruncateForSocial(string text)
    {
        return text.Length > 60 ? text.Substring(0, 57) + "..." : text;
    }

    private string GetPlatformHashtag(string newsletterName)
    {
        var cleanName = newsletterName.Replace(" ", "");
        return $"#{cleanName} #Newsletter";
    }

    public void PostScheduledContent()
    {
        if (_scheduledPosts.Count > 0)
        {
            Console.WriteLine($"📱 [{_platform}@{_handle}] Posting {_scheduledPosts.Count} scheduled posts:");
            foreach (var post in _scheduledPosts)
            {
                Console.WriteLine($"   Posted: {post}");
            }
            _scheduledPosts.Clear();
        }
    }

    public void ToggleAutoPost()
    {
        _autoPost = !_autoPost;
        Console.WriteLine($"📱 [{_platform}@{_handle}] Auto-posting {(_autoPost ? "enabled" : "disabled")}");
    }

    public void ShowSubscriberInfo()
    {
        Console.WriteLine($"   📱 {_platform}@{_handle} - Auto-post: {_autoPost}, Scheduled: {_scheduledPosts.Count}");
    }
}

// Publishing system
public class PublishingSystem
{
    private Dictionary<string, Newsletter> _newsletters;

    public PublishingSystem()
    {
        _newsletters = new Dictionary<string, Newsletter>();
    }

    public Newsletter CreateNewsletter(string name)
    {
        var newsletter = new Newsletter(name);
        _newsletters[name] = newsletter;
        Console.WriteLine($"📰 Created newsletter: {name}");
        return newsletter;
    }

    public Newsletter GetNewsletter(string name)
    {
        return _newsletters.GetValueOrDefault(name);
    }

    public void ShowAllNewsletters()
    {
        Console.WriteLine($"\n📰 Publishing System ({_newsletters.Count} newsletters):");
        foreach (var newsletter in _newsletters.Values)
        {
            Console.WriteLine($"   {newsletter.Name}: {newsletter.ArticleCount} articles, {newsletter.SubscriberCount} subscribers");
        }
    }

    public void BroadcastToAll(string message)
    {
        Console.WriteLine($"\n📢 Broadcasting to all newsletters: \"{message}\"");
        foreach (var newsletter in _newsletters.Values)
        {
            newsletter.PublishArticle($"BROADCAST: {message}");
        }
    }
}

// Usage
var publishingSystem = new PublishingSystem();

Console.WriteLine("\n=== Newsletter Subscription System ===");

// Create newsletters
var techNews = publishingSystem.CreateNewsletter("Tech Weekly");
var healthTips = publishingSystem.CreateNewsletter("Health & Wellness");
var marketUpdates = publishingSystem.CreateNewsletter("Market Insights");

// Create subscribers
var emailSub1 = new EmailSubscriber("John Doe", "john@email.com", false);
var emailSub2 = new EmailSubscriber("Jane Smith", "jane@email.com", true);
var mobileSub1 = new MobileAppSubscriber("TechFan", "device123");
var mobileSub2 = new MobileAppSubscriber("HealthGuru", "device456");
var rssFeed = new RSSFeedSubscriber("Aggregated News", "https://mysite.com/rss");
var twitterBot = new SocialMediaSubscriber("Twitter", "NewsBot", true);
var linkedinPage = new SocialMediaSubscriber("LinkedIn", "CompanyPage", false);

// Subscribe to newsletters
techNews.Subscribe(emailSub1);
techNews.Subscribe(mobileSub1);
techNews.Subscribe(rssFeed);
techNews.Subscribe(twitterBot);

healthTips.Subscribe(emailSub2);
healthTips.Subscribe(mobileSub2);
healthTips.Subscribe(rssFeed);

marketUpdates.Subscribe(emailSub1);
marketUpdates.Subscribe(linkedinPage);
marketUpdates.Subscribe(rssFeed);

Console.WriteLine("\n--- Publishing Articles ---");

// Publish articles
techNews.PublishArticle("AI Revolution: New GPT Model Breaks Records");
healthTips.PublishArticle("10 Simple Exercises You Can Do at Your Desk");
marketUpdates.PublishArticle("Federal Reserve Announces Interest Rate Decision");

Console.WriteLine("\n--- More Articles ---");
techNews.PublishArticle("Quantum Computing Breakthrough Achieved");
healthTips.PublishArticle("The Science of Sleep: Why 8 Hours Isn't Enough");

Console.WriteLine("\n--- Newsletter Status ---");
publishingSystem.ShowAllNewsletters();

Console.WriteLine("\n--- Subscriber Information ---");
Console.WriteLine("Email Subscribers:");
emailSub1.ShowSubscriberInfo();
emailSub2.ShowSubscriberInfo();

Console.WriteLine("Mobile Subscribers:");
mobileSub1.ShowSubscriberInfo();
mobileSub2.ShowSubscriberInfo();

Console.WriteLine("Other Subscribers:");
rssFeed.ShowSubscriberInfo();
twitterBot.ShowSubscriberInfo();
linkedinPage.ShowSubscriberInfo();

Console.WriteLine("\n--- RSS Feed ---");
rssFeed.ShowFeed();

Console.WriteLine("\n--- Social Media ---");
mobileSub1.TogglePushNotifications();
linkedinPage.PostScheduledContent();

Console.WriteLine("\n--- Broadcast Message ---");
publishingSystem.BroadcastToAll("System Maintenance Scheduled for Tonight");
```

## Benefits
- **Loose Coupling**: Subjects and observers are loosely coupled
- **Dynamic Relationships**: Observers can be added/removed at runtime
- **Broadcast Communication**: One subject can notify many observers
- **Open/Closed Principle**: Easy to add new observer types without modifying subjects

## Drawbacks
- **Memory Leaks**: Observers may not be properly removed
- **Performance**: Notifying many observers can be slow
- **Unexpected Updates**: Complex chains of updates can be hard to debug
- **Ordering Issues**: No guarantee of notification order

## When to Use
✅ **Use When:**
- Multiple objects need to be notified of state changes
- You want loose coupling between objects
- The number of observers varies or changes at runtime
- You need broadcast-style communication

❌ **Avoid When:**
- There's only one observer (use direct method calls)
- Performance is critical and you have many observers
- The update logic is complex and interdependent
- Simple callback mechanisms are sufficient

## Observer vs Other Patterns

| Pattern | Purpose | Key Difference |
|---------|---------|----------------|
| **Observer** | One-to-many notifications | Push-based, automatic updates |
| **Mediator** | Many-to-many communication | Centralized communication hub |
| **Publisher-Subscriber** | Decoupled messaging | Often includes message queuing |
| **Event Sourcing** | State change tracking | Stores events, not just notifications |

## Best Practices
1. **Weak References**: Use weak references to prevent memory leaks
2. **Exception Handling**: Handle exceptions in observer notifications
3. **Async Notifications**: Consider async notifications for better performance
4. **Observer Management**: Provide clear subscription/unsubscription mechanisms
5. **Update Batching**: Batch multiple updates when possible

## Common Mistakes
1. **Memory Leaks**: Not unsubscribing observers
2. **Circular Dependencies**: Creating circular update chains
3. **Heavy Processing**: Doing expensive work in update methods
4. **No Error Handling**: Not handling exceptions in observer code

## Modern C# Features
```csharp
// Using events (built-in observer pattern)
public class ModernSubject
{
    public event Action<string> StateChanged;
    
    private string _state;
    public string State
    {
        get => _state;
        set
        {
            _state = value;
            StateChanged?.Invoke(value);
        }
    }
}

// Using IObservable<T> and IObserver<T>
public class ReactiveSubject : IObservable<string>
{
    private readonly List<IObserver<string>> _observers = new();
    
    public IDisposable Subscribe(IObserver<string> observer)
    {
        _observers.Add(observer);
        return new Unsubscriber(_observers, observer);
    }
    
    public void Notify(string value)
    {
        foreach (var observer in _observers)
        {
            observer.OnNext(value);
        }
    }
    
    private class Unsubscriber : IDisposable
    {
        private readonly List<IObserver<string>> _observers;
        private readonly IObserver<string> _observer;
        
        public Unsubscriber(List<IObserver<string>> observers, IObserver<string> observer)
        {
            _observers = observers;
            _observer = observer;
        }
        
        public void Dispose() => _observers.Remove(_observer);
    }
}

// Using Reactive Extensions (Rx.NET)
public class RxObserver
{
    public void SetupObservable()
    {
        var observable = Observable.Create<string>(observer =>
        {
            // Setup subscription logic
            return Disposable.Empty;
        });
        
        observable
            .Where(x => x.Length > 3)
            .Select(x => x.ToUpper())
            .Subscribe(x => Console.WriteLine(x));
    }
}

// Using weak event pattern
public class WeakEventSubject
{
    private readonly ConditionalWeakTable<object, List<Action<string>>> _observers = new();
    
    public void Subscribe(object owner, Action<string> callback)
    {
        if (!_observers.TryGetValue(owner, out var callbacks))
        {
            callbacks = new List<Action<string>>();
            _observers.Add(owner, callbacks);
        }
        callbacks.Add(callback);
    }
    
    public void Notify(string message)
    {
        foreach (var pair in _observers)
        {
            foreach (var callback in pair.Value)
            {
                callback(message);
            }
        }
    }
}
```

## Testing Observers
```csharp
[Test]
public void Subject_NotifyObservers_AllObserversReceiveUpdate()
{
    // Arrange
    var subject = new ConcreteSubject();
    var observer1 = new Mock<IObserver>();
    var observer2 = new Mock<IObserver>();
    
    subject.RegisterObserver(observer1.Object);
    subject.RegisterObserver(observer2.Object);
    
    // Act
    subject.State = "New State";
    
    // Assert
    observer1.Verify(o => o.Update(subject), Times.Once);
    observer2.Verify(o => o.Update(subject), Times.Once);
}

[Test]
public void Subject_RemoveObserver_ObserverNoLongerNotified()
{
    // Arrange
    var subject = new ConcreteSubject();
    var observer = new Mock<IObserver>();
    
    subject.RegisterObserver(observer.Object);
    subject.RemoveObserver(observer.Object);
    
    // Act
    subject.State = "New State";
    
    // Assert
    observer.Verify(o => o.Update(It.IsAny<ISubject>()), Times.Never);
}
```

## Summary
The Observer pattern is like having a news subscription service - when something newsworthy happens (subject state changes), all subscribers (observers) automatically get notified. It's perfect for implementing reactive systems where multiple parts of your application need to respond to state changes without being tightly coupled.

The pattern shines in scenarios like GUI frameworks (model-view updates), real-time systems (stock prices, weather data), and event-driven architectures. Modern C# provides excellent built-in support through events, IObservable<T>, and Reactive Extensions, making it easier than ever to implement robust observer patterns.

The key insight is that Observer pattern enables loose coupling between the source of changes and the consumers of those changes, making your system more flexible and maintainable while supporting dynamic subscription relationships that can change at runtime.
