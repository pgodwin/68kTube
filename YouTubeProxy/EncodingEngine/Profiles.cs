using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeProxy.Models;

namespace YouTubeProxy.EncodingEngine
{
    public class Profiles : Dictionary<string, EncodingProfile>
    {

        
        public static Profiles LoadProfiles(string path = "profiles.json")
        {
            return JsonConvert.DeserializeObject<Profiles> (File.ReadAllText(path));
        }

        public void Add(EncodingProfile profile)
        {
            if (this.ContainsKey(profile.Name))
            {
                throw new Exception("Profile name already taken");
            }
            this.Add(profile.Name, profile);
        }

        


    }
}
