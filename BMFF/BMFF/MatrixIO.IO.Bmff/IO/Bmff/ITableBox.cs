﻿namespace MatrixIO.IO.Bmff
{
    // TODO: There should be a better way to do this.  We don't want people using ITableBox directly under any circumstances.
    public interface ITableBox { }
    public interface ITableBox<EntryType> : ITableBox
    {
        EntryType[] Entries { get; }
    }
}