using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace TvLight.Discovery
{
    public static class MacDiscovery
    {
        public static List<MacOnline> DiscoverOnline(IPAddress subnet)
        {
            var ips = Pinger.PingAllAsync(subnet).Result;
            var arp = Arp.GetAll();
            return arp
                .Where(ae => ips.OnlineIps.ContainsKey(ae.Ip))
                .Select(ae => new MacOnline(ae.Mac, ips.OnlineIps.First(ip => Equals(ae.Ip, ip.Key)).Key))
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
