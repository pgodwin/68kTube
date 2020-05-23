﻿using System.IO;
using System.Text;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Data Entry Url Box ("url ")
    /// </summary>
    [Box("url ", "Data Entry Url Box")]
    public sealed class DataEntryUrlBox : FullBox
    {
        public DataEntryUrlBox()
            : base() { }

        public DataEntryUrlBox(Stream stream)
            : base(stream) { }

        public DataEntryUrlBox(string location)
        {
            Location = location;
        }

        public string Location { get; set; }

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + (string.IsNullOrEmpty(Location) ? 0 : (ulong)Encoding.UTF8.GetByteCount(Location));
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            Location = stream.ReadNullTerminatedUTF8String();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            if (!string.IsNullOrEmpty(Location))
            {
                stream.WriteUTF8String(Location);
            }
        }
    }
}