using System.Drawing;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace LightSync.HomeAssistant.Commands
{
    internal class SetLightStateCommand : IHomeAssistantCommand
    {
        private readonly string _entityId;
        private readonly TimeSpan _transitionTime;

        private readonly bool? _on;
        private readonly Color? _colour;

        public HomeAssistantCommandType CommandType => HomeAssistantCommandType.NoResposne;

        private SetLightStateCommand(string entityId, TimeSpan transitionTime)
        {
            _entityId = entityId;
            _transitionTime = transitionTime;
        }

        public SetLightStateCommand(bool on, string entityId, TimeSpan transitionTime = default) : this(entityId, transitionTime)
        {
            _on = on;
        }

        public SetLightStateCommand(Color colour, string entityId, TimeSpan transitionTime = default) : this(entityId, transitionTime)
        {
            _colour = colour;
        }

        public SetLightStateCommand(bool on, Color colour, string entityId, TimeSpan transitionTime = default) : this(entityId, transitionTime)
        {
            _on = on;
            _colour = colour;
        }

        public JsonNode GetMessage(uint messageId)
        {
            var lightOffCommand = (_on.HasValue && !_on.Value) || (_colour.HasValue && _colour.Value.A <= 0);
            var service = lightOffCommand ? "turn_off" : "turn_on";

            var dataObject = new JsonObject()
            {
                ["transition"] = _transitionTime.TotalSeconds
            };

            var messageObject = new JsonObject()
            {
                ["id"] = messageId,
                ["type"] = "call_service",
                ["domain"] = "light",
                ["service"] = service,
                ["service_data"] = dataObject,
                ["target"] = new JsonObject()
                {
                    ["entity_id"] = _entityId
                }
            };

            if (lightOffCommand)
                return messageObject;

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

            return messageObject;
        }

        public void MessageReceived(JsonElement element)
        {
            //Unused - Fire only
        }
    }
}
