using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace TvLight.Discovery
{
    // Code taken from https://www.justinmklam.com/posts/2018/02/ping-sweeper/

    public static class PingerThreaded
    {
        public static PingResult PingAll(IPAddress subnet)
        {
            // NOTE: This is extremely lame but works for now...
            var baseIp = String.Join('.', subnet.ToString().Split('.').Take(3)) + ".";

            var pr = new PingResult();
            var sw = Stopwatch.StartNew();
            Parallel.For((long) 2, 255, i =>
            {
                using var p = new Ping();
                Interlocked.Increment(ref pr.IpsPinged);
                var ip = IPAddress.Parse(baseIp + i);
                var result = p.Send(ip, 200);
                if (result?.Status == IPStatus.Success)
                    pr.OnlineIps[ip] = true;
            });
            pr.Elapsed = sw.Elapsed;
            return pr;
        }
    }
}