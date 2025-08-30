namespace DesignPatterns.Behavioral.Mediator
{
    // Mediator interface
    public interface IChatMediator
    {
        void SendMessage(string message, User user);
        void AddUser(User user);
        void RemoveUser(User user);
    }

    // Abstract colleague
    public abstract class User
    {
        protected IChatMediator _mediator;
        public string Name { get; }

        public User(IChatMediator mediator, string name)
        {
            _mediator = mediator;
            Name = name;
        }

        public abstract void Send(string message);
        public abstract void Receive(string message, string from);
    }

    // Concrete colleagues
    public class ChatUser : User
    {
        public ChatUser(IChatMediator mediator, string name) : base(mediator, name)
        {
        }

        public override void Send(string message)
        {
            Console.WriteLine($"{Name} sends: {message}");
            _mediator.SendMessage(message, this);
        }

        public override void Receive(string message, string from)
        {
            Console.WriteLine($"{Name} receives from {from}: {message}");
        }
    }

    public class AdminUser : User
    {
        public AdminUser(IChatMediator mediator, string name) : base(mediator, name)
        {
        }

        public override void Send(string message)
        {
            Console.WriteLine($"[ADMIN] {Name} broadcasts: {message}");
            _mediator.SendMessage($"[ADMIN ANNOUNCEMENT] {message}", this);
        }

        public override void Receive(string message, string from)
        {
            Console.WriteLine($"[ADMIN] {Name} receives from {from}: {message}");
        }
    }

    // Concrete mediator
    public class ChatRoom : IChatMediator
    {
        private readonly List<User> _users = new();

        public void AddUser(User user)
        {
            _users.Add(user);
            Console.WriteLine($"{user.Name} joined the chat room");
        }

        public void RemoveUser(User user)
        {
            _users.Remove(user);
            Console.WriteLine($"{user.Name} left the chat room");
        }

        public void SendMessage(string message, User sender)
        {
            foreach (var user in _users)
            {
                if (user != sender)
                {
                    user.Receive(message, sender.Name);
                }
            }
        }
    }

    // Air Traffic Control example
    public interface IAirTrafficControlMediator
    {
        void RequestLanding(Aircraft aircraft);
        void RequestTakeoff(Aircraft aircraft);
        void RequestRunwayChange(Aircraft aircraft, string newRunway);
        void RegisterAircraft(Aircraft aircraft);
    }

    public abstract class Aircraft
    {
        protected IAirTrafficControlMediator _mediator;
        public string CallSign { get; }

        public Aircraft(IAirTrafficControlMediator mediator, string callSign)
        {
            _mediator = mediator;
            CallSign = callSign;
        }

        public abstract void RequestLanding();
        public abstract void RequestTakeoff();
        public abstract void ReceiveInstruction(string instruction);
    }

    public class PassengerAircraft : Aircraft
    {
        public PassengerAircraft(IAirTrafficControlMediator mediator, string callSign)
            : base(mediator, callSign)
        {
        }

        public override void RequestLanding()
        {
            Console.WriteLine($"Passenger aircraft {CallSign} requesting landing permission");
            _mediator.RequestLanding(this);
        }

        public override void RequestTakeoff()
        {
            Console.WriteLine($"Passenger aircraft {CallSign} requesting takeoff clearance");
            _mediator.RequestTakeoff(this);
        }

        public override void ReceiveInstruction(string instruction)
        {
            Console.WriteLine($"Passenger aircraft {CallSign} received: {instruction}");
        }
    }

    public class CargoAircraft : Aircraft
    {
        public CargoAircraft(IAirTrafficControlMediator mediator, string callSign)
            : base(mediator, callSign)
        {
        }

        public override void RequestLanding()
        {
            Console.WriteLine($"Cargo aircraft {CallSign} requesting landing permission");
            _mediator.RequestLanding(this);
        }

        public override void RequestTakeoff()
        {
            Console.WriteLine($"Cargo aircraft {CallSign} requesting takeoff clearance");
            _mediator.RequestTakeoff(this);
        }

        public override void ReceiveInstruction(string instruction)
        {
            Console.WriteLine($"Cargo aircraft {CallSign} received: {instruction}");
        }
    }

    public class AirTrafficControl : IAirTrafficControlMediator
    {
        private readonly List<Aircraft> _aircraftList = new();
        private readonly List<string> _availableRunways = new() { "Runway 1", "Runway 2", "Runway 3" };
        private readonly Dictionary<string, bool> _runwayStatus = new();

        public AirTrafficControl()
        {
            foreach (var runway in _availableRunways)
            {
                _runwayStatus[runway] = true; // true = available
            }
        }

        public void RegisterAircraft(Aircraft aircraft)
        {
            _aircraftList.Add(aircraft);
            aircraft.ReceiveInstruction($"Welcome to control zone. You are aircraft #{_aircraftList.Count}");
        }

        public void RequestLanding(Aircraft aircraft)
        {
            var availableRunway = _runwayStatus.FirstOrDefault(r => r.Value).Key;
            if (availableRunway != null)
            {
                _runwayStatus[availableRunway] = false;
                aircraft.ReceiveInstruction($"Landing cleared on {availableRunway}");

                // Simulate landing completion after instruction
                Task.Delay(100).ContinueWith(_ =>
                {
                    _runwayStatus[availableRunway] = true;
                    aircraft.ReceiveInstruction($"Landing completed. Please taxi to gate. {availableRunway} is now available");
                });
            }
            else
            {
                aircraft.ReceiveInstruction("All runways busy. Please hold pattern at 10,000 feet");
            }
        }

        public void RequestTakeoff(Aircraft aircraft)
        {
            var availableRunway = _runwayStatus.FirstOrDefault(r => r.Value).Key;
            if (availableRunway != null)
            {
                _runwayStatus[availableRunway] = false;
                aircraft.ReceiveInstruction($"Takeoff cleared on {availableRunway}");

                // Simulate takeoff completion after instruction
                Task.Delay(100).ContinueWith(_ =>
                {
                    _runwayStatus[availableRunway] = true;
                    aircraft.ReceiveInstruction($"Takeoff completed. Contact departure control. {availableRunway} is now available");
                });
            }
            else
            {
                aircraft.ReceiveInstruction("All runways busy. Please hold at gate");
            }
        }

        public void RequestRunwayChange(Aircraft aircraft, string newRunway)
        {
            if (_runwayStatus.ContainsKey(newRunway) && _runwayStatus[newRunway])
            {
                aircraft.ReceiveInstruction($"Runway change approved. Proceed to {newRunway}");
            }
            else
            {
                aircraft.ReceiveInstruction($"Runway change denied. {newRunway} is not available");
            }
        }
    }

    // UI Components example
    public interface IDialogMediator
    {
        void Notify(Component sender, string eventType);
    }

    public abstract class Component
    {
        protected IDialogMediator _dialog;

        public Component(IDialogMediator dialog)
        {
            _dialog = dialog;
        }
    }

    public class Button : Component
    {
        public string Text { get; set; }

        public Button(IDialogMediator dialog, string text) : base(dialog)
        {
            Text = text;
        }

        public void Click()
        {
            Console.WriteLine($"Button '{Text}' clicked");
            _dialog.Notify(this, "click");
        }
    }

    public class TextBox : Component
    {
        public string Value { get; set; } = "";

        public TextBox(IDialogMediator dialog) : base(dialog)
        {
        }

        public void SetText(string text)
        {
            Value = text;
            Console.WriteLine($"TextBox value changed to: '{text}'");
            _dialog.Notify(this, "textChanged");
        }
    }

    public class CheckBox : Component
    {
        public bool IsChecked { get; set; }

        public CheckBox(IDialogMediator dialog) : base(dialog)
        {
        }

        public void Toggle()
        {
            IsChecked = !IsChecked;
            Console.WriteLine($"CheckBox {(IsChecked ? "checked" : "unchecked")}");
            _dialog.Notify(this, "toggled");
        }
    }

    public class ListBox : Component
    {
        public List<string> Items { get; } = new();
        public string? SelectedItem { get; set; }

        public ListBox(IDialogMediator dialog) : base(dialog)
        {
        }

        public void AddItem(string item)
        {
            Items.Add(item);
            Console.WriteLine($"Item '{item}' added to ListBox");
        }

        public void SelectItem(string item)
        {
            if (Items.Contains(item))
            {
                SelectedItem = item;
                Console.WriteLine($"Item '{item}' selected in ListBox");
                _dialog.Notify(this, "selectionChanged");
            }
        }
    }

    public class LoginDialog : IDialogMediator
    {
        private readonly TextBox _usernameTextBox;
        private readonly TextBox _passwordTextBox;
        private readonly CheckBox _rememberMeCheckBox;
        private readonly Button _loginButton;
        private readonly Button _forgotPasswordButton;
        private readonly ListBox _recentUsersListBox;

        public LoginDialog()
        {
            _usernameTextBox = new TextBox(this);
            _passwordTextBox = new TextBox(this);
            _rememberMeCheckBox = new CheckBox(this);
            _loginButton = new Button(this, "Login");
            _forgotPasswordButton = new Button(this, "Forgot Password");
            _recentUsersListBox = new ListBox(this);

            // Populate recent users
            _recentUsersListBox.AddItem("john@example.com");
            _recentUsersListBox.AddItem("jane@example.com");
            _recentUsersListBox.AddItem("admin@example.com");
        }

        public void Notify(Component sender, string eventType)
        {
            if (sender == _usernameTextBox && eventType == "textChanged")
            {
                ValidateLoginButton();
            }
            else if (sender == _passwordTextBox && eventType == "textChanged")
            {
                ValidateLoginButton();
            }
            else if (sender == _loginButton && eventType == "click")
            {
                AttemptLogin();
            }
            else if (sender == _forgotPasswordButton && eventType == "click")
            {
                HandleForgotPassword();
            }
            else if (sender == _recentUsersListBox && eventType == "selectionChanged")
            {
                if (_recentUsersListBox.SelectedItem != null)
                {
                    _usernameTextBox.SetText(_recentUsersListBox.SelectedItem);
                }
            }
            else if (sender == _rememberMeCheckBox && eventType == "toggled")
            {
                Console.WriteLine($"Remember me preference: {_rememberMeCheckBox.IsChecked}");
            }
        }

        private void ValidateLoginButton()
        {
            bool isValid = !string.IsNullOrEmpty(_usernameTextBox.Value) &&
                          !string.IsNullOrEmpty(_passwordTextBox.Value);
            Console.WriteLine($"Login button {(isValid ? "enabled" : "disabled")}");
        }

        private void AttemptLogin()
        {
            Console.WriteLine($"Attempting login for user: {_usernameTextBox.Value}");
            Console.WriteLine($"Remember me: {_rememberMeCheckBox.IsChecked}");

            if (_usernameTextBox.Value.Contains("admin"))
            {
                Console.WriteLine("Login successful! Welcome, Admin!");
            }
            else
            {
                Console.WriteLine("Login successful! Welcome!");
            }
        }

        private void HandleForgotPassword()
        {
            if (!string.IsNullOrEmpty(_usernameTextBox.Value))
            {
                Console.WriteLine($"Password reset email sent to: {_usernameTextBox.Value}");
            }
            else
            {
                Console.WriteLine("Please enter your username first");
            }
        }

        public void SimulateUserInteraction()
        {
            Console.WriteLine("=== Login Dialog Simulation ===");
            _recentUsersListBox.SelectItem("john@example.com");
            _passwordTextBox.SetText("password123");
            _rememberMeCheckBox.Toggle();
            _loginButton.Click();
        }
    }
}
