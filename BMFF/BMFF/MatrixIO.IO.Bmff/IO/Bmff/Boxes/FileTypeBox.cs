﻿using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// File Type Box ("ftyp")
    /// </summary>
    [Box("ftyp", "File Type Box")]
    public sealed class FileTypeBox : Box
    {
        public FileTypeBox()
            : base() { }

        public FileTypeBox(Stream stream)
            : base(stream) { }

        public FileTypeBox(FourCC majorBrand, uint minorVersion, FourCC[] compatibleBrands)
            : this()
        {
            MajorBrand = majorBrand;
            MinorVersion = minorVersion;
            foreach (FourCC fourCC in compatibleBrands)
            {
                CompatibleBrands.Add(fourCC);
            }
        }

        public FourCC MajorBrand { get; set; }

        public uint MinorVersion { get; set; }

        public IList<FourCC> CompatibleBrands { get; } = new List<FourCC>();

        internal override ulong CalculateSize()
        {
            return base.CalculateSize() + 4 + 4 + ((ulong)CompatibleBrands.Count * 4);
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            MajorBrand = new FourCC(stream.ReadBEUInt32());
            MinorVersion = stream.ReadBEUInt32();

            ulong remainingBytes = EffectiveSize - CalculateSize();
            ulong numBrands = remainingBytes / 4;

            for (ulong i = 0; i < numBrands; i++)
            {
                CompatibleBrands.Add(new FourCC(stream.ReadBEUInt32()));
            }
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);
            stream.WriteBytes(MajorBrand.GetBytes());
            stream.WriteBEUInt32(MinorVersion);

            foreach (FourCC compatibleBrand in CompatibleBrands)
            {
                stream.WriteBytes(compatibleBrand.GetBytes());
            }
        }

        public bool IsCompatibleWith(FourCC brand)
        {
            return MajorBrand == brand || CompatibleBrands.Contains(brand);
        }

        public bool IsCompatibleWith(int brand) => IsCompatibleWith(new FourCC(brand));

        public bool IsCompatibleWith(uint brand) => IsCompatibleWith(new FourCC(brand));

        public bool IsCompatibleWith(string brand) => IsCompatibleWith(new FourCC(brand));

        public bool IsCompatibleWith(byte[] brand) => IsCompatibleWith(new FourCC(brand));
    }
}