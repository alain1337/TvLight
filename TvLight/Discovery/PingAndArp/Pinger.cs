using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace TvLight.Discovery
{
    // Code taken from https://www.justinmklam.com/posts/2018/02/ping-sweeper/

    public static class Pinger
    {
        public static async Task<PingResult> PingAllAsync(IPAddress subnet)
        {
            var tasks = new List<Task>();

            // NOTE: This is extremely lame but works for now...
            var baseIp = String.Join('.', subnet.ToString().Split('.').Take(3)) + ".";
            var pr = new PingResult();
            var sw = Stopwatch.StartNew();
            for (var i = 2; i < 255 ; i++)
            {
                var task = PingAndUpdateAsync(baseIp + i, pr);
                tasks.Add(task);
            }
            await Task.WhenAll(tasks).ContinueWith(t => { pr.Elapsed = sw.Elapsed; });

            return pr;
        }

        static async Task PingAndUpdateAsync(string ip, PingResult pr)
        {
            using var ping = new System.Net.NetworkInformation.Ping();
            Interlocked.Increment(ref pr.IpsPinged);
            var reply = await ping.SendPingAsync(ip, 500);
            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                pr.OnlineIps[reply.Address] = true;
        }
    }

    public class PingResult
    {
        public readonly ConcurrentDictionary<IPAddress, bool> OnlineIps = new ConcurrentDictionary<IPAddress, bool>();
        public int IpsPinged;
        public TimeSpan Elapsed;
    }
}
