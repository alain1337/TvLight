using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using TvLight.Pinger;

namespace TvLight.Devices
{
    public class Device
    {
        public string Name { get; }
        public PhysicalAddress Mac { get; }
        public IpDevice Ip { get; private set; }

        public Device(string name, PhysicalAddress mac)
        {
            Name = name;
            Mac = mac;
        }

        public IpDeviceStatus GetStatus()
        {
            if (Ip != null && Ip.CheckOnline() == IpDeviceStatus.Online)
                return IpDeviceStatus.Online;

            Ip = null;
            var ip = ArpMonitor.Instance.TryGetIp(Mac);
            if (ip == null)
                return IpDeviceStatus.Offline;
            Ip = new IpDevice(ip, Mac);
            return Ip.CheckOnline();
        }
    }
}
