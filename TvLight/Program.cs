using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using TvLight.Classes;
using TvLight.Devices;
using TvLight.Hue;
using TvLight.Pinger;

namespace TvLight
{
    internal static class Program
    {
        static IPAddress SubnetAddress { get; } = IPAddress.Parse("192.168.1.0");
        static IPAddress SubnetMask { get; } = IPAddress.Parse("255.255.255.0");
        public static readonly Dictionary<PhysicalAddress, string> KnownMacs = new Dictionary<PhysicalAddress, string> {
            { PhysicalAddress.Parse("EC-B5-FA-18-6B-9D"), "Hue Bridge"},
            { PhysicalAddress.Parse("20-3D-BD-F0-DC-60"), "TV Schlafzimmer" },
            { PhysicalAddress.Parse("00-05-CD-25-00-81"), "TV Wohnzimmer" }
        };

        static void Main()
        {
            var hueMac = KnownMacs.First();
            var hue = new HueBridge(hueMac.Value, hueMac.Key);
            Console.WriteLine($"{hue.Name}\t{hue.GetStatus()}");
            if (hue.Ip.Status != IpDeviceStatus.Online)
                throw new Exception("Hue Bridge is not online");

            var tvs = KnownMacs
                .Where(kvp => kvp.Value.StartsWith("TV "))
                .Select(kvp => new Device(kvp.Value, kvp.Key))
                .OrderBy(d => d.Name)
                .ToList();

            Console.WriteLine("TVs:");
            foreach (var tv in tvs)
                Console.WriteLine($"\t{tv.Name}\t{tv.GetStatus()}");
        }
    }
}
