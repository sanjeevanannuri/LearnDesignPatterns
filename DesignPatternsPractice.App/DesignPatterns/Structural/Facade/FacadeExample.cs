namespace DesignPatterns.Structural.Facade
{
    // Complex subsystem classes
    public class CPU
    {
        public void Freeze() => Console.WriteLine("CPU: Freezing processor...");
        public void Jump(long position) => Console.WriteLine($"CPU: Jumping to position {position}");
        public void Execute() => Console.WriteLine("CPU: Executing...");
    }

    public class Memory
    {
        public void Load(long position, byte[] data)
            => Console.WriteLine($"Memory: Loading {data.Length} bytes at position {position}");
    }

    public class HardDrive
    {
        public byte[] Read(long lba, int size)
        {
            Console.WriteLine($"HardDrive: Reading {size} bytes from LBA {lba}");
            return new byte[size];
        }
    }

    // Facade - provides a simple interface to the complex subsystem
    public class ComputerFacade
    {
        private readonly CPU _cpu;
        private readonly Memory _memory;
        private readonly HardDrive _hardDrive;

        public ComputerFacade()
        {
            _cpu = new CPU();
            _memory = new Memory();
            _hardDrive = new HardDrive();
        }

        public void StartComputer()
        {
            Console.WriteLine("Starting computer...");
            _cpu.Freeze();
            _memory.Load(0x00, _hardDrive.Read(0x00, 1024));
            _cpu.Jump(0x00);
            _cpu.Execute();
            Console.WriteLine("Computer started successfully!");
        }
    }

    // Real-world example: Home Theater System
    public class Amplifier
    {
        public void On() => Console.WriteLine("Amplifier: Turning on");
        public void SetVolume(int volume) => Console.WriteLine($"Amplifier: Setting volume to {volume}");
        public void Off() => Console.WriteLine("Amplifier: Turning off");
    }

    public class DvdPlayer
    {
        public void On() => Console.WriteLine("DVD Player: Turning on");
        public void Play(string movie) => Console.WriteLine($"DVD Player: Playing '{movie}'");
        public void Stop() => Console.WriteLine("DVD Player: Stopping");
        public void Off() => Console.WriteLine("DVD Player: Turning off");
    }

    public class Projector
    {
        public void On() => Console.WriteLine("Projector: Turning on");
        public void SetInput(string input) => Console.WriteLine($"Projector: Setting input to {input}");
        public void Off() => Console.WriteLine("Projector: Turning off");
    }

    public class Lights
    {
        public void Dim(int level) => Console.WriteLine($"Lights: Dimming to {level}%");
        public void On() => Console.WriteLine("Lights: Turning on");
    }

    // Home Theater Facade
    public class HomeTheaterFacade
    {
        private readonly Amplifier _amplifier;
        private readonly DvdPlayer _dvdPlayer;
        private readonly Projector _projector;
        private readonly Lights _lights;

        public HomeTheaterFacade()
        {
            _amplifier = new Amplifier();
            _dvdPlayer = new DvdPlayer();
            _projector = new Projector();
            _lights = new Lights();
        }

        public void WatchMovie(string movie)
        {
            Console.WriteLine("\nGet ready to watch a movie...");
            _lights.Dim(10);
            _projector.On();
            _projector.SetInput("DVD");
            _amplifier.On();
            _amplifier.SetVolume(5);
            _dvdPlayer.On();
            _dvdPlayer.Play(movie);
        }

        public void EndMovie()
        {
            Console.WriteLine("\nShutting down movie theater...");
            _dvdPlayer.Stop();
            _dvdPlayer.Off();
            _amplifier.Off();
            _projector.Off();
            _lights.On();
        }
    }
}
