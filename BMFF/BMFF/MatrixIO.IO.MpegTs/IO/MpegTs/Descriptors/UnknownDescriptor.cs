﻿using System;

namespace MatrixIO.IO.MpegTs.Descriptors
{
    public sealed class UnknownDescriptor : TsDescriptor
    {
        public UnknownDescriptor(byte[] buffer, int offset) : base(buffer, offset)
        {
            Data = new byte[_length];
            Buffer.BlockCopy(buffer, _position, Data, 0, _length);
        }

        public override int Length
        {
            get => 2 + Data.Length;
            protected set => base.Length = value;
        }

        public byte[] Data { get; set; }
    }
}