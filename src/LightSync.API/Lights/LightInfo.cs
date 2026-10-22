using System.Drawing;

namespace LightSync.API.Lights
{
    public abstract class LightInfo : ILightInfo
    {
        private readonly Lock _nameLock = new();
        private readonly Lock _colourLock = new();

        private volatile bool _isOn;
        private string _name;
        private Color _colour;

        public string Identifier { get; }

        public bool IsOn
        {
            get => _isOn;
            protected set
            {
                _isOn = value;
                OnChanged?.Invoke(this, new(value));
            }
        }

        public string Name
        {
            get { lock(_nameLock) return _name; }
            protected set
            {
                lock(_nameLock) _name = value;
                NameChanged?.Invoke(this, new(value));
            }
        }

        public Color Colour
        {
            get { lock (_colourLock) return _colour; }
            protected set
            {
                lock(_colourLock) _colour = value;
                ColourChanged?.Invoke(this, new(value));
            }
        }

        public event EventHandler<OnChangedEventArgs>? OnChanged;
        public event EventHandler<NameChangedEventArgs>? NameChanged;
        public event EventHandler<ColourChangedEventArgs>? ColourChanged;

        protected LightInfo(string name, string identifier)
        {
            _name = name;
            Identifier = identifier;
        }

        public async Task SetOnAsync(bool on, CancellationToken cancellationToken = default) => await SetOnAsync(on, TimeSpan.Zero, cancellationToken);
        public async Task SetColourAsync(Color colour, CancellationToken cancellationToken = default) => await SetColourAsync(colour, TimeSpan.Zero, cancellationToken);

        public abstract Task SetColourAsync(Color colour, TimeSpan transitionTime, CancellationToken cancellationToken = default);
        public abstract Task SetOnAsync(bool on, TimeSpan transitionTime, CancellationToken cancellationToken = default);
    }
}
