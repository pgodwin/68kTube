using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;

namespace BmffViewer
{
    /// <summary>
    /// Interaction logic for Log.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        private ObservableTraceListener _LogTraceListener;
        public ObservableTraceListener LogTraceListener
        {
            get
            {
                Debug.WriteLine("Getting LogTraceListener");
                if (_LogTraceListener == null)
                {
                    _LogTraceListener = new ObservableTraceListener();
                    _LogTraceListener.Messages.Add(String.Format("Starting log at {0}.", DateTime.Now));
                    Trace.Listeners.Add(_LogTraceListener);
                }
                return _LogTraceListener;
            }
        }

        public LogWindow()
        {
            InitializeComponent();


        }
    }
}
