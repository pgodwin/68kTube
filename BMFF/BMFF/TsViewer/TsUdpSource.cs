using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MatrixIO.IO;

namespace TsViewer
{
    public class TsUdpSource : TsSource
    {
        private readonly object _syncObject = new Object();

        private Socket _socket;
        private volatile bool _running;

        protected IPAddress _localAddress = IPAddress.Any;
        protected IPAddress _sourceAddress;
        protected IPAddress _groupAddress;

        protected bool _multicast = true;
        protected int _receiveBufferSize = 128 * 1024;
        private readonly TimeAverage _bitrate = new TimeAverage();
        private readonly TimeAverage _packetrate = new TimeAverage();

        private readonly Stopwatch _highResTimer = new Stopwatch();
        private readonly BlockingCollection<UdpPacket> _packetQueue = new BlockingCollection<UdpPacket>(new ConcurrentQueue<UdpPacket>());
        private Thread _processingThread;

        private readonly ConcurrentQueue<byte[]> _bufferPool = new ConcurrentQueue<byte[]>();
        private readonly ConcurrentQueue<UdpPacket> _packetPool = new ConcurrentQueue<UdpPacket>();

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private long _total;
        private int _count;
        private TimeSpan _average;

        ~TsUdpSource()
        {
            Stop();
        }

        public double Bitrate
        {
            get { return _bitrate.GetAverage(10); }
        }

        public double Packetrate
        {
            get { return _packetrate.GetAverage(10); }
        }

        public TimeSpan ProcessingTime
        {
            get { lock (_stopwatch) return _average; }
        }

        public override void Start(Uri uri)
        {
            lock (_syncObject)
            {
                if (_running) return;
                _running = true;

                Uri = uri;
                switch (uri.HostNameType)
                {
                    case UriHostNameType.IPv4:
                    case UriHostNameType.IPv6:
                        _sourceAddress = IPAddress.Parse(uri.Host);
                        break;
                    default:
                        var addresses = Dns.GetHostAddresses(uri.DnsSafeHost);
                        if (addresses.Length < 1) throw new ArgumentException("Host not found.");
                        _sourceAddress = addresses[0];
                        break;
                }

                if ((_sourceAddress.AddressFamily == AddressFamily.InterNetwork &&
                     _sourceAddress.GetAddressBytes()[0] >= 224 && _sourceAddress.GetAddressBytes()[0] <= 239) ||
                    (_sourceAddress.AddressFamily == AddressFamily.InterNetworkV6 && _sourceAddress.IsIPv6Multicast))
                {
                    _multicast = true;
                }

                if (_sourceAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    _localAddress = IPAddress.IPv6Any;

                _socket = new Socket(_sourceAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, false);
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, _receiveBufferSize);
                _socket.Bind(new IPEndPoint(_localAddress, uri.Port));
                switch (_sourceAddress.AddressFamily)
                {
                    case AddressFamily.InterNetwork:
                        if (_multicast)
                            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                                                    new MulticastOption(_sourceAddress));
                        break;
                    case AddressFamily.InterNetworkV6:
                        if (_multicast)
                            _socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership,
                                                    new IPv6MulticastOption(_sourceAddress));
                        break;
                }
                var socketArgs = new SocketAsyncEventArgs();
                byte[] buffer;
                if (!_bufferPool.TryDequeue(out buffer)) buffer = new byte[9000];
                socketArgs.SetBuffer(buffer, 0, buffer.Length);
                socketArgs.Completed += ReceiveCompleted;

                _bitrate.Start();
                _packetrate.Start();
                _highResTimer.Start();

                _processingThread = new Thread(ProcessPackets) {IsBackground = true};
                _processingThread.Start();

                _socket.ReceiveAsync(socketArgs);
            }
        }

        private void ReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            UdpPacket packet;
            if (!_packetPool.TryDequeue(out packet)) packet = new UdpPacket();

            packet.Buffer = e.Buffer;
            packet.Length = e.BytesTransferred;
            packet.ReceivedFrom = (IPEndPoint) e.RemoteEndPoint;
            packet.Tick = _highResTimer.ElapsedTicks;

            _packetQueue.Add(packet);

            // Start receiving next packet as quickly as possible.
            byte[] newBuffer;
            if (!_bufferPool.TryDequeue(out newBuffer)) newBuffer = new byte[65507];

            e.SetBuffer(newBuffer, 0, newBuffer.Length);
            if(_running) _socket.ReceiveAsync(e);
        }

        public void ProcessPackets()
        {
            foreach (var packet in _packetQueue.GetConsumingEnumerable())
            {
                Debug.WriteLine("Received " + packet.Length + " bytes at " + packet.Tick + " from " +
                                packet.ReceivedFrom +
                                (packet.Length > 0 && packet.Buffer != null
                                     ? " with a first byte of 0x" + packet.Buffer[0].ToString("X2")
                                     : ""));

                _stopwatch.Reset();
                _stopwatch.Start();

                _bitrate.Add(packet.Length*8);
                _packetrate.Add(1);

                ProcessPacket(packet);

                _bufferPool.Enqueue(packet.Buffer);
                packet.Buffer = null;
                _packetPool.Enqueue(packet);

                _stopwatch.Stop();

                _total += _stopwatch.ElapsedTicks;
                if (++_count >= 100)
                {
                    _average = new TimeSpan(_total/_count);
                    _total = _count = 0;
                }

                Debug.WriteLine("Processed packet in " + _stopwatch.Elapsed);
            }
            Debug.WriteLine("Exiting packet processing thread.");
        }

        internal virtual void ProcessPacket(UdpPacket packet)
        {
            Demuxer.ProcessInput(packet.Buffer, 0, packet.Length);
        }

        public override void Stop()
        {
            lock (_syncObject)
            {
                if (!_running) return;
                Trace.WriteLine("Stopping.");
                _packetQueue.CompleteAdding();
                _running = false;
            }
        }

        internal class UdpPacket
        {
            public byte[] Buffer { get; set; }
            public int Length { get; set; }
            public IPEndPoint ReceivedFrom { get; set; }
            public long Tick { get; set; }
        }
    }
}
