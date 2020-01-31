using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using TvLight.Devices;
using TvLight.Pinger;

namespace TvLight.Hue
{
    public class HueBridge : Device
    {
        public HueBridge(string name, PhysicalAddress mac) : base(DeviceType.HueBridge, name, mac)
        {
            _httpClient = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });
        }

        public List<Light> GetLights()
        {
            var doc = TransactCommand(HttpMethod.Get, "lights");
            var lights = new List<Light>();
            foreach (var light in doc.RootElement.EnumerateObject())
                lights.Add(new Light(this, light));
            return lights;
        }

        JsonDocument TransactCommand(HttpMethod method, string url, string bodyJson = null)
        {
            string result;
            if (method == HttpMethod.Get)
                result = _httpClient.GetStringAsync(ApiRoot + url).Result;
            else
                throw new Exception("Unsupported method: " + method);
            return JsonDocument.Parse(result);
        }

        readonly HttpClient _httpClient;

        string ApiRoot => $"https://{Ip.Ip}/api/{Credentials.Username}/";
    }
}
