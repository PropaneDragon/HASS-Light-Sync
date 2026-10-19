using LightSync.HomeAssistant.Commands;

namespace LightSync.HomeAssistant
{
    internal interface IConnectionManager
    {
        bool IsConnected { get; }

        Task<bool> ConnectAsync(CancellationToken cancellationToken = default);
        Task AddCommandAsync(IHomeAssistantCommand command, CancellationToken cancellationToken = default);

        bool RemoveCommand(IHomeAssistantCommand command);
        bool RemoveCommand(uint commandId);
    }
}