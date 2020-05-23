﻿using System;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Movie Header Box ("mvhd")
    /// </summary>
    [Box("mvhd", "Movie Header Box")]
    public sealed class MovieHeaderBox : FullBox
    {
        private ulong _creationTime;
        private ulong _modificationTime;
        private int _rate;
        private short _volume;

        public MovieHeaderBox()
            : base()
        {
            _rate = 0x00010000; // 1.0 normal rate
            _volume = 0x0100; // 1.0 full volume
            Matrix = new int[] { 0x00010000, 0, 0, 0, 0x00010000, 0, 0, 0, 0x40000000 }; // Unity Matrix
        }

        public MovieHeaderBox(Stream stream)
            : base(stream) { }

        public DateTime CreationTime
        {
            get => Convert1904Time(_creationTime);
            set => _creationTime = Convert1904Time(value);
        }

        public DateTime ModificationTime
        {
            get => Convert1904Time(_modificationTime);
            set => _modificationTime = Convert1904Time(value);
        }

        public uint TimeScale { get; set; }

        public ulong Duration { get; set; }

        public double Rate
        {
            get => (double)_rate / ((int)ushort.MaxValue + 1);
            set => _rate = checked((int)Math.Round(value * ((int)short.MaxValue + 1)));
        }

        public double Volume
        {
            get => (double)_volume / ((int)byte.MaxValue + 1);
            set => _volume = checked((short)Math.Round(value * ((int)byte.MaxValue + 1)));
        }

        public byte[] Reserved { get; private set; }

        public int[] Matrix { get; private set; }

        public byte[] PreDefined { get; private set; }

        public uint NextTrackID { get; set; }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() +
                (ulong)(Version == 1 ? 8 + 8 + 4 + 8 : 4 + 4 + 4 + 4) + 4 + 2 + 2 + (2 * 4) + (9 * 4) + (6 * 4) + 4;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            if (Version == 1)
            {
                _creationTime = stream.ReadBEUInt64();
                _modificationTime = stream.ReadBEUInt64();
                TimeScale = stream.ReadBEUInt32();
                Duration = stream.ReadBEUInt64();
            }
            else // if(Version == 0)
            {
                _creationTime = stream.ReadBEUInt32();
                _modificationTime = stream.ReadBEUInt32();
                TimeScale = stream.ReadBEUInt32();
                Duration = stream.ReadBEUInt32();
            }

            _rate = stream.ReadBEInt32();
            _volume = stream.ReadBEInt16();
            Reserved = stream.ReadBytes(2 + (2 * 4));
            for (int i = 0; i < 9; i++) Matrix[i] = stream.ReadBEInt32();
            PreDefined = stream.ReadBytes(6 * 4);
            NextTrackID = stream.ReadBEUInt32();
        }

        protected override void SaveToStream(Stream stream)
        {
            if (Version == 0 &&
                (_creationTime > uint.MaxValue ||
                _modificationTime > uint.MaxValue ||
                Duration > uint.MaxValue))
            {
                Version = 1;
            }

            base.SaveToStream(stream);

            if (Version == 1)
            {
                stream.WriteBEUInt64(_creationTime);
                stream.WriteBEUInt64(_modificationTime);
                stream.WriteBEUInt32(TimeScale);
                stream.WriteBEUInt64(Duration);
            }
            else // if(Version == 0)
            {
                stream.WriteBEUInt32(checked((uint)_creationTime));
                stream.WriteBEUInt32(checked((uint)_modificationTime));
                stream.WriteBEUInt32(TimeScale);
                stream.WriteBEUInt32(checked((uint)Duration));
            }
            stream.WriteBEInt32(_rate);
            stream.WriteBEInt16(_volume);
            stream.WriteBytes(Reserved);
            for (int i = 0; i < 9; i++) stream.WriteBEInt32(Matrix[i]);
            stream.WriteBytes(PreDefined);
            stream.WriteBEUInt32(NextTrackID);
        }

        internal static readonly DateTime _1904BaseTime = new DateTime(1904, 1, 1);

        internal static DateTime Convert1904Time(ulong secondsSince1904)
        {
            return _1904BaseTime + TimeSpan.FromSeconds(checked((double)secondsSince1904));
        }

        internal static ulong Convert1904Time(DateTime time)
        {
            return checked((ulong)Math.Round((time - _1904BaseTime).TotalSeconds));
        }
    }
}