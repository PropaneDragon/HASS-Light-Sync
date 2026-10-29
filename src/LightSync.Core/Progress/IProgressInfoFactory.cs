namespace LightSync.Core.Progress
{
    public enum ProgressType
    {
        Indeterminate,
        Normal
    }

    public interface IProgressInfoFactory
    {
        IProgressInfo CreateProgressInfo(string? message = null, ProgressType progressType = ProgressType.Indeterminate);
        IProgressInfo CreateAndShowProgressInfo(string? message = null, ProgressType progress = ProgressType.Indeterminate);
    }
}
