﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MatrixIO.IO.Bmff
{
    /// <summary>
    /// BaseMedia encapsulates a tree of Boxes in a random-access stream.
    /// </summary>
    public class BaseMedia : ISuperBox
    {
        protected Stream _SourceStream;

        private const string DefaultName = "Unknown File";

        public BaseMedia() { }
        public BaseMedia(Stream stream)
        {
            if (!stream.CanSeek) new ArgumentException("BaseMedia requires a random-access stream.");
            _SourceStream = stream;
        }

        private bool _childrenLoaded = false;

        private readonly IList<Box> _children = new List<Box>();

        public IList<Box> Children
        {
            get 
            {
                if (!_childrenLoaded)
                {
                    _childrenLoaded = true;
                    if (_SourceStream != null)
                    {
                        //Debug.WriteLine("Loading Children: ");
                        
                        Box box = null;
                        do
                        {
                            box = Box.FromStream(_SourceStream);
                            if (box != null)
                            {
                                _children.Add(box);
                                //Debug.WriteLine("\t" + box);
                            }
                        } while (box != null);
                    }
                }
                return _children;
            }
        }

        public void SaveTo(Stream stream)
        {
            foreach (Box box in _children)
            {
                box.ToStream(stream);
            }
        }

        protected ulong GetBoxWriteSize(Box box)
        {
            return box.CalculateSize();
        }

        public IEnumerator<Box> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    }
}