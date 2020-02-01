﻿using System;
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
            foreach (var device in devices.Devices)
                device.GetStatus();
            var hue = (HueBridge)devices.Devices.First(d => d.Type == DeviceType.HueBridge);
            Console.WriteLine("Bridge:");
            Console.WriteLine($"\t{hue.Name,-30}\t{hue.Ip.Mac}\t{hue.Ip.Ip}\t{hue.GetStatus()}");
            if (hue.Ip.Status != IpDeviceStatus.Online)
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
            var tvs = devices.Devices.Where(d => d.Type == DeviceType.Tv).Cast<Tv>().ToList();
            foreach (var tv in tvs)
                Console.WriteLine($"\t{tv.Name,-30}\t{tv.Mac}\t{tv.GetStatus()}\t{String.Join(',', tv.Controls)}");
            Console.WriteLine();

            Console.WriteLine("Starting DeviceMonitor, [Enter] to stop");
            var monitor = new DeviceMonitor(tvs);
            monitor.DeviceChanged += (sender, data) =>
                {
                    Console.WriteLine($"\t{DateTime.Now:T}\t{data.Device.Name}\t{data.PreviousStatus}\t->\t{data.CurrentStatus}");
                    if (data.Device is Tv tv)
                    {
                        switch (data.CurrentStatus)
                        {
                            case IpDeviceStatus.Online:
                                foreach (var light in tv.Controls)
                                    groups.FirstOrDefault(l => l.Name == light)?.TurnOn();
                                break;
                            case IpDeviceStatus.Offline:
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
