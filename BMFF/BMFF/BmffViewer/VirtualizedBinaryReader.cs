using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Collections;

namespace BmffViewer
{
    public class VirtualizedBinaryChunk : IEnumerable<byte?>
    {
        public long Location { get; private set; }
        private byte?[] _Bytes;
        public int Length { get { return _Bytes.Length; } }

        public VirtualizedBinaryChunk(long location, byte?[] bytes)
        {
            Location = location;
            _Bytes = bytes;
        }

        public IEnumerator<byte?> GetEnumerator()
        {
            foreach (byte? b in _Bytes) yield return b;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            StringBuilder chunk = new StringBuilder();

            chunk.Append(String.Format("{0:X8} ", Location));
            for (int i = 0; i < _Bytes.Length; i++)
            {
                byte? b = _Bytes[i];
                if (i % 8 == 0) chunk.Append(" ");
                if(b.HasValue) chunk.Append(String.Format("{0:X2} ", b.Value));
                else chunk.Append("   ");
            }
            chunk.Append(String.Format(" "));
            foreach(byte? b in _Bytes)
            {
                if (b.HasValue)
                {
                    if (!char.IsControl((char)b.Value)) chunk.Append((char)b.Value);
                    else chunk.Append(".");
                }
                else chunk.Append(" ");
            }

            return chunk.ToString();
        }
    }

    public class VirtualizedBinaryReader : IList<VirtualizedBinaryChunk>, IList
    {
        private Stream _BaseStream;

        private long EffectiveOffset;
        private long EffectiveLength;

        public long Offset { get; private set; }
        public long Length { get; private set; }
        public int ChunkSize { get; private set; }

        public VirtualizedBinaryReader(Stream stream, long offset = 0, long? length = null, int chunkSize = 16)
        {
            _BaseStream = stream;
            Offset = offset;
            Length = length.HasValue ? length.Value : stream.Length - offset;
            ChunkSize = chunkSize;

            EffectiveLength = Length;
            if (Offset % ChunkSize == 0) EffectiveOffset = Offset;
            else
            {
                // Doesn't begin on a chunk boundary.  Move the effective offset back to the nearest chunk boundary
                // and add an equal amount to the effective length
                EffectiveOffset = (Offset / ChunkSize) * ChunkSize;
                EffectiveLength += Offset - EffectiveOffset;
            }

            if (EffectiveLength % ChunkSize != 0)
                // Doesn't end on a chunk boundary.  Round the length up to the nearest even number of chunks.
                EffectiveLength = ((EffectiveLength / ChunkSize) + 1) * ChunkSize;
            
            
            Debug.WriteLine("ChunkSize: " + ChunkSize);
            Debug.WriteLine("Offset: " + Offset);
            Debug.WriteLine("Length: " + Length);
            Debug.WriteLine("EffectiveOffset: " + EffectiveOffset);
            Debug.WriteLine("EffectiveLength: " + EffectiveLength);
            Debug.WriteLine("Count: " + Count);
            
        }

        public int IndexOf(VirtualizedBinaryChunk item)
        {
            if (item.Location < EffectiveOffset) return -1;
            if (item.Location >= EffectiveOffset + EffectiveLength) return -1;
            if (item.Location % ChunkSize == 0) return (int)(item.Location / ChunkSize);
            else return -1;
        }

        public void Insert(int index, VirtualizedBinaryChunk item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public VirtualizedBinaryChunk this[int index]
        {
            get
            {
                //Debug.WriteLine("Accessing Item " + index);
                List<byte?> bytes = new List<byte?>();

                long chunkOffset = ((long)index * (long)ChunkSize) + EffectiveOffset;
                int prefix = chunkOffset < Offset ? (int)(Offset - EffectiveOffset) : 0;
                int suffix = chunkOffset + ChunkSize > Offset + Length ? (int)((EffectiveOffset + EffectiveLength) - (Offset + Length)) : 0;
                for(int i=0; i<prefix; i++) bytes.Add(null);

                int size = ChunkSize - prefix - suffix;
                if (size < 0) size = 0;

                byte[] readBytes = new byte[size];
                if (size > 0)
                {
                    try
                    {
                        _BaseStream.Seek(chunkOffset + prefix, SeekOrigin.Begin);
                        _BaseStream.Read(readBytes, 0, readBytes.Length);
                        for (int i = 0; i < readBytes.Length; i++) bytes.Add(readBytes[i]);
                    }
                    catch (System.IO.EndOfStreamException)
                    {
                        Debug.WriteLine("WARNING: Data not available for VirtualizedBinaryChunk at offset " + chunkOffset);
                    }
                }
                for (int i = 0; i < suffix; i++) bytes.Add(null);

                return new VirtualizedBinaryChunk(chunkOffset, bytes.ToArray());
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public void Add(VirtualizedBinaryChunk item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(VirtualizedBinaryChunk item)
        {
            if (item.Location % ChunkSize == 0 && item.Location >= Offset && item.Location < Offset + Length) return true;
            else return false;
        }

        public void CopyTo(VirtualizedBinaryChunk[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public int Count
        {
            get { return (int) (EffectiveLength / ChunkSize); }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(VirtualizedBinaryChunk item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<VirtualizedBinaryChunk> GetEnumerator()
        {
            for (int i = 0; i < Count; i++) yield return this[i];
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Add(object value)
        {
            throw new NotSupportedException();
        }

        public bool Contains(object value)
        {
            return Contains(value as VirtualizedBinaryChunk);
        }

        public int IndexOf(object value)
        {
            return IndexOf(value as VirtualizedBinaryChunk);
        }

        public void Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        public bool IsFixedSize
        {
            get { return true; }
        }

        public void Remove(object value)
        {
            throw new NotSupportedException();
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        public bool IsSynchronized
        {
            get { return false; }
        }
        private object _SyncRoot = new object();
        public object SyncRoot
        {
            get { return _SyncRoot; }
        }
    }
}
