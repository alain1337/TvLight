using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace TvLight.Hue
{
    public class Group : ISwitchable
    {
        public HueBridge Bridge { get; }
        public string Id { get; }
        public string Name { get; }
        public bool TurnedOn { get; private set; }

        public void TurnOn()
        {
            using var result = Bridge.TransactCommand(HttpMethod.Put, $"groups/{Id}/action", "{ \"on\": true }");
            TurnedOn = true;
        }

        public void TurnOff()
        {
            using var result = Bridge.TransactCommand(HttpMethod.Put, $"groups/{Id}/action", "{ \"on\": false }");
            TurnedOn = false;
        }

        public Group(HueBridge bridge, JsonProperty json)
        {
            Id = json.Name;
            Bridge = bridge;
            Name = json.Value.GetProperty("name").GetString();
            TurnedOn = json.Value.GetProperty("state").GetProperty("all_on").GetBoolean();
        }
    }
}
