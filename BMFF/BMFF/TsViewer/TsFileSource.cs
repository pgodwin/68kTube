using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using MatrixIO.IO.MpegTs;

namespace TsViewer
{
    public class TsFileSource : TsSource
    {
        private Thread _readThread;
        private volatile bool _running;

        public TsFileSource() { }

        ~TsFileSource()
        {
            Stop();
        }

        public void Start(string path)
        {
            Start(new Uri(path));
        }

        public override void Start(Uri uri)
        {
            Uri = uri;
            _running = true;
            _readThread = new Thread(ReadFile);
            _readThread.Start(this);
        }

        public override void Stop()
        {
            if (!_running) return;
            Debug.WriteLine("Stopping background thread.");
            _running = false;
            if (_readThread.Join(1000)) return;
            _readThread.Abort();
        }

        private enum TsStreamType
        {
            Standard,
            AACS,
            ATSC_FEC,
        }

        private void ReadFile(object data)
        {
            try
            {

                var fileStream = File.Open(Uri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                int packetLength = TsPacket.Length;
                var type = TsStreamType.Standard;
                if (Uri.LocalPath.EndsWith(".m2ts", true, CultureInfo.InvariantCulture))
                {
                    packetLength = AacsPacket.Length;
                    type = TsStreamType.AACS;
                }

                var buffer = new byte[packetLength*7];
                int length;
                do
                {
                    if ((length = fileStream.Read(buffer, 0, buffer.Length)) <= 0) continue;

                    switch (type)
                    {
                        case TsStreamType.Standard:
                            Debug.WriteLine("Read " + length + " bytes from " +
                                            Uri.Segments[Uri.Segments.Length - 1] + " with a first byte of 0x" +
                                            buffer[0].ToString("X2"));
                            Demuxer.ProcessInput(buffer, 0, length);
                            break;
                        case TsStreamType.AACS:
                            for (int i = 0; i < length/packetLength; i++)
                            {
                                var packet = new AacsPacket(buffer, i*packetLength);
                                Debug.WriteLine("Read AACS Packet");
                                Demuxer.ProcessPacket(packet.TsPacket);
                            }
                            break;
                    }
                } while (length > 0 && _running);
                fileStream.Close();
            }
            finally
            {
                _running = false;
            }
        }
    }
}
