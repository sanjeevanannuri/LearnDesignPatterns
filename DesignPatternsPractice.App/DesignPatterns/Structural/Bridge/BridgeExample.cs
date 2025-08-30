namespace DesignPatterns.Structural.Bridge
{
    // Implementation interface
    public interface IDevice
    {
        bool IsEnabled();
        void Enable();
        void Disable();
        int GetVolume();
        void SetVolume(int percent);
        int GetChannel();
        void SetChannel(int channel);
    }

    // Concrete Implementations
    public class TV : IDevice
    {
        private bool _on = false;
        private int _volume = 30;
        private int _channel = 1;

        public bool IsEnabled() => _on;
        public void Enable() => _on = true;
        public void Disable() => _on = false;
        public int GetVolume() => _volume;
        public void SetVolume(int percent) => _volume = percent;
        public int GetChannel() => _channel;
        public void SetChannel(int channel) => _channel = channel;
    }

    public class Radio : IDevice
    {
        private bool _on = false;
        private int _volume = 50;
        private int _channel = 101;

        public bool IsEnabled() => _on;
        public void Enable() => _on = true;
        public void Disable() => _on = false;
        public int GetVolume() => _volume;
        public void SetVolume(int percent) => _volume = percent;
        public int GetChannel() => _channel;
        public void SetChannel(int channel) => _channel = channel;
    }

    // Abstraction
    public class RemoteControl
    {
        protected IDevice device;

        public RemoteControl(IDevice device)
        {
            this.device = device;
        }

        public virtual string TogglePower()
        {
            if (device.IsEnabled())
            {
                device.Disable();
                return "Device turned OFF";
            }
            else
            {
                device.Enable();
                return "Device turned ON";
            }
        }

        public virtual string VolumeDown()
        {
            device.SetVolume(device.GetVolume() - 10);
            return $"Volume set to {device.GetVolume()}";
        }

        public virtual string VolumeUp()
        {
            device.SetVolume(device.GetVolume() + 10);
            return $"Volume set to {device.GetVolume()}";
        }

        public virtual string ChannelDown()
        {
            device.SetChannel(device.GetChannel() - 1);
            return $"Channel set to {device.GetChannel()}";
        }

        public virtual string ChannelUp()
        {
            device.SetChannel(device.GetChannel() + 1);
            return $"Channel set to {device.GetChannel()}";
        }
    }

    // Refined Abstraction
    public class AdvancedRemoteControl : RemoteControl
    {
        public AdvancedRemoteControl(IDevice device) : base(device) { }

        public string Mute()
        {
            device.SetVolume(0);
            return "Device muted";
        }
    }
}