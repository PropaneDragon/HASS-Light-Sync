namespace LightSync.UI.Progress
{
    internal enum ProgressType
    {
        Indeterminate,
        Normal
    }

    internal interface IProgressInfoFactory
    {
        IProgressInfo CreateProgressInfo(string? message = null, ProgressType progressType = ProgressType.Indeterminate);
        IProgressInfo CreateAndShowProgressInfo(string? message = null, ProgressType progress = ProgressType.Indeterminate);
    }
}
