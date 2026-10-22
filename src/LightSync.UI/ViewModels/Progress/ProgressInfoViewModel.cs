using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using LightSync.UI.Progress;
using System;

namespace LightSync.UI.ViewModels.Progress
{
    internal partial class ProgressInfoViewModel : ObservableObject, IDisposable
    {
        public IProgressInfo AssociatedProgressInfo { get; }

        [ObservableProperty]
        public partial bool IsComplete { get; private set; }

        [ObservableProperty]
        public partial bool IsIndeterminate { get; private set; }

        [ObservableProperty]
        public partial double ProgressPercentage { get; private set; }

        [ObservableProperty]
        public partial string? Message { get; private set; }

        public ProgressInfoViewModel(IProgressInfo progressInfo)
        {
            AssociatedProgressInfo = progressInfo;

            IsIndeterminate = progressInfo.IsIndeterminate;
            ProgressPercentage = progressInfo.ProgressPercentage;
            Message = progressInfo.Message;

            HookEvents();
        }

        public void Dispose()
        {
            UnhookEvents();
        }

        private void HookEvents()
        {
            AssociatedProgressInfo.MessageChanged += AssociatedProgressInfo_MessageChanged;
            AssociatedProgressInfo.ProgressPercentageChanged += AssociatedProgressInfo_ProgressPercentageChanged;
            AssociatedProgressInfo.IsIndeterminateChanged += AssociatedProgressInfo_IsIndeterminateChanged;
            AssociatedProgressInfo.Completed += AssociatedProgressInfo_Completed;
        }

        private void UnhookEvents()
        {
            AssociatedProgressInfo.MessageChanged -= AssociatedProgressInfo_MessageChanged;
            AssociatedProgressInfo.ProgressPercentageChanged -= AssociatedProgressInfo_ProgressPercentageChanged;
            AssociatedProgressInfo.IsIndeterminateChanged -= AssociatedProgressInfo_IsIndeterminateChanged;
            AssociatedProgressInfo.Completed -= AssociatedProgressInfo_Completed;
        }

        private void AssociatedProgressInfo_Completed(object? sender, EventArgs e)
        {
            Dispatcher.UIThread.Post(() => IsComplete = true);
        }

        private void AssociatedProgressInfo_ProgressPercentageChanged(object? sender, ProgressPercentageChangedEventArgs e)
        {
            Dispatcher.UIThread.Post(() => ProgressPercentage = e.ProgressPercentage);
        }

        private void AssociatedProgressInfo_MessageChanged(object? sender, MessageChangedEventArgs e)
        {
            Dispatcher.UIThread.Post(() => Message = e.Message);
        }

        private void AssociatedProgressInfo_IsIndeterminateChanged(object? sender, IsIndeterminateChangedEventArgs e)
        {
            Dispatcher.UIThread.Post(() => IsIndeterminate = e.IsIndeterminate);
        }
    }
}
