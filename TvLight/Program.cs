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

        static void Main()
        {
            var db = new DeviceDb();
            if (db.Devices.Count(d => d.Type == DeviceType.HueBridge) != 1)
                throw new Exception("Exactly one Hue Bridge must be defines in devices.json");
            var hue = new HueBridge(db.Devices.First(d => d.Type == DeviceType.HueBridge));
            Console.WriteLine("Bridge:");
            Console.WriteLine($"\t{hue.Device.Name}\t{hue.Device.GetStatus()}");
            if (hue.Device.Ip.Status != IpDeviceStatus.Online)
                throw new Exception("Hue Bridge is not online");
            Console.WriteLine();

            Console.WriteLine("TVs:");
            foreach (var tv in db.Devices.Where(d => d.Type == DeviceType.Tv))
                Console.WriteLine($"\t{tv.Name}\t{tv.GetStatus()}");
            Console.WriteLine();
        }
    }
}
