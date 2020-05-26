using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YouTubeProxy.Models;

namespace YouTubeProxy.Controllers.Proxy
{
    public class SettingsController : Controller
    {
        [HttpGet]
        [HttpPost]
        public IActionResult Index()
        {
            var profiles = EncodingEngine.Profiles.LoadProfiles();


            var currentProfile = Request.Cookies.ContainsKey("encoder") ? Request.Cookies["encoder"] : Settings.DefaultProfile;


            if (Request.Method == "POST")
            {
                currentProfile = Request.Form["profile"];
                Response.Cookies.Append("encoder", currentProfile, new CookieOptions() { Expires = DateTime.Now.AddYears(1) });

                var returnUrl = Request.Query["returnUrl"].FirstOrDefault() ?? Request.Form["returnUrl"].FirstOrDefault();

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
            }
            
            var result = new SettingsViewModel()
            {
                DoubleSize = false,
                CurrentProfile = currentProfile,
                Profiles = profiles.Select(p => p.Value).ToList()
            };

            
            
            return View(result);
        }
    }
}