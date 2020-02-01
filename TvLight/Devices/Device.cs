using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;

namespace TvLight.Devices
{
    [DebuggerDisplay("{Name}")]
    public class Device
    {
        public DeviceType Type { get; }
        public string Name { get; }
        public PhysicalAddress Mac { get; }
        public OnlineStatus OnlineStatus { get; } = new OnlineStatus();

        public Device(DeviceType type, string name, PhysicalAddress mac)
        {
            Type = type;
            Name = name;
            Mac = mac;
        }
    }

    public enum DeviceType
    {
        HueBridge,
        Tv
    }
}
