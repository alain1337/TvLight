using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace TvLight.Hue
{
    [DebuggerDisplay("Group {Name} {TurnedOn}")]
    public class Group : ISwitchable
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
            using var result = Bridge.TransactCommand(HttpMethod.Put, $"groups/{Id}/action", "{ \"on\": " + on.ToString().ToLower() + " }");
            // Console.WriteLine(result.RootElement.ToString());
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
