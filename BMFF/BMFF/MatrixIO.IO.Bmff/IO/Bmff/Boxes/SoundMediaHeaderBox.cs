﻿using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Sound Media Header Box ("smhd")
    /// </summary>
    [Box("smhd", "Sound Media Header Box")]
    public sealed class SoundMediaHeaderBox : FullBox
    {
        public SoundMediaHeaderBox()
            : base() { }

        public SoundMediaHeaderBox(Stream stream)
            : base(stream) { }

        public short Balance { get; set; }

        private ushort Reserved { get; set; }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 2 + 2;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            Balance = stream.ReadBEInt16();
            Reserved = stream.ReadBEUInt16();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEInt16(Balance);
            stream.WriteBEUInt16(Reserved);
        }
    }
}