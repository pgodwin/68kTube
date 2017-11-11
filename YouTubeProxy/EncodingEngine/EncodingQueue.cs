using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeProxy.EncodingEngine
{
    public class EncodingQueue : Queue<EncodeDetail>
    {

        private static EncodingQueue _currentQueue;

        public static EncodingQueue GetQueue()
        {
            if (_currentQueue == null)
                _currentQueue = new EncodingEngine.EncodingQueue();

            return _currentQueue;
        }


        private Encoder encoder = new Encoder(Profiles.LoadProfiles());

        public const int MaxJobs = 4;

        public List<EncodeDetail> RunningJobs = new List<EncodeDetail>();
        
        public void AddToQueue(EncodeDetail details)
        {
            EncodeStore.GetStore().AddOrUpdateVideo(details);
            this.Enqueue(details);
            this.RunNextJob();
        }

        /// <summary>
        /// Runs the next job in the queue
        /// </summary>
        public void RunNextJob()
        {
            if (RunningJobs.Count < MaxJobs && this.Count > 0)
            {
                var nextJob = this.Dequeue();
                RunningJobs.Add(nextJob);
                Task.Factory.StartNew(() => encoder.Encode(nextJob))
                    // Remove from our running jobs list
                    .ContinueWith((t) => RunningJobs.Remove(nextJob))
                    // Add the completed job to our encode queue
                    // Run the next job in the queue
                    .ContinueWith((t) => RunNextJob());
            }
        }

    }

   

 
}
