using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TsViewer
{
    public class TsRtpSource : TsUdpSource
    {
        internal override void ProcessPacket(UdpPacket packet)
        {
            base.ProcessPacket(packet);
        }

        private class RtpPacket
        {
            private byte header1; // TODO: Set default version here.
            // 2 bit version
            // 1 bit Padding Indicator -- indicated here by null padding
            // 1 bit Extension Indicator -- indicated here by null extension
            // 4 bit CSRC identifier Count -- indicated here by ContributingSources array length

            private byte header2 = 0x80; // Marker should always be 1
            // 1 bit Marker
            // 7 bit Payload Type
            private byte PayloadType { get; set; }
            
            // 16 bit Sequence Number
            public ushort SequenceNumber { get; set; }

            // 32 bit Timestamp
            public uint TimeStamp { get; set; }

            // 32 bit Synchronization Source RC Identifier
            public uint SynchronizationSource { get; set; }

            // 32 bit Contributing Source RC identifier -- 0 or more per identifier count above
            public IList<uint> ContributingSources { get; private set; }

            // Optional Extension with 16 bit profile-specific identifier and 16 bit length.
            public RtpExtension Extension { get; set; }

            public byte[] Payload { get; set; }

            public byte[] Padding { get; set; }

            public RtpPacket()
            {
                ContributingSources = new List<uint>();
            }

            public RtpPacket(byte[] buffer, int offset) : this()
            {
                int position = offset;

                header1 = buffer[offset++];
                header2 = buffer[offset++];
                SequenceNumber = (ushort)(buffer[offset++] << 8 & buffer[offset++]);
                SynchronizationSource = (uint)(buffer[offset++] << 24 & buffer[offset++] << 16 & buffer[offset++] << 8 & buffer[offset++]);
                // TODO: Load Optional Contributing Sources
                // TODO: Load Optional Extension
                // TODO: Load Payload
                // TODO: Load Optional Padding
            }
        }

        private class RtpExtension
        {
            public uint Identifier { get; set; }
            public byte[] Data;
        }
    }
}
