ï»¿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notify.Utils.Ffmpeg
{
    internal class TempFile : IDisposable
    {
        private readonly string tempFile;
        private readonly IFileSystem fileSystem;

        public TempFile(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;

            tempFile = fileSystem.GetTempFileName();
        }

        public void Dispose() => fileSystem.DeleteFile(tempFile);
    }
}
