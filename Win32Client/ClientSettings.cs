using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win32Client
{
    public class ClientSettings
    {
        const string SettingsFile = "ClientSettings.json";

        public static ClientSettings GetSettings()
        {
            if (File.Exists(SettingsFile))
            {
                try
                {
                    return JsonConvert.DeserializeObject<ClientSettings>(File.ReadAllText(SettingsFile));
                }
                catch (Exception ex)
                {
                    // Log it?
                }
            }
            return ClientSettings.DefaultSettings;
        }

        

        public static ClientSettings DefaultSettings
        {
            get
            {
                return new ClientSettings
                {
                    BaseUrl = "http://localhost:9000/",
                    Profile = "h263"
                };
            }
        }

        public string BaseUrl
        {
            get;set;
        }

        public string Profile { get; set; }

        public void SaveSettings()
        {
            File.WriteAllText(SettingsFile, JsonConvert.SerializeObject(this));
        }
    }
}
