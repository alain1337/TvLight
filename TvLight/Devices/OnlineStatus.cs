﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace TvLight.Devices
{
    public class OnlineStatus
    {
        public OnlineStatusOnline Status { get; private set; }
        public DateTimeOffset? OnlineSince { get; private set; }
        public DateTimeOffset? OfflineSince { get; private set; }
        public IPAddress Ip { get; private set; }

        static readonly TimeSpan DebounceInterval = TimeSpan.FromSeconds(5);

        public bool SignalOnline(IPAddress ip)
        {
            OnlineSince = DateTimeOffset.Now;
            OfflineSince = null;
            Ip = ip;
            return RefreshStatus();
        }

        public bool RefreshStatus()
        {
            OnlineStatusOnline newState = Status;
            if (!OnlineSince.HasValue && !OfflineSince.HasValue)
                newState = OnlineStatusOnline.Unknown;
            else if (OnlineSince.HasValue && !OfflineSince.HasValue || OnlineSince > OfflineSince)
                newState = OnlineStatusOnline.Online;
            else if (OfflineSince + DebounceInterval > OnlineSince)
                newState = OnlineStatusOnline.Offline;

            if (Status == newState)
                return false;

            // TODO: Trigger event
            Status = newState;
            return true;
        }

        public enum OnlineStatusOnline
        {
            Unknown,
            Offline,
            Online
        }
    }
}
