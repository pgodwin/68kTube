using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MatrixIO.IO.MpegTs;

namespace TsViewer
{
    public abstract class TsSource : INotifyPropertyChanged
    {
        private readonly TsDemuxer _demuxer = new TsDemuxer();
        public TsDemuxer Demuxer
        {
            get { return _demuxer; }
        }

        private Uri _uri;
        public Uri Uri
        {
            get
            {
                return _uri;
            }
            protected set
            {
                _uri = value;
                OnPropertyChanged("Uri");
            }
        }
        
        public abstract void Start(Uri uri);
        public abstract void Stop();

        public static TsSource Create(string uri)
        {
            return Create(new Uri(uri));
        }
        public static TsSource Create(Uri uri)
        {
            switch(uri.Scheme)
            {
                case "file":
                    return Create<TsFileSource>(uri);
                case "udp":
                    return Create<TsUdpSource>(uri);
                default:
                    throw new ArgumentException("Unsupported scheme '" + uri.Scheme + "'");
            }
        }
        public static T Create<T>(string uri) where T: TsSource, new()
        {
            return Create<T>(new Uri(uri));
        }
        public static T Create<T>(Uri uri) where T: TsSource, new()
        {
            var newT = new T();
            newT.Start(uri);
            return newT;
        }

        public override string ToString()
        {
            return Uri == null ? "No Uri" : Uri.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
