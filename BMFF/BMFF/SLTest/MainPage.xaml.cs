using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MatrixIO.IO.MpegTs;
using MatrixIO.IO.MpegTs.Streams;

namespace SLTest
{
    public partial class MainPage : UserControl
    {
        public ObservableCollection<TsUdpMediaStreamSource> Sources { get; private set; }

        public MainPage()
        {
            Debug.WriteLine("Loading...");
            new MpegTsPortabilityFactory();
            Sources = new ObservableCollection<TsUdpMediaStreamSource>();
            InitializeComponent();

            Loaded += OnLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (App.Current.HasElevatedPermissions)
                Sources.Add(new TsUdpMediaStreamSource(new Uri("udp://224.0.0.42:4242/")));
            else
                Debug.WriteLine("This application requires elevated permissions to run.");
        }

        private void SourceTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var stream = e.NewValue as TsStream;
            if (stream != null)
            {
                MessageBox.Show("New selected stream is " + stream.Type + " with " + stream.Info.Count + " descriptors.");
                var streamSource = new TestStreamSource(stream);
                mediaElement.SetSource(streamSource);
            }
        }

        private void mediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show(e.ErrorException.ToString());
        }

        private void mediaElement_BufferingProgressChanged(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => bufferingProgress.Value = ((MediaElement) e.OriginalSource).BufferingProgress);
        }
    }
}
