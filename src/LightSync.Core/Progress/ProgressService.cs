namespace LightSync.Core.Progress
{
    public class ProgressService : IProgressService
    {
        private static readonly Lock _itemsInProgressLock = new();
        private static readonly List<IProgressInfo> _itemsInProgress = [];

        public bool IsIndeterminate => ItemsInProgress.All(item => item.IsIndeterminate);

        public double OverallProgressPercentage
        {
            get
            {
                var itemsInProgress = ItemsInProgress.Where(item => !item.IsIndeterminate).ToList();
                if (itemsInProgress.Count <= 0) return 0;
                return itemsInProgress.Sum(item => item.ProgressPercentage) / itemsInProgress.Count;
            }
        }

        public string Title
        {
            get
            {
                var itemsInProgress = ItemsInProgress;
                return itemsInProgress.Count > 1 ? "Processing" : itemsInProgress.FirstOrDefault()?.Message ?? "Processing";
            }
        }

        public IReadOnlyList<IProgressInfo> ItemsInProgress
        {
            get { lock (_itemsInProgressLock) { return [.. _itemsInProgress]; } }
        }

        public event EventHandler<ProgressInfoEventArgs>? ProgressInfoAdded;
        public event EventHandler<ProgressInfoEventArgs>? ProgressInfoRemoved;
        public event EventHandler<IsIndeterminateChangedEventArgs>? IsIndeterminateChanged;
        public event EventHandler<ProgressPercentageChangedEventArgs>? OverallProgressPercentageChanged;
        public event EventHandler<MessageChangedEventArgs>? TitleChanged;

        public ProgressService()
        {

        }

        public void ShowProgress(IProgressInfo progressInfo)
        {
            lock (_itemsInProgressLock)
            {
                _itemsInProgress.Add(progressInfo);
            }

            HookEvents(progressInfo);

            ProgressInfoAdded?.Invoke(this, new(progressInfo));
            OverallProgressPercentageChanged?.Invoke(this, new(OverallProgressPercentage));
            IsIndeterminateChanged?.Invoke(this, new(IsIndeterminate));
            TitleChanged?.Invoke(this, new(Title));
        }

        public void Dispose()
        {
            foreach (var item in ItemsInProgress)
            {
                UnhookEvents(item);
            }
        }

        private void HookEvents(IProgressInfo progressInfo)
        {
            progressInfo.ProgressPercentageChanged += ProgressInfo_ProgressPercentageChanged;
            progressInfo.IsIndeterminateChanged += ProgressInfo_IsIndeterminateChanged;
            progressInfo.Completed += ProgressInfo_Completed;

            if (progressInfo.CompletedTask.Task.IsCompleted)
                RemoveAndUnhookEvents(progressInfo);
        }

        private void UnhookEvents(IProgressInfo progressInfo)
        {
            progressInfo.ProgressPercentageChanged -= ProgressInfo_ProgressPercentageChanged;
            progressInfo.IsIndeterminateChanged -= ProgressInfo_IsIndeterminateChanged;
            progressInfo.Completed -= ProgressInfo_Completed;
        }

        private void RemoveAndUnhookEvents(IProgressInfo progressInfo)
        {
            lock (_itemsInProgressLock)
            {
                _itemsInProgress.Remove(progressInfo);
            }

            UnhookEvents(progressInfo);

            ProgressInfoRemoved?.Invoke(this, new(progressInfo));
        }

        private void ProgressInfo_ProgressPercentageChanged(object? sender, ProgressPercentageChangedEventArgs e)
        {
            OverallProgressPercentageChanged?.Invoke(this, new(OverallProgressPercentage));
        }

        private void ProgressInfo_IsIndeterminateChanged(object? sender, IsIndeterminateChangedEventArgs e)
        {
            IsIndeterminateChanged?.Invoke(this, new(IsIndeterminate));
        }

        private void ProgressInfo_Completed(object? sender, EventArgs e)
        {
            if (sender is not IProgressInfo progressInfo)
                throw new InvalidOperationException($"Attempted to complete an invalid {typeof(IProgressInfo)} object.");

            RemoveAndUnhookEvents(progressInfo);

            OverallProgressPercentageChanged?.Invoke(this, new(OverallProgressPercentage));
            IsIndeterminateChanged?.Invoke(this, new(IsIndeterminate));
            TitleChanged?.Invoke(this, new(Title));
        }
    }
}
