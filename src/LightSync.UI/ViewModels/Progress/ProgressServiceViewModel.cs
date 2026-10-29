using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using LightSync.Core.Progress;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace LightSync.UI.ViewModels.Progress
{
    internal partial class ProgressServiceViewModel : ObservableObject, IDisposable
    {
        public IProgressService AssociatedProcessService { get; }

        public bool HasMultipleProgress => ItemsInProgress.Count > 1;

        [ObservableProperty]
        public partial bool IsIndeterminate { get; private set; }

        [ObservableProperty]
        public partial double OverallProgressPercentage { get; private set; }

        [ObservableProperty]
        public partial string Title { get; private set; }

        public ObservableCollection<ProgressInfoViewModel> ItemsInProgress { get; } = [];

        public ProgressServiceViewModel(IProgressService progressService)
        {
            AssociatedProcessService = progressService;

            foreach (var item in progressService.ItemsInProgress)
            {
                ItemsInProgress.Add(new(item));
            }

            IsIndeterminate = progressService.IsIndeterminate;
            OverallProgressPercentage = progressService.OverallProgressPercentage;
            Title = progressService.Title;

            HookEvents();
        }

        public void Dispose()
        {
            UnhookEvents();

            foreach (var item in ItemsInProgress)
            {
                item.Dispose();
            }
        }

        private void HookEvents()
        {
            ItemsInProgress.CollectionChanged += ItemsInProgress_CollectionChanged;

            AssociatedProcessService.ProgressInfoAdded += AssociatedProcessService_ProgressInfoAdded;
            AssociatedProcessService.ProgressInfoRemoved += AssociatedProcessService_ProgressInfoRemoved;
            AssociatedProcessService.IsIndeterminateChanged += AssociatedProcessService_IsIndeterminateChanged;
            AssociatedProcessService.TitleChanged += AssociatedProcessService_TitleChanged;
            AssociatedProcessService.OverallProgressPercentageChanged += AssociatedProcessService_OverallProgressPercentageChanged;
        }

        private void UnhookEvents()
        {
            ItemsInProgress.CollectionChanged -= ItemsInProgress_CollectionChanged;

            AssociatedProcessService.ProgressInfoAdded -= AssociatedProcessService_ProgressInfoAdded;
            AssociatedProcessService.ProgressInfoRemoved -= AssociatedProcessService_ProgressInfoRemoved;
            AssociatedProcessService.IsIndeterminateChanged -= AssociatedProcessService_IsIndeterminateChanged;
            AssociatedProcessService.TitleChanged -= AssociatedProcessService_TitleChanged;
            AssociatedProcessService.OverallProgressPercentageChanged -= AssociatedProcessService_OverallProgressPercentageChanged;
        }

        private void ItemsInProgress_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HasMultipleProgress));
        }

        private void AssociatedProcessService_OverallProgressPercentageChanged(object? sender, ProgressPercentageChangedEventArgs e)
        {
            Dispatcher.UIThread.Post(() => OverallProgressPercentage = e.ProgressPercentage);
        }

        private void AssociatedProcessService_ProgressInfoAdded(object? sender, ProgressInfoEventArgs e)
        {
            Dispatcher.UIThread.Post(() => ItemsInProgress.Add(new(e.ProgressInfo)));
        }

        private void AssociatedProcessService_IsIndeterminateChanged(object? sender, IsIndeterminateChangedEventArgs e)
        {
            Dispatcher.UIThread.Post(() => IsIndeterminate = e.IsIndeterminate);
        }

        private void AssociatedProcessService_TitleChanged(object? sender, MessageChangedEventArgs e)
        {
            Dispatcher.UIThread.Post(() => Title = e.Message ?? "Processing");
        }

        private void AssociatedProcessService_ProgressInfoRemoved(object? sender, ProgressInfoEventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                var associatedViewModel = ItemsInProgress.FirstOrDefault(item => item.AssociatedProgressInfo == e.ProgressInfo);
                if (associatedViewModel == null)
                    return;

                ItemsInProgress.Remove(associatedViewModel);

                associatedViewModel.Dispose();
            });
        }
    }
}
