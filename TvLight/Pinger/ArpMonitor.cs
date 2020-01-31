using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace TvLight.Pinger
{
    public class ArpMonitor
    {
        public IPAddress TryGetIp(PhysicalAddress mac)
        {
            lock (_arpLock)
            {
                CondRefresh();
                return _arpEntries.FirstOrDefault(ae => ae.Mac.Equals(mac))?.Ip;
            }
        }

        readonly object _arpLock = new object();
        List<ArpEntry> _arpEntries;
        DateTime? _arpUpdated;
        readonly TimeSpan _refreshAfter = TimeSpan.FromSeconds(1);

        void CondRefresh()
        {
            if (_arpUpdated.HasValue && _arpUpdated > DateTime.Now.Add(-_refreshAfter))
                return;
            _arpEntries = Arp.GetAll();
            _arpUpdated = DateTime.Now;
        }

        public static ArpMonitor Instance => TheInstance.Value;

        static readonly Lazy<ArpMonitor> TheInstance = new Lazy<ArpMonitor>(() => new ArpMonitor());
        ArpMonitor()
        {
        }
    }
}
