using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using TvLight.Classes;
using TvLight.Devices;
using TvLight.Discovery;
using TvLight.Discovery.NMap;
using TvLight.Discovery.PingAndArp;
using TvLight.Hue;
using TvLight.Monitor;

namespace TvLight
{
    internal static class Program
    {
        static IPAddress SubnetAddress { get; } = IPAddress.Parse("192.168.1.0");

        static void Main()
        {
            var scanner = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? (IScanner)new PingAndArp() : (IScanner)new NMapScanner();

            Console.WriteLine($"Inital IP scan of {SubnetAddress} using {scanner.GetType().Name}");
            var sw = Stopwatch.StartNew();
            var online = scanner.Scan(SubnetAddress).Online;
            foreach (var entry in online)
                Console.WriteLine($"\t{entry.Ip}\t{entry.Mac}");
            Console.WriteLine($"Scan tool {sw.Elapsed}");
            Console.WriteLine();

            var devices = DeviceList.CreateFromFile("devices.json");
            if (devices.Devices.Count(d => d.Type == DeviceType.HueBridge) != 1)
                throw new Exception("Exactly one Hue Bridge must be defined in devices.json");
            devices.ProcessDiscovery(online);

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

            var switchable = groups.Concat<ISwitchable>(lights).ToList();
            devices.UpdateDevices(switchable);

            Console.WriteLine("TVs:");
            var tvs = new DeviceList(devices.Devices.Where(d => d.Type == DeviceType.Tv));
            tvs.ProcessDiscovery(online);
            foreach (var tv in tvs.Devices.Cast<Tv>())
                Console.WriteLine($"\t{tv.Name,-30}\t{tv.Mac}\t{tv.OnlineStatus.Status}\t{String.Join(',', tv.ControlNames)}");
            Console.WriteLine();

            var monitor = new DeviceMonitor(SubnetAddress, scanner, tvs);
            monitor.DeviceChanged += (sender, data) =>
                {
                    Console.WriteLine($"\t{DateTime.Now:T}\t{data.Device.Name}\t{data.FromStatus}\t->\t{data.ToStatus}");
                    if (!(data.Device is Tv tv))
                        return;

                    switch (data.ToStatus)
                    {
                        case OnlineStatusOnline.Online:
                            foreach (var sw in tv.ControlDevices)
                            {
                                sw.TurnOn();
                                Console.WriteLine($"\t{DateTime.Now:T}\t{sw.Name} turned on");
                            }

                            break;
                        case OnlineStatusOnline.Offline:
                            foreach (var sw in tv.ControlDevices)
                            {
                                sw.TurnOff();
                                Console.WriteLine($"\t{DateTime.Now:T}\t{sw.Name} turned off");
                            }

                            break;
                    }
                };
            monitor.Start();

            var waitEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (sender, args) => 
            { 
                waitEvent.Set();
                args.Cancel = true;
            };

            Console.WriteLine("DeviceMonitor started, Ctrl-C to stop");
            waitEvent.WaitOne();
            monitor.Stop();
            Console.WriteLine("DeviceMonitor stopped");
        }
    }
}
