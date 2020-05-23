﻿using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// JPEG2000 Codestream Box ("jp2c")
    /// </summary>
    [Box("jp2c", "Contiguous Codestream Box")]
    public sealed class ContiguousCodestreamBox : Box
    {
        public ContiguousCodestreamBox() : base() { }
        public ContiguousCodestreamBox(Stream stream) : base(stream) { }
    }
}
