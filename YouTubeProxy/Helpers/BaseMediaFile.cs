using MatrixIO.IO.Bmff;
using MatrixIO.IO.Bmff.Boxes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeProxy.Helpers
{
    /// <summary>
    /// From the original BaseMedia BMFF Viewer code. 
    /// https://github.com/jeremypoulter/basemedia/blob/master/BMFF/BmffViewer/BaseMediaFile.cs
    /// </summary>
    /// <license>
    /// ﻿Copyright (c) 2011, Matrix.IO, LLC and the CSJ2K and BMFF Teams
    /// All rights reserved.
    /// 
    /// Redistribution and use in source and binary forms, with or without modification, are permitted provided that
    /// the following conditions are met:
    /// 
    /// * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    /// 
    /// * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    /// 
    /// * Neither the name of CSJ2K Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
    /// 
    /// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
    /// </license>
    public class BaseMediaFile : BaseMedia, IDisposable
    {
        protected FileInfo _FileInfo;
        public string Name { get { return _FileInfo.Name; } }
        public string NameWithoutExtension { get { return System.IO.Path.GetFileNameWithoutExtension(FullName); } }
        public string FullName { get { return _FileInfo.FullName; } }
        public string Extension { get { return _FileInfo.Extension; } }
        public string DirectoryName { get { return _FileInfo.DirectoryName; } }

        public Stream SourceStream { get { return base._SourceStream; } }
        private readonly bool DisposeSourceStream = false;

        protected BaseMediaFile() { }
        public BaseMediaFile(string name) : this(new FileInfo(name)) { }
        public BaseMediaFile(FileInfo fileInfo) : this(fileInfo.OpenRead())
        {
            _FileInfo = fileInfo;
            DisposeSourceStream = true;
        }
        public BaseMediaFile(FileStream stream)
            : base(new BufferedStream(stream))
        {
            if (_FileInfo == null)
                _FileInfo = new FileInfo(stream.Name);
        }

        public void SaveAs(string path)
        {
            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                SaveTo(stream);
                stream.Close();
            }
        }

        public bool DecompressMovieHeader()
        {
            MovieBox moov = this.Children.OfType<MovieBox>().SingleOrDefault();
            if (moov == null) return false;
            CompressedMovieBox cmov = moov.Children.OfType<CompressedMovieBox>().SingleOrDefault();
            if (cmov == null) return false;
            DataCompressionBox dcom = cmov.Children.OfType<DataCompressionBox>().SingleOrDefault();
            if (dcom == null) return false;
            CompressedMovieDataBox cmvd = cmov.Children.OfType<CompressedMovieDataBox>().SingleOrDefault();
            if (cmvd == null) return false;

            if (dcom.Format != "zlib") throw new Exception("Unsupported compression type: " + dcom.Format);

            Stream contentStream = cmvd.GetContentStream();
            contentStream.ReadByte();
            contentStream.ReadByte();
            Stream compressedStream = new DeflateStream(contentStream, CompressionMode.Decompress, true);
            MemoryStream uncompressedStream = new MemoryStream();
            compressedStream.CopyTo(uncompressedStream);
            uncompressedStream.Flush();
            compressedStream.Dispose();
            uncompressedStream.Seek(0, SeekOrigin.Begin);

            BaseMedia uncompressedMoov = new BaseMedia(uncompressedStream);

            MovieBox newMoov = uncompressedMoov.Children.OfType<MovieBox>().SingleOrDefault();
            if (newMoov != null)
            {
                int insertionPoint = Children.IndexOf(moov);
                Children.Remove(moov);
                FreeSpaceBox free = new FreeSpaceBox(moov.EffectiveSize);
                ((ObservableCollection<Box>)Children).Insert(insertionPoint, free);
                ((ObservableCollection<Box>)Children).Add(newMoov);
            }
            return true;
        }

        public bool FastStart(string path, bool removeFreeSpace = true)
        {
            using (BaseMediaFile clone = new BaseMediaFile(this._FileInfo))
            {
                clone.DecompressMovieHeader();

                MovieBox moov = clone.Children.OfType<MovieBox>().SingleOrDefault();
                if (moov == null) return false;
                MovieDataBox mdat = clone.Children.OfType<MovieDataBox>().SingleOrDefault();
                if (mdat == null) return false;

                clone.Children.Remove(moov);
                clone.Children.Insert(clone.Children.IndexOf(mdat), moov);

                if (removeFreeSpace)
                {
                    Box[] freeSpaceBoxes = clone.Children.Where((box) => { return box.Type == "ftyp"  // ftyp breaks QuickTime3
                        || box.Type == "free" || box.Type == "skip" || box.Type == "junk" || box.Type == "wide"; }).ToArray();
                    foreach (Box free in freeSpaceBoxes)
                    {
                        clone.Children.Remove(free);
                        Debug.WriteLine("Removing '" + free + "'");
                    }
                }

                Debug.WriteLine("Calculating new 'mdat' offset.");
                ulong mdatOffset = 0;
                foreach (Box box in clone.Children)
                {
                    if (box is MovieDataBox) break;
                    else mdatOffset += GetBoxWriteSize(box);
                }

                bool isPositiveAdjustment = mdatOffset > mdat.Offset.Value;
                ulong chunkAdjustment = mdatOffset > mdat.Offset.Value ? mdatOffset - mdat.Offset.Value : mdat.Offset.Value - mdatOffset;

                Debug.WriteLine("Looking for 'stco' and 'co64' boxes that need to be updated.");

                var traks = moov.Children.OfType<TrackBox>();
                foreach (TrackBox track in traks)
                {
                    MediaBox mdia = track.Children.OfType<MediaBox>().SingleOrDefault();
                    if (mdia != null)
                    {
                        MediaInformationBox minf = mdia.Children.OfType<MediaInformationBox>().SingleOrDefault();
                        if (minf != null)
                        {
                            SampleTableBox stbl = minf.Children.OfType<SampleTableBox>().Single();
                            if (stbl != null)
                            {
                                Debug.WriteLine("Updating 'stco' offsets.");
                                bool upgradeToChunkLongOffsetBox = false;
                                ChunkOffsetBox stco = stbl.Children.OfType<ChunkOffsetBox>().SingleOrDefault();
                                if (stco != null)
                                {
                                    for (int i = 0; i < stco.Entries.Count(); i++)
                                    {
                                        try
                                        {
                                            if (isPositiveAdjustment)
                                                stco.Entries[i] = checked((uint)((ulong)stco.Entries[i] + chunkAdjustment));
                                            else
                                                stco.Entries[i] = checked((uint)((ulong)stco.Entries[i] - chunkAdjustment));
                                        }
                                        catch (OverflowException)
                                        {
                                            upgradeToChunkLongOffsetBox = true;
                                            break;
                                        }
                                    }
                                }

                                if (upgradeToChunkLongOffsetBox)
                                {
                                    Debug.WriteLine("Overflow encountered.  Converting 'stco' to 'co64' to compensate.");
                                    // TODO: Copy and adjust stco values
                                }

                                ChunkLargeOffsetBox co64 = stbl.Children.OfType<ChunkLargeOffsetBox>().SingleOrDefault();
                                if (co64 != null)
                                {
                                    Debug.WriteLine("Updating 'co64' offsets.");
                                    for (int i = 0; i < co64.Entries.Count(); i++)
                                    {
                                        if (isPositiveAdjustment)
                                            co64.Entries[i] = checked(co64.Entries[i] + chunkAdjustment);
                                        else
                                            co64.Entries[i] = checked(co64.Entries[i] - chunkAdjustment);
                                    }
                                }
                            }
                        }
                    }
                }

                clone.SaveAs(path);
                return true;
            }
        }

        public void Close()
        {
            SourceStream.Close();
        }

        public override string ToString()
        {
            return Name;
        }

        public void Dispose()
        {
            if (DisposeSourceStream) SourceStream.Dispose();
        }
    }
}
