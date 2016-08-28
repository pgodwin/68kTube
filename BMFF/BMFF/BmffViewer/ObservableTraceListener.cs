using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace BmffViewer
{
    public class ObservableTraceListener : TraceListener
    {
        public ObservableCollection<string> Messages { get; private set; }

        private StringBuilder NewMessageCache = new StringBuilder();

        public ObservableTraceListener()
        {
            Messages = new ObservableCollection<string>();
        }

        public override void Write(string message)
        {
            NewMessageCache.Insert(0, message);
        }

        public override void WriteLine(string message)
        {
            if (NewMessageCache.Length > 0)
            {
                NewMessageCache.AppendLine(message);
                Messages.Insert(0, NewMessageCache.ToString());
                NewMessageCache.Clear();
            }
            else Messages.Insert(0, message);
        }
    }
}
