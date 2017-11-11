using Nancy;
using YouTubeProxy.Models;

namespace SelfHost.Modules {
    public class HomeModule : NancyModule {
        public HomeModule() {
            Get[""] = Get["/"] = Get["/Home/Index"] = HelloWorld;
        }

        private dynamic HelloWorld(dynamic parameters) {
            return View["Index"];
        }
    }
}
