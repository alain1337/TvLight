using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using TvLight.Devices;
using TvLight.Pinger;

namespace TvLight.Hue
{
    public class HueBridge : Device
    {
        public HueBridge(string name, PhysicalAddress mac) : base(name, mac)
        {
        }
    }
}
