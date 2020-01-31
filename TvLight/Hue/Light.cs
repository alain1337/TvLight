using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace TvLight.Hue
{
    public class Light
    {
        public HueBridge Bridge { get; }
        public string Id { get; }
        public string Name { get; }
        public bool TurnedOn { get; private set; }

        public void TurnOn()
        {
            Bridge.TransactCommand(HttpMethod.Put, $"lights/{Id}/state", "{ \"on\": true }");
        }

        public void TurnOff()
        {
            Bridge.TransactCommand(HttpMethod.Put, $"lights/{Id}/state", "{ \"on\": false }");
        }

        public Light(HueBridge bridge, JsonProperty json)
        {
            Id = json.Name;
            Bridge = bridge;
            Name = json.Value.GetProperty("name").GetString();
            TurnedOn = json.Value.GetProperty("state").GetProperty("on").GetBoolean();
        }
    }
}
