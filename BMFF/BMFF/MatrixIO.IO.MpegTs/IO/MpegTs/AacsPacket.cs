﻿using System;

namespace MatrixIO.IO.MpegTs
{
    public class AacsPacket
    {
        public const int Length = 4 + 188;

        public AacsPacket(byte[] buffer, int offset)
        {
            CopyControl = (AACSCopyControl)((buffer[offset] & 0xC0) >> 6);
            Timestamp = (uint)(((buffer[offset++] << 24) & 0x3F) | (buffer[offset++] << 16) | (buffer[offset++] << 8) | buffer[offset++]);

            if (CopyControl == AACSCopyControl.Unencrypted)
            {
                TsPacket = new TsPacket(buffer, offset);
            }
            else
            {
                throw new FormatException("Encrypted content not currently supported.");
            }
        }

        public AACSCopyControl CopyControl { get; private set; }

        /// <summary>27Mhz Clock</summary>
        public uint Timestamp { get; private set; }

        public TsPacket TsPacket { get; private set; }
    }
}