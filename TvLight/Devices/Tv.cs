using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using TvLight.Hue;

namespace TvLight.Devices
{
    [DebuggerDisplay("TV {Name}")]
    class Tv : Device
    {
        public string[] ControlNames { get; private set; } = new string[0];
        public List<ISwitchable> ControlDevices { get; } = new List<ISwitchable>();

        public void UpdateControls(List<ISwitchable> controls)
        {
            foreach (var name in ControlNames)
            {
                var control = controls.FirstOrDefault(s => String.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
                if (control != null)
                    ControlDevices.Add(control);
                else
                    Console.WriteLine("Warning: Group or light not found: " + name);
            }
        }

        public Tv(string name, PhysicalAddress mac, JsonElement json) : base(DeviceType.Tv, name, mac)
        {
            if (!json.TryGetProperty("controls", out var controls))
                return;
            ControlNames = controls.ValueKind == JsonValueKind.Array ? controls.EnumerateArray().Select(e => e.GetString()).ToArray() : new[] { controls.GetString() };
        }
    }
}
