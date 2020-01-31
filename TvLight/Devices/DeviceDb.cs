using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;

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
                Devices.Add(new Device(
                    Enum.Parse<DeviceType>(deviceJson.GetProperty("type").GetString()),
                    deviceJson.GetProperty("name").GetString(),
                    PhysicalAddress.Parse(deviceJson.GetProperty("mac").GetString())));
        }
    }
}
