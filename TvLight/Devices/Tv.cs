using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;

namespace TvLight.Devices
{
    class Tv : Device
    {
        public string[] Controls { get; private set; } = new string[0];
        public Tv(string name, PhysicalAddress mac, JsonElement json) : base(DeviceType.Tv, name, mac)
        {
            if (!json.TryGetProperty("controls", out var controls))
                return;
            Controls = controls.ValueKind == JsonValueKind.Array ? controls.EnumerateArray().Select(e => e.GetString()).ToArray() : new[] { controls.GetString() };
        }
    }
}
