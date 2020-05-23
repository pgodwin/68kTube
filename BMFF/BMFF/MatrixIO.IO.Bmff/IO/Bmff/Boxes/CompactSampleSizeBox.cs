﻿using System;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Compact Sample Size Box ("stz2")
    /// </summary>
    [Box("stz2", "Compact Sample Size Box")]
    public sealed class CompactSampleSizeBox : FullBox, ITableBox<CompactSampleSizeBox.CompactSampleSizeEntry>
    {
        public CompactSampleSizeBox() 
            : base() { }

        public CompactSampleSizeBox(Stream stream) 
            : base(stream) { }

        public CompactSampleSizeEntry[] Entries { get; set; }

        private uint Reserved { get; set; }

        private byte _fieldSize;

        public byte FieldSize
        {
            get => _fieldSize;
            set
            {
                if (_fieldSize != 4 && _fieldSize != 8 && _fieldSize != 16)
                {
                    throw new ArgumentOutOfRangeException("FieldSize must be 4, 8 or 16.");
                }

                _fieldSize = value;
            }
        }

        public int EntryCount => Entries.Length;

        internal override ulong CalculateSize()
        {
            ulong calculatedSize = base.CalculateSize() + 3 + 1;

            if (FieldSize == 4)
            {
                calculatedSize += (ulong)Math.Ceiling((double)Entries.Length / 2D);
            }
            else if (FieldSize == 8)
            {
                calculatedSize += (ulong)Entries.Length;
            }
            else if (FieldSize == 16)
            {
                calculatedSize += (ulong)Entries.Length * 2;
            }

            return calculatedSize;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            Reserved = stream.ReadBEUInt24();
            _fieldSize = stream.ReadOneByte();
            uint entryCount = stream.ReadBEUInt32();

            var entries = new List<CompactSampleSizeEntry>();

            for (uint i = 0; i < entryCount; i++)
            {
                if (FieldSize == 4)
                {
                    byte twoFieldSizes = stream.ReadOneByte();

                    entries.Add(new CompactSampleSizeEntry((ushort)(twoFieldSizes & 0xFF00 >> 4)));

                    if (i < entryCount)
                    {
                        entries.Add(new CompactSampleSizeEntry((ushort)(twoFieldSizes & 0x00FF)));
                    }
                }
                else if (FieldSize == 8)
                {
                    entries.Add(new CompactSampleSizeEntry(stream.ReadOneByte()));
                }
                else if (FieldSize == 16)
                {
                    entries.Add(new CompactSampleSizeEntry(stream.ReadBEUInt16()));
                }
            }

            Entries = entries.ToArray();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt24(Reserved);
            stream.WriteByte(FieldSize);
            stream.WriteBEUInt32((uint)Entries.Length);

            if (FieldSize == 4)
            {
                for (int i = 0; i < Entries.Length; i += 2)
                {
                    byte twoFieldSizes = (byte)((Entries[i].EntrySize & 0x00FF) << 4);

                    if (i + 1 < Entries.Length)
                    {
                        twoFieldSizes |= (byte)(Entries[i].EntrySize & 0x00FF);
                    }

                    stream.WriteByte(twoFieldSizes);
                }
            }
            else
            {
                foreach (CompactSampleSizeEntry compactSampleSizeEntry in Entries)
                {
                    if (FieldSize == 8)
                    {
                        stream.WriteByte((byte)compactSampleSizeEntry.EntrySize);
                    }
                    else if (FieldSize == 16)
                    {
                        stream.WriteBEUInt16(compactSampleSizeEntry.EntrySize);
                    }
                }
            }
        }

        public readonly struct CompactSampleSizeEntry
        {
            public CompactSampleSizeEntry(ushort entrySize)
            {
                EntrySize = entrySize;
            }

            public ushort EntrySize { get; }

            public static implicit operator uint(CompactSampleSizeEntry entry)
            {
                return entry.EntrySize;
            }

            public static implicit operator CompactSampleSizeEntry(ushort entrySize)
            {
                return new CompactSampleSizeEntry(entrySize);
            }
        }
    }
}