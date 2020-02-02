using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace TvLight.Devices
{
    public class OnlineStatus
    {
        public OnlineStatusOnline Status { get; private set; }
        public DateTimeOffset? OnlineLast { get; private set; }
        public DateTimeOffset? OfflineSince { get; private set; }
        public IPAddress Ip { get; private set; }

        static readonly TimeSpan DebounceInterval = TimeSpan.FromSeconds(10);

        public OnlineStatusChange SignalOnline(IPAddress ip)
        {
            OnlineLast = DateTimeOffset.Now;
            OfflineSince = null;
            Ip = ip;
            return RefreshStatus();
        }

        public OnlineStatusChange SignalOffline()
        {
            OfflineSince = DateTimeOffset.Now;
            return RefreshStatus();
        }

        OnlineStatusChange RefreshStatus()
        {
            var newState = Status;
            if (!OnlineLast.HasValue && !OfflineSince.HasValue)
                newState = OnlineStatusOnline.Unknown;
            else if (OnlineLast.HasValue && !OfflineSince.HasValue || OnlineLast > OfflineSince)
                newState = OnlineStatusOnline.Online;
            else if (OfflineSince + DebounceInterval < DateTimeOffset.Now)
                newState = OnlineStatusOnline.Offline;

            if (Status == newState)
                return new OnlineStatusChange(false, Status, Status);

            if (newState == OnlineStatusOnline.Offline)
                OnlineLast = null;
            else if (newState == OnlineStatusOnline.Online)
                OfflineSince = null;

            var change = new OnlineStatusChange(true, Status, newState);
            Status = newState;
            return change;
        }
    }

    public enum OnlineStatusOnline
    {
        Unknown,
        Offline,
        Online
    }

    public class OnlineStatusChange
    {
        public OnlineStatusChange(bool changed, OnlineStatusOnline fromStatus, OnlineStatusOnline toStatus)
        {
            Changed = changed;
            FromStatus = fromStatus;
            ToStatus = toStatus;
        }

        public bool Changed { get; }
        public OnlineStatusOnline FromStatus { get; }
        public OnlineStatusOnline ToStatus { get; }
    }
}
