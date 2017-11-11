using Nancy;
using Nancy.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeProxy.Models;

namespace YouTubeProxy.Modules
{
    public class SettingsModule : NancyModule
    {
        public SettingsModule()
        {
            Get["/settings"] = GetProfiles;
            Post["/settings"] = GetProfiles;
        }



        private dynamic GetProfiles(dynamic parameters)
        {
            var profiles = EncodingEngine.Profiles.LoadProfiles();
            var currentProfile = Request.Cookies.ContainsKey("encoder") ? Request.Cookies["encoder"] : Settings.DefaultProfile;

            try
            {
                if (Request.Method == "POST") 
                    currentProfile = Request.Form.profile;
            }
            catch
            {
                // form not submitted
            }

            var result = new SettingsViewModel()
            {
                DoubleSize = false,
                CurrentProfile = currentProfile,
                Profiles = profiles.Select(p => p.Value).ToList()
            };
            
            return View["Index", result].WithCookie(new NancyCookie("encoder", currentProfile, DateTime.Now.AddYears(1)));

        }
    }
}
