using System.Drawing;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace LightSync.HomeAssistant.Commands
{
    internal class SetLightStateCommand : IHomeAssistantCommand
    {
        private readonly bool? _on;
        private readonly Color? _colour;

        private string _entityId;

        public HomeAssistantCommandType CommandType => HomeAssistantCommandType.NoResposne;

        private SetLightStateCommand(string entityId)
        {
            _entityId = entityId;
        }

        public SetLightStateCommand(bool on, string entityId) : this(entityId)
        {
            _on = on;
        }

        public SetLightStateCommand(Color colour, string entityId) : this(entityId)
        {
            _colour = colour;
        }

        public SetLightStateCommand(bool on, Color colour, string entityId) : this(entityId)
        {
            _on = on;
            _colour = colour;
        }

        public JsonNode GetMessage(uint messageId)
        {
            var lightOffCommand = (_on.HasValue && !_on.Value) || (_colour.HasValue && _colour.Value.A <= 0);
            var service = lightOffCommand ? "turn_off" : "turn_on";

            var messageObject = new JsonObject()
            {
                ["id"] = messageId,
                ["type"] = "call_service",
                ["domain"] = "light",
                ["service"] = service,
                ["target"] = new JsonObject()
                {
                    ["entity_id"] = _entityId
                }
            };

            if (lightOffCommand)
                return messageObject;

            var dataObject = new JsonObject()
            {
                ["transition"] = _on == lightOffCommand ? 1 : 0
            };

            if (_colour.HasValue)
            {
                dataObject["brightness"] = _colour.Value.A;
                dataObject["rgb_color"] = new JsonArray()
                {
                    _colour.Value.R,
                    _colour.Value.G,
                    _colour.Value.B
                };
            }

            messageObject["service_data"] = dataObject;

            return messageObject;
        }

        public void MessageReceived(JsonElement element)
        {
        }
    }
}
