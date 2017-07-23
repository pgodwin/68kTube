using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Win32Client.Helpers;

namespace Win32Client.Models
{
   
    public class StreamStatus
    {
        [ColumnOrder(1)]
        public bool Success { get; set; }

        [ColumnOrder(2)]
        public string VideoId { get; set; }

        [ColumnOrder(3)]
        public string RtspUrl { get; set; }
    }
    
}
