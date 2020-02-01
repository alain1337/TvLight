using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TvLight.Devices;
using TvLight.Pinger;

namespace TvLight.Monitor
{
    public class DeviceMonitor
    {
        DeviceList<Device> Devices { get; }
        IpDeviceStatus[] DeviceStatus { get; set; }

        public event EventHandler<DeviceChangeData> DeviceChanged;
        

        public DeviceMonitor(DeviceList<Device> devices)
        {
            Devices = devices.ToArray();
            DeviceStatus = Devices.Select(d => d.GetStatus()).ToArray();
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
                var newStatus = Devices.Select(d => d.GetStatus()).ToArray();
                for (int i=0; i < newStatus.Length; i++)
                    if (DeviceStatus[i] != newStatus[i])
                        DeviceChanged?.Invoke(this, new DeviceChangeData(Devices[i], DeviceStatus[i], newStatus[i]));
                DeviceStatus = newStatus;
                cts.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));
            }
        }
    }

    public class DeviceChangeData
    {
        public DeviceChangeData(Device device, IpDeviceStatus previousStatus, IpDeviceStatus currentStatus)
        {
            Device = device;
            PreviousStatus = previousStatus;
            CurrentStatus = currentStatus;
        }

        public Device Device { get; }
        public IpDeviceStatus PreviousStatus { get; }
        public IpDeviceStatus CurrentStatus { get; }
    }
}
