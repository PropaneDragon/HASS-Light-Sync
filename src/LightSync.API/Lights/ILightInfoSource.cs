namespace LightSync.API.Lights
{
    public interface ILightInfoSource : IAsyncDisposable
    {
        IReadOnlyList<ILightInfo> Lights { get; }

        Task InitialiseAsync(CancellationToken cancellationToken = default);
    }
}
