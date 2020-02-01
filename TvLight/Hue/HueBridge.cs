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

        public List<Group> GetGroups()
        {
            var doc = TransactCommand(HttpMethod.Get, "groups");
            var groups = new List<Group>();
            foreach (var light in doc.RootElement.EnumerateObject())
                groups.Add(new Group(this, light));
            return groups;
        }

        internal JsonDocument TransactCommand(HttpMethod method, string url, string bodyJson = null)
        {
            var requestUri = ApiRoot + url;
            string result;
            if (method == HttpMethod.Get)
                result = _httpClient.GetStringAsync(requestUri).Result;
            else if (method == HttpMethod.Put)
                result = _httpClient.PutAsync(requestUri, new StringContent(bodyJson)).Result.Content.ReadAsStringAsync().Result;
            else
                throw new Exception("Unsupported method: " + method);
            return JsonDocument.Parse(result);
        }

        readonly HttpClient _httpClient;

        string ApiRoot => $"https://{Ip.Ip}/api/{Credentials.Username}/";
    }
}
