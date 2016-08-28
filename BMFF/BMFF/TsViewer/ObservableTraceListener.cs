using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace TsViewer
{
    public class ObservableTraceListener : TraceListener
    {
        public DispatchingObservableCollection<string> Messages { get; private set; }

        private readonly StringBuilder _newMessageCache = new StringBuilder();

        public ObservableTraceListener()
        {
            Messages = new DispatchingObservableCollection<string>();
        }

        public override void Write(string message)
        {
            _newMessageCache.Insert(0, message);
        }

        public override void WriteLine(string message)
        {
            if (_newMessageCache.Length > 0)
            {
                _newMessageCache.AppendLine(message);
                Messages.Insert(0, _newMessageCache.ToString());
                _newMessageCache.Clear();
            }
            else Messages.Insert(0, message);
        }
    }
}
