using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TvLight.Pinger
{
    // Code taken from https://www.justinmklam.com/posts/2018/02/ping-sweeper/

    public class Pinger
    {
        readonly IPAddress _subnet;

        public Pinger(IPAddress subnet)
        {
            _subnet = subnet;
        }

        public async Task<PingResult> PingAllAsync()
        {
            var tasks = new List<Task>();

            // NOTE: This is extremely lame but works for now...
            var baseIp = String.Join('.', _subnet.ToString().Split('.').Take(3)) + ".";
            var pr = new PingResult();
            var sw = Stopwatch.StartNew();
            for (var i = 2; i < 255 ; i++)
            {
                var p = new System.Net.NetworkInformation.Ping();
                var task = PingAndUpdateAsync(p, baseIp + i, pr);
                tasks.Add(task);
            }
            await Task.WhenAll(tasks).ContinueWith(t => { pr.Elapsed = sw.Elapsed; });

            return pr;
        }

        private async Task PingAndUpdateAsync(System.Net.NetworkInformation.Ping ping, string ip, PingResult pr)
        {
            Interlocked.Increment(ref pr.IpsPinged);
            var reply = await ping.SendPingAsync(ip, 500);
            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                Interlocked.Increment(ref pr.IpsSuccessful);
        }
    }

    public class PingResult
    {
        public int IpsPinged;
        public int IpsSuccessful;
        public TimeSpan Elapsed;
    }
}
