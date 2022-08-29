ï»¿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notify.Utils.Ffmpeg
{
    internal interface IFileSystem
    {
        string GetTempFileName();
        void DeleteFile(string path);
    }

    internal class FileSystem : IFileSystem
    {
        public void DeleteFile(string path) => File.Delete(path);

        public string GetTempFileName() => Path.GetTempFileName();
    }
}
