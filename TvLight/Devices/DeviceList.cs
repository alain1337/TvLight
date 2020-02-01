﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using TvLight.Discovery;
using TvLight.Hue;

namespace TvLight.Devices
{
    public class DeviceList
    {
        public List<Device> Devices { get; }

        public DeviceList(IEnumerable<Device> devices)
        {
            Devices = devices.ToList();
        }

        public static DeviceList CreateFromFile(string filename)
        {
            var dl = new DeviceList();
            var jsonBytes = File.ReadAllBytes(filename);
            using var jsonDoc = JsonDocument.Parse(jsonBytes);
            foreach (var deviceJson in jsonDoc.RootElement.GetProperty("devices").EnumerateArray())
            {
                var type = deviceJson.GetProperty("type").GetString();
                var name = deviceJson.GetProperty("name").GetString();
                var mac = PhysicalAddress.Parse(deviceJson.GetProperty("mac").GetString());
                switch (type)
                {
                    case "HueBridge":
                        dl.Devices.Add(new HueBridge(name, mac));
                        break;
                    case "Tv":
                        dl.Devices.Add(new Tv(name, mac, deviceJson));
                        break;
                    default:
                        throw new Exception("Unknown device type: " + type);
                }
            }

            return dl;
        }

        public bool ProcessDiscovery(IEnumerable<MacOnline> online)
        {
            var unvisitedDevices = Devices.ToList();

            var changes = false;
            foreach (var on in online)
            foreach (var device in Devices.Where(d => d.Mac.Equals(on.Mac)))
            {
                unvisitedDevices.Remove(device);
                if (device.OnlineStatus.SignalOnline(on.Ip))
                    changes = true;
            }

            foreach (var device in unvisitedDevices)
                if (device.OnlineStatus.SignalOffline())
                    changes = true;
            return changes;
        }

        DeviceList()
        {
            Devices = new List<Device>();
        }
    }
}
