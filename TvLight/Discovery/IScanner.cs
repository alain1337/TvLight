using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace TvLight.Discovery
{
    public interface IScanner
    {
        ScannerResult Scan(IPAddress subnet);
    }

    public class ScannerResult
    {
        public List<IpAndMac> Online { get; } = new List<IpAndMac>();
    }

    public class IpAndMac
    {
        public IpAndMac(IPAddress ip, PhysicalAddress mac = null)
        {
            Ip = ip;
            Mac = mac;
        }

        public IPAddress Ip { get; }
        public PhysicalAddress Mac { get; set; }
    }
}
