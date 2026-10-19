using System.Text.Json;
using System.Text.Json.Nodes;

namespace LightSync.HomeAssistant.Commands
{
    internal enum HomeAssistantCommandType
    {
        NoResposne,
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
