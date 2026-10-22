using System;
using System.Threading.Tasks;

namespace LightSync.UI.Progress
{
    internal interface IProgressInfo : IDisposable
    {
        bool IsIndeterminate { get; set; }
        double ProgressPercentage { get; set; }
        string? Message { get; set; }

        TaskCompletionSource CompletedTask { get; }

        event EventHandler<EventArgs>? Completed;
        event EventHandler<IsIndeterminateChangedEventArgs>? IsIndeterminateChanged;
        event EventHandler<ProgressPercentageChangedEventArgs>? ProgressPercentageChanged;
        event EventHandler<MessageChangedEventArgs>? MessageChanged;

        public void Complete();
    }

    internal sealed class IsIndeterminateChangedEventArgs(bool isIndeterminate) : EventArgs
    {
        public bool IsIndeterminate { get; } = isIndeterminate;
    }

    internal sealed class ProgressPercentageChangedEventArgs(double progressPercentage) : EventArgs
    {
        public double ProgressPercentage { get; } = progressPercentage;
    }

    internal sealed class MessageChangedEventArgs(string? message) : EventArgs
    {
        public string? Message { get; } = message;
    }
}
