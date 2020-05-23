﻿using System.IO;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Unknown Box Type
    /// </summary>
    public sealed class UnknownBox : Box, IContentBox
    {
        public UnknownBox() : base() { }
        public UnknownBox(BoxType type) : base()
        {
            Type = type;
        }
        public UnknownBox(Stream stream) : base(stream) { }
    }
}