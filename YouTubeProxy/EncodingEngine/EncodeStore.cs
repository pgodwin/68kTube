using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeProxy.EncodingEngine
{
    /// <summary>
    /// Dodgy engine for keeping track of the existing encodes
    /// </summary>
    public class EncodeStore : ConcurrentDictionary<string, EncodeDetail>
    {
        private static EncodeStore _store;
        public static EncodeStore GetStore()
        {
            if (_store == null)
                _store = new EncodeStore();
            return _store;
        }

        /// <summary>
        /// Adds or updates the video encoding details
        /// </summary>
        /// <param name="details"></param>
        public void AddOrUpdateVideo(EncodeDetail details)
        {
            // Key is always encodeProfile name + "|" + video id
            var key = details.EncodeProfileName + "|" + details.VideoId;
            this.AddOrUpdate(key, details, (k, originalValue) => originalValue = details);
            
        }

        /// <summary>
        /// Returns the encoding details for the specified video/profile. If none are found null is returned.
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="profile"></param>
        /// <returns></returns>
        public EncodeDetail GetDetails(string videoId, string profile)
        {
            var key = profile + "|" + videoId;
            if (this.ContainsKey(key))
                return this[key];

            return null;
        }
    }
}
