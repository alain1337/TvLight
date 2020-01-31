using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using TvLight.Hue;

namespace TvLight.Devices
{
    public class DeviceDb
    {
        public List<Device> Devices { get; } = new List<Device>();

        public DeviceDb()
        {
            var jsonBytes = File.ReadAllBytes("devices.json");
            using var jsonDoc = JsonDocument.Parse(jsonBytes);
            foreach (var deviceJson in jsonDoc.RootElement.GetProperty("devices").EnumerateArray())
            {
                var type = deviceJson.GetProperty("type").GetString();
                var name = deviceJson.GetProperty("name").GetString();
                var mac = PhysicalAddress.Parse(deviceJson.GetProperty("mac").GetString());
                switch (type)
                {
                    case "HueBridge":
                        Devices.Add(new HueBridge(name, mac));
                        break;
                    case "Tv":
                        Devices.Add(new Device(DeviceType.Tv, name, mac));
                        break;
                    default:
                        throw new Exception("Unknown device type: " + type);
                }
            }
        }
    }
}
