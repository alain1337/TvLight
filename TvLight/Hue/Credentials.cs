using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace TvLight.Hue
{
    public static class Credentials
    {
        public static string Username
        {
            get 
            {
                var jsonBytes = File.ReadAllBytes("credentials.json");
                using var jsonDoc = JsonDocument.Parse(jsonBytes);
                return jsonDoc.RootElement.GetProperty("username").GetString();
            }
        }
    }
}
