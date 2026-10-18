using System.Text.Json;
using System.Text.Json.Nodes;

namespace LightSync.HomeAssistant.Commands
{
    internal class GetStatesHomeAssistantCommand : IHomeAssistantCommand
    {
        public bool StatesReceived { get; private set; } = false;
        public List<JsonElement> StateElements { get; } = [];
        public HomeAssistantCommandType CommandType => HomeAssistantCommandType.OneShot;

        public JsonNode GetMessage(uint messageId)
        {
            return new JsonObject()
            {
                ["id"] = messageId,
                ["type"] = "get_states"
            };
        }

        public void MessageReceived(JsonElement element)
        {
            if (!element.TryGetProperty("success", out var successElement) || !element.TryGetProperty("result", out var resultElement))
                return;

            var success = successElement.GetBoolean();
            var resultLength = resultElement.GetArrayLength();

            if (resultLength < 1)
                return;

            StateElements.Clear();
            StateElements.AddRange([.. resultElement.EnumerateArray()]);

            StatesReceived = true;
        }
    }
}
