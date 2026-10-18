using System.Text.Json;
using System.Text.Json.Nodes;

namespace LightSync.HomeAssistant.Commands
{
    internal enum HomeAssistantCommandType
    {
        OneShot,
        Continuous
    }

    internal interface IHomeAssistantCommand
    {
        HomeAssistantCommandType CommandType { get; }

        JsonNode GetMessage(uint messageId);
        void MessageReceived(JsonElement element);
    }
}
