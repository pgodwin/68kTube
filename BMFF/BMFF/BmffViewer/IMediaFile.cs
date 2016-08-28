using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MatrixIO.IO.Bmff;

namespace BmffViewer
{
    public interface IMediaFile
    {
        string Name { get; }
        string FullName { get; }
        string Extension { get; }
        string DirectoryName { get; }

        Stream SourceStream { get; }
    }
}
