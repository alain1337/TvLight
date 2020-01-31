using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace TvLight.Pinger
{
    public class Pinger
    {
        public IPAddress Subnet { get; }

        public Pinger(IPAddress subnet)
        {
            Subnet = subnet;
        }
    }
}
