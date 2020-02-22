using System.Linq;
using System.Net;

namespace TvLight.Discovery.PingAndArp
{
    public class PingAndArp : IScanner
    {
        public ScannerResult Scan(IPAddress subnet)
        {
            var result = new ScannerResult();

            //var ips = Pinger.PingAllAsync(subnet).Result;
            var ips = PingerThreaded.PingAll(subnet);
            //Console.WriteLine($"PingAll took {ips.Elapsed}");
            result.Online.AddRange(Arp.GetAll()
                .Where(ae => ips.OnlineIps.ContainsKey(ae.Ip))
                .Select(ae => new IpAndMac(ips.OnlineIps.First(ip => Equals(ae.Ip, ip.Key)).Key, ae.Mac)));
            
            return result;
        }
    }
}
