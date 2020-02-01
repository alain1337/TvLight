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

        public event EventHandler<ProcessDiscoveryChange> DeviceChanged;

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
                var changes = Triggers.ProcessDiscovery(MacDiscovery.DiscoverOnline(SubnetAddress));
                foreach (var change in changes)
                    DeviceChanged?.Invoke(this, change);

                cts.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(5));
            }
        }
    }
}
