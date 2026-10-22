using System;
using System.Collections.Generic;

namespace LightSync.UI.Progress
{
    internal interface IProgressService : IDisposable
    {
        bool IsIndeterminate { get; }
        double OverallProgressPercentage { get; }
        string Title { get; }

        IReadOnlyList<IProgressInfo> ItemsInProgress { get; }

        event EventHandler<ProgressInfoEventArgs>? ProgressInfoAdded;
        event EventHandler<ProgressInfoEventArgs>? ProgressInfoRemoved;
        event EventHandler<IsIndeterminateChangedEventArgs>? IsIndeterminateChanged;
        event EventHandler<ProgressPercentageChangedEventArgs>? OverallProgressPercentageChanged;
        event EventHandler<MessageChangedEventArgs>? TitleChanged;

        void ShowProgress(IProgressInfo progressInfo);
    }

    internal sealed class ProgressInfoEventArgs(IProgressInfo progressInfo) : EventArgs
    {
        public IProgressInfo ProgressInfo { get; } = progressInfo;
    }
}
