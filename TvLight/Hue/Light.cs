using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace TvLight.Hue
{
    [DebuggerDisplay("Light {Name} {TurnedOn}")]
    public class Light : ISwitchable
    {
        public HueBridge Bridge { get; }
        public string Id { get; }
        public string Name { get; }
        public bool TurnedOn { get; private set; }

        public void TurnOn()
        {
            SetState(true);
            TurnedOn = true;
        }

        public void TurnOff()
        {
            SetState(false);
            TurnedOn = false;
        }

        void SetState(bool on)
        {
            using var result = Bridge.TransactCommand(HttpMethod.Put, $"lights/{Id}/state", "{ \"on\": " + on.ToString().ToLower() + " }");
            // Console.WriteLine(result.RootElement.ToString());
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
