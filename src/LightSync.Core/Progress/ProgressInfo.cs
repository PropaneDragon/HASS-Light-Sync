namespace LightSync.Core.Progress
{
    internal class ProgressInfo : IProgressInfo
    {
        private readonly Lock _progressPercentageLock = new();
        private readonly Lock _messageLock = new();

        private volatile bool _isIndeterminate;
        private double _progressPercentage;
        private string? _message;

        public double ProgressPercentage
        {
            get { lock(_progressPercentageLock) return _progressPercentage; }
            set
            {
                lock (_progressPercentageLock) { _progressPercentage = value; }
                IsIndeterminate = false;
                ProgressPercentageChanged?.Invoke(this, new(value));
            }
        }

        public string? Message
        {
            get { lock(_messageLock) return _message; }
            set
            {
                lock (_messageLock) { _message = value; }
                MessageChanged?.Invoke(this, new(value));
            }
        }

        public bool IsIndeterminate
        {
            get => _isIndeterminate;
            set
            {
                _isIndeterminate = value;
                IsIndeterminateChanged?.Invoke(this, new(value));
            }
        }

        public TaskCompletionSource CompletedTask { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public event EventHandler<EventArgs>? Completed;
        public event EventHandler<IsIndeterminateChangedEventArgs>? IsIndeterminateChanged;
        public event EventHandler<ProgressPercentageChangedEventArgs>? ProgressPercentageChanged;
        public event EventHandler<MessageChangedEventArgs>? MessageChanged;

        public ProgressInfo()
        {
        }

        public void Dispose()
        {
            Complete();
        }

        public void Complete()
        {
            ProgressPercentage = 100;

            CompletedTask.TrySetResult();
            Completed?.Invoke(this, new());
        }
    }
}
