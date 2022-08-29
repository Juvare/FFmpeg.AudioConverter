ï»¿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notify.Utils.Ffmpeg
{
    public interface IAudioConverter
    {
        Stream ConvertTo(Stream input, InputFormat format);
    }

    public class AudioConverter : IAudioConverter
    {
        private readonly IFileSystem fileSystem;

        public AudioConverter()
        {
            fileSystem = new FileSystem();
        }

        internal AudioConverter(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public Stream ConvertTo(Stream input, InputFormat format)
        {
            ArgumentNullException.ThrowIfNull(input);
            ArgumentNullException.ThrowIfNull(format);

            using var destinationFile = new TempFile(fileSystem);

            throw new NotImplementedException();
        }
    }
}
