using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace TvLight.Discovery
{
    public static class Arp
    {
        public static List<ArpEntry> GetAll()
        {
            var isWin = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            var psi = isWin ?
                new ProcessStartInfo
                {
                    FileName = "arp.exe",
                    Arguments = "-a",
                    RedirectStandardOutput = true
                } :
                new ProcessStartInfo
                {
                    FileName = "arp",
                    Arguments = "-e",
                    RedirectStandardOutput = true
                };
            var process = Process.Start(psi) ?? throw new Exception($"Failed to start {psi.FileName}");
            process.WaitForExit();
            if (process.ExitCode != 0)
                throw new Exception($"{psi.FileName} returned {process.ExitCode}");
            var output = process.StandardOutput.ReadToEnd();
            var result = new List<ArpEntry>();
            foreach (var line in output.Split('\n').Skip(isWin ? 3 : 1))
            {
                if (isWin)
                {
                    var fields = Regex.Split(line, @"\s+").Where(s => !String.IsNullOrWhiteSpace(s)).ToArray();
                    if (fields.Length < 3)
                        continue;
                    result.Add(new ArpEntry(IPAddress.Parse(fields[0]), PhysicalAddress.Parse(fields[1].ToUpper())));
                }

                {
                    var ma = BsdArpRe.Match(line);
                    if (!ma.Success)
                        continue;
                    result.Add(new ArpEntry(IPAddress.Parse(ma.Groups["ip"].Value), PhysicalAddress.Parse(ma.Groups["mac"].Value)));
                }
            }

            return result;
        }

        static readonly Regex BsdArpRe = new Regex(@"\((?<ip>[0-9.]+).*\s(?<mac>[0-9a-f]{2}(?::[0-9a-f]{2}){5})");
    }

    public class ArpEntry
    {
        public IPAddress Ip { get; }
        public PhysicalAddress Mac { get; }

        public ArpEntry(IPAddress ip, PhysicalAddress mac)
        {
            Ip = ip;
            Mac = mac;
        }
    }
}
