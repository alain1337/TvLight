using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using TvLight.Hue;

namespace TvLight.Devices
{
    public class DeviceList
    {
        public List<Device> Devices { get; } = new List<Device>();

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

        DeviceList()
        {
        }
    }
}
