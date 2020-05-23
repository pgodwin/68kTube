﻿using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Compressed Movie Data Atom ("cmvd")
    /// </summary>
    [Box("cmvd", "Compressed Movie Data Atom")]
    public sealed class CompressedMovieDataBox : Box, IContentBox
    {
        public CompressedMovieDataBox() 
            : base() { }

        public CompressedMovieDataBox(Stream stream)
            : base(stream) { }

        public uint UncompressedSize { get; set; }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            UncompressedSize = stream.ReadBEUInt32();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            stream.WriteBEUInt32(UncompressedSize);
        }
    }
}