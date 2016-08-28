using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MatrixIO.IO.MpegTs;

namespace SLTest
{
    public class TsUdpMediaStreamSource : MediaStreamSource
    {
        public Uri Url { get; private set; }
        private readonly UdpAnySourceMulticastClient _udpClient;
        public TsDemuxer Demuxer { get; private set; }

        public TsUdpMediaStreamSource(Uri url)
        {
            Url = url;
            Demuxer = new TsDemuxer();

            Debug.WriteLine("Creating UDP Client...");
            _udpClient = new UdpAnySourceMulticastClient(IPAddress.Parse(url.Host), url.Port);
            Debug.WriteLine("Joining Multicast Group...");
            _udpClient.BeginJoinGroup(JoinedGroup, null);

        }
        private void JoinedGroup(IAsyncResult ar)
        {
            Debug.WriteLine("Ending Multicast Group Join...");
            _udpClient.EndJoinGroup(ar);

            _udpClient.ReceiveBufferSize = 128 * 1024;

            Debug.WriteLine("Beginning Receive...");
            var packetBuffer = new byte[9000];
            _udpClient.BeginReceiveFromGroup(packetBuffer, 0, packetBuffer.Length, ReceivedPacket, packetBuffer);
        }
        private void ReceivedPacket(IAsyncResult ar)
        {
            //Debug.WriteLine("Receiving Packet...");
            var packetBuffer = ar.AsyncState as byte[];
            try
            {
                IPEndPoint source;
                var length = _udpClient.EndReceiveFromGroup(ar, out source);
                //Debug.WriteLine("Received " + length + " byte packet from " + source);
                Demuxer.ProcessInput(packetBuffer, 0, length);

            }
            catch(SocketException e)
            {
                Debug.WriteLine("SocketException: " + e.SocketErrorCode);
            }
            catch(Exception e)
            {
                var ex = e;
                while (ex != null)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                    ex = ex.InnerException;
                }
            }

            if (packetBuffer != null)
            {
                _udpClient.BeginReceiveFromGroup(packetBuffer, 0, packetBuffer.Length, ReceivedPacket, packetBuffer);
            }
        }

        protected override void CloseMedia()
        {
            throw new NotImplementedException();
        }

        protected override void GetDiagnosticAsync(MediaStreamSourceDiagnosticKind diagnosticKind)
        {
            throw new NotImplementedException();
        }

        protected override void GetSampleAsync(MediaStreamType mediaStreamType)
        {
            throw new NotImplementedException();
        }

        protected override void OpenMediaAsync()
        {
            throw new NotImplementedException();
        }

        protected override void SeekAsync(long seekToTime)
        {
            throw new NotImplementedException();
        }

        protected override void SwitchMediaStreamAsync(MediaStreamDescription mediaStreamDescription)
        {
            throw new NotImplementedException();
        }
    }
}
