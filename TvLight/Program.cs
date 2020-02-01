using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using TvLight.Classes;
using TvLight.Devices;
using TvLight.Discovery;
using TvLight.Hue;
using TvLight.Monitor;

namespace TvLight
{
    internal static class Program
    {
        static IPAddress SubnetAddress { get; } = IPAddress.Parse("192.168.1.0");

        static void Main()
        {
            var devices = DeviceList.CreateFromFile("devices.json");
            if (devices.Devices.Count(d => d.Type == DeviceType.HueBridge) != 1)
                throw new Exception("Exactly one Hue Bridge must be defines in devices.json");
            devices.ProcessDiscovery(MacDiscovery.DiscoverOnline(SubnetAddress));

            var hue = (HueBridge)devices.Devices.First(d => d.Type == DeviceType.HueBridge);
            Console.WriteLine("Bridge:");
            Console.WriteLine($"\t{hue.Name,-30}\t{hue.Mac}\t{hue.OnlineStatus.Ip}\t{hue.OnlineStatus.Status}");
            if (hue.OnlineStatus.Status != OnlineStatusOnline.Online)
                throw new Exception("Hue Bridge is not online");
            Console.WriteLine();

            var lights = hue.GetLights();
            Console.WriteLine("Lights:");
            foreach (var light in lights)
                Console.WriteLine($"\t{light.Name,-30}\t{light.TurnedOn}");
            Console.WriteLine();

            var groups = hue.GetGroups();
            Console.WriteLine("Groups:");
            foreach (var grp in groups)
                Console.WriteLine($"\t{grp.Name,-30}\t{grp.TurnedOn}");
            Console.WriteLine();

            Console.WriteLine("TVs:");
            var tvs = new DeviceList(devices.Devices.Where(d => d.Type == DeviceType.Tv));
            tvs.ProcessDiscovery(MacDiscovery.DiscoverOnline(SubnetAddress));
            foreach (var tv in tvs.Devices.Cast<Tv>())
                Console.WriteLine($"\t{tv.Name,-30}\t{tv.Mac}\t{tv.OnlineStatus.Status}\t{String.Join(',', tv.Controls)}");
            Console.WriteLine();

            Console.WriteLine("DeviceMonitor started, [Enter] to stop");
            var monitor = new DeviceMonitor(SubnetAddress, tvs);
            monitor.DeviceChanged += (sender, data) =>
                {
                    Console.WriteLine($"\t{DateTime.Now:T}\t{data.Device.Name}\t{data.FromStatus}\t->\t{data.ToStatus}");
                    if (data.Device is Tv tv)
                    {
                        switch (data.ToStatus)
                        {
                            case OnlineStatusOnline.Online:
                                foreach (var light in tv.Controls)
                                    groups.FirstOrDefault(l => l.Name == light)?.TurnOn();
                                break;
                            case OnlineStatusOnline.Offline:
                                foreach (var light in tv.Controls)
                                    groups.FirstOrDefault(l => l.Name == light)?.TurnOff();
                                break;
                        }
                    }
                };
            monitor.Start();

            Console.ReadLine();
            Console.WriteLine("Stopping DeviceMonitor");
            monitor.Stop();
        }
    }
}
