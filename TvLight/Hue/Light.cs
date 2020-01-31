using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace TvLight.Hue
{
    public class Light
    {
        public HueBridge Bridge { get; }
        public string Name { get; }
        public bool TurnedOn { get; private set; }

        public void TurnOn()
        {
            throw new NotImplementedException();
        }

        public void TurnOff()
        {
            throw new NotImplementedException();
        }

        public Light(HueBridge bridge, JsonProperty json)
        {
            Bridge = bridge;
            Name = json.Value.GetProperty("name").GetString();
            TurnedOn = json.Value.GetProperty("state").GetProperty("on").GetBoolean();
        }
    }
}
