using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;

namespace TvLight.Pinger
{
    [DebuggerDisplay("{Ip} {Status}")]

    public class IpDevice
    {
        public IPAddress Ip { get; }
        public PhysicalAddress Mac { get; }
        public IpDeviceStatus Status { get; internal set; }

        public IpDevice(IPAddress ip, PhysicalAddress mac)
        {
            Ip = ip;
            Mac = mac;
        }

        public IpDeviceStatus CheckOnline()
        {
            var ping = new Ping();
            var reply = ping.Send(Ip, 1000);
            Status = reply?.Status == IPStatus.Success ? IpDeviceStatus.Online : IpDeviceStatus.Offline;
            return Status;
        }
    }

    public enum IpDeviceStatus
    {
        Unknown,
        Online,
        Offline
    }
}