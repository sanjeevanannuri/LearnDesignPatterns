namespace DesignPatterns.Behavioral.Command
{
    // Command interface
    public interface ICommand
    {
        void Execute();
        void Undo();
    }

    // Receiver - Light
    public class Light
    {
        private bool _isOn = false;
        public string Location { get; private set; }

        public Light(string location)
        {
            Location = location;
        }

        public void TurnOn()
        {
            _isOn = true;
            Console.WriteLine($"{Location} light is ON");
        }

        public void TurnOff()
        {
            _isOn = false;
            Console.WriteLine($"{Location} light is OFF");
        }

        public bool IsOn => _isOn;
    }

    // Receiver - Stereo
    public class Stereo
    {
        public string Location { get; private set; }
        private bool _isOn = false;
        private int _volume = 0;

        public Stereo(string location)
        {
            Location = location;
        }

        public void On()
        {
            _isOn = true;
            Console.WriteLine($"{Location} stereo is ON");
        }

        public void Off()
        {
            _isOn = false;
            Console.WriteLine($"{Location} stereo is OFF");
        }

        public void SetVolume(int volume)
        {
            _volume = volume;
            Console.WriteLine($"{Location} stereo volume set to {volume}");
        }

        public int GetVolume() => _volume;
        public bool IsOn => _isOn;
    }

    // Concrete Commands
    public class LightOnCommand : ICommand
    {
        private readonly Light _light;

        public LightOnCommand(Light light)
        {
            _light = light;
        }

        public void Execute()
        {
            _light.TurnOn();
        }

        public void Undo()
        {
            _light.TurnOff();
        }
    }

    public class LightOffCommand : ICommand
    {
        private readonly Light _light;

        public LightOffCommand(Light light)
        {
            _light = light;
        }

        public void Execute()
        {
            _light.TurnOff();
        }

        public void Undo()
        {
            _light.TurnOn();
        }
    }

    public class StereoOnWithVolumeCommand : ICommand
    {
        private readonly Stereo _stereo;
        private int _previousVolume;

        public StereoOnWithVolumeCommand(Stereo stereo)
        {
            _stereo = stereo;
        }

        public void Execute()
        {
            _previousVolume = _stereo.GetVolume();
            _stereo.On();
            _stereo.SetVolume(11);
        }

        public void Undo()
        {
            _stereo.SetVolume(_previousVolume);
            _stereo.Off();
        }
    }

    public class StereoOffCommand : ICommand
    {
        private readonly Stereo _stereo;
        private int _previousVolume;

        public StereoOffCommand(Stereo stereo)
        {
            _stereo = stereo;
        }

        public void Execute()
        {
            _previousVolume = _stereo.GetVolume();
            _stereo.Off();
        }

        public void Undo()
        {
            _stereo.On();
            _stereo.SetVolume(_previousVolume);
        }
    }

    // Null Object Pattern for Command
    public class NoCommand : ICommand
    {
        public void Execute() { }
        public void Undo() { }
    }

    // Macro Command (composite command)
    public class MacroCommand : ICommand
    {
        private readonly ICommand[] _commands;

        public MacroCommand(ICommand[] commands)
        {
            _commands = commands;
        }

        public void Execute()
        {
            foreach (var command in _commands)
            {
                command.Execute();
            }
        }

        public void Undo()
        {
            // Undo in reverse order
            for (int i = _commands.Length - 1; i >= 0; i--)
            {
                _commands[i].Undo();
            }
        }
    }

    // Invoker - Remote Control
    public class RemoteControl
    {
        private readonly ICommand[] _onCommands;
        private readonly ICommand[] _offCommands;
        private ICommand _undoCommand;

        public RemoteControl()
        {
            _onCommands = new ICommand[7];
            _offCommands = new ICommand[7];

            var noCommand = new NoCommand();
            for (int i = 0; i < 7; i++)
            {
                _onCommands[i] = noCommand;
                _offCommands[i] = noCommand;
            }
            _undoCommand = noCommand;
        }

        public void SetCommand(int slot, ICommand onCommand, ICommand offCommand)
        {
            _onCommands[slot] = onCommand;
            _offCommands[slot] = offCommand;
        }

        public void OnButtonPressed(int slot)
        {
            _onCommands[slot].Execute();
            _undoCommand = _onCommands[slot];
        }

        public void OffButtonPressed(int slot)
        {
            _offCommands[slot].Execute();
            _undoCommand = _offCommands[slot];
        }

        public void UndoButtonPressed()
        {
            _undoCommand.Undo();
        }

        public override string ToString()
        {
            var stringBuilder = new System.Text.StringBuilder();
            stringBuilder.AppendLine("\n------ Remote Control ------");
            for (int i = 0; i < _onCommands.Length; i++)
            {
                stringBuilder.AppendLine($"[slot {i}] {_onCommands[i].GetType().Name}    {_offCommands[i].GetType().Name}");
            }
            stringBuilder.AppendLine($"[undo] {_undoCommand.GetType().Name}");
            return stringBuilder.ToString();
        }
    }

    // Real-world example: Text Editor Commands
    public class TextEditor
    {
        private string _content = "";

        public void Write(string text)
        {
            _content += text;
            Console.WriteLine($"Text written: '{text}'. Current content: '{_content}'");
        }

        public void Delete(int length)
        {
            if (length <= _content.Length)
            {
                string deleted = _content.Substring(_content.Length - length);
                _content = _content.Substring(0, _content.Length - length);
                Console.WriteLine($"Deleted: '{deleted}'. Current content: '{_content}'");
            }
        }

        public string GetContent() => _content;
    }

    public class WriteCommand : ICommand
    {
        private readonly TextEditor _editor;
        private readonly string _text;

        public WriteCommand(TextEditor editor, string text)
        {
            _editor = editor;
            _text = text;
        }

        public void Execute()
        {
            _editor.Write(_text);
        }

        public void Undo()
        {
            _editor.Delete(_text.Length);
        }
    }

    public class DeleteCommand : ICommand
    {
        private readonly TextEditor _editor;
        private readonly int _length;
        private string _deletedText = "";

        public DeleteCommand(TextEditor editor, int length)
        {
            _editor = editor;
            _length = length;
        }

        public void Execute()
        {
            string content = _editor.GetContent();
            if (_length <= content.Length)
            {
                _deletedText = content.Substring(content.Length - _length);
                _editor.Delete(_length);
            }
        }

        public void Undo()
        {
            _editor.Write(_deletedText);
        }
    }
}
