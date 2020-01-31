using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace TvLight.Pinger
{
    public static class Arp
    {
        public static List<ArpEntry> GetAll()
        {
            var psi = new ProcessStartInfo
            {
                FileName = "arp.exe",
                Arguments = "-a",
                RedirectStandardOutput = true
            };
            var process = Process.Start(psi) ?? throw new Exception($"Failed to start {psi.FileName}");
            process.WaitForExit();
            if (process.ExitCode != 0)
                throw new Exception($"{psi.FileName} returned {process.ExitCode}");
            var output = process.StandardOutput.ReadToEnd();
            var result = new List<ArpEntry>();
            var types = new[] { "dynamic", "static" };
            foreach (var line in output.Split('\n'))
            {
                var fields = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (fields.Length < 3 || !types.Contains(fields[2]))
                    continue;
                result.Add(new ArpEntry(IPAddress.Parse(fields[0]), PhysicalAddress.Parse(fields[1].ToUpper()), fields[2]));
            }

            return result;
        }

        public static void PingSubnet(IPAddress subnet, IPAddress mask)
        {
            // NOTE: This is extremely lame but works for now...
            var baseIp = String.Join('.', subnet.ToString().Split('.').Take(3)) + ".";
            Parallel.For(1, 256, i =>
            {
                var address = baseIp + i;
                var ping = new Ping();
                ping.Send(address, 100);
            });
        }
    }

    public class ArpEntry : IpDevice
    {
        public string Type { get; }

        public ArpEntry(IPAddress ip, PhysicalAddress mac, string type) : base(ip, mac)
        {
            Type = type;
        }
    }
}
