namespace DesignPatterns.Behavioral.Memento
{
    // Memento interface
    public interface IMemento
    {
        string GetName();
        DateTime GetDate();
    }

    // Originator
    public class TextEditor
    {
        private string _content = "";
        private string _cursorPosition = "0";
        private string _selection = "";

        public string Content => _content;
        public string CursorPosition => _cursorPosition;
        public string Selection => _selection;

        public void Write(string text)
        {
            _content += text;
            _cursorPosition = _content.Length.ToString();
            Console.WriteLine($"Wrote: '{text}'. Content now: '{_content}'");
        }

        public void SetContent(string content)
        {
            _content = content;
            _cursorPosition = _content.Length.ToString();
            Console.WriteLine($"Content set to: '{_content}'");
        }

        public void SetCursor(string position)
        {
            _cursorPosition = position;
            Console.WriteLine($"Cursor moved to position: {_cursorPosition}");
        }

        public void SetSelection(string selection)
        {
            _selection = selection;
            Console.WriteLine($"Selected text: '{_selection}'");
        }

        public void Delete(int length)
        {
            if (_content.Length >= length)
            {
                _content = _content.Substring(0, _content.Length - length);
                _cursorPosition = _content.Length.ToString();
                Console.WriteLine($"Deleted {length} characters. Content now: '{_content}'");
            }
        }

        // Save current state
        public IMemento Save()
        {
            Console.WriteLine($"Saving editor state: '{_content}'");
            return new EditorMemento(_content, _cursorPosition, _selection);
        }

        // Restore from memento
        public void Restore(IMemento memento)
        {
            if (memento is EditorMemento editorMemento)
            {
                _content = editorMemento.GetContent();
                _cursorPosition = editorMemento.GetCursorPosition();
                _selection = editorMemento.GetSelection();
                Console.WriteLine($"Restored editor state: '{_content}'");
            }
        }

        // Private memento class
        private class EditorMemento : IMemento
        {
            private readonly string _content;
            private readonly string _cursorPosition;
            private readonly string _selection;
            private readonly DateTime _date;

            public EditorMemento(string content, string cursorPosition, string selection)
            {
                _content = content;
                _cursorPosition = cursorPosition;
                _selection = selection;
                _date = DateTime.Now;
            }

            public string GetContent() => _content;
            public string GetCursorPosition() => _cursorPosition;
            public string GetSelection() => _selection;

            public string GetName()
            {
                return $"Editor state: '{_content.Substring(0, Math.Min(_content.Length, 20))}{(_content.Length > 20 ? "..." : "")}'";
            }

            public DateTime GetDate() => _date;
        }
    }

    // Caretaker
    public class EditorHistory
    {
        private readonly Stack<IMemento> _history = new();
        private readonly TextEditor _editor;

        public EditorHistory(TextEditor editor)
        {
            _editor = editor;
        }

        public void Backup()
        {
            var memento = _editor.Save();
            _history.Push(memento);
            Console.WriteLine($"Backup created: {memento.GetName()}");
        }

        public void Undo()
        {
            if (_history.Count > 0)
            {
                var memento = _history.Pop();
                _editor.Restore(memento);
                Console.WriteLine($"Undone to: {memento.GetName()}");
            }
            else
            {
                Console.WriteLine("No more states to undo");
            }
        }

        public void ShowHistory()
        {
            Console.WriteLine($"History contains {_history.Count} states:");
            var states = _history.ToArray();
            for (int i = 0; i < states.Length; i++)
            {
                Console.WriteLine($"  {i + 1}. {states[i].GetName()} - {states[i].GetDate():HH:mm:ss}");
            }
        }
    }

    // Game save example
    public class GameCharacter
    {
        public string Name { get; private set; }
        public int Level { get; private set; }
        public int Health { get; private set; }
        public int Mana { get; private set; }
        public int Experience { get; private set; }
        public string Location { get; private set; }
        public List<string> Inventory { get; private set; }

        public GameCharacter(string name)
        {
            Name = name;
            Level = 1;
            Health = 100;
            Mana = 50;
            Experience = 0;
            Location = "Starting Village";
            Inventory = new List<string> { "Basic Sword", "Health Potion" };
        }

        public void LevelUp()
        {
            Level++;
            Health += 20;
            Mana += 10;
            Console.WriteLine($"{Name} leveled up to {Level}! Health: {Health}, Mana: {Mana}");
        }

        public void GainExperience(int exp)
        {
            Experience += exp;
            Console.WriteLine($"{Name} gained {exp} experience. Total: {Experience}");
        }

        public void TakeDamage(int damage)
        {
            Health = Math.Max(0, Health - damage);
            Console.WriteLine($"{Name} took {damage} damage. Health: {Health}");
        }

        public void MoveTo(string newLocation)
        {
            Location = newLocation;
            Console.WriteLine($"{Name} moved to {Location}");
        }

        public void AddItem(string item)
        {
            Inventory.Add(item);
            Console.WriteLine($"{Name} acquired {item}");
        }

        public IMemento CreateSave()
        {
            Console.WriteLine($"Creating save for {Name} at {Location}");
            return new GameSave(Name, Level, Health, Mana, Experience, Location, new List<string>(Inventory));
        }

        public void LoadSave(IMemento memento)
        {
            if (memento is GameSave save)
            {
                Name = save.GetName();
                Level = save.GetLevel();
                Health = save.GetHealth();
                Mana = save.GetMana();
                Experience = save.GetExperience();
                Location = save.GetLocation();
                Inventory = new List<string>(save.GetInventory());
                Console.WriteLine($"Loaded save: {Name} Level {Level} at {Location}");
            }
        }

        private class GameSave : IMemento
        {
            private readonly string _name;
            private readonly int _level;
            private readonly int _health;
            private readonly int _mana;
            private readonly int _experience;
            private readonly string _location;
            private readonly List<string> _inventory;
            private readonly DateTime _saveDate;

            public GameSave(string name, int level, int health, int mana, int experience, string location, List<string> inventory)
            {
                _name = name;
                _level = level;
                _health = health;
                _mana = mana;
                _experience = experience;
                _location = location;
                _inventory = inventory;
                _saveDate = DateTime.Now;
            }

            public string GetName() => $"{_name} - Level {_level} at {_location}";
            public DateTime GetDate() => _saveDate;
            public string GetCharacterName() => _name;
            public int GetLevel() => _level;
            public int GetHealth() => _health;
            public int GetMana() => _mana;
            public int GetExperience() => _experience;
            public string GetLocation() => _location;
            public List<string> GetInventory() => _inventory;
        }
    }

    public class GameSaveManager
    {
        private readonly Dictionary<string, Stack<IMemento>> _saveSlots = new();

        public void SaveGame(GameCharacter character, string slotName)
        {
            if (!_saveSlots.ContainsKey(slotName))
            {
                _saveSlots[slotName] = new Stack<IMemento>();
            }

            var save = character.CreateSave();
            _saveSlots[slotName].Push(save);
            Console.WriteLine($"Game saved to slot '{slotName}': {save.GetName()}");
        }

        public void LoadGame(GameCharacter character, string slotName)
        {
            if (_saveSlots.ContainsKey(slotName) && _saveSlots[slotName].Count > 0)
            {
                var save = _saveSlots[slotName].Peek();
                character.LoadSave(save);
                Console.WriteLine($"Game loaded from slot '{slotName}': {save.GetName()}");
            }
            else
            {
                Console.WriteLine($"No save found in slot '{slotName}'");
            }
        }

        public void ShowSaveSlots()
        {
            Console.WriteLine("Available save slots:");
            foreach (var slot in _saveSlots)
            {
                if (slot.Value.Count > 0)
                {
                    var latestSave = slot.Value.Peek();
                    Console.WriteLine($"  {slot.Key}: {latestSave.GetName()} - {latestSave.GetDate():yyyy-MM-dd HH:mm:ss}");
                }
                else
                {
                    Console.WriteLine($"  {slot.Key}: Empty");
                }
            }
        }
    }

    // Calculator example with operation history
    public class Calculator
    {
        private double _value;

        public double Value => _value;

        public void Add(double operand)
        {
            _value += operand;
            Console.WriteLine($"Added {operand}. Result: {_value}");
        }

        public void Subtract(double operand)
        {
            _value -= operand;
            Console.WriteLine($"Subtracted {operand}. Result: {_value}");
        }

        public void Multiply(double operand)
        {
            _value *= operand;
            Console.WriteLine($"Multiplied by {operand}. Result: {_value}");
        }

        public void Divide(double operand)
        {
            if (operand != 0)
            {
                _value /= operand;
                Console.WriteLine($"Divided by {operand}. Result: {_value}");
            }
            else
            {
                Console.WriteLine("Cannot divide by zero");
            }
        }

        public void Clear()
        {
            _value = 0;
            Console.WriteLine("Calculator cleared. Result: 0");
        }

        public IMemento SaveState()
        {
            return new CalculatorMemento(_value);
        }

        public void RestoreState(IMemento memento)
        {
            if (memento is CalculatorMemento calcMemento)
            {
                _value = calcMemento.GetValue();
                Console.WriteLine($"Calculator state restored. Value: {_value}");
            }
        }

        private class CalculatorMemento : IMemento
        {
            private readonly double _value;
            private readonly DateTime _timestamp;

            public CalculatorMemento(double value)
            {
                _value = value;
                _timestamp = DateTime.Now;
            }

            public double GetValue() => _value;
            public string GetName() => $"Calculator state: {_value}";
            public DateTime GetDate() => _timestamp;
        }
    }

    public class CalculatorHistory
    {
        private readonly Stack<IMemento> _history = new();
        private readonly Calculator _calculator;

        public CalculatorHistory(Calculator calculator)
        {
            _calculator = calculator;
        }

        public void SaveState()
        {
            var memento = _calculator.SaveState();
            _history.Push(memento);
            Console.WriteLine($"State saved: {memento.GetName()}");
        }

        public void Undo()
        {
            if (_history.Count > 0)
            {
                var memento = _history.Pop();
                _calculator.RestoreState(memento);
                Console.WriteLine($"Undone to: {memento.GetName()}");
            }
            else
            {
                Console.WriteLine("No previous states to restore");
            }
        }

        public int GetHistoryCount()
        {
            return _history.Count;
        }
    }
}
