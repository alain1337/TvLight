using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using TvLight.Devices;
using TvLight.Pinger;

namespace TvLight.Hue
{
    public class HueBridge
    {
        public Device Device { get; }

        public HueBridge(Device device)
        {
            Device = device;
        }
    }
}
