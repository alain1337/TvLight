using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using TvLight.Devices;
using TvLight.Discovery;

namespace TvLight.Monitor
{
    public class DeviceMonitor
    {
        public DeviceList Triggers { get; }
        public IPAddress SubnetAddress { get; }

        public event EventHandler<DeviceChangeData> DeviceChanged;

        public DeviceMonitor(IPAddress subnetAddress, DeviceList triggers)
        {
            SubnetAddress = subnetAddress;
            Triggers = triggers;
        }

        public void Start()
        {
            Stop();
            _threadCancellationTokenSource = new CancellationTokenSource();
            _thread = new Thread(() => Thread(_threadCancellationTokenSource));
            _thread.Start();
        }

        CancellationTokenSource _threadCancellationTokenSource;
        Thread _thread;

        public void Stop()
        {
            if (_threadCancellationTokenSource == null)
                return;

            _threadCancellationTokenSource.Cancel();
            if (!_thread.Join(TimeSpan.FromSeconds(30)))
                throw new Exception("Thread didn't terminate");
            _threadCancellationTokenSource = null;
            _thread = null;
        }

        void Thread(CancellationTokenSource cts)
        {
            while (!cts.IsCancellationRequested)
            {
                // Capture everything to avoid treading issues
                var devices = Triggers.Devices.ToArray();
                var currentStatus = Triggers.Devices.Select(d => d.OnlineStatus.Status).ToArray();
                var online = MacDiscovery.DiscoverOnline(SubnetAddress);
                for (int i = 0; i < devices.Length; i++)
                {
                    var on = online.FirstOrDefault(mo => mo.Mac.Equals(devices[i].Mac));
                    if (on == null)
                        continue;
                    if (devices[i].OnlineStatus.SignalOnline(on.Ip))
                        DeviceChanged?.Invoke(this, new DeviceChangeData(devices[i], currentStatus[i], devices[i].OnlineStatus.Status));
                }
                cts.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));
            }
        }
    }

    public class DeviceChangeData
    {
        public DeviceChangeData(Device device, OnlineStatusOnline previousStatus, OnlineStatusOnline currentStatus)
        {
            Device = device;
            PreviousStatus = previousStatus;
            CurrentStatus = currentStatus;
        }

        public Device Device { get; }
        public OnlineStatusOnline PreviousStatus { get; }
        public OnlineStatusOnline CurrentStatus { get; }
    }
}
