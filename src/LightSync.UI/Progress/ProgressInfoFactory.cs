namespace LightSync.UI.Progress
{
    internal class ProgressInfoFactory : IProgressInfoFactory
    {
        private readonly IProgressService _progressService;

        public ProgressInfoFactory(IProgressService progressService)
        {
            _progressService = progressService;
        }

        public IProgressInfo CreateProgressInfo(string? message = null, ProgressType progressType = ProgressType.Indeterminate) => new ProgressInfo()
        {
            Message = message,
            IsIndeterminate = progressType == ProgressType.Indeterminate
        };

        public IProgressInfo CreateAndShowProgressInfo(string? message = null, ProgressType progressType = ProgressType.Indeterminate)
        {
            var progressInfo = CreateProgressInfo(message, progressType);
            _progressService.ShowProgress(progressInfo);

            return progressInfo;
        }
    }
}
