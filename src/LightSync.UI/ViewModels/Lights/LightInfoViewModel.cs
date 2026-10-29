using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using LightSync.API.Lights;
using LightSync.UI.Extensions;
using System;
using System.Threading;

namespace LightSync.UI.ViewModels.Lights
{
    internal partial class LightInfoViewModel : ObservableObject, IDisposable
    {
        private bool _isUpdatingFromEvent = false;

        public ILightInfo AssociatedLightInfo { get; init; }

        [ObservableProperty]
        public partial bool IsOn { get; private set; }

        [ObservableProperty]
        public partial string? Name { get; private set; }

        [ObservableProperty]
        public partial string? Identifier { get; private set; }

        [ObservableProperty]
        public partial Color Colour { get; set; }

        public LightInfoViewModel(ILightInfo lightInfo)
        {
            _isUpdatingFromEvent = true;

            AssociatedLightInfo = lightInfo;

            Name = lightInfo.Name;
            Identifier = lightInfo.Identifier;
            IsOn = lightInfo.IsOn;
            Colour = lightInfo.Colour.ToAvalonia();

            SubscribeToEvents();

            _isUpdatingFromEvent = false;
        }

        public void Dispose()
        {
            UnsubscribeFromEvents();
        }

        partial void OnIsOnChanged(bool oldValue, bool newValue)
        {
            if (_isUpdatingFromEvent || oldValue == newValue)
                return;

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            _ = AssociatedLightInfo.SetOnAsync(newValue, cancellationTokenSource.Token);
        }

        partial void OnColourChanged(Color oldValue, Color newValue)
        {
            if (_isUpdatingFromEvent || !IsColourNoticeablyDifferent(oldValue, newValue))
                return;

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            _ = AssociatedLightInfo.SetColourAsync(newValue.ToSystem(), cancellationTokenSource.Token);
        }

        private static bool IsColourNoticeablyDifferent(Color oldValue, Color newValue) => true;

        private void SubscribeToEvents()
        {
            AssociatedLightInfo.NameChanged += OnNameChanged;
            AssociatedLightInfo.OnChanged += OnOnChanged;
            AssociatedLightInfo.ColourChanged += OnColourChanged;
        }

        private void UnsubscribeFromEvents()
        {
            AssociatedLightInfo.NameChanged -= OnNameChanged;
            AssociatedLightInfo.ColourChanged -= OnColourChanged;
        }

        private void OnNameChanged(object? sender, NameChangedEventArgs e)
        {
            _isUpdatingFromEvent = true;

            Dispatcher.UIThread.Post(() => Name = e.Name);

            _isUpdatingFromEvent = false;
        }

        private void OnOnChanged(object? sender, OnChangedEventArgs e)
        {
            _isUpdatingFromEvent = true;

            Dispatcher.UIThread.Post(() => IsOn = e.IsOn);

            _isUpdatingFromEvent = false;
        }

        private void OnColourChanged(object? sender, ColourChangedEventArgs e)
        {
            _isUpdatingFromEvent = true;

            if (e.Colour == Colour.ToSystem())
                return;

            Dispatcher.UIThread.Post(() => Colour = e.Colour.ToAvalonia());

            _isUpdatingFromEvent = false;
        }
    }
}
