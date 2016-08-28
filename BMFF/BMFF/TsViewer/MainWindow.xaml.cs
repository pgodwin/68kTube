using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MatrixIO.IO.MpegTs;

namespace TsViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DispatchingObservableCollection<TsSource> Sources { get; set; }
        private readonly Timer _updateTimer = new Timer(1000);
        private readonly TextWriterTraceListener _logWriter = new TextWriterTraceListener(@"C:\Temp\TsViewer.txt");

        public MainWindow()
        {
            Trace.Listeners.Add(_logWriter);

            Sources = new DispatchingObservableCollection<TsSource>();
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            _updateTimer.Elapsed += _updateTimer_Elapsed;
            _updateTimer.AutoReset = true;
#if DEBUG
            debugMenu.Visibility = Visibility.Visible;
#endif
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            foreach (var source in Sources)
            {
                source.Stop();
            }
        }

        private static readonly string[] RateUnits = { "", "K", "M", "G", "T", "P", "Z"};

        void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            double averagePacketSize = 0;

            
            var totalBitrate = Sources.Count > 0
                                   ? Sources.OfType<TsUdpSource>().Select(udpSource => udpSource.Bitrate).Sum()
                                   : 0;
            averagePacketSize = totalBitrate / 8;
            int bitrateUnits = 0;
            while(bitrateUnits < RateUnits.Length && totalBitrate > 1024)
            {
                totalBitrate /= 1024;
                bitrateUnits++;
            }
            string statusBarBitrateText = String.Format("Bitrate: {0:0.000}{1}bps", totalBitrate, RateUnits[bitrateUnits]);

            var totalPacketrate = Sources.Count > 0
                                      ? Sources.OfType<TsUdpSource>().Select(udpSource => udpSource.Packetrate).Sum()
                                      : 0;
            averagePacketSize /= totalPacketrate;
            int packetrateUnits = 0;
            while (packetrateUnits < RateUnits.Length && totalPacketrate > 1024)
            {
                totalPacketrate /= 1024;
                packetrateUnits++;
            }
            string statusBarPacketrateText = String.Format("Packetrate: {0:0.000}{1}pps", totalPacketrate, RateUnits[packetrateUnits]);

            int packetsizeUnits = 0;
            while (packetsizeUnits < RateUnits.Length && averagePacketSize > 1024)
            {
                averagePacketSize /= 1024;
                packetsizeUnits++;
            }
            string statusBarPacketsizeText = String.Format("Avg. Packet: {0:0.000}{1}B", averagePacketSize, RateUnits[packetsizeUnits]);

            double droppedPackets = Sources.OfType<TsUdpSource>().Select(udpSource => udpSource.Demuxer.Streams.Select(stream => stream.DroppedPackets).Sum()).Sum();
            int droppedPacketUnits = 0;
            while (droppedPacketUnits < RateUnits.Length && droppedPackets > 1024)
            {
                droppedPackets /= 1024;
                droppedPacketUnits++;
            }
            string statusBarDroppedPacketText = String.Format("Dropped Packets: {0:0.###}{1}", droppedPackets, RateUnits[droppedPacketUnits]);

            var totalProcessingTime = Sources.Count > 0
                                          ? Sources.OfType<TsUdpSource>().Select(
                                              udpSource => udpSource.ProcessingTime.Ticks).Sum()
                                          : 0;
            string statusBarProcessingTimeText = String.Format("Time/Packet: {0}", new TimeSpan((long)totalProcessingTime));

            statusBarBitrate.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal,
                new Action(delegate()
                               {
                                   statusBarBitrate.Text = statusBarBitrateText;
                                   statusBarPacketrate.Text = statusBarPacketrateText;
                                   statusBarPacketsize.Text = statusBarPacketsizeText;
                                   statusBarDroppedPackets.Text = statusBarDroppedPacketText;
                                   statusBarProcessingTime.Text = statusBarProcessingTimeText;
                               }));
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _updateTimer.Start();
            try
            {
                Sources.Add(TsSource.Create(@"C:\temp\00011.m2ts"));
                //Sources.Add(TsSource.Create("udp://@224.0.0.42:4242"));
            }
            catch (Exception err)
            {
                Trace.WriteLine(err.ToString());
            }
        }

        private void MenuItem_Break_Click(object sender, RoutedEventArgs e)
        {
            Debugger.Break();
        }

        private LogWindow _logWindow;
        private void MenuItem_Log_Click(object sender, RoutedEventArgs e)
        {
            if (_logWindow == null)
            {
                _logWindow = new LogWindow();
                _logWindow.Closed += (sender2, e2) => { _logWindow = null; };
                _logWindow.Show();
            }
            else
            {
                _logWindow.Activate();
            }
        }
    }
}
