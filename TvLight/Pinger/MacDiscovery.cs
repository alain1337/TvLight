using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace TvLight.Pinger
{
    public static class MacDiscovery
    {
        public static List<MacOnline> DiscoverOnline(IPAddress subnet)
        {
            var ips = Pinger.PingAllAsync(subnet).Result;
            var arp = Arp.GetAll();
            return ips.OnlineIps.Keys
                .Where(ip => arp.Exists(ae => Equals(ae.Ip, ip)))
                .Select(ip => new MacOnline(arp.First(ae => Equals(ae.Ip, ip)).Mac, ip))
                .ToList();
        }
    }

    public class MacOnline
    {
        public MacOnline(PhysicalAddress mac, IPAddress ip)
        {
            Mac = mac;
            Ip = ip;
        }

        public PhysicalAddress Mac { get; }
        public IPAddress Ip { get; }
    }
}
