using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeProxy.Helpers
{
    public class QuickTimeContainerConverter
    {
        public static void MakeQuickTime3Compatible(string sourceFile)
        {
            var fileInfo = new FileInfo(sourceFile);
            var originalFullPath = fileInfo.FullName;
            var file = new BaseMediaFile(fileInfo);
            var destination = Path.Combine(file.DirectoryName, file.NameWithoutExtension + "_temp" + file.Extension);
            
            file.FastStart(destination);
            file.Close();
            file.Dispose();

            fileInfo.Delete();
            var destinationInfo = new FileInfo(destination);
            destinationInfo.MoveTo(originalFullPath);
            
        }
    }
}
