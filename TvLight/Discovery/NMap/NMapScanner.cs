using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace TvLight.Discovery.NMap
{
    public class NMapScanner : IScanner
    {
        public ScannerResult Scan(IPAddress subnet)
        {
            // Step 1: nmap is only used to scan the range. It actually doesn't see my LG TV
            // If issue is found, use sudo for nmap so it gets the MAC

            var psi = new ProcessStartInfo
                {
                    FileName = "nmap",
                    Arguments = $"-sP {subnet}/24",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true
            };
            using var process = Process.Start(psi) ?? throw new Exception($"Failed to start {psi.FileName}");
            process.WaitForExit();
            var output = process.StandardOutput.ReadToEnd().Split(Environment.NewLine);

            /*
            var result = new ScannerResult();
            var hostRe = new Regex(@"^Nmap scan report for (?<name>\S+) \((?<ip>\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\)");
            var macRe = new Regex(@"^MAC Ip: (?<mac>(?:[0-9A-F]{2}:){5}[0-9A-F]{2})");
            IpAndMac entry = null;
            foreach (var line in output)
            {
                var ma = hostRe.Match(line);
                if (ma.Success)
                {
                    if (entry != null)
                        result.Online.Add(entry);
                    entry = new IpAndMac(IPAddress.Parse(ma.Groups["ip"].Value));
                }

                if (entry == null)
                    continue;
                ma = macRe.Match(line);
                if (ma.Success)
                    entry.Mac = PhysicalAddress.Parse(ma.Groups["mac"].Value.ToUpper().Replace(':', '-'));
            }

            if (entry?.Mac != null)
                result.Online.Add(entry);

            return result;
            */

            // Step 2: Get arp entries
            var arp = Arp.GetAll();

            // Stop 3: Ping all arp entries
            var result = new ScannerResult();
            using var ping = new Ping();
            foreach (var entry in arp)
                if (ping.Send(entry.Ip, 200)?.Status == IPStatus.Success)
                    result.Online.Add(entry);
            return result;
        }
    }
}
