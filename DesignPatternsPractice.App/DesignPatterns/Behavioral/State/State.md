# State Pattern

## Overview
The State pattern allows an object to alter its behavior when its internal state changes, making it appear as if the object changed its class. Think of it like a **vending machine** - the same button press behaves differently depending on whether the machine is idle, has money inserted, or is dispensing a product.

## Problem It Solves
Imagine you're building a document editor with different editing modes:
- **View Mode**: Can only scroll and view content
- **Edit Mode**: Can modify text and formatting
- **Comment Mode**: Can add comments but not edit text
- **Review Mode**: Can accept/reject changes

Without State pattern, you might have complex conditional logic:
```csharp
// BAD: Complex conditional logic
public class DocumentEditor
{
    private enum EditorState { View, Edit, Comment, Review }
    private EditorState _currentState;

    public void HandleClick(int position)
    {
        if (_currentState == EditorState.View)
        {
            // Move cursor for viewing
        }
        else if (_currentState == EditorState.Edit)
        {
            // Place cursor for editing
        }
        else if (_currentState == EditorState.Comment)
        {
            // Add comment at position
        }
        else if (_currentState == EditorState.Review)
        {
            // Show change details
        }
    }

    public void HandleKeyPress(char key)
    {
        if (_currentState == EditorState.View)
        {
            // Maybe scroll or search
        }
        else if (_currentState == EditorState.Edit)
        {
            // Insert character
        }
        // More complex conditions...
    }
}
```

This approach leads to large, unwieldy conditional statements scattered throughout the code.

## Real-World Analogy
Think of a **traffic light system**:
1. **Red State**: Cars must stop, pedestrians can cross
2. **Yellow State**: Cars should prepare to stop, pedestrians wait
3. **Green State**: Cars can go, pedestrians must wait
4. **Flashing Red State**: Treat as stop sign

Each state handles the same inputs (timer expiry, sensor detection) completely differently. A car sensor during Green state might extend the timer, but during Red state it has no effect.

Or consider an **ATM machine**:
- **Idle State**: Shows welcome screen, accepts card
- **Card Inserted State**: Requests PIN
- **Authenticated State**: Shows menu options
- **Transaction State**: Processes withdrawal/deposit
- **Dispensing State**: Dispenses cash and receipt

Each state responds to button presses and card insertions in completely different ways.

## Implementation Details

### Basic Structure
```csharp
// Context - maintains reference to current state
public class Context
{
    private IState _currentState;

    public Context(IState initialState)
    {
        _currentState = initialState;
    }

    public void SetState(IState state)
    {
        Console.WriteLine($"Context: Changing state to {state.GetType().Name}");
        _currentState = state;
    }

    public void Request1()
    {
        _currentState.Handle1(this);
    }

    public void Request2()
    {
        _currentState.Handle2(this);
    }
}

// State interface
public interface IState
{
    void Handle1(Context context);
    void Handle2(Context context);
}

// Concrete states
public class ConcreteStateA : IState
{
    public void Handle1(Context context)
    {
        Console.WriteLine("ConcreteStateA handles request1");
        Console.WriteLine("ConcreteStateA wants to change state to ConcreteStateB");
        context.SetState(new ConcreteStateB());
    }

    public void Handle2(Context context)
    {
        Console.WriteLine("ConcreteStateA handles request2");
    }
}

public class ConcreteStateB : IState
{
    public void Handle1(Context context)
    {
        Console.WriteLine("ConcreteStateB handles request1");
    }

    public void Handle2(Context context)
    {
        Console.WriteLine("ConcreteStateB handles request2");
        Console.WriteLine("ConcreteStateB wants to change state to ConcreteStateA");
        context.SetState(new ConcreteStateA());
    }
}
```

### Key Components
1. **Context**: Maintains reference to current state and delegates requests
2. **State Interface**: Defines interface for all concrete states
3. **Concrete States**: Implement behavior specific to each state

## Example from Our Code
```csharp
// Media player context
public class MediaPlayer
{
    private IPlayerState _currentState;
    private string _currentTrack;
    private int _volume;
    private bool _isShuffleOn;
    private bool _isRepeatOn;
    private List<string> _playlist;
    private int _currentTrackIndex;

    public MediaPlayer()
    {
        _playlist = new List<string>();
        _volume = 50;
        _isShuffleOn = false;
        _isRepeatOn = false;
        _currentTrackIndex = 0;
        
        // Start in stopped state
        _currentState = new StoppedState();
        
        Console.WriteLine("🎵 Media Player initialized");
    }

    // State management
    public void SetState(IPlayerState state)
    {
        Console.WriteLine($"🔄 State changed: {_currentState?.GetType().Name} → {state.GetType().Name}");
        _currentState = state;
        _currentState.OnEnterState(this);
    }

    public IPlayerState GetCurrentState() => _currentState;

    // Properties
    public string CurrentTrack => _currentTrack;
    public int Volume => _volume;
    public bool IsShuffleOn => _isShuffleOn;
    public bool IsRepeatOn => _isRepeatOn;
    public List<string> Playlist => _playlist;
    public int CurrentTrackIndex => _currentTrackIndex;
    public bool HasTracks => _playlist.Count > 0;

    // Playlist management
    public void AddTrack(string track)
    {
        _playlist.Add(track);
        Console.WriteLine($"➕ Added track: {track} (Total: {_playlist.Count})");
    }

    public void SetCurrentTrack(int index)
    {
        if (index >= 0 && index < _playlist.Count)
        {
            _currentTrackIndex = index;
            _currentTrack = _playlist[index];
            Console.WriteLine($"🎧 Current track: {_currentTrack}");
        }
    }

    public void SetVolume(int volume)
    {
        _volume = Math.Max(0, Math.Min(100, volume));
        Console.WriteLine($"🔊 Volume: {_volume}%");
    }

    public void ToggleShuffle()
    {
        _isShuffleOn = !_isShuffleOn;
        Console.WriteLine($"🔀 Shuffle: {(_isShuffleOn ? "ON" : "OFF")}");
    }

    public void ToggleRepeat()
    {
        _isRepeatOn = !_isRepeatOn;
        Console.WriteLine($"🔁 Repeat: {(_isRepeatOn ? "ON" : "OFF")}");
    }

    // Player controls - delegated to current state
    public void Play() => _currentState.Play(this);
    public void Pause() => _currentState.Pause(this);
    public void Stop() => _currentState.Stop(this);
    public void Next() => _currentState.Next(this);
    public void Previous() => _currentState.Previous(this);

    // Status display
    public void ShowStatus()
    {
        Console.WriteLine($"\n🎵 Media Player Status:");
        Console.WriteLine($"   State: {_currentState.GetType().Name}");
        Console.WriteLine($"   Current Track: {_currentTrack ?? "None"}");
        Console.WriteLine($"   Track {_currentTrackIndex + 1} of {_playlist.Count}");
        Console.WriteLine($"   Volume: {_volume}%");
        Console.WriteLine($"   Shuffle: {(_isShuffleOn ? "ON" : "OFF")}");
        Console.WriteLine($"   Repeat: {(_isRepeatOn ? "ON" : "OFF")}");
        Console.WriteLine($"   Playlist: [{string.Join(", ", _playlist)}]");
    }
}

// Player state interface
public interface IPlayerState
{
    void OnEnterState(MediaPlayer player);
    void Play(MediaPlayer player);
    void Pause(MediaPlayer player);
    void Stop(MediaPlayer player);
    void Next(MediaPlayer player);
    void Previous(MediaPlayer player);
}

// Stopped state
public class StoppedState : IPlayerState
{
    public void OnEnterState(MediaPlayer player)
    {
        Console.WriteLine("⏹️ Player stopped");
    }

    public void Play(MediaPlayer player)
    {
        if (player.HasTracks)
        {
            if (player.CurrentTrack == null && player.Playlist.Count > 0)
            {
                player.SetCurrentTrack(0);
            }
            
            Console.WriteLine($"▶️ Starting playback: {player.CurrentTrack}");
            player.SetState(new PlayingState());
        }
        else
        {
            Console.WriteLine("❌ No tracks in playlist");
        }
    }

    public void Pause(MediaPlayer player)
    {
        Console.WriteLine("❌ Cannot pause - player is stopped");
    }

    public void Stop(MediaPlayer player)
    {
        Console.WriteLine("ℹ️ Player is already stopped");
    }

    public void Next(MediaPlayer player)
    {
        if (player.HasTracks)
        {
            var nextIndex = (player.CurrentTrackIndex + 1) % player.Playlist.Count;
            player.SetCurrentTrack(nextIndex);
            Console.WriteLine("⏭️ Selected next track (not playing)");
        }
        else
        {
            Console.WriteLine("❌ No tracks in playlist");
        }
    }

    public void Previous(MediaPlayer player)
    {
        if (player.HasTracks)
        {
            var prevIndex = player.CurrentTrackIndex == 0 ? 
                player.Playlist.Count - 1 : player.CurrentTrackIndex - 1;
            player.SetCurrentTrack(prevIndex);
            Console.WriteLine("⏮️ Selected previous track (not playing)");
        }
        else
        {
            Console.WriteLine("❌ No tracks in playlist");
        }
    }
}

// Playing state
public class PlayingState : IPlayerState
{
    public void OnEnterState(MediaPlayer player)
    {
        Console.WriteLine($"▶️ Now playing: {player.CurrentTrack}");
    }

    public void Play(MediaPlayer player)
    {
        Console.WriteLine("ℹ️ Already playing");
    }

    public void Pause(MediaPlayer player)
    {
        Console.WriteLine("⏸️ Playback paused");
        player.SetState(new PausedState());
    }

    public void Stop(MediaPlayer player)
    {
        Console.WriteLine("⏹️ Playback stopped");
        player.SetState(new StoppedState());
    }

    public void Next(MediaPlayer player)
    {
        if (player.HasTracks)
        {
            int nextIndex;
            
            if (player.IsShuffleOn)
            {
                var random = new Random();
                nextIndex = random.Next(player.Playlist.Count);
            }
            else
            {
                nextIndex = player.CurrentTrackIndex + 1;
                
                if (nextIndex >= player.Playlist.Count)
                {
                    if (player.IsRepeatOn)
                    {
                        nextIndex = 0;
                    }
                    else
                    {
                        Console.WriteLine("📻 Reached end of playlist");
                        player.SetState(new StoppedState());
                        return;
                    }
                }
            }
            
            player.SetCurrentTrack(nextIndex);
            Console.WriteLine($"⏭️ Skipped to: {player.CurrentTrack}");
        }
    }

    public void Previous(MediaPlayer player)
    {
        if (player.HasTracks)
        {
            var prevIndex = player.CurrentTrackIndex == 0 ? 
                player.Playlist.Count - 1 : player.CurrentTrackIndex - 1;
            
            player.SetCurrentTrack(prevIndex);
            Console.WriteLine($"⏮️ Back to: {player.CurrentTrack}");
        }
    }
}

// Paused state
public class PausedState : IPlayerState
{
    public void OnEnterState(MediaPlayer player)
    {
        Console.WriteLine($"⏸️ Paused: {player.CurrentTrack}");
    }

    public void Play(MediaPlayer player)
    {
        Console.WriteLine($"▶️ Resuming: {player.CurrentTrack}");
        player.SetState(new PlayingState());
    }

    public void Pause(MediaPlayer player)
    {
        Console.WriteLine("ℹ️ Already paused");
    }

    public void Stop(MediaPlayer player)
    {
        Console.WriteLine("⏹️ Stopped from pause");
        player.SetState(new StoppedState());
    }

    public void Next(MediaPlayer player)
    {
        if (player.HasTracks)
        {
            var nextIndex = (player.CurrentTrackIndex + 1) % player.Playlist.Count;
            player.SetCurrentTrack(nextIndex);
            Console.WriteLine($"⏭️ Next track ready: {player.CurrentTrack} (paused)");
        }
    }

    public void Previous(MediaPlayer player)
    {
        if (player.HasTracks)
        {
            var prevIndex = player.CurrentTrackIndex == 0 ? 
                player.Playlist.Count - 1 : player.CurrentTrackIndex - 1;
            player.SetCurrentTrack(prevIndex);
            Console.WriteLine($"⏮️ Previous track ready: {player.CurrentTrack} (paused)");
        }
    }
}

// Usage demonstration
var player = new MediaPlayer();

Console.WriteLine("=== Media Player State Pattern Demo ===");

player.ShowStatus();

// Try operations with no tracks
Console.WriteLine("\n--- Testing with empty playlist ---");
player.Play();
player.Next();

// Add some tracks
Console.WriteLine("\n--- Adding tracks ---");
player.AddTrack("Song 1 - Artist A");
player.AddTrack("Song 2 - Artist B");
player.AddTrack("Song 3 - Artist C");
player.AddTrack("Song 4 - Artist D");

// Test basic operations
Console.WriteLine("\n--- Basic playback operations ---");
player.Play();
player.ShowStatus();

player.Pause();
player.Play();
player.Next();
player.Previous();

// Test volume and settings
Console.WriteLine("\n--- Testing settings ---");
player.SetVolume(75);
player.ToggleShuffle();
player.ToggleRepeat();

// Test navigation
Console.WriteLine("\n--- Testing navigation ---");
player.Next();
player.Next();
player.Stop();

player.ShowStatus();

// Test from different states
Console.WriteLine("\n--- Testing from stopped state ---");
player.Previous();
player.Play();

Console.WriteLine("\n--- Testing pause state ---");
player.Pause();
player.Next();
player.Play();

Console.WriteLine("\n--- Testing repeat mode ---");
// Navigate to last track
while (player.CurrentTrackIndex < player.Playlist.Count - 1)
{
    player.Next();
}
player.Next(); // Should wrap around due to repeat

Console.WriteLine("\n=== Final Status ===");
player.ShowStatus();
```

## Real-World Examples

### 1. **ATM Machine**
```csharp
// ATM machine context
public class ATMMachine
{
    private IATMState _currentState;
    private decimal _balance;
    private string _cardNumber;
    private int _pinAttempts;
    private bool _cardInserted;

    public ATMMachine(decimal initialBalance = 1000)
    {
        _balance = initialBalance;
        _pinAttempts = 0;
        _cardInserted = false;
        _currentState = new IdleState();
        
        Console.WriteLine("🏧 ATM Machine initialized");
    }

    // State management
    public void SetState(IATMState state)
    {
        Console.WriteLine($"🔄 ATM State: {_currentState?.GetType().Name} → {state.GetType().Name}");
        _currentState = state;
        _currentState.OnEnterState(this);
    }

    public IATMState GetCurrentState() => _currentState;

    // Properties
    public decimal Balance => _balance;
    public string CardNumber => _cardNumber;
    public int PinAttempts => _pinAttempts;
    public bool CardInserted => _cardInserted;

    // ATM operations - delegated to current state
    public void InsertCard(string cardNumber) => _currentState.InsertCard(this, cardNumber);
    public void EjectCard() => _currentState.EjectCard(this);
    public void EnterPin(string pin) => _currentState.EnterPin(this, pin);
    public void SelectWithdrawal() => _currentState.SelectWithdrawal(this);
    public void SelectDeposit() => _currentState.SelectDeposit(this);
    public void EnterAmount(decimal amount) => _currentState.EnterAmount(this, amount);
    public void ConfirmTransaction() => _currentState.ConfirmTransaction(this);
    public void CancelTransaction() => _currentState.CancelTransaction(this);

    // Internal operations
    public void SetCardInfo(string cardNumber)
    {
        _cardNumber = cardNumber;
        _cardInserted = true;
        _pinAttempts = 0;
        Console.WriteLine($"💳 Card inserted: {cardNumber.Substring(0, 4)}****{cardNumber.Substring(cardNumber.Length - 4)}");
    }

    public void RemoveCard()
    {
        Console.WriteLine($"💳 Card ejected: {_cardNumber?.Substring(0, 4)}****{_cardNumber?.Substring(_cardNumber.Length - 4)}");
        _cardNumber = null;
        _cardInserted = false;
        _pinAttempts = 0;
    }

    public bool ValidatePin(string pin)
    {
        _pinAttempts++;
        // Simulate PIN validation (in real system, this would check against secure database)
        var isValid = pin == "1234"; // Simple validation for demo
        
        if (isValid)
        {
            Console.WriteLine("✅ PIN verified");
            _pinAttempts = 0;
        }
        else
        {
            Console.WriteLine($"❌ Invalid PIN (Attempt {_pinAttempts}/3)");
        }
        
        return isValid;
    }

    public bool IsAccountLocked() => _pinAttempts >= 3;

    public bool Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("❌ Invalid withdrawal amount");
            return false;
        }
        
        if (amount > _balance)
        {
            Console.WriteLine($"❌ Insufficient funds. Balance: ${_balance:F2}");
            return false;
        }
        
        _balance -= amount;
        Console.WriteLine($"💰 Withdrawal successful: ${amount:F2}. New balance: ${_balance:F2}");
        return true;
    }

    public bool Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("❌ Invalid deposit amount");
            return false;
        }
        
        _balance += amount;
        Console.WriteLine($"💰 Deposit successful: ${amount:F2}. New balance: ${_balance:F2}");
        return true;
    }

    public void ShowStatus()
    {
        Console.WriteLine($"\n🏧 ATM Status:");
        Console.WriteLine($"   State: {_currentState.GetType().Name}");
        Console.WriteLine($"   Card Inserted: {_cardInserted}");
        Console.WriteLine($"   Card Number: {(_cardNumber != null ? _cardNumber.Substring(0, 4) + "****" + _cardNumber.Substring(_cardNumber.Length - 4) : "None")}");
        Console.WriteLine($"   PIN Attempts: {_pinAttempts}/3");
        Console.WriteLine($"   Account Balance: ${_balance:F2}");
    }
}

// ATM state interface
public interface IATMState
{
    void OnEnterState(ATMMachine atm);
    void InsertCard(ATMMachine atm, string cardNumber);
    void EjectCard(ATMMachine atm);
    void EnterPin(ATMMachine atm, string pin);
    void SelectWithdrawal(ATMMachine atm);
    void SelectDeposit(ATMMachine atm);
    void EnterAmount(ATMMachine atm, decimal amount);
    void ConfirmTransaction(ATMMachine atm);
    void CancelTransaction(ATMMachine atm);
}

// Idle state - waiting for card
public class IdleState : IATMState
{
    public void OnEnterState(ATMMachine atm)
    {
        Console.WriteLine("🏧 ATM ready. Please insert your card.");
    }

    public void InsertCard(ATMMachine atm, string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            Console.WriteLine("❌ Invalid card");
            return;
        }
        
        atm.SetCardInfo(cardNumber);
        atm.SetState(new CardInsertedState());
    }

    public void EjectCard(ATMMachine atm) => Console.WriteLine("❌ No card to eject");
    public void EnterPin(ATMMachine atm, string pin) => Console.WriteLine("❌ Please insert card first");
    public void SelectWithdrawal(ATMMachine atm) => Console.WriteLine("❌ Please insert card first");
    public void SelectDeposit(ATMMachine atm) => Console.WriteLine("❌ Please insert card first");
    public void EnterAmount(ATMMachine atm, decimal amount) => Console.WriteLine("❌ Please insert card first");
    public void ConfirmTransaction(ATMMachine atm) => Console.WriteLine("❌ No transaction to confirm");
    public void CancelTransaction(ATMMachine atm) => Console.WriteLine("❌ No transaction to cancel");
}

// Card inserted state - waiting for PIN
public class CardInsertedState : IATMState
{
    public void OnEnterState(ATMMachine atm)
    {
        Console.WriteLine("🔐 Please enter your PIN:");
    }

    public void InsertCard(ATMMachine atm, string cardNumber)
    {
        Console.WriteLine("❌ Card already inserted");
    }

    public void EjectCard(ATMMachine atm)
    {
        atm.RemoveCard();
        atm.SetState(new IdleState());
    }

    public void EnterPin(ATMMachine atm, string pin)
    {
        if (atm.ValidatePin(pin))
        {
            atm.SetState(new AuthenticatedState());
        }
        else if (atm.IsAccountLocked())
        {
            Console.WriteLine("🔒 Account locked due to too many failed attempts");
            atm.RemoveCard();
            atm.SetState(new IdleState());
        }
        else
        {
            Console.WriteLine($"🔐 Please try again ({3 - atm.PinAttempts} attempts remaining):");
        }
    }

    public void SelectWithdrawal(ATMMachine atm) => Console.WriteLine("❌ Please enter PIN first");
    public void SelectDeposit(ATMMachine atm) => Console.WriteLine("❌ Please enter PIN first");
    public void EnterAmount(ATMMachine atm, decimal amount) => Console.WriteLine("❌ Please enter PIN first");
    public void ConfirmTransaction(ATMMachine atm) => Console.WriteLine("❌ Please enter PIN first");
    public void CancelTransaction(ATMMachine atm) => EjectCard(atm);
}

// Authenticated state - showing menu
public class AuthenticatedState : IATMState
{
    public void OnEnterState(ATMMachine atm)
    {
        Console.WriteLine("✅ Authentication successful!");
        Console.WriteLine("📋 Please select a transaction:");
        Console.WriteLine("   1. Withdrawal");
        Console.WriteLine("   2. Deposit");
        Console.WriteLine("   3. Check Balance");
    }

    public void InsertCard(ATMMachine atm, string cardNumber) => Console.WriteLine("❌ Card already inserted");
    
    public void EjectCard(ATMMachine atm)
    {
        Console.WriteLine("👋 Thank you for using our ATM");
        atm.RemoveCard();
        atm.SetState(new IdleState());
    }

    public void EnterPin(ATMMachine atm, string pin) => Console.WriteLine("❌ Already authenticated");

    public void SelectWithdrawal(ATMMachine atm)
    {
        Console.WriteLine("💰 Withdrawal selected. Enter amount:");
        atm.SetState(new WithdrawalState());
    }

    public void SelectDeposit(ATMMachine atm)
    {
        Console.WriteLine("💰 Deposit selected. Enter amount:");
        atm.SetState(new DepositState());
    }

    public void EnterAmount(ATMMachine atm, decimal amount) => Console.WriteLine("❌ Please select transaction type first");
    public void ConfirmTransaction(ATMMachine atm) => Console.WriteLine("❌ No transaction to confirm");
    public void CancelTransaction(ATMMachine atm) => EjectCard(atm);
}

// Withdrawal state - processing withdrawal
public class WithdrawalState : IATMState
{
    private decimal _amount;

    public void OnEnterState(ATMMachine atm)
    {
        Console.WriteLine("💰 Withdrawal transaction:");
        Console.WriteLine("   Quick amounts: $20, $40, $60, $80, $100");
        Console.WriteLine("   Or enter custom amount:");
    }

    public void InsertCard(ATMMachine atm, string cardNumber) => Console.WriteLine("❌ Transaction in progress");
    public void EjectCard(ATMMachine atm) => Console.WriteLine("❌ Complete or cancel transaction first");
    public void EnterPin(ATMMachine atm, string pin) => Console.WriteLine("❌ Transaction in progress");
    public void SelectWithdrawal(ATMMachine atm) => Console.WriteLine("❌ Already in withdrawal mode");
    public void SelectDeposit(ATMMachine atm) => Console.WriteLine("❌ Complete current transaction first");

    public void EnterAmount(ATMMachine atm, decimal amount)
    {
        _amount = amount;
        Console.WriteLine($"💰 Withdrawal amount: ${amount:F2}");
        Console.WriteLine("❓ Confirm transaction? (Y/N)");
    }

    public void ConfirmTransaction(ATMMachine atm)
    {
        if (_amount <= 0)
        {
            Console.WriteLine("❌ Please enter amount first");
            return;
        }

        if (atm.Withdraw(_amount))
        {
            Console.WriteLine("💵 Please take your cash");
            Console.WriteLine("🧾 Receipt printed");
            atm.SetState(new AuthenticatedState());
        }
        else
        {
            Console.WriteLine("❌ Transaction failed. Please try again.");
            _amount = 0;
        }
    }

    public void CancelTransaction(ATMMachine atm)
    {
        Console.WriteLine("❌ Withdrawal cancelled");
        atm.SetState(new AuthenticatedState());
    }
}

// Deposit state - processing deposit
public class DepositState : IATMState
{
    private decimal _amount;

    public void OnEnterState(ATMMachine atm)
    {
        Console.WriteLine("💰 Deposit transaction:");
        Console.WriteLine("   Enter deposit amount:");
    }

    public void InsertCard(ATMMachine atm, string cardNumber) => Console.WriteLine("❌ Transaction in progress");
    public void EjectCard(ATMMachine atm) => Console.WriteLine("❌ Complete or cancel transaction first");
    public void EnterPin(ATMMachine atm, string pin) => Console.WriteLine("❌ Transaction in progress");
    public void SelectWithdrawal(ATMMachine atm) => Console.WriteLine("❌ Complete current transaction first");
    public void SelectDeposit(ATMMachine atm) => Console.WriteLine("❌ Already in deposit mode");

    public void EnterAmount(ATMMachine atm, decimal amount)
    {
        _amount = amount;
        Console.WriteLine($"💰 Deposit amount: ${amount:F2}");
        Console.WriteLine("📄 Please insert cash and confirm");
    }

    public void ConfirmTransaction(ATMMachine atm)
    {
        if (_amount <= 0)
        {
            Console.WriteLine("❌ Please enter amount first");
            return;
        }

        if (atm.Deposit(_amount))
        {
            Console.WriteLine("✅ Deposit completed successfully");
            Console.WriteLine("🧾 Receipt printed");
            atm.SetState(new AuthenticatedState());
        }
        else
        {
            Console.WriteLine("❌ Deposit failed. Please try again.");
            _amount = 0;
        }
    }

    public void CancelTransaction(ATMMachine atm)
    {
        Console.WriteLine("❌ Deposit cancelled");
        atm.SetState(new AuthenticatedState());
    }
}

// Usage
var atm = new ATMMachine(500.00m);

Console.WriteLine("\n=== ATM Machine State Pattern Demo ===");

atm.ShowStatus();

// Test invalid operations in idle state
Console.WriteLine("\n--- Testing idle state ---");
atm.EnterPin("1234");
atm.SelectWithdrawal();

// Insert card
Console.WriteLine("\n--- Inserting card ---");
atm.InsertCard("1234567890123456");

// Test PIN entry
Console.WriteLine("\n--- Testing PIN entry ---");
atm.EnterPin("0000"); // Wrong PIN
atm.EnterPin("1111"); // Wrong PIN  
atm.EnterPin("1234"); // Correct PIN

// Test authenticated operations
Console.WriteLine("\n--- Testing authenticated state ---");
atm.SelectWithdrawal();
atm.EnterAmount(100);
atm.ConfirmTransaction();

// Test deposit
Console.WriteLine("\n--- Testing deposit ---");
atm.SelectDeposit();
atm.EnterAmount(50);
atm.ConfirmTransaction();

// Check balance and finish
atm.ShowStatus();
atm.EjectCard();

Console.WriteLine("\n--- Testing account lockout ---");
atm.InsertCard("9876543210987654");
atm.EnterPin("0000");
atm.EnterPin("1111");
atm.EnterPin("2222"); // Should lock account

atm.ShowStatus();
```

### 2. **Order Processing System**
```csharp
// Order context
public class Order
{
    private IOrderState _currentState;
    private string _orderId;
    private List<OrderItem> _items;
    private decimal _totalAmount;
    private DateTime _orderDate;
    private string _customerName;
    private string _shippingAddress;
    private string _paymentMethod;

    public Order(string orderId, string customerName)
    {
        _orderId = orderId;
        _customerName = customerName;
        _items = new List<OrderItem>();
        _orderDate = DateTime.Now;
        _currentState = new PendingState();
        
        Console.WriteLine($"📦 Order created: {orderId} for {customerName}");
    }

    // State management
    public void SetState(IOrderState state)
    {
        Console.WriteLine($"📋 Order {_orderId}: {_currentState?.GetType().Name} → {state.GetType().Name}");
        _currentState = state;
        _currentState.OnEnterState(this);
    }

    public IOrderState GetCurrentState() => _currentState;

    // Properties
    public string OrderId => _orderId;
    public string CustomerName => _customerName;
    public List<OrderItem> Items => _items;
    public decimal TotalAmount => _totalAmount;
    public DateTime OrderDate => _orderDate;
    public string ShippingAddress => _shippingAddress;
    public string PaymentMethod => _paymentMethod;

    // Order operations - delegated to current state
    public void AddItem(OrderItem item) => _currentState.AddItem(this, item);
    public void RemoveItem(string itemId) => _currentState.RemoveItem(this, itemId);
    public void SetShippingAddress(string address) => _currentState.SetShippingAddress(this, address);
    public void SetPaymentMethod(string paymentMethod) => _currentState.SetPaymentMethod(this, paymentMethod);
    public void SubmitOrder() => _currentState.SubmitOrder(this);
    public void ProcessPayment() => _currentState.ProcessPayment(this);
    public void Ship() => _currentState.Ship(this);
    public void Deliver() => _currentState.Deliver(this);
    public void Cancel() => _currentState.Cancel(this);
    public void Return() => _currentState.Return(this);

    // Internal operations
    public void AddItemInternal(OrderItem item)
    {
        _items.Add(item);
        RecalculateTotal();
        Console.WriteLine($"➕ Added: {item.Name} x{item.Quantity} @ ${item.Price:F2}");
    }

    public void RemoveItemInternal(string itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            _items.Remove(item);
            RecalculateTotal();
            Console.WriteLine($"➖ Removed: {item.Name}");
        }
    }

    public void SetShippingAddressInternal(string address)
    {
        _shippingAddress = address;
        Console.WriteLine($"📍 Shipping address: {address}");
    }

    public void SetPaymentMethodInternal(string paymentMethod)
    {
        _paymentMethod = paymentMethod;
        Console.WriteLine($"💳 Payment method: {paymentMethod}");
    }

    private void RecalculateTotal()
    {
        _totalAmount = _items.Sum(item => item.Price * item.Quantity);
        Console.WriteLine($"💰 Order total: ${_totalAmount:F2}");
    }

    public bool IsValidForSubmission()
    {
        return _items.Count > 0 && 
               !string.IsNullOrWhiteSpace(_shippingAddress) && 
               !string.IsNullOrWhiteSpace(_paymentMethod);
    }

    public void ShowOrderDetails()
    {
        Console.WriteLine($"\n📦 Order Details: {_orderId}");
        Console.WriteLine($"   Customer: {_customerName}");
        Console.WriteLine($"   State: {_currentState.GetType().Name}");
        Console.WriteLine($"   Order Date: {_orderDate:MM/dd/yyyy HH:mm}");
        Console.WriteLine($"   Total Amount: ${_totalAmount:F2}");
        Console.WriteLine($"   Shipping Address: {_shippingAddress ?? "Not set"}");
        Console.WriteLine($"   Payment Method: {_paymentMethod ?? "Not set"}");
        Console.WriteLine($"   Items ({_items.Count}):");
        
        foreach (var item in _items)
        {
            Console.WriteLine($"     - {item.Name} x{item.Quantity} @ ${item.Price:F2} = ${item.Price * item.Quantity:F2}");
        }
    }
}

// Order item
public class OrderItem
{
    public string Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public OrderItem(string id, string name, decimal price, int quantity = 1)
    {
        Id = id;
        Name = name;
        Price = price;
        Quantity = quantity;
    }
}

// Order state interface
public interface IOrderState
{
    void OnEnterState(Order order);
    void AddItem(Order order, OrderItem item);
    void RemoveItem(Order order, string itemId);
    void SetShippingAddress(Order order, string address);
    void SetPaymentMethod(Order order, string paymentMethod);
    void SubmitOrder(Order order);
    void ProcessPayment(Order order);
    void Ship(Order order);
    void Deliver(Order order);
    void Cancel(Order order);
    void Return(Order order);
}

// Pending state - order being created
public class PendingState : IOrderState
{
    public void OnEnterState(Order order)
    {
        Console.WriteLine("📝 Order is being prepared...");
    }

    public void AddItem(Order order, OrderItem item) => order.AddItemInternal(item);
    public void RemoveItem(Order order, string itemId) => order.RemoveItemInternal(itemId);
    public void SetShippingAddress(Order order, string address) => order.SetShippingAddressInternal(address);
    public void SetPaymentMethod(Order order, string paymentMethod) => order.SetPaymentMethodInternal(paymentMethod);

    public void SubmitOrder(Order order)
    {
        if (order.IsValidForSubmission())
        {
            Console.WriteLine("✅ Order submitted for processing");
            order.SetState(new SubmittedState());
        }
        else
        {
            Console.WriteLine("❌ Cannot submit order: Missing required information");
            if (order.Items.Count == 0) Console.WriteLine("   - Add at least one item");
            if (string.IsNullOrWhiteSpace(order.ShippingAddress)) Console.WriteLine("   - Set shipping address");
            if (string.IsNullOrWhiteSpace(order.PaymentMethod)) Console.WriteLine("   - Set payment method");
        }
    }

    public void ProcessPayment(Order order) => Console.WriteLine("❌ Submit order first");
    public void Ship(Order order) => Console.WriteLine("❌ Submit order first");
    public void Deliver(Order order) => Console.WriteLine("❌ Submit order first");
    
    public void Cancel(Order order)
    {
        Console.WriteLine("❌ Order cancelled during preparation");
        order.SetState(new CancelledState());
    }
    
    public void Return(Order order) => Console.WriteLine("❌ Cannot return unsubmitted order");
}

// Submitted state - waiting for payment processing
public class SubmittedState : IOrderState
{
    public void OnEnterState(Order order)
    {
        Console.WriteLine("📋 Order submitted, awaiting payment processing...");
    }

    public void AddItem(Order order, OrderItem item) => Console.WriteLine("❌ Cannot modify submitted order");
    public void RemoveItem(Order order, string itemId) => Console.WriteLine("❌ Cannot modify submitted order");
    public void SetShippingAddress(Order order, string address) => Console.WriteLine("❌ Cannot modify submitted order");
    public void SetPaymentMethod(Order order, string paymentMethod) => Console.WriteLine("❌ Cannot modify submitted order");
    public void SubmitOrder(Order order) => Console.WriteLine("ℹ️ Order already submitted");

    public void ProcessPayment(Order order)
    {
        Console.WriteLine("💳 Processing payment...");
        // Simulate payment processing
        Thread.Sleep(1000);
        Console.WriteLine("✅ Payment processed successfully");
        order.SetState(new ProcessingState());
    }

    public void Ship(Order order) => Console.WriteLine("❌ Process payment first");
    public void Deliver(Order order) => Console.WriteLine("❌ Process payment first");
    
    public void Cancel(Order order)
    {
        Console.WriteLine("❌ Order cancelled before payment");
        order.SetState(new CancelledState());
    }
    
    public void Return(Order order) => Console.WriteLine("❌ Cannot return unpaid order");
}

// Processing state - payment confirmed, preparing for shipment
public class ProcessingState : IOrderState
{
    public void OnEnterState(Order order)
    {
        Console.WriteLine("⚙️ Order is being processed and prepared for shipment...");
    }

    public void AddItem(Order order, OrderItem item) => Console.WriteLine("❌ Cannot modify order in processing");
    public void RemoveItem(Order order, string itemId) => Console.WriteLine("❌ Cannot modify order in processing");
    public void SetShippingAddress(Order order, string address) => Console.WriteLine("❌ Cannot modify shipping address during processing");
    public void SetPaymentMethod(Order order, string paymentMethod) => Console.WriteLine("❌ Cannot modify payment method after processing");
    public void SubmitOrder(Order order) => Console.WriteLine("ℹ️ Order already being processed");
    public void ProcessPayment(Order order) => Console.WriteLine("ℹ️ Payment already processed");

    public void Ship(Order order)
    {
        Console.WriteLine("📦 Order shipped!");
        Console.WriteLine($"📍 Shipped to: {order.ShippingAddress}");
        order.SetState(new ShippedState());
    }

    public void Deliver(Order order) => Console.WriteLine("❌ Ship order first");
    
    public void Cancel(Order order)
    {
        Console.WriteLine("⚠️ Order cancelled during processing (refund will be issued)");
        order.SetState(new CancelledState());
    }
    
    public void Return(Order order) => Console.WriteLine("❌ Cannot return unshipped order");
}

// Shipped state - order in transit
public class ShippedState : IOrderState
{
    public void OnEnterState(Order order)
    {
        Console.WriteLine("🚚 Order is in transit...");
    }

    public void AddItem(Order order, OrderItem item) => Console.WriteLine("❌ Cannot modify shipped order");
    public void RemoveItem(Order order, string itemId) => Console.WriteLine("❌ Cannot modify shipped order");
    public void SetShippingAddress(Order order, string address) => Console.WriteLine("❌ Cannot modify address for shipped order");
    public void SetPaymentMethod(Order order, string paymentMethod) => Console.WriteLine("❌ Cannot modify payment for shipped order");
    public void SubmitOrder(Order order) => Console.WriteLine("ℹ️ Order already shipped");
    public void ProcessPayment(Order order) => Console.WriteLine("ℹ️ Payment already processed");
    public void Ship(Order order) => Console.WriteLine("ℹ️ Order already shipped");

    public void Deliver(Order order)
    {
        Console.WriteLine("✅ Order delivered successfully!");
        order.SetState(new DeliveredState());
    }

    public void Cancel(Order order) => Console.WriteLine("❌ Cannot cancel shipped order (contact customer service)");
    public void Return(Order order) => Console.WriteLine("❌ Cannot return order until delivered");
}

// Delivered state - order completed
public class DeliveredState : IOrderState
{
    public void OnEnterState(Order order)
    {
        Console.WriteLine("🎉 Order delivered! Thank you for your business!");
    }

    public void AddItem(Order order, OrderItem item) => Console.WriteLine("❌ Cannot modify delivered order");
    public void RemoveItem(Order order, string itemId) => Console.WriteLine("❌ Cannot modify delivered order");
    public void SetShippingAddress(Order order, string address) => Console.WriteLine("❌ Cannot modify delivered order");
    public void SetPaymentMethod(Order order, string paymentMethod) => Console.WriteLine("❌ Cannot modify delivered order");
    public void SubmitOrder(Order order) => Console.WriteLine("ℹ️ Order already completed");
    public void ProcessPayment(Order order) => Console.WriteLine("ℹ️ Order already completed");
    public void Ship(Order order) => Console.WriteLine("ℹ️ Order already delivered");
    public void Deliver(Order order) => Console.WriteLine("ℹ️ Order already delivered");
    public void Cancel(Order order) => Console.WriteLine("❌ Cannot cancel delivered order");

    public void Return(Order order)
    {
        Console.WriteLine("📬 Return request initiated");
        order.SetState(new ReturnedState());
    }
}

// Cancelled state - order cancelled
public class CancelledState : IOrderState
{
    public void OnEnterState(Order order)
    {
        Console.WriteLine("❌ Order has been cancelled");
    }

    public void AddItem(Order order, OrderItem item) => Console.WriteLine("❌ Cannot modify cancelled order");
    public void RemoveItem(Order order, string itemId) => Console.WriteLine("❌ Cannot modify cancelled order");
    public void SetShippingAddress(Order order, string address) => Console.WriteLine("❌ Cannot modify cancelled order");
    public void SetPaymentMethod(Order order, string paymentMethod) => Console.WriteLine("❌ Cannot modify cancelled order");
    public void SubmitOrder(Order order) => Console.WriteLine("❌ Order is cancelled");
    public void ProcessPayment(Order order) => Console.WriteLine("❌ Order is cancelled");
    public void Ship(Order order) => Console.WriteLine("❌ Order is cancelled");
    public void Deliver(Order order) => Console.WriteLine("❌ Order is cancelled");
    public void Cancel(Order order) => Console.WriteLine("ℹ️ Order is already cancelled");
    public void Return(Order order) => Console.WriteLine("❌ Cannot return cancelled order");
}

// Returned state - order returned
public class ReturnedState : IOrderState
{
    public void OnEnterState(Order order)
    {
        Console.WriteLine("📬 Order has been returned");
    }

    public void AddItem(Order order, OrderItem item) => Console.WriteLine("❌ Cannot modify returned order");
    public void RemoveItem(Order order, string itemId) => Console.WriteLine("❌ Cannot modify returned order");
    public void SetShippingAddress(Order order, string address) => Console.WriteLine("❌ Cannot modify returned order");
    public void SetPaymentMethod(Order order, string paymentMethod) => Console.WriteLine("❌ Cannot modify returned order");
    public void SubmitOrder(Order order) => Console.WriteLine("❌ Order is returned");
    public void ProcessPayment(Order order) => Console.WriteLine("❌ Order is returned");
    public void Ship(Order order) => Console.WriteLine("❌ Order is returned");
    public void Deliver(Order order) => Console.WriteLine("❌ Order is returned");
    public void Cancel(Order order) => Console.WriteLine("❌ Order is returned");
    public void Return(Order order) => Console.WriteLine("ℹ️ Order is already returned");
}

// Usage
var order = new Order("ORD-001", "John Smith");

Console.WriteLine("\n=== Order Processing State Pattern Demo ===");

order.ShowOrderDetails();

// Build the order
Console.WriteLine("\n--- Building order ---");
order.AddItem(new OrderItem("ITEM-001", "Laptop", 999.99m));
order.AddItem(new OrderItem("ITEM-002", "Mouse", 29.99m));
order.AddItem(new OrderItem("ITEM-003", "Keyboard", 79.99m));

order.SetShippingAddress("123 Main St, Anytown, USA 12345");
order.SetPaymentMethod("Credit Card ****1234");

// Try to submit incomplete order
Console.WriteLine("\n--- Attempting to submit ---");
order.SubmitOrder();

// Process the order
Console.WriteLine("\n--- Processing order ---");
order.ProcessPayment();
order.Ship();
order.Deliver();

order.ShowOrderDetails();

// Test different scenarios
Console.WriteLine("\n--- Testing return ---");
order.Return();

Console.WriteLine("\n--- Testing cancelled order ---");
var cancelledOrder = new Order("ORD-002", "Jane Doe");
cancelledOrder.AddItem(new OrderItem("ITEM-004", "Phone", 599.99m));
cancelledOrder.SetShippingAddress("456 Oak Ave, Another City, USA 67890");
cancelledOrder.SetPaymentMethod("Debit Card ****5678");
cancelledOrder.Cancel();
cancelledOrder.Ship(); // Should fail

Console.WriteLine("\n=== Final Status ===");
order.ShowOrderDetails();
cancelledOrder.ShowOrderDetails();
```

## Benefits
- **Eliminates Conditionals**: Removes large conditional statements
- **Behavior Localization**: Each state's behavior is contained in its own class
- **Easy Extension**: Adding new states doesn't require modifying existing code
- **State Transitions**: Clear and explicit state transition logic
- **Maintainability**: Easier to understand and modify state-specific behavior

## Drawbacks
- **Class Proliferation**: Can result in many small state classes
- **Shared State Access**: States may need access to context's private data
- **Complex Transitions**: Complex state transition logic can be hard to follow
- **Memory Overhead**: Creating state objects can use more memory

## When to Use
✅ **Use When:**
- Object behavior depends significantly on its state
- You have many conditional statements based on object state
- State transitions are complex and need to be explicit
- Different states have substantially different behaviors

❌ **Avoid When:**
- Object has only a few states with simple behavior
- State changes are rare or predictable
- The overhead of multiple classes isn't justified
- Simple conditional logic is sufficient

## State vs Other Patterns

| Pattern | Purpose | Key Difference |
|---------|---------|----------------|
| **State** | State-dependent behavior | Behavior changes with state |
| **Strategy** | Algorithm selection | Algorithm choice is independent |
| **Command** | Encapsulates requests | Focuses on request encapsulation |
| **Template Method** | Algorithm structure | Defines algorithm skeleton |

## Best Practices
1. **State Interface**: Keep state interface focused and cohesive
2. **Context Access**: Provide necessary context access to states
3. **State Transitions**: Make state transitions explicit and clear
4. **Error Handling**: Handle invalid operations gracefully in each state
5. **State Factory**: Consider using factory for state creation

## Common Mistakes
1. **Leaky Abstraction**: Exposing too much context to states
2. **God States**: Creating states that handle too many responsibilities
3. **Missing Transitions**: Not handling all possible state transitions
4. **State Pollution**: Adding non-state-specific logic to state classes

## Modern C# Features
```csharp
// Using records for immutable states
public abstract record PlayerState
{
    public abstract PlayerState Play(MediaPlayer player);
    public abstract PlayerState Pause(MediaPlayer player);
    public abstract PlayerState Stop(MediaPlayer player);
}

public record StoppedState : PlayerState
{
    public override PlayerState Play(MediaPlayer player) => new PlayingState();
    public override PlayerState Pause(MediaPlayer player) => this;
    public override PlayerState Stop(MediaPlayer player) => this;
}

// Using switch expressions for state transitions
public class ModernStateMachine
{
    public IState TransitionTo(string trigger, IState currentState) =>
        (trigger, currentState) switch
        {
            ("play", StoppedState) => new PlayingState(),
            ("pause", PlayingState) => new PausedState(),
            ("stop", PlayingState or PausedState) => new StoppedState(),
            _ => currentState
        };
}

// Using state machines with pattern matching
public class PatternMatchingState
{
    public void Handle(object input, IState state) =>
        (input, state) switch
        {
            (PlayCommand, StoppedState s) => s.Play(),
            (PauseCommand, PlayingState s) => s.Pause(),
            (StopCommand, not StoppedState s) => s.Stop(),
            _ => throw new InvalidOperationException("Invalid state transition")
        };
}
```

## Testing State Patterns
```csharp
[Test]
public void MediaPlayer_StateTransitions_WorkCorrectly()
{
    // Arrange
    var player = new MediaPlayer();
    player.AddTrack("Test Song");
    
    // Act & Assert
    Assert.IsInstanceOf<StoppedState>(player.GetCurrentState());
    
    player.Play();
    Assert.IsInstanceOf<PlayingState>(player.GetCurrentState());
    
    player.Pause();
    Assert.IsInstanceOf<PausedState>(player.GetCurrentState());
    
    player.Stop();
    Assert.IsInstanceOf<StoppedState>(player.GetCurrentState());
}

[Test]
public void ATM_InvalidOperations_AreRejected()
{
    // Arrange
    var atm = new ATMMachine();
    
    // Act & Assert - should not throw, just handle gracefully
    atm.EnterPin("1234"); // Should be rejected in idle state
    atm.SelectWithdrawal(); // Should be rejected in idle state
    
    Assert.IsInstanceOf<IdleState>(atm.GetCurrentState());
}
```

## Summary
The State pattern is like having different personalities for the same object - depending on the current state, the object behaves completely differently to the same inputs. It's perfect for modeling objects that have distinct modes of operation, like media players, ATM machines, document editors, or order processing systems.

The pattern transforms complex conditional logic into clean, maintainable state classes where each state knows exactly how to handle different operations. Instead of having massive if-else statements scattered throughout your code, you have focused, single-responsibility state classes that make the system's behavior clear and easy to extend.

The key insight is that State pattern makes state transitions explicit and localizes state-specific behavior, turning what could be a tangled mess of conditional logic into a clean, understandable state machine where adding new states or modifying existing behavior becomes straightforward and safe.
